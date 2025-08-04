use serde::{Deserialize, Serialize};

/// Represents a unique user identity.
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Identifier {
    /// The username associated with this identity.
    pub username: String,
    /// 256-bit identifier (SHA-256 or similar).
    pub fingerprint: [u8; 32],
    /// DER-encoded RSA public key bytes for transport and verification.
    pub public_key: Vec<u8>,
}
