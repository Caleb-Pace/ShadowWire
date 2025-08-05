use futures::{SinkExt, StreamExt, stream::SplitSink};
use shared::protocol::identity::Identification;
use tokio::net::TcpStream;
use tokio_tungstenite::{MaybeTlsStream, WebSocketStream, connect_async};
use tungstenite::{Bytes, protocol::Message};

const SERVER_URL: &str = "ws://127.0.0.1:9001";

pub struct Connection {
    ws_write: Option<SplitSink<WebSocketStream<MaybeTlsStream<TcpStream>>, Message>>,
}

impl Connection {
    pub fn new() -> Self {
        Connection { ws_write: None }
    }

    async fn authenticate(&mut self, identity: Identification) {
        if let Some(ref mut write) = self.ws_write {
            let bin = identity.to_bytes().expect("Failed to serialize identity");
            write
                .send(Message::Binary(Bytes::from(bin)))
                .await
                .expect("Failed to send authentication message");
        }
    }

    pub async fn connect(&mut self, identity: Identification) {
        // Placeholder for connection logic
        println!("Connecting to the server...");

        let (ws_stream, _) = connect_async(SERVER_URL).await.expect("Failed to connect");
        println!("Connected to the server");

        let (write, mut read) = ws_stream.split();
        self.ws_write = Some(write);

        // Authenticate the user
        self.authenticate(identity).await;

        // Send a message
        if let Some(ref mut write) = self.ws_write {
            write
                .send(Message::Text("Hello WebSocket".into()))
                .await
                .unwrap();
        }

        // Read a response
        if let Some(msg) = read.next().await {
            match msg {
                Ok(msg) => println!("Received: \"{}\"", msg),
                Err(e) => eprintln!("Error reading message: \"{}\"", e),
            }
        }
    }
}
