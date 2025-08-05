use futures::{
    SinkExt, StreamExt,
    stream::{SplitSink, SplitStream},
};
use std::sync::Arc;
use std::{net::SocketAddr, sync::Weak};
use tokio::net::TcpStream;
use tokio::sync::Mutex;
use tokio_tungstenite::{
    WebSocketStream, accept_async,
    tungstenite::{Bytes, protocol::Message},
};

use crate::{dispatcher_manager::DispatcherManager, identifier::Identifier, users::UserManager};

struct WebSocket {
    addr: SocketAddr,
    read: Mutex<SplitStream<WebSocketStream<TcpStream>>>,
    write: Mutex<SplitSink<WebSocketStream<TcpStream>, Message>>,
}

pub struct Dispatcher {
    identifier: Option<Identifier>,
    dispatcher_manager_ref: Option<Arc<Mutex<DispatcherManager>>>,
    user_manager_ref: Option<Arc<Mutex<UserManager>>>,
    websocket: Option<WebSocket>,
    self_ref: Option<Weak<Mutex<Dispatcher>>>,
}

impl Dispatcher {
    pub fn new() -> Arc<Mutex<Self>> {
        let dispatcher = Arc::new(Mutex::new(Dispatcher {
            identifier: None,
            dispatcher_manager_ref: None,
            user_manager_ref: None,
            websocket: None,
            self_ref: None,
        }));

        // Insert the weak self reference
        let weak = Arc::downgrade(&dispatcher);
        {
            // Here you have to lock mutex to set self_ref:
            let mut guard = futures::executor::block_on(dispatcher.lock());
            guard.self_ref = Some(weak);
        }

        dispatcher
    }

    async fn identify_user(&mut self, bin: Bytes) {
        use sha2::Digest;

        shared::protocol::identity::Identification::from_bytes(&bin)
            .map(|identity| {
                let mut hasher = sha2::Sha256::new();
                hasher.update(&identity.public_key);
                let result = hasher.finalize();

                let mut fingerprint = [0u8; 32];
                fingerprint.copy_from_slice(&result);

                self.identifier = Some(Identifier {
                    username: identity.username,
                    public_key: identity.public_key,
                    fingerprint: fingerprint,
                });
            })
            .unwrap_or_else(|_| {
                eprintln!("Failed to deserialize user identification from bytes");
            });

        self.register_user().await;
        self.register_dispatcher().await;
    }

    async fn register_user(&mut self) {
        if let Some(user_manager) = &self.user_manager_ref {
            let mut user_manager_guard = user_manager.lock().await;

            if user_manager_guard
                .get_user(self.identifier.as_ref().unwrap().fingerprint)
                .is_none()
            {
                if let Some(identifier) = &self.identifier {
                    user_manager_guard.add_user(identifier.clone());
                }
            }
        } else {
            panic!("User manager reference is not set, cannot register user!");
        }
    }

    async fn register_dispatcher(&mut self) {
        if let Some(dispatcher_manager) = &self.dispatcher_manager_ref {
            let mut manager_guard = dispatcher_manager.lock().await;

            if let Some(identifier) = &self.identifier {
                // Extract the weak reference from the Option.
                if let Some(self_ref) = &self.self_ref {
                    manager_guard.register_dispatcher(identifier.clone(), self_ref.clone());
                } else {
                    eprintln!("self_ref is None, cannot register dispatcher");
                }
            } else {
                eprintln!("Identifier is None, cannot register dispatcher!");
            }
        } else {
            panic!("Dispatcher manager reference is not set, cannot register dispatcher!");
        }
    }

    fn lookup_request(&self) {
        unimplemented!(
            "Lookup request functionality not implemented yet! (identifier: {:#?})",
            self.identifier.as_ref().map(|id| id.fingerprint)
        );
    }

    fn relay_messages(&self, recipient_identifier: &Identifier) {
        unimplemented!(
            "Message relaying functionality not implemented yet! (identifier: {:#?})",
            recipient_identifier.fingerprint
        );
    }

    pub async fn init_websocket_session(
        &mut self,
        stream: TcpStream,
        addr: SocketAddr,
        dispatcher_manager_ref: Arc<Mutex<DispatcherManager>>,
        user_manager_ref: Arc<Mutex<UserManager>>,
    ) {
        self.dispatcher_manager_ref = Some(dispatcher_manager_ref);
        self.user_manager_ref = Some(user_manager_ref);

        // Initialize the WebSocket session
        let ws_stream: WebSocketStream<TcpStream> = accept_async(stream)
            .await
            .expect("Error during WebSocket handshake");
        let (write, read) = ws_stream.split();

        // Store the WebSocket
        self.websocket = Some(WebSocket {
            addr,
            read: Mutex::new(read),
            write: Mutex::new(write),
        });
        println!("WebSocket session initialized for {}", addr);

        self.handle_connections().await;
    }

    async fn handle_connections(&mut self) {
        if self.websocket.is_none() {
            eprintln!("WebSocket not initialized, cannot handle connections!");
            return;
        }

        loop {
            let incoming_message = {
                let ws = self.websocket.as_ref().unwrap();
                let mut read_guard = ws.read.lock().await;
                read_guard.next().await
            };

            match incoming_message {
                Some(Ok(Message::Binary(bin))) => {
                    println!("Received binary message: {:?}", bin);

                    // Identify the user if not already done
                    if self.identifier.is_none() {
                        self.identify_user(bin).await;
                        println!(
                            "User identified (\"{:?}\")",
                            self.identifier.as_ref().unwrap().username
                        );
                    }
                }
                Some(Ok(Message::Text(txt))) => {
                    println!("Received: \"{}\"", txt);

                    let ws = self.websocket.as_ref().unwrap();
                    let mut write_guard = ws.write.lock().await;
                    if write_guard
                        .send(Message::text(format!("Echo: {}", txt)))
                        .await
                        .is_err()
                    {
                        eprintln!("Failed to send, closing {}", ws.addr);
                        break;
                    }
                }
                Some(Ok(Message::Close(_))) => {
                    // After session ends, deregister
                    if let Some(dispatcher_manager) = &self.dispatcher_manager_ref {
                        if let Some(id) = &self.identifier {
                            let mut manager_guard = dispatcher_manager.lock().await;
                            manager_guard.deregister_dispatcher(id.clone());
                        }
                    }

                    let ws = self.websocket.as_ref().unwrap();
                    println!(
                        "{} disconnected! ({})",
                        self.identifier
                            .as_ref()
                            .map_or("Unknown", |id| &id.username),
                        ws.addr
                    );
                    break;
                }
                Some(_) => {}
                None => break,
            }
        }
    }

    pub async fn send_message(&self, message_binary: Bytes) {
        if self.websocket.is_none() {
            eprintln!("WebSocket not initialized, cannot handle connections!");
            return;
        }
        let ws = self.websocket.as_ref().unwrap();

        let mut write_guard = ws.write.lock().await;

        if write_guard
            .send(Message::Binary(message_binary))
            .await
            .is_err()
        {
            eprintln!("Failed to send, closing {}", ws.addr);
        }
    }
}
