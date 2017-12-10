using GlobalPlatform.NET.Interfaces;
using GlobalPlatform.NET.SCP02.Reference;

namespace GlobalPlatform.NET.SCP02.Interfaces
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
        /// The sequence counter contained in the INITIALIZE UPDATE response that was used to seed
        /// this secure channel session.
        /// </summary>
        byte[] SequenceCounter { get; }

        /// <summary>
        /// The card challenge contained in the INITIALIZE UPDATE response that was used to seed this
        /// secure channel session.
        /// </summary>
        byte[] CardChallenge { get; }

        /// <summary>
        /// The host challenge contained in the INITIALIZE UPDATE command that was issued before
        /// establishing this secure channel session.
        /// </summary>
        byte[] HostChallenge { get; }

        /// <summary>
        /// The card cryptogram contained in the INITIALIZE UPDATE response that was used to seed
        /// this secure channel session.
        /// </summary>
        byte[] CardCryptogram { get; }

        /// <summary>
        /// The host cryptogram that should be sent to the card using a subsequent EXTERNAL
        /// AUTHENTICATE command.
        /// </summary>
        byte[] HostCryptogram { get; }
    }
}
