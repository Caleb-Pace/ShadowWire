using ShadowWire.Shared.Users;

namespace ShadowWire.Server.Network;

public readonly struct ClientSessionConfig(Func<byte[], byte[], Task> routeMessageAsync, ContactManager userRegistry)
{
    public readonly Func<byte[], byte[], Task> routeMessageAsync = routeMessageAsync;
    public readonly ContactManager userRegistry = userRegistry;
}
