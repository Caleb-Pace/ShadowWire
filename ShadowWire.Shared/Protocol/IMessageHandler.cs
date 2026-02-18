namespace ShadowWire.Shared.Protocol;

public interface IMessageHandler<TContext, TRequest>
    where TContext : class
    where TRequest : struct
{
    public Task<IEncodable> HandleAsync(TContext context, TRequest request);
}
