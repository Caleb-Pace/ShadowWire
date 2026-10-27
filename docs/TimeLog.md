Project setup: 20m
Brainstorm: 3h 1m
    - Comms Sequence Diagrams: 2h 2m
Planning:
    - MVP: 15m
README updates: 2h 47m
docs/:
    - Restructure: 5m

Websocket library research: 9m
Project structure planning: 27m
Formatting: 1m
Stack (Spans) vs Heap usage research: 16m
Protocol structure research: 20m

Shared:
- Moving & setup: 8m
- Contacts: 1h 7m
    - Planning: 23m
    - Persistence: 35m
        - Bson issue: 41m
        - Fingerprint lookup issue: 6m
    - Contact Packaging: 23m
    - Codec improvement: 53m
    - Contact Codec consolidation: 33m
    - Manager updates: 32m
    - Fingerprint: 48m
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
    - Planning: 2h 30m
    - MessageKind: 1h 17m
    - MessageHandlerRegistry: 1h 3m
    - IMessageHandler: 15m
    - IEncodable: 8m
    - IProtocolMessage: 6m
    - IConversationStorable: 9m
    - Messages/
      - Acknowledge: 1m
      - AuthenticationRequest: 42m
      - AuthenticationSuccess: 4m
      - AuthenticationFailure: 1m
      - BadRequest: 7m
      - ~~ContactMessageBase~~: 6m
      - LookupRequest: 11m
      - LookupResponse: 8m
      - Message: 48m
      - Unauthenticated: 1m
- BinaryEncoding/
    - Planning: 1h 21m
    - SpanReader: 1h 8m
    - SpanWriter: 2h 56m
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
    - Planning: 57m
    - ServerMessageHandlerRegistry: 28m
    - AuthenticationHandler: 1h 3m
    - MessageHandler: 17m
- User registry (ContactManager): 7m

Client:
- Testing: 6m
- Planning: 3h 3m
- Test setup: 3m
- WebSocket setup: 15m
- Version setup: 1m
- Register with Server: 20m
- Security refactor re-implementation: 10m
- Network/
    - Connection: 35m
- Security/
    - Planning: 54m
    - Algorithms/
        - Asymmetric/
            - IAsymmetricAlgorithm: 48m
            - RsaAlgorithm: 38m
        - Symmetric/
            - ISymmetricAlgorithm: 4m
            - Aes256GcmAlgorithm: 47m
        - Hashing/
            - IHashingAlgorithm: 18m
            - Sha256Hasher: 6m
    - ~~Cryptography~~: 48m
    - CryptoAlgorithmSuite: 5m
        - Research: 17m
    - DerKeyPair: 14m
    - KeyResolver: 22m
    - IKeyStorage: 2m
    - BasicKeyStorage: 6m
- Services/
    - Planning: 21m
    - IMessageService: 2m
    - AuthenticationService: 5m
- Storage/
    - Planning: 1h 45m
    - IConversationRepository: 12m
    - StoredEntry: 7m
    - SqliteConversationRepository: 3m
- Handlers/
    - ClientMessageHandlerRegistry: 6m
    - MessageHandler: 5m
- ClientContext: 8m
- EventBus: 7m
- TUI/
    - Planning: 1h 24m
    - Tui: 38m
    - KeyReader: 16m
    - Components/
        - BaseComponent: 15m
        - TextInput: 1h 33m
        - MultilineTextInput: 45m
        - Text: 24m
    - Events/
        - UiEvent: 6m
        - IEventProducer: 1m
        - IEventConsumer: 1m
    - Scenes/
        - IScene: 10m
        - Home: 24m
        - Chat: 
