use std::collections::HashMap;

use crate::{dispatcher::Dispatcher, identifier::Identifier};

pub struct DispatcherManager {
    registry: HashMap<[u8; 32], Dispatcher>,
}

impl DispatcherManager {
    pub fn new() -> Self {
        DispatcherManager {
            registry: HashMap::new(),
        }
    }

    pub fn register_dispatcher(&mut self, identifier: Identifier, dispatcher: Dispatcher) {
        self.registry
            .insert(identifier.fingerprint.clone(), dispatcher);
    }

    pub fn deregister_dispatcher(&mut self, identifier: Identifier) {
        self.registry.remove(&identifier.fingerprint);
    }

    pub fn get_dispatcher(&self, recipient_identifier: Identifier) -> Option<&Dispatcher> {
        self.registry.get(&recipient_identifier.fingerprint)
    }
}
