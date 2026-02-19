using ShadowWire.Shared.Users;
using System.Collections.Concurrent;

namespace ShadowWire.Server.Network;

/// <summary>
/// Manages active <see cref="ClientSession"/> instances and allows lookup by fingerprint.
/// </summary>
/// <remarks>
/// Fingerprint lookup is only available for authenticated sessions.<br/>
/// Thread-safe.
/// </remarks>
public class SessionManager()
{
    private readonly ConcurrentDictionary<Guid, ClientSession> _sessionsById = new();
    private readonly ConcurrentDictionary<byte[], Guid> _sessionIdByFingerprint = new(new FingerprintComparer());


    /// <summary>
    /// Adds a session to the manager.
    /// </summary>
    /// <param name="session">The session to add.</param>
    /// <returns><see langword="true"/> if the session was added; <see langword="false"/> if a session with the same ID already exists.</returns>
    public bool TryAdd(ClientSession session)
        => _sessionsById.TryAdd(session.Id, session);

    /// <summary>
    /// Removes a session from the manager, along with its fingerprint mapping if present.
    /// </summary>
    /// <param name="session">The session to remove.</param>
    /// <returns><see langword="true"/> if the session existed and was removed; otherwise <see langword="false"/>.</returns>
    public bool TryRemove(ClientSession session)
    {
        bool result = _sessionsById.TryRemove(session.Id, out var _);

        if (session.ClientIdentity.HasValue)
            _sessionIdByFingerprint.TryRemove(session.ClientIdentity.Value.Fingerprint, out var _);

        return result;
    }

    /// <summary>
    /// Sets or updates the fingerprint for a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="fingerprint">The fingerprint to associate with the session.</param>
    /// <remarks>
    /// Removes the previous fingerprint mapping if one existed.
    /// </remarks>
    public void SetFingerprint(Guid sessionId, byte[] fingerprint)
    {
        if (!_sessionsById.TryGetValue(sessionId, out var session))
            return;

        // Cleanup old fingerprint (Edge case)
        var previousFingerprint = session.ClientIdentity?.Fingerprint;
        if (previousFingerprint != null)
            _sessionIdByFingerprint.TryRemove(previousFingerprint, out var _);

        _sessionIdByFingerprint[fingerprint] = sessionId;
    }

    /// <summary>
    /// Attempts to get a session by fingerprint.
    /// </summary>
    /// <param name="fingerprint">The fingerprint to lookup.</param>
    /// <param name="session">The matching session, if found.</param>
    /// <returns>
    /// <see langword="true"/> if a session was found; otherwise <see langword="false"/>.<br/>
    /// </returns>
    public bool TryGetSessionByFingerprint(byte[] fingerprint, out ClientSession session)
    {
        session = default;

        if (!_sessionIdByFingerprint.TryGetValue(fingerprint, out var guid))
            return false;

        // Sanity check: Fingerprint should always map to a tracked session
        if (!_sessionsById.TryGetValue(guid, out var _session))
            return false; // TODO: Implement logging - Warning, shouldn't occur (fingerprint maps to old/untracked session).

        session = _session;
        return true;
    }
}
