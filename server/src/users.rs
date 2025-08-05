use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use std::fs::File;
use std::io::BufReader;

use shared::identifier::Identifier;

const USERS_FILE: &str = "users.json";

#[derive(Serialize, Deserialize)]
pub struct UserManager {
    users: Vec<Identifier>,
    by_username: HashMap<String, usize>,
    by_fingerprint: HashMap<[u8; 32], usize>,
}

impl UserManager {
    pub fn new() -> Self {
        let mut user_manager = UserManager {
            users: Vec::new(),
            by_username: HashMap::new(),
            by_fingerprint: HashMap::new(),
        };

        // Load existing users from file
        user_manager.load();

        user_manager
    }

    pub fn add_user(&mut self, identifier: Identifier) {
        let index = self.users.len();
        self.by_username.insert(identifier.username.clone(), index);
        self.by_fingerprint.insert(identifier.fingerprint, index);

        self.users.push(identifier);

        self.save();
    }

    pub fn get_user(&self, fingerprint: [u8; 32]) -> Option<&Identifier> {
        self.by_fingerprint
            .get(&fingerprint)
            .and_then(|&index| self.users.get(index))
    }

    pub fn get_user_by_username(&self, username: &str) -> Option<&Identifier> {
        self.by_username
            .get(username)
            .and_then(|&index| self.users.get(index))
    }

    fn load(&mut self) {
        self.users = File::open(USERS_FILE)
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

        let mut file = File::create(USERS_FILE).unwrap();
        file.write_all(json.as_bytes())
            .expect("Failed to save to users");
    }
}
