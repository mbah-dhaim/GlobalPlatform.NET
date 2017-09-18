namespace GlobalPlatform.NET.Commands.Interfaces
{
    public interface IApduBuilder
    {
        CommandApdu AsApdu();

        byte[] AsBytes();
    }
}
