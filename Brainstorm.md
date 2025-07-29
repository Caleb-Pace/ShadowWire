## Architecture
- Centralised server. *(Availability)*
	- Clients pass messages through server.
	- Limits client knowledge of each other.
	- Simplifies networking: Clients only need to know 1 IP address.
	- Can distribute public keys.
	- Can hold list of users, allowing users to be referenced by a username.
	- **Con:** Server has to be trusted
- Peer-to-peer.
	- More secure for direct communication.
	- No server needed.
	- **Con:** Both clients need to be online to receive messages.

## Features
### Core
- Send messages (TX)
- Receive messages (RX)
- Store received messages and metadata like sender.
	- Possibly keep original input to preserve Integrity.
### Networking/Encryption
- End-to-end encryption.
- Web socket connection (to server or other client).
- Military grade encryption?
- Each user has a key pair, used for authentication and setting up communications.
- Asymmetric encryption will be used to: ***\[RSA or ECC\]***
	- Verify the users. *(Authenticity)*
	- Create a shared key for symmetric encryption. ***\[DH or ECDH\]***
- Symmetric encryption will be used to send the messages. *(Confidentiality)* ***\[AES\]***
### Messages
- Include a system similar to HMAC (Hashed Message Authentication Code) *(Authenticity)*
- Messages are signed with senders private key. *(Non-Repudiation)*
- **Need** a better way to ensure freshness.
- Compress messages for efficiency. (Before or After encryption?)
- Includes timestamp. (Verify at server)
### Client
- Password to access user.
	- Encrypt private key.
	- Stores salted hash of password.
### Server
- Username storage (like a database), stores:
	- Username.
	- User public key.
	- Current IP (if not actively connected will be blank/null).
- Messages are cached/stored if user is not online. Once online messages will be sent.
- Verify message timestamps.
- Could encrypt traffic between clients and server (as well as end-to-end encryption) to anonymise users from each other on the same network.
	  *(If they were packet sniffing you could see who receives the message you sent because it would be the same if only end-to-end encrypted)*
- Transfer messages between clients.
- Registration.
	- Initial registration is encrypted.
	- Provide challenge phrase when registering, to prove user has private key. 
### Later/Future
- Use Post-Quantum Cryptography (PQC) algorithms.
- Key reset (for compromised keys) - Requires old key.
- Notification sounds to TUI.
- File transfer support.
- Group chat system.
- Multiple user system for clients. (Locally based).
	- Create/Register user.
	- Login (works with password encrypted private keys).
- Support for private key on external drive.

## Interface (TUI)
- Alert for when message is received.
- Colour support for messages.
- Coloured UI.
- Recipient selector.
- Clear separation between sent and received messages.
- Load old messages.