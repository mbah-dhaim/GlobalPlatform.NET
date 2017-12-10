using System.Collections.Generic;
using Iso7816;

namespace GlobalPlatform.NET.Interfaces
{
    public interface ISecureChannelSession<TSecureChannelSession> : ISecureChannelSessionEstablisher<TSecureChannelSession>
        where TSecureChannelSession : ISecureChannelSession<TSecureChannelSession>
    {
        CommandApdu SecureApdu(CommandApdu apdu);

        IEnumerable<CommandApdu> SecureApdu(params CommandApdu[] apdus);

        IEnumerable<CommandApdu> SecureApdu(IEnumerable<CommandApdu> apdus);
    }

    public interface ISecureChannelSessionEstablisher<TSecureChannelSession>
        where TSecureChannelSession : ISecureChannelSession<TSecureChannelSession>
    {
        /// <summary>
        /// Establishes the secure channel session, after which it can secure commands to the card.
        /// </summary>
        /// <returns></returns>
        TSecureChannelSession Establish();

        /// <summary>
        /// Attempts to establish the secure channel session, after which it can secure commands to the card.
        /// </summary>
        /// <returns></returns>
        bool TryEstablish(out TSecureChannelSession secureChannelSession);
    }
}
