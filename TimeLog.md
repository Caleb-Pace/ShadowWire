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
    - Planning: 20m
    - Persistence: 35m
        - Bson issue: 41m
        - Fingerprint lookup issue: 6m
    - Contact Packaging: 23m
    - Codec improvement: 53m
    - Manager updates: 24m
    - Fingerprint: 41m
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
    - Planning: 1h 38m
    - MessageKind: 1h 13m
    - MessageHandlerRegistry: 1h 3m
    - IMessageHandler: 15m
    - IEncodable: 8m
    - Messages/
      - Acknowledge: 1m
      - AuthenticationRequest: 24m
      - AuthenticationSuccess: 4m
      - AuthenticationFailure: 1m
      - BadRequest: 5m
      - ContactMessageBase: 6m
      - LookupRequest: 10m
      - LookupResponse: 3m
      - Message: 24m
- BinaryEncoding/
    - Planning: 1h 21m
    - SpanReader: 54m
    - SpanWriter: 2h 4m
    - ByteArrayUtils: 36m
- Version: 41m

Server:
- Testing: 3m
- WebSocket (echo) server setup: 45m
- Identity retrieval test: 12m
- Network/
    - Planning: 55m
    - RelayServer: 2h 19m
    - ClientSession: 34m
    - ClientSessionConfig: 14m
    - MessageRouter: 5m
    - SessionManager: 1h 15m
- Handlers/
    - Planning: 53m
    - ServerMessageHandlerRegistry: 28m
    - AuthenticationHandler: 1h 3m
    - MessageHandler: 16m
- User registry (ContactManager): 7m

Client:
- Testing: 1m
- Test setup: 3m
- WebSocket setup: 15m
- Connection: 35m
- Cryptography: 33m
    - Research: 12m
- Register with Server: 20m
- Services/
    - Planning: 21m
    - IMessageService: 2m
    - AuthenticationService: 3m
- Handlers/
    - ClientMessageHandlerRegistry: 6m
