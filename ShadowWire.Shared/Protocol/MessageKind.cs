namespace ShadowWire.Shared.Protocol;

public enum MessageKind : byte
{
    //~/ System
    Acknowledge = 0,
    Error = 1,
    //ServiceDenied      = 2, // <Later>
    //ServerAnnouncement = 3, // <Later>
    //UpdateAvailable    = 4, // <Later>
    //RateLimitExceeded    = 5, // <Later>
    //E2EIntegrityRequest  = 6, // <Later>
    //E2EIntegrityResponse = 7, // <Later>

    //~/ Authentication
    IdentifyRequest = 10, // For connecting to the server
    //IdentificationChallengePhrase   = 11, // <Later>
    //IdentificationChallengeResponse = 12, // <Later>

    //~/ User lookup
    LookupRequest  = 13,
    LookupResponse = 14,

    //~/ Username Exchange
    //UsernameRequest  = 15, // <Later>
    //UsernameResponse = 16, // <Later>

    //~/ Messages
    Message = 20,
    //CompositeMessage = 21, // <Later>
    //File = 22, // <Later>
    OfflineMessagesRequest  = 23,  // Retrieve stored messages
    OfflineMessagesResponse = 24,

    //~/ Status
    //HeartBeat      = 30, // <Later>
    //Status         = 31, // <Later>
    //IndicateTyping = 32, // <Later>
}
