use rsa::{RsaPrivateKey, RsaPublicKey};

const PRIVATE_KEY_FILE: &str = "sw_key_private.der";
const PUBLIC_KEY_FILE: &str = "sw_key_public.der";

/// Generates a new 2048-bit RSA keypair, saves them to disk, and returns the pair.
fn generate_rsa_keypair() -> (RsaPrivateKey, RsaPublicKey) {
    let mut rng = rand::thread_rng(); // Use thread-local random number generator
    let bits = 2048;

    let priv_key = RsaPrivateKey::new(&mut rng, bits).expect("failed to generate a key");
    let pub_key = RsaPublicKey::from(&priv_key);

    save_rsa_keypair(&priv_key, &pub_key).expect("failed to save keys");

    load_rsa_keypair().expect("failed to load keys after generation")
}

/// Saves the given RSA private and public keys to disk in DER format.
fn save_rsa_keypair(priv_key: &RsaPrivateKey, pub_key: &RsaPublicKey) -> std::io::Result<()> {
    use rsa::pkcs1::EncodeRsaPublicKey;
    use rsa::pkcs8::EncodePrivateKey;
    use std::fs::File;
    use std::io::Write;

    // Save private key in DER format
    let priv_der = priv_key
        .to_pkcs8_der()
        .map_err(|e| std::io::Error::new(std::io::ErrorKind::Other, e))?;
    let mut priv_file = File::create(PRIVATE_KEY_FILE)?;
    priv_file.write_all(priv_der.as_bytes())?;

    // Save public key in DER format
    let pub_der = pub_key
        .to_pkcs1_der()
        .map_err(|e| std::io::Error::new(std::io::ErrorKind::Other, e))?;
    let mut pub_file = File::create(PUBLIC_KEY_FILE)?;
    pub_file.write_all(pub_der.as_bytes())?;

    Ok(())
}

/// Loads the RSA private and public keys from disk, returning None if either is missing or invalid.
fn load_rsa_keypair() -> Option<(RsaPrivateKey, RsaPublicKey)> {
    use rsa::pkcs1::DecodeRsaPublicKey;
    use rsa::pkcs8::DecodePrivateKey;
    use std::fs;

    // Load private key from DER file
    let priv_bytes = fs::read(PRIVATE_KEY_FILE).ok()?;
    let priv_key = RsaPrivateKey::from_pkcs8_der(&priv_bytes).ok()?;

    // Load public key from DER file
    let pub_bytes = fs::read(PUBLIC_KEY_FILE).ok()?;
    let pub_key = RsaPublicKey::from_pkcs1_der(&pub_bytes).ok()?;

    Some((priv_key, pub_key))
}

/// Returns the existing RSA keypair if found, otherwise generates and saves a new keypair.
pub fn resolve_rsa_keypair() -> (RsaPrivateKey, RsaPublicKey) {
    if let Some((priv_key, pub_key)) = load_rsa_keypair() {
        println!("Loaded existing RSA keys.");
        (priv_key, pub_key)
    } else {
        println!("Keys not found. Generating new RSA keypair...");
        generate_rsa_keypair()
    }
}
