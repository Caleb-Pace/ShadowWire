namespace ShadowWire.Shared.Protocol;

public interface IMessageHandler<TContext, TRequest, TResponse>
    where TContext : class
    where TRequest : struct
    where TResponse : struct, IEncodable
{
    Task<TResponse> HandleAsync(TContext context, TRequest request);
}
