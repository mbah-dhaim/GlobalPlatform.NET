using System.Collections.Generic;

namespace GlobalPlatform.NET.SecureChannel.Interfaces
{
    public interface ISecureChannelSession<out TSecureChannelSession> : ISecureChannelSessionEstablisher<TSecureChannelSession>
        where TSecureChannelSession : ISecureChannelSession<TSecureChannelSession>
    {
        Apdu SecureApdu(Apdu apdu);

        IEnumerable<Apdu> SecureApdu(params Apdu[] apdus);

        IEnumerable<Apdu> SecureApdu(IEnumerable<Apdu> apdus);
    }

    public interface ISecureChannelSessionEstablisher<out TSecureChannelSession>
        where TSecureChannelSession : ISecureChannelSession<TSecureChannelSession>
    {
        TSecureChannelSession Establish();
    }
}
