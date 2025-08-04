mod connection;
mod keys;
mod user;

#[tokio::main]
async fn main() {
    let username = user::resolve_user();
    println!("Logged in as \"{}\"", username);

    let (_priv_key, _pub_key) = keys::resolve_rsa_keypair();
    println!("RSA Keypair resolved successfully.");

    let connection = connection::Connection::new();
    connection.connect().await;
}
