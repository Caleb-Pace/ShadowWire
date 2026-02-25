namespace ShadowWire.Shared.Protocol;

public enum MessageKind : byte
{
    //~/ System
    Acknowledge = 0,
    //NegativeAcknowledge = 1, // <Later>
    //ServerAnnouncement  = 2, // <Later>
    BadRequest = 3,
    //ServiceDenied        = 4, // <Later>
    //RateLimitExceeded    = 5, // <Later>
    //UpdateAvailable      = 6, // <Later>
    //E2EIntegrityRequest  = 8, // <Later>
    //E2EIntegrityResponse = 9, // <Later>
    //ErrorReport          = 10, // <Later>

    //~/ Authentication
    AuthenticationRequest = 20, // For connecting to the server
    //AuthenticationChallenge = 21, // <Later>
    //AuthenticationChallengeResponse = 22, // <Later>
    AuthenticationSuccess = 23,
    AuthenticationFailure = 24,

    //~/ User lookup
    LookupRequest = 30,
    LookupResponse = 31,

    //~/ Username Exchange
    //UsernameRequest  = 32, // <Later>
    //UsernameResponse = 33, // <Later>

    //~/ Messages
    Message = 40,
    //CompositeMessage = 41, // <Later>
    OfflineMessagesRequest  = 42, // Retrieve stored messages
    OfflineMessagesResponse = 43,
    //File      = 50, // <Later>
    //FileBlock = 51, // <Later>

    //~/ Status
    //HeartBeat      = 60, // <Later>
    //Status         = 61, // <Later>
    //IndicateTyping = 62, // <Later>
}
