using GlobalPlatform.NET.SecureChannel.SCP02;

namespace GlobalPlatform.NET.SecureChannel
{
    public interface ISecureChannelProtocolPicker
    {
        IScp02SessionBuilder UsingScp02();
    }

    public class SecureChannelSession : ISecureChannelProtocolPicker
    {
        public static ISecureChannelProtocolPicker Build => new SecureChannelSession();

        public IScp02SessionBuilder UsingScp02() => new SCP02.SecureChannelSession();
    }
}
