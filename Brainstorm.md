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
	- A new (random) key is generated for each message.
	- The key is then encrypted with the recipients public key.
### Messages
- Include a system similar to HMAC (Hashed Message Authentication Code) *(Authenticity)*
- Messages are signed with senders private key and the signature is appended to beginning of the message. *(Non-Repudiation)*
- **Need** a better way to ensure freshness.
	- TTL for messages.
	- Have some timestamp stored within them.
- Compress messages for efficiency. (Before or After encryption?)
- Includes timestamp. (Verify at server)
- Large messages are broken down to fit within limit. (Is this needed? You could just put hard limit in)
	- A slightly different protocol will be used for composite messages. So that the recipient knows to reconstruct them.
### Client
- Password to access user.
	- Encrypt private key.
	- Stores salted hash of password.
- Messages are stored locally.
	- Encrypted using AES.
		- Key derived from private key, password for private key, and optionally some hardware/environment derived key (this would lock this information to the device).
- Block other users.
	- Lets server know.
	- Can be done by identity/contact.
		- Not sending or storing messages sent from a blocked identity.
	- Or done by IP blocking. (Less reliable).
		- Same as above but also denying messages to the client from any IP that has used the blocked identity.
		- Downside is this requires storing user IP addresses on the server. Which is just persisting knowledge the server should not keep.
- MitM attack detection (Zero-trust server architecture).
	- Once an the fingerprint or alternative has been shared you can verify that the server is sending the correct key.
	- If the server does not send the correct key this can be detected and should alert the user with some sort of pop-up or warning that the server has modified or send the wrong the key.
	- **Note:** A MitM using a compromised server giving out different public keys only works on lookup requests which is the only time a client will ask the server for a public key. *(A user who already has a contact for the recipient will not be affect by this method).*
### Server
- Username storage (like a database), stores:
	- Username.
	- User public key.
	- Current IP (if not actively connected will be blank/null).
- Messages are cached/stored if user is not online. Once online messages will be sent.
	- Because there is no session the messages will have per message keys.
	- They will be stored as recipient, encrypted AES key, and the encrypted message contents.
		*(Same as normal messages).*
		- The AES key is encrypted with the recipient public key.
		- The message contents is encrypted with the AES key.
	- Have a Time To Live (TTL) on messages so server doesn't use up a lot of disk usage.
	- Compress messages.
- Verify message timestamps.
- Could encrypt traffic between clients and server (as well as end-to-end encryption) to anonymise users from each other on the same network.
	  *(If they were packet sniffing you could see who receives the message you sent because it would be the same if only end-to-end encrypted)*
- Transfer messages between clients.
- Registration.
	- Initial registration is encrypted.
	- Provide challenge phrase when registering, to prove user has private key. (Mitigates interception/registration hijack attacks).
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
- Transfer/Sharing of messages (or rather message history) between client devices.
- Multiple device support. Phone and Computer can receive the messages.
- Optional feature to disable signing for chats. (Improved speed but cannot prevent repudiation).

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

## Communication (Sequence Diagrams)
### Registration with Server
*(After WebSocket has been established).*

**Basic (MVP):**

| Client |           | Server | Action          | Description                |
| ------ | --------- | ------ | --------------- | -------------------------- |
| Client | --**-->** | Server | Identification  | Sends public key to server |
| Client | **<--**-- | Server | Acknowledgement |                            |

**Standard:**

| Client |          | Server | Action             | Description                                                                                                                                                       |
| ------ | -------- | ------ | ------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Client | --**->** | Server | Identification     | Sends public key to server.                                                                                                                                       |
| Client | **<-**-- | Server | Challenge Phrase   | Sends a unique and/or random message/phrase for user to sign. This is to verify the client has the private key.                                                   |
| Client | --**->** | Server | Challenge Response | Client signs the challenge phrase. The Server then verifies this with the public key.                                                                             |
| Client | **<-**-- | Server | Acknowledgement    | or some sort of rejection response (if the phrase was wrong). Possibly even with a punishment for trying to break the system like denying service to that device. |
- If the Identity provided is new to the server it will add it in it's users list.
- The WebSocket is then tagged and the client is now able to receive their messages.

### Lookup (Temporary)
*(Quality of life feature for the MVP. Will be removed or improved later so that server does not know the usernames).*
Format: `WhoIs?{username}` or `WhoIs?{fingerprint}`

| Client |          | Server | Action         | Description                                                            |
| ------ | -------- | ------ | -------------- | ---------------------------------------------------------------------- |
| Client | --**->** | Server | Lookup Request | Who is `{username}`/`{fingerprint}`                                    |
| Client | **<-**-- | Server | Response       | The public key (and fingerprint) for the associated user or *unknown*. |
- This is then used to complete a contact and allow for communication to that user.
- Future: hash username and follow

#### Zero Trust key (Future)
*(This is to verify that the server is not compromised and mitigates MitM attacks).*

This relies on the fingerprint (or derivative it) being shared outside of ShadowWire. This is then used to verify that the server has sent the correct public key and has not been substituted/replaced/tampered with.

### Message

| Clients |          | Server | Action                    | Description                                                                                                   |
| ------- | -------- | ------ | ------------------------- | ---------------------------------------------------------------------------------------------------- |
| A       | --**->** | Server | Send message              | A sends encry                                                                                                 |
| B       | **<-**-- | Server |  B receives A's message and decrypts it.<br>B also securely stores message locally for later viewing. ly  ly  ly  ly  ly  ly  ly  ly  ly  |
| B       | --**->** | Server | Acknowledgement                                                                                                                           |
#### Read receipts
- Server should acknowledge that message has been either delivered to user (online recipient, triggered by acknowledgement) or stored (offline recipient).
- Read/Seen receipt goes as follows:

| Clients |          | Server | Action       | Description                                          |
| ------- | -------- | ------ | ------------ | ---------------------------------------------------- |
| B       | --**->** | Server | Seen message | B opens/views the message from A.                    |
| A       | **<-**-- | Server | (Relayed)    | The server notifies A that B has seen their message. |

### Username exchange (Later)
*(Functionally similar to the messaging system but set on a different protocol).*

| Clients |          | Server | Action            | Description                   |
| ------- | -------- | ------ | ----------------- | ----------------------------- |
| A       | --**->** | Server | Username Request  | A requests B's username.      |
| B       | **<-**-- | Server | (Relayed)         |                               |
| B       | --**->** | Server | Username Response | B responds with its username. |
| A       | **<-**-- | Server | (Relayed)         |                               |

### Update (Future)
*(Occurs during registration).*

*(The public key is hard-coded and only the developer has the key to sign updates).*
This system is to automatically update the clients.

**Update:**

| Client |          | Server | Action             | Description                                                                                                                                                       |
| ------ | -------- | ------ | ------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Client | --**->** | Server | Identification     | Sends public key and current version to server.<br>Server then checks if there is an update.                                                                      |
| Client | **<-**-- | Server | Update request     | Server tells client their is an update and sends update details.                                                                                                  |
| Client | --**->** | Server | Acknowledgement    | Client is ready to proceed.<br>Client maybe denied if updated is forced and they decline.                                                                         |
| Client | **<-**-- | Server | Challenge Phrase   | Sends a unique and/or random message/phrase for user to sign. This is to verify the client has the private key.                                                   |
| Client | --**->** | Server | Challenge Response | Client signs the challenge phrase. The Server then verifies this with the public key.                                                                             |
| Client | **<-**-- | Server | Acknowledgement    | or some sort of rejection response (if the phrase was wrong). Possibly even with a punishment for trying to break the system like denying service to that device. |
