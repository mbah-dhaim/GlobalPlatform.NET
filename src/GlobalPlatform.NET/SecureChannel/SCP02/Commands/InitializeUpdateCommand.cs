using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SecureChannel.Cryptography;
using System;

namespace GlobalPlatform.NET.SecureChannel.SCP02.Commands
{
    public interface IInitializeUpdateKeyVersionPicker
    {
        IInitializeUpdateHostChallengePicker WithEngineeringKeys();

        IInitializeUpdateHostChallengePicker WithKeyVersion(byte version);
    }

    public interface IInitializeUpdateHostChallengePicker
    {
        IApduBuilder WithHostChallenge(byte[] hostChallenge);

        IApduBuilder WithHostChallenge(out byte[] hostChallenge);
    }

    /// <summary>
    /// The INITIALIZE UPDATE command is used, during explicit initiation of a Secure Channel, to
    /// transmit card and session data between the card and the host. This command initiates the
    /// initiation of a Secure Channel Session.
    /// <para>
    /// At any time during a current Secure Channel, the INITIALIZE UPDATE command can be issued to
    /// the card in order to initiate a new Secure Channel Session.
    /// </para>
    /// </summary>
    public class InitializeUpdateCommand : CommandBase<InitializeUpdateCommand, IInitializeUpdateKeyVersionPicker>,
        IInitializeUpdateKeyVersionPicker,
        IInitializeUpdateHostChallengePicker
    {
        private byte[] hostChallenge;

        public IInitializeUpdateHostChallengePicker WithEngineeringKeys()
        {
            this.P1 = 0x00;

            return this;
        }

        public IInitializeUpdateHostChallengePicker WithKeyVersion(byte version)
        {
            if (version < 1 || version > 0x7F)
            {
                throw new ArgumentException("Version number must be between 1-7F (inclusive).", nameof(version));
            }

            this.P1 = version;

            return this;
        }

        public IApduBuilder WithHostChallenge(byte[] hostChallenge)
        {
            Ensure.HasCount(hostChallenge, nameof(hostChallenge), 8);

            this.hostChallenge = hostChallenge;

            return this;
        }

        public IApduBuilder WithHostChallenge(out byte[] hostChallenge)
        {
            this.hostChallenge = hostChallenge = SecureRandom.GetBytes(8);

            return this;
        }

        public override Apdu AsApdu() => Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.InitializeUpdate, this.P1, this.P2, this.hostChallenge, 0x00);
    }
}
