use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use std::fs::File;
use std::io::BufReader;

use crate::identifier::Identifier;

#[derive(Serialize, Deserialize)]
pub struct ContactsList {
    filepath: String,
    users: Vec<Identifier>,
    by_username: HashMap<String, usize>,
    by_fingerprint: HashMap<[u8; 32], usize>,
}

impl ContactsList {
    pub fn new(filepath: String) -> Self {
        let mut contacts_list = ContactsList {
            filepath,
            users: Vec::new(),
            by_username: HashMap::new(),
            by_fingerprint: HashMap::new(),
        };

        // Load existing contacts from file
        contacts_list.load();

        contacts_list
    }

    pub fn add(&mut self, identifier: Identifier) {
        let index = self.users.len();
        self.by_username.insert(identifier.username.clone(), index);
        self.users.push(identifier);

        self.save();
    }

    pub fn get_by_fingerprint(&self, fingerprint: &[u8; 32]) -> Option<&Identifier> {
        self.by_fingerprint
            .get(fingerprint)
            .and_then(|&index| self.users.get(index))
    }

    pub fn get_by_username(&self, username: &str) -> Option<&Identifier> {
        self.by_username
            .get(username)
            .and_then(|&index| self.users.get(index))
    }

    fn load(&mut self) {
        self.users = File::open(self.filepath.clone())
            .ok()
            .map(BufReader::new)
            .and_then(|r| serde_json::from_reader(r).ok())
            .unwrap_or_default();

        // Rebuild the lookup maps
        for (index, user) in self.users.iter().enumerate() {
            self.by_username.insert(user.username.clone(), index);
            self.by_fingerprint.insert(user.fingerprint, index);
        }
    }

    fn save(&self) {
        use std::io::Write;

        let json = serde_json::to_string_pretty(&self.users).unwrap();

        let mut file = File::create(self.filepath.clone()).unwrap();
        file.write_all(json.as_bytes())
            .expect("Failed to save to contacts");
    }
}
