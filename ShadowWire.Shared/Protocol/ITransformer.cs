namespace ShadowWire.Shared.Protocol;

public interface ITransformer<T>
{
    /// <summary>
    /// Transforms an object into a binary.
    /// </summary>
    byte[] Encode(T obj);

    /// <summary>
    /// Transforms binary data back into an object.
    /// </summary>
    T Decode(byte[] data);
}