use serde::{Deserialize, Serialize};
use serde_cbor;

/// Identity protocol structure, serialized with CBOR (Concise Binary Object Representation).
#[derive(Serialize, Deserialize, Clone, Debug)]
pub struct Identification {
    /// The username associated with this identity.
    pub username: String,
    /// DER-encoded RSA public key bytes for transport and verification.
    pub public_key: Vec<u8>,
}

impl Identification {
    /// Serializes the identity to CBOR bytes.
    pub fn to_bytes(&self) -> Result<Vec<u8>, serde_cbor::Error> {
        serde_cbor::to_vec(self)
    }

    /// Deserializes the identity from CBOR bytes.
    pub fn from_bytes(bytes: &[u8]) -> Result<Self, serde_cbor::Error> {
        serde_cbor::from_slice(bytes)
    }
}
