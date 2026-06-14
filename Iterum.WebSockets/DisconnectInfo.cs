namespace Iterum.Network;

public enum DisconnectExceptionKind : byte
{
    None       = 0,
    SocketDrop = 1,
    IoError    = 2,
    Other      = 3,
}

public readonly struct DisconnectInfo
{
    public readonly int CloseCode;
    public readonly DisconnectExceptionKind ExceptionKind;

    public DisconnectInfo(int closeCode, DisconnectExceptionKind exceptionKind)
    {
        CloseCode = closeCode;
        ExceptionKind = exceptionKind;
    }
}
