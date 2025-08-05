use rsa::pkcs1::EncodeRsaPublicKey;

mod connection;
mod keys;
mod user;

#[tokio::main]
async fn main() {
    let username = user::resolve_user();
    println!("Logged in as \"{}\"", username);

    let (_priv_key, pub_key) = keys::resolve_rsa_keypair();
    println!("RSA Keypair resolved successfully.");

    let pub_key_bin = pub_key
        .to_pkcs1_der()
        .expect("Failed to DER encode public key");
    let identity = shared::protocol::identity::Identification {
        username,
        public_key: pub_key_bin.to_vec(),
    };

    let mut connection = connection::Connection::new();
    connection.connect(identity).await;
}
