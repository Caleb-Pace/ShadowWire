## 1. Rust as the Language
I chose Rust to gain more experience with the language. Rust's strong focus on memory safety and security makes it well-suited for a secure messaging app. From previous experience, I know Rust produces small binaries, which I find appealing — I like my programs to be efficient.
## 2. Architecture: Centralised Server (vs. P2P)
I went with a centralized server architecture because it keeps things simple for the end user — they don't need to know each other's IP addresses or mess with network configuration. This setup also adds a layer of anonymity, since users don't connect to each other directly.

Peer-to-peer (P2P) is still a potential future alternative to the server-based model.
## 3. Interface: CLI First → TUI Later
Getting it working is the priority at this stage, so I'm starting with a simple command-line interface.
I'll be adding a TUI (text-based user interface) afterward.

I decided against a GUI because it's unnecessarily complex for what I need right now. A TUI provides the same kind of interactivity but is faster to implement, and as a bonus, it gives the app a special feel that enhances the user experience.
### 4. Message security: Sign-then-Encrypt
I chose to use a **Sign-then-Encrypt** structure for securing messages.

Encrypting the message provides confidentiality, while signing it proves authenticity and ensures non-repudiation — meaning the sender cannot deny having sent the message.

Sign-then-Encrypt hides the signature, which adds a layer of anonymity by protecting the sender's identity (except from the intended recipient). It also imposes less trust on the server: since the signature is encrypted, the server can’t inspect or tamper with it. This makes the architecture more resistant to malicious or compromised servers.
### 5. RSA for asymmetric encryption
I chose **RSA-OAEP** for encrypting messages and **RSA-PSS** for signing them. This simplifies the design by allowing the same key pair to be used for both encryption and signing.

I used padded RSA algorithms because their padding schemes provide stronger security guarantees compared to plain RSA.

RSA is widely used and battle-tested in practice, which adds passive security since it has been scrutinized extensively by cryptographers and attackers alike. More importantly, RSA supports both signing and encryption, making it a natural fit for this project.
### 6. Protocol: WebSockets
WebSockets were chosen because they are fast, lightweight, and efficient — ideal for real-time messaging.
They allow full-duplex communication over a single connection, which simplifies message handling and reduces latency compared to polling or traditional HTTP.