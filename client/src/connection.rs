use futures::{SinkExt, StreamExt};
use tokio_tungstenite::connect_async;
use tungstenite::protocol::Message;

const SERVER_URL: &str = "ws://127.0.0.1:9001";

pub struct Connection {}

impl Connection {
    pub fn new() -> Self {
        Connection {}
    }

    pub async fn connect(&self) {
        // Placeholder for connection logic
        println!("Connecting to the server...");

        let (ws_stream, _) = connect_async(SERVER_URL).await.expect("Failed to connect");
        println!("Connected to the server");

        let (mut write, mut read) = ws_stream.split();

        // Send a message
        write
            .send(Message::Text("Hello WebSocket".into()))
            .await
            .unwrap();

        // Read a response
        if let Some(msg) = read.next().await {
            match msg {
                Ok(msg) => println!("Received: \"{}\"", msg),
                Err(e) => eprintln!("Error reading message: \"{}\"", e),
            }
        }
    }
}
