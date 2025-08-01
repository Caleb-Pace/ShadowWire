mod keys;
mod user;

fn main() {
    println!("Hello from the client!");
    shared::test();

    let username = user::resolve_user();
    println!("Logged in as \"{}\"", username);

    let (_priv_key, _pub_key) = keys::resolve_rsa_keypair();
    println!("RSA Keypair resolved successfully.");
}
