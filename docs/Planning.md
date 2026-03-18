## MVP (Minimum Viable Product)

### Client - Basic CLI
- [x] Setup
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
- [x] Accept new users.
	- *Note: this initial step will need to be secured later*
	- [x] Retrieve public key.
	- [x] Retrieve username.
- [x] Store user information.
	- Username
	- Public key
- [x] Distribute user data between users (usernames and public keys).
