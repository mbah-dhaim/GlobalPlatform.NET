using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
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
            this.hostCryptogram = hostCryptogram;

            return this;
        }

        public override Apdu AsApdu() => Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.ExternalAuthenticate, this.P1, this.P2, this.hostCryptogram, 0x02);
    }
}
