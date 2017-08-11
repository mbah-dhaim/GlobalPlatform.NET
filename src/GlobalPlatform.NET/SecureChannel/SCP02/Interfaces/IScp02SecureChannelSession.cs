using GlobalPlatform.NET.SecureChannel.Interfaces;
using GlobalPlatform.NET.SecureChannel.SCP02.Reference;

namespace GlobalPlatform.NET.SecureChannel.SCP02.Interfaces
{
    public interface IScp02SecureChannelSession : ISecureChannelSession<IScp02SecureChannelSession>
    {
        /// <summary>
        /// Returns true if authentication has successfully taken place. 
        /// </summary>
        bool IsEstablished { get; }

        /// <summary>
        /// The security level of the established session. 
        /// </summary>
        SecurityLevel SecurityLevel { get; }

        /// <summary>
        /// The latest C-MAC used when sending messages to the card. 
        /// </summary>
        byte[] CMac { get; }

        /// <summary>
        /// The session key used to generate C-MACs. 
        /// </summary>
        byte[] CMacKey { get; }

        /// <summary>
        /// The latest R-MAC used when receiving messages from the card. 
        /// </summary>
        byte[] RMac { get; }

        /// <summary>
        /// The session key used to generate R-MACs. 
        /// </summary>
        byte[] RMacKey { get; }

        /// <summary>
        /// The session key used to encrypt APDU payloads (S-ENC). 
        /// </summary>
        byte[] EncryptionKey { get; }

        /// <summary>
        /// The session key used to encrypt sensitive data (DEK), e.g. keys. 
        /// </summary>
        byte[] DataEncryptionKey { get; }

        /// <summary>
        /// The host cryptogram that should be sent to the card using a subsequent EXTERNAL
        /// AUTHENTICATE command
        /// </summary>
        /// .
        byte[] HostCryptogram { get; }

        /// <summary>
        /// Returns the EXTERNAL AUTHENTICATE command needed to complete explicit authentication. 
        /// </summary>
        Apdu ExternalAuthenticateCommand { get; }
    }
}
