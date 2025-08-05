use futures::{SinkExt, StreamExt, stream::SplitSink};
use rsa::{RsaPrivateKey, RsaPublicKey, pkcs1::EncodeRsaPublicKey};
use shared::protocol::identity::Identification;
use tokio::net::TcpStream;
use tokio_tungstenite::{MaybeTlsStream, WebSocketStream, connect_async};
use tungstenite::{Bytes, protocol::Message};

use crate::messaging::encrypt;

const SERVER_URL: &str = "ws://127.0.0.1:9001";

pub struct Connection {
    ws_write: Option<SplitSink<WebSocketStream<MaybeTlsStream<TcpStream>>, Message>>,
    pub_key: Option<RsaPublicKey>,
    priv_key: Option<RsaPrivateKey>,
}

impl Connection {
    pub fn new() -> Self {
        Connection {
            ws_write: None,
            pub_key: None,
            priv_key: None,
        }
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

    pub async fn connect(
        &mut self,
        username: String,
        pub_key: RsaPublicKey,
        priv_key: RsaPrivateKey,
    ) {
        // Placeholder for connection logic
        println!("Connecting to the server...");

        let (ws_stream, _) = connect_async(SERVER_URL).await.expect("Failed to connect");
        println!("Connected to the server");

        let (write, mut read) = ws_stream.split();
        self.ws_write = Some(write);
        self.priv_key = Some(priv_key);
        self.pub_key = Some(pub_key);

        // Authenticate the user
        if let Some(pub_key) = self.pub_key.as_ref() {
            let pub_key_bin = pub_key
                .to_pkcs1_der()
                .expect("Failed to DER encode public key");
            let identity = shared::protocol::identity::Identification {
                username,
                public_key: pub_key_bin.to_vec(),
            };

            self.authenticate(identity).await;
        } else {
            eprintln!("No public key available for authentication");
            return;
        }

        self.send_message(
            self.pub_key.clone().expect("Public key not set"),
            "Hello from client!",
        )
        .await;

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

    pub async fn send_message(&mut self, recipient_pub_key: RsaPublicKey, message: &str) {
        if let Some(ref mut write) = self.ws_write {
            write
                .send(Message::Binary(encrypt(
                    message,
                    recipient_pub_key,
                    self.priv_key.clone().expect("Private key not set"),
                )))
                .await
                .expect("Failed to send message");
        }
    }
}
