namespace ShadowWire.Shared.Protocol;

public interface IMessageHandler<TContext, TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TContext context, TRequest request);
}
