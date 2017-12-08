using System.Collections.Generic;
using Iso7816;

namespace GlobalPlatform.NET.SecureChannel.Interfaces
{
    public interface ISecureChannelSession<out TSecureChannelSession> : ISecureChannelSessionEstablisher<TSecureChannelSession>
        where TSecureChannelSession : ISecureChannelSession<TSecureChannelSession>
    {
        CommandApdu SecureApdu(CommandApdu apdu);

        IEnumerable<CommandApdu> SecureApdu(params CommandApdu[] apdus);

        IEnumerable<CommandApdu> SecureApdu(IEnumerable<CommandApdu> apdus);
    }

    public interface ISecureChannelSessionEstablisher<out TSecureChannelSession>
        where TSecureChannelSession : ISecureChannelSession<TSecureChannelSession>
    {
        TSecureChannelSession Establish();
    }
}
