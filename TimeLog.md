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
- Contacts: 1h 6m
    - Planning: 20m
    - Persistence: 35m
        - Bson issue: 41m
        - Fingerprint lookup issue: 6m
    - Contact Packaging: 23m
    - Codec improvement: 53m
    - Contact Codec consolidation: 33m
    - Manager updates: 24m
    - Fingerprint: 46m
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
    - Planning: 2h 12m
    - MessageKind: 1h 17m
    - MessageHandlerRegistry: 1h 3m
    - IMessageHandler: 15m
    - IEncodable: 8m
    - IProtocolMessage: 6m
    - Messages/
      - Acknowledge: 1m
      - AuthenticationRequest: 42m
      - AuthenticationSuccess: 4m
      - AuthenticationFailure: 1m
      - BadRequest: 7m
      - ~~ContactMessageBase~~: 6m
      - LookupRequest: 11m
      - LookupResponse: 8m
      - Message: 30m
      - Unauthenticated: 1m
- BinaryEncoding/
    - Planning: 1h 21m
    - SpanReader: 1h 2m
    - SpanWriter: 2h 46m
    - ByteArrayUtils: 36m
- Version: 45m
    - Research: 15m

Server:
- Testing: 6m
- WebSocket (echo) server setup: 45m
- Identity retrieval test: 12m
- Network/
    - Planning: 1h 1m
    - RelayServer: 2h 24m
    - ClientSession: 34m
    - ClientSessionConfig: 15m
    - MessageRouter: 13m
    - SessionManager: 1h 15m
- Handlers/
    - Planning: 53m
    - ServerMessageHandlerRegistry: 28m
    - AuthenticationHandler: 1h 3m
    - MessageHandler: 17m
- User registry (ContactManager): 7m

Client:
- Testing: 6m
- Planning: 2h 3m
- Test setup: 3m
- WebSocket setup: 15m
- Version setup: 1m
- Connection: 35m
- Security/
    - Planning: 46m
    - Algorithms/
        - Asymmetric/
            - IAsymmetricAlgorithm: 43m
        - Symmetric/
            - ISymmetricAlgorithm: 4m
        - Hashing/
            - IHashingAlgorithm: 14m
    - Cryptography: 41m
        - Research: 12m
    - CryptoAlgorithms: 2m
    - DerKeyPair: 14m
    - KeyResolver: 22m
    - IKeyStorage: 2m
    - BasicKeyStorage: 6m
- Register with Server: 20m
- Services/
    - Planning: 21m
    - IMessageService: 2m
    - AuthenticationService: 5m
- Handlers/
    - ClientMessageHandlerRegistry: 6m
- ClientContext: 8m
- EventBus: 7m
