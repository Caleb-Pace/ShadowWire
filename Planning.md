## MVP (Minimum Viable Product)
### Client - Basic CLI
- [ ] Setup
	- [ ] Generate & store key pair.
	- [ ] Get & store username.
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