mod dispatcher;
mod identifier;
mod users;

use std::{collections::HashMap, sync::Arc};
use tokio::net::TcpListener;
use tokio::sync::Mutex;

use crate::dispatcher::{Dispatcher, DispatcherRegistry};
use crate::users::UserManager;

#[tokio::main]
async fn main() {
    let listener = TcpListener::bind("127.0.0.1:9001")
        .await
        .expect("Can't bind");
    println!("Listening on ws://127.0.0.1:9001");

    let registry: DispatcherRegistry = Arc::new(Mutex::new(HashMap::new()));
    let user_manager: Arc<Mutex<UserManager>> = Arc::new(Mutex::new(UserManager::new()));

    // Accept incoming connections
    while let Ok((stream, addr)) = listener.accept().await {
        println!("New connection from {}", addr);

        let id = addr.port() as u64;

        // Create a new dispatcher for the incoming connection
        let dispatcher = Arc::new(Mutex::new(Dispatcher::new()));
        {
            // Register the dispatcher in the global registry
            let mut registry_guard = registry.lock().await;
            registry_guard.insert(id, Arc::clone(&dispatcher));
        }

        let registry_ref = Arc::clone(&registry);
        let user_manager_ref = Arc::clone(&user_manager);

        tokio::spawn(async move {
            let mut dispatcher_guard = dispatcher.lock().await;
            dispatcher_guard
                .init_websocket_session(stream, addr, registry_ref, user_manager_ref)
                .await;
        });
    }
}
