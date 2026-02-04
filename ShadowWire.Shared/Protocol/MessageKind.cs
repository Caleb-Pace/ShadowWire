namespace ShadowWire.Shared.Protocol;

public enum MessageKind : byte
{
    Acknowledge = 0,
    Error = 1,
    Identification = 2, // For connecting to the server
    Lookup = 3,         // Find user
    Message = 4,
    OfflineMessagesRequest = 5, // Retrieve stored messages


    /* <Later protocols>
    Lookup           = , // Find user // TODO: replace legacy system
    UsernameExchange = , // 

    CompositeMessage = , // 
    File             = , // 

    ClientUpdate      = , // 
    RateLimitExceeded = , // 
    E2EIntegrityCheck = , // 
    
    HeartBeat      = , // 
    Status         = , // 
    IndicateTyping = , // 

    ServiceDenied      = , // 
    ServerAnnouncement = , // 
    */
}
