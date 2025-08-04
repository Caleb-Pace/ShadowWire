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

use crate::{identifier::Identifier, users::UserManager};

pub type DispatcherRegistry = Arc<Mutex<HashMap<u64, Arc<Mutex<Dispatcher>>>>>;

struct WebSocket {
    addr: SocketAddr,
    read: Mutex<SplitStream<WebSocketStream<TcpStream>>>,
    write: Mutex<SplitSink<WebSocketStream<TcpStream>, Message>>,
}

pub struct Dispatcher {
    identifier: Option<Identifier>,
    registry_ref: Option<DispatcherRegistry>,
    user_manager_ref: Option<Arc<Mutex<UserManager>>>,
    websocket: Option<WebSocket>,
}

impl Dispatcher {
    pub fn new() -> Self {
        Dispatcher {
            identifier: None,
            registry_ref: None,
            user_manager_ref: None,
            websocket: None,
        }
    }

    fn identify_user(&mut self) {}

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
        registry_ref: DispatcherRegistry,
        user_manager_ref: Arc<Mutex<UserManager>>,
    ) {
        self.registry_ref = Some(registry_ref);
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

    async fn handle_connections(&self) {
        if self.websocket.is_none() {
            eprintln!("WebSocket not initialized, cannot handle connections!");
            return;
        }
        let ws = self.websocket.as_ref().unwrap();

        loop {
            let incoming_message = {
                let mut read_guard = ws.read.lock().await;
                read_guard.next().await
            };

            match incoming_message {
                Some(Ok(Message::Text(txt))) => {
                    println!("Received: {}", txt);

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
                    let id = ws.addr.port() as u64;

                    // After session ends, remove from registry
                    if let Some(registry) = &self.registry_ref {
                        let mut registry_guard = registry.lock().await;
                        registry_guard.remove(&id);
                    }

                    println!("{} ({}) disconnected!", ws.addr, id);
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
