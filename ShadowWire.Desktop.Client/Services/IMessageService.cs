using ShadowWire.Shared.Protocol;

namespace ShadowWire.Desktop.Client.Services;

public interface IMessageService<TRequest>
    where TRequest : struct, IEncodable
{
    Task SendAsync(TRequest request);
}
