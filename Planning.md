## MVP (Minimum Viable Product)
### Client - Basic CLI
```
client/
└── src/
    ├── main.rs
    ├── keys.rs          // key generation, saving, loading
    ├── user.rs          // user info storage & retrieval
    ├── connection.rs    // networking, server comms
    ├── messaging.rs     // message encrypt/decrypt, format, sending/receiving
    └── cache.rs         // message caching
```
- [ ] Setup
	- [x] Generate & store key pair.
	- [x] Get & store username.
		- *Note: may need to do duplicate detection later*
	- [ ] Register with server:
		- [ ] Send public key and username to server.
- [ ] Messaging:
	- [ ] Send messages. (Send to server).
	- [ ] Receive messages. (Query from server).
	- [ ] Encrypt messages with other users public key.
### Server
- [ ] Accept new users.
	- *Note: this initial step will need to be secured later*
	- [ ] Retrieve public key.
	- [ ] Retrieve username.
- [ ] Store user information.
	- Username
	- Public key
- [ ] Store messages for users. (Client send).
- [ ] Relay messages when requested. (Client receive).