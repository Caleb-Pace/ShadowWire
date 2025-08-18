## Architecture
- Centralised server. *(Availability)*
	- Clients pass messages through server.
	- Limits client knowledge of each other.
	- Simplifies networking: Clients only need to know 1 IP address.
	- Can distribute public keys.
	- Can hold list of users, allowing users to be referenced by a username.
	- **Con:** Server has to be trusted
- Zero-trust server. *(Availability)*
	- You have to share the fingerprint (of your public key) with the other user.
		- Think like pre-sharing a key.
		- Cannot be done through the server (assume the server is compromised).
		- Public key should be salted to make it harder to trace back to / track users.
	- This prevents man in the middle attacks by verifying the public key given.
- Peer-to-Peer.
	- More secure for direct communication.
	- No server needed.
	- **Con:** Both clients need to be online to receive messages.
	- Connecting.
		- IP addresses are shared through known server.
			- Still relies on some server trust. (Use the zero-trust model)
		- Or manually, IP addresses can be used directly (completely bypassing the need for a server).
	- Using P2P.
		- Can set option to prefer P2P for specific contacts.
			- Can handle IP transfer through server (like sending a custom message) so that you aren't made discoverable.
		- Optional to enable P2P communications in a chat. (Like through a command).
		- Enable or disable being discoverable for P2P (aka giving your IP to the server to distribute).

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
	- TTL for messages.
	- Have some timestamp stored within them.
- Compress messages for efficiency. (Before or After encryption?)
- Includes timestamp. (Verify at server)
### Client
- Password to access user.
	- Encrypt private key.
	- Stores salted hash of password.
- Block other users.
	- Lets server know.
- MitM attack detection (Zero-trust server architecture).
	- Once an the fingerprint or alternative has been shared you can verify that the server is sending the correct key.
	- If the server does not send the correct key this can be detected and should alert the user with some sort of pop-up or warning that the server has modified or send the wrong the key.
### Server
- Username storage (like a database), stores:
	- Username.
	- User public key.
	- Current IP (if not actively connected will be blank/null).
- Messages are cached/stored if user is not online. Once online messages will be sent.
	- Have a Time To Live (TTL) on messages so server doesn't use up a lot of disk usage.
	- Compress messages.
- Verify message timestamps.
- Could encrypt traffic between clients and server (as well as end-to-end encryption) to anonymise users from each other on the same network.
	  *(If they were packet sniffing you could see who receives the message you sent because it would be the same if only end-to-end encrypted)*
- Transfer messages between clients.
- Registration.
	- Initial registration is encrypted.
	- Provide challenge phrase when registering, to prove user has private key.
- Rate limit messages. (maybe like only 2 messages per second).
	- Helps to avoid DoS.
- Blocking.
	- If a client sends a message to a client that has blocked them then it will:
		- Immediately dump message.
		- Or just store the message on the server.
		- Could also send message failed to send. But that tells sender they are blocked which is not always good thing.
- Delivered and Read receipts.
	- Lets the sender know when a message was delivered.
	- Lets the sender know when the message was opened/read.
### Later/Future
#### General/Both
- Use Post-Quantum Cryptography (PQC) algorithms.
- File transfer support.
- Group chat system.
- Key reset (for compromised keys) - Requires old key.
- Communications Nodes (think like TOR network).
	- Helps to anonymise users further.
	- Multiple servers to handle communication.
	- Clients can pick preferences based of entry notes?
	- Messages are encrypted between nodes.
#### Client specific
- Support for private key on external drive.
- Notifications.
	- Have a discrete mode.
	- Don't show messages or sender in notification.
	- Sound notification.
- Multiple user system for clients. (Locally based).
	- Create/Register user.
	- Login (works with password encrypted private keys).
- Shoulder surfing attack mitigation.
	- Use camera to detect faces.
	- If multiple faces are detected tab out or close.
	- Have a setup thing to select camera.
	- On start/home screen show how many faces are detected.
	- Toggleable: allow it to be enabled and disabled.
	- Either close after triggering, show fake messages, or open a random app (pretends that the command prompt was a weird app artefact).
- Duress password (stored along side user data such as key pair and username).
	- Displays fake messages
	- (Optional) Deletes and overwrites messages and contacts stored on device. (wipes device)
	- Could have multiple duress passwords/codes.

## Interface (TUI)
- Alert for when message is received.
- Colour support for messages.
- Coloured UI.
- Recipient selector.
- Clear separation between sent and received messages.
- Load old messages.