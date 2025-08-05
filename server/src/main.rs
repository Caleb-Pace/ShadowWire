mod dispatcher;
mod dispatcher_manager;
mod users;

use std::sync::Arc;
use tokio::net::TcpListener;
use tokio::sync::Mutex;

use crate::dispatcher::Dispatcher;
use crate::dispatcher_manager::DispatcherManager;
use crate::users::UserManager;

#[tokio::main]
async fn main() {
    let listener = TcpListener::bind("127.0.0.1:9001")
        .await
        .expect("Can't bind");
    println!("Listening on ws://127.0.0.1:9001");

    let dispatcher_manager: Arc<Mutex<DispatcherManager>> =
        Arc::new(Mutex::new(DispatcherManager::new()));
    let user_manager: Arc<Mutex<UserManager>> = Arc::new(Mutex::new(UserManager::new()));

    // Accept incoming connections
    while let Ok((stream, addr)) = listener.accept().await {
        println!("New connection from {}", addr);

        // Clone manager references for the new dispatcher
        let dispatcher_manager_ref = Arc::clone(&dispatcher_manager);
        let user_manager_ref = Arc::clone(&user_manager);

        tokio::spawn(async move {
            // Create a new dispatcher for the incoming connection
            let dispatcher = Dispatcher::new();
            {
                let mut lock = dispatcher.lock().await;
                lock.init_websocket_session(stream, addr, dispatcher_manager_ref, user_manager_ref)
                    .await;
            }
        });
    }
}
