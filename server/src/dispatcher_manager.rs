use std::{
    collections::HashMap,
    sync::{Arc, Weak},
};

use tokio::sync::Mutex;

use shared::identifier::Identifier;
use crate::dispatcher::Dispatcher;

pub struct DispatcherManager {
    registry: HashMap<[u8; 32], Weak<Mutex<Dispatcher>>>,
}

impl DispatcherManager {
    pub fn new() -> Self {
        DispatcherManager {
            registry: HashMap::new(),
        }
    }

    pub fn register_dispatcher(
        &mut self,
        identifier: Identifier,
        dispatcher: Weak<Mutex<Dispatcher>>,
    ) {
        self.registry
            .insert(identifier.fingerprint.clone(), dispatcher);
    }

    fn prune_registry(&mut self) {
        self.registry.retain(|_, weak| weak.strong_count() > 0);
    }

    pub fn deregister_dispatcher(&mut self, identifier: Identifier) {
        self.registry.remove(&identifier.fingerprint);

        self.prune_registry(); // Shouldn't matter, just a fallback.
    }

    pub fn get_dispatcher(
        &self,
        recipient_identifier: Identifier,
    ) -> Option<Arc<Mutex<Dispatcher>>> {
        self.registry
            .get(&recipient_identifier.fingerprint)
            .and_then(|weak| weak.upgrade())
    }
}
