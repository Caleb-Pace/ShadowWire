namespace ShadowWire.Shared;

public readonly struct Version : IEquatable<Version>, IComparable<Version>
{
    private readonly ulong _value;
    public ulong Packed => _value;
    public static int Size => sizeof(ulong);

    public ushort Major => (ushort)(_value >> 48);
    public ushort Minor => (ushort)(_value >> 32);
    public ushort Patch => (ushort)(_value >> 16);
    public ushort Build => (ushort)_value;


    public Version(ushort major, ushort minor, ushort patch, ushort build)
    {
        _value = ((ulong)major << 48)
               | ((ulong)minor << 32)
               | ((ulong)patch << 16)
               | build;
    }

    public Version(ulong packedVersion)
        => _value = packedVersion;


    public bool Equals(Version other)
        => this._value == other._value;

    public int CompareTo(Version other)
        => _value.CompareTo(other._value);

    public override bool Equals(object? obj)
        => obj is Version other && Equals(other);
    
    public override int GetHashCode()
        => _value.GetHashCode();

    public override string ToString()
        => $"{Major}.{Minor}.{Patch}.{Build}";


    public static bool operator ==(Version left, Version right)
        => (left._value == right._value);
    public static bool operator !=(Version left, Version right)
        => (left._value != right._value);
    public static bool operator >(Version left, Version right)
        => (left._value > right._value);
    public static bool operator <(Version left, Version right)
        => (left._value < right._value);
    public static bool operator >=(Version left, Version right)
        => (left._value >= right._value);
    public static bool operator <=(Version left, Version right)
        => (left._value <= right._value);
}
