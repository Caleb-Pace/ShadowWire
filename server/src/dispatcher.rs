use futures::{
    SinkExt, StreamExt,
    stream::{SplitSink, SplitStream},
};
use std::net::SocketAddr;
use std::{collections::HashMap, sync::Arc};
use tokio::net::TcpStream;
use tokio::sync::Mutex;
use tokio_tungstenite::{
    WebSocketStream, accept_async,
    tungstenite::{Bytes, protocol::Message},
};

use crate::{dispatcher_manager::DispatcherManager, identifier::Identifier, users::UserManager};

pub type DispatcherRegistry = Arc<Mutex<HashMap<u64, Arc<Mutex<Dispatcher>>>>>;

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
}

impl Dispatcher {
    pub fn new() -> Self {
        Dispatcher {
            identifier: None,
            dispatcher_manager_ref: None,
            user_manager_ref: None,
            websocket: None,
        }
    }

    fn identify_user(&mut self) {
        unimplemented!("User identification functionality not implemented yet!");
    }

    fn register_user(&self) {
        unimplemented!(
            "Registration request functionality not implemented yet! (identifier: {:#?})",
            self.identifier.as_ref().map(|id| id.fingerprint)
        );
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
                Some(Ok(Message::Text(txt))) => {
                    println!("Received: \"{}\"", txt);

                    // Identify the user if not already done
                    if self.identifier.is_none() {
                        self.identify_user();
                    }

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
