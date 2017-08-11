using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SecureChannel.Interfaces;
using GlobalPlatform.NET.SecureChannel.SCP02.Cryptography;
using GlobalPlatform.NET.SecureChannel.SCP02.Interfaces;
using GlobalPlatform.NET.SecureChannel.SCP02.Reference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.SecureChannel.SCP02
{
    public interface IScp02SessionBuilder
    {
        IScp02SecurityLevelPicker UsingOption15();
    }

    public interface IScp02SecurityLevelPicker
    {
        IScp02EncryptionKeyPicker UsingSecurityLevel(SecurityLevel securityLevel);
    }

    public interface IScp02EncryptionKeyPicker
    {
        IScp02MacKeyPicker UsingEncryptionKey(byte[] encryptionKey);
    }

    public interface IScp02MacKeyPicker
    {
        IScp02DataEncryptionKeyPicker UsingMacKey(byte[] macKey);
    }

    public interface IScp02DataEncryptionKeyPicker
    {
        IScp02HostChallengePicker UsingDataEncryptionKey(byte[] dataEncryptionKey);
    }

    public interface IScp02HostChallengePicker
    {
        IScp02InitializeUpdateResponsePicker UsingHostChallenge(byte[] hostChallenge);
    }

    public interface IScp02InitializeUpdateResponsePicker
    {
        ISecureChannelSessionEstablisher<IScp02SecureChannelSession> UsingInitializeUpdateResponse(byte[] initializeUpdateResponse);
    }

    public class SecureChannelSession : IScp02SecureChannelSession,
        IScp02SessionBuilder,
        IScp02SecurityLevelPicker,
        IScp02EncryptionKeyPicker,
        IScp02MacKeyPicker,
        IScp02DataEncryptionKeyPicker,
        IScp02HostChallengePicker,
        IScp02InitializeUpdateResponsePicker
    {
        private byte[] encryptionKey;
        private byte[] macKey;
        private byte[] dataEncryptionKey;
        private byte[] hostChallenge;
        private byte[] initializeUpdateResponse;

        private IEnumerable<byte> SequenceCounter => this.initializeUpdateResponse.Skip(12).Take(2);

        private IEnumerable<byte> CardChallenge => this.initializeUpdateResponse.Skip(14).Take(6);

        private IEnumerable<byte> CardCryptogram => this.initializeUpdateResponse.Skip(20).Take(8);

        public bool IsEstablished { get; private set; }

        public SecurityLevel SecurityLevel { get; private set; }

        public byte[] CMac { get; private set; }

        public byte[] CMacKey { get; private set; }

        public byte[] RMac { get; private set; }

        public byte[] RMacKey { get; private set; }

        public byte[] EncryptionKey { get; private set; }

        public byte[] DataEncryptionKey { get; private set; }

        public byte[] HostCryptogram { get; private set; }

        public Apdu ExternalAuthenticateCommand => Commands.ExternalAuthenticateCommand.Build
            .WithSecurityLevel(this.SecurityLevel)
            .UsingHostCryptogram(this.HostCryptogram)
            .AsApdu();

        public Apdu SecureApdu(Apdu apdu)
        {
            if (apdu.CLA != ApduClass.GlobalPlatform)
            {
                return apdu;
            }

            bool CheckSecurityLevelOption(byte mask)
            {
                return ((byte)this.SecurityLevel & mask) == mask;
            }

            if (CheckSecurityLevelOption(0b00000001) || apdu.INS == ApduInstruction.ExternalAuthenticate)
            {
                apdu.CLA = ApduClass.SecureMessaging;

                var mac = this.GenerateCmac(apdu);

                var data = new List<byte>();
                data.AddRange(apdu.CommandData);
                data.AddRange(mac);

                apdu.CommandData = data.ToArray();

                this.CMac = mac;
            }

            if (CheckSecurityLevelOption(0b00000010) && apdu.INS != ApduInstruction.ExternalAuthenticate)
            {
                byte[] commandData = apdu.CommandData.Take(apdu.CommandData.Length - 8).ToArray();
                byte[] mac = apdu.CommandData.TakeLast(8).ToArray();

                byte[] paddedCommandData = commandData.Pad().ToArray();

                byte[] enc = Crypto.TripleDes.Encrypt(paddedCommandData, this.EncryptionKey);

                var data = new List<byte>();
                data.AddRange(enc);
                data.AddRange(mac);

                apdu.CommandData = data.ToArray();
            }

            return apdu;
        }

        private byte[] GenerateCmac(Apdu apdu)
        {
            byte[] icv = this.CMac.All(x => x == 0x00) ? this.CMac : Crypto.Des.Encrypt(this.CMac, this.CMacKey.Take(8).ToArray());

            byte[] mac = Crypto.Mac.SingleDesPlusFinalTripleDes(GetDataForCmac(apdu), this.CMacKey, icv);

            return mac;
        }

        /// <summary>
        /// Returns the parts of an APDU that are used in C-MAC generation. This method pre-emptively
        /// increases the length of Lc by 8 bytes, to accommodate the MAC being added to the
        /// CommandData field.
        /// <para> Based on section E.4.4 of the v2.3 GlobalPlatform Card Specification. </para>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private static byte[] GetDataForCmac(Apdu apdu)
        {
            var apduData = new List<byte> { (byte)apdu.CLA, (byte)apdu.INS, apdu.P1, apdu.P2, apdu.Lc };

            apduData.AddRange(apdu.CommandData);

            byte[] padded = apduData.ToArray().Pad().ToArray();

            return padded;
        }

        public IEnumerable<Apdu> SecureApdu(params Apdu[] apdus)
            => apdus.Select(apdu => this.SecureApdu(apdu));

        public IEnumerable<Apdu> SecureApdu(IEnumerable<Apdu> apdus)
            => this.SecureApdu(apdus.ToArray());

        public IScp02SecurityLevelPicker UsingOption15()
        {
            return this;
        }

        public IScp02EncryptionKeyPicker UsingSecurityLevel(SecurityLevel securityLevel)
        {
            this.SecurityLevel = securityLevel;

            return this;
        }

        public IScp02MacKeyPicker UsingEncryptionKey(byte[] encryptionKey)
        {
            this.encryptionKey = encryptionKey;

            return this;
        }

        public IScp02DataEncryptionKeyPicker UsingMacKey(byte[] macKey)
        {
            this.macKey = macKey;

            return this;
        }

        public IScp02HostChallengePicker UsingDataEncryptionKey(byte[] dataEncryptionKey)
        {
            this.dataEncryptionKey = dataEncryptionKey;

            return this;
        }

        public IScp02InitializeUpdateResponsePicker UsingHostChallenge(byte[] hostChallenge)
        {
            this.hostChallenge = hostChallenge;

            return this;
        }

        public ISecureChannelSessionEstablisher<IScp02SecureChannelSession> UsingInitializeUpdateResponse(byte[] initializeUpdateResponse)
        {
            this.initializeUpdateResponse = initializeUpdateResponse;

            return this;
        }

        public IScp02SecureChannelSession Establish()
        {
            this.GenerateSessionKeys();
            this.VerifyCardCryptogram();

            this.HostCryptogram = this.GenerateCryptogram(CryptogramType.Host);

            this.IsEstablished = true;

            return this;
        }

        internal void GenerateSessionKeys()
        {
            this.EncryptionKey = this.GenerateSessionKey(SessionKeyType.SEnc, this.encryptionKey);
            this.CMacKey = this.GenerateSessionKey(SessionKeyType.CMac, this.macKey);
            this.RMacKey = this.GenerateSessionKey(SessionKeyType.RMac, this.macKey);
            this.DataEncryptionKey = this.GenerateSessionKey(SessionKeyType.Dek, this.dataEncryptionKey);
        }

        internal byte[] GenerateDerivationData(SessionKeyType sessionKeyType)
        {
            var data = new List<byte> { 0x01, (byte)sessionKeyType };

            data.AddRange(this.SequenceCounter);
            data.AddRange(Enumerable.Repeat<byte>(0x00, 12));

            return data.ToArray();
        }

        internal byte[] GenerateSessionKey(SessionKeyType sessionKeyType, byte[] staticKey)
            => Crypto.TripleDes.Encrypt(this.GenerateDerivationData(sessionKeyType), staticKey);

        internal void VerifyCardCryptogram()
        {
            var actual = this.CardCryptogram;
            var generated = this.GenerateCryptogram(CryptogramType.Card);

            if (!actual.SequenceEqual(generated))
            {
                throw new Exception("Card cryptogram could not be verified.");
            }
        }

        internal enum CryptogramType
        {
            Card,
            Host
        }

        internal byte[] GenerateCryptogram(CryptogramType cryptogramType)
        {
            var data = new List<byte>();

            switch (cryptogramType)
            {
                case CryptogramType.Card:
                    data.AddRange(this.hostChallenge);
                    data.AddRange(this.SequenceCounter);
                    data.AddRange(this.CardChallenge);
                    break;

                case CryptogramType.Host:
                    data.AddRange(this.SequenceCounter);
                    data.AddRange(this.CardChallenge);
                    data.AddRange(this.hostChallenge);
                    break;
            }

            data.Pad();

            byte[] cryptogram = Crypto.Mac.FullTripleDes(data.ToArray(), this.EncryptionKey);

            return cryptogram;
        }
    }
}
