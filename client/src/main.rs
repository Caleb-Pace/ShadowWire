mod keys;

fn main() {
    println!("Hello from the client!");
    shared::test();

    let (_priv_key, _pub_key) = keys::resolve_rsa_keypair();
    println!("RSA Keypair resolved successfully.");
}
