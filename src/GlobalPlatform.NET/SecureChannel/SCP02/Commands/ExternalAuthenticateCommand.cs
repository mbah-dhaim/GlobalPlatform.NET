using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SecureChannel.SCP02.Reference;

namespace GlobalPlatform.NET.SecureChannel.SCP02.Commands
{
    public interface ISecurityLevelPicker
    {
        IHostCryptogramPicker WithSecurityLevel(SecurityLevel securityLevel);
    }

    public interface IHostCryptogramPicker
    {
        IApduBuilder UsingHostCryptogram(byte[] hostCryptogram);
    }

    /// <summary>
    /// The EXTERNAL AUTHENTICATE command is used by the card, during explicit initiation of a Secure
    /// Channel, to authenticate the host and to determine the level of security required for all
    /// subsequent commands.
    /// <para> A successful execution of the INITIALIZE UPDATE command shall precede this command. </para>
    /// </summary>
    public class ExternalAuthenticateCommand : CommandBase<ExternalAuthenticateCommand, ISecurityLevelPicker>,
        ISecurityLevelPicker,
        IHostCryptogramPicker
    {
        private byte[] hostCryptogram;

        public IHostCryptogramPicker WithSecurityLevel(SecurityLevel securityLevel)
        {
            this.P1 = (byte)securityLevel;

            return this;
        }

        public IApduBuilder UsingHostCryptogram(byte[] hostCryptogram)
        {
            Ensure.HasCount(hostCryptogram, nameof(hostCryptogram), 8);

            this.hostCryptogram = hostCryptogram;

            return this;
        }

        public override Apdu AsApdu() => Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.ExternalAuthenticate, this.P1, this.P2, this.hostCryptogram, 0x02);
    }
}
