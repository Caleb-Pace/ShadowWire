Project setup: 20m
Brainstorm: 3h 1m
- Comms Sequence Diagrams: 2h 2m
Planning:
- MVP: 14m

Websocket library research: 9m
Project structure planning: 27m
Formatting: 1m
Stack (Spans) vs Heap usage research: 16m
Protocol structure research: 20m

Shared:
- Moving & setup: 8m
- Contacts: 49m
    - Planning: 5m
    - Persistence: 35m
        - Bson issue: 41m
        - Fingerprint lookup issue: 6m
    - Contact Packaging: 20m
    - FingerprintComparer: 58m
    - Codec improvement: 43m
    - Manager updates: 23m
- UsernameManager: 2h 2m
- Protocols/ (Removed): 10m
    - Planning: 47m
        - Protcol List: 45m
    - IProtocol: 57m
    - ProtocolId: 4m
    - ProtocolHeader: 1h 14m
    - Protocols
        - Error: 57m
        - Authentication: 23m
    - Protocol Pattern correction: 10m
- Protocol/
    - Planning: 1h 19m
    - MessageKind: 1h 9m
    - MessageHandlerRegistry: 1h 3m
    - IMessageHandler: 15m
    - IMessageHandlerAdapter: 13m
    - IEncodable: 8m
    - Messages/
      - Acknowledge: 1m
      - BadRequest: 4m
      - AuthenticationRequest: 20m
      - AuthenticationSuccess: 4m
      - AuthenticationFailure: 1m
      - Message: 23m
- BinaryEncoding/
    - Planning: 40m
    - SpanReader: 38m
    - SpanWriter: 22m
    - ByteArrayUtils: 36m

Server:
- WebSocket (echo) server setup: 45m
- Identity retrieval test: 12m
- Network/
    - Planning: 55m
    - RelayServer: 1h 23m
    - ClientSession: 34m
    - ClientSessionConfig: 13m
    - MessageRouter: 5m
    - SessionManager: 58m
- Handlers/
    - Planning: 53m
    - ServerMessageHandlerRegistry: 25m
    - AuthenticationHandler: 49m
    - MessageHandler: 16m
- User registry: 7m

Client:
- WebSocket setup: 15m
- Connection: 35m
- Cryptography: 33m
    - Research: 12m
- Register with Server: 15m
- Services/
    - Planning: 16m
- Handlers/
    - ClientMessageHandlerRegistry: 6m
