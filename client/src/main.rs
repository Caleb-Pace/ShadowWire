mod connection;
mod keys;
mod messaging;
mod user;

#[tokio::main]
async fn main() {
    let username = user::resolve_user();
    println!("Logged in as \"{}\"", username);

    let (priv_key, pub_key) = keys::resolve_rsa_keypair();
    println!("RSA Keypair resolved successfully.");

    let mut connection = connection::Connection::new();
    connection.connect(username, pub_key, priv_key).await;
}
