## MVP (Minimum Viable Product)

### Client - Basic CLI
```
Client/
│── Program.cs
│── Network/
│   └── Connection.cs        // Client connection to server
│── Security/
│   └── Cryptography.cs      // Key management and encryption
│── Messaging.cs             // Handle messages
└── CliUI.cs
```
- [ ] Setup
	- [x] Generate & store key pair.
	- [x] Get & store username.
		- *Note: may need to do duplicate detection later*
	- [x] Register with server:
		- [x] Send public key and username to server.
- [ ] Messaging:
	- [ ] Send messages. (Send to server).
	- [ ] Receive messages. (Query from server).
	- [ ] Encrypt messages with other users public key.
### Server
```
RelayServer/
│── Program.cs
│── Network/
│   ├── RelayServer.cs       // Starts TCP listener, manages clients
│   └── SessionHandler.cs    // Handles communication with a connected client
│── Data/
│   ├── UserRegistry.cs      // Stores username + public key
│   └── MessageStore.cs      // Stores undelivered messages
```
- [ ] Accept new users.
	- *Note: this initial step will need to be secured later*
	- [ ] Retrieve public key.
	- [ ] Retrieve username.
- [ ] Store user information.
	- Username
	- Public key
- [ ] Distribute user data between users (usernames and public keys).
- [ ] Store messages for users. (Client send).
- [ ] Relay messages when requested. (Client receive).