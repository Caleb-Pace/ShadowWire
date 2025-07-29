## 1. Rust as the Language
I chose Rust to gain more experience with the language. Rust's strong focus on memory safety and security makes it well-suited for a secure messaging app. From previous experience, I know Rust produces small binaries, which I find appealing — I like my programs to be efficient.
## 2. Architecture: Centralized Server (vs. P2P)
I went with a centralized server architecture because it keeps things simple for the end user — they don't need to know each other's IP addresses or mess with network configuration. This setup also adds a layer of anonymity, since users don't connect to each other directly.

Peer-to-peer (P2P) is still a potential future alternative to the server-based model.
## 3. Interface: CLI First → TUI Later
Getting it working is the priority at this stage, so I'm starting with a simple command-line interface.
I'll be adding a TUI (text-based user interface) afterward.

I decided against a GUI because it's unnecessarily complex for what I need right now. A TUI provides the same kind of interactivity but is faster to implement, and as a bonus, it gives the app a special feel that enhances the user experience.
