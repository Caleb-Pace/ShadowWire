struct Dispatcher {
    username: String,
    websocket: (), // Placeholder for WebSocket type
}

impl Dispatcher {
    fn new(username: String, websocket: ()) -> Self {
        // TODO: Check if user is registered.
        Dispatcher { username, websocket }
    }

    fn register_user(&self) {
        unimplemented!("Registration request functionality not implemented yet! (username: {})", self.username);
    }

    fn lookup_request(&self) {
        unimplemented!("Lookup request functionality not implemented yet! (username: {})", self.username);
    }

    fn relay_messages(&self, recipient_username: &str) {
        unimplemented!("Message relaying functionality not implemented yet! (username: {})", recipient_username);
    }

    pub fn handle_connection(&self) {
        unimplemented!("Request handling functionality not implemented yet! (username: {})", self.username);
    }

    pub fn send_message(&self, message: &str) -> String {
        unimplemented!("Sending message functionality not implemented yet! (message: {})", self.username, message);
    }
}