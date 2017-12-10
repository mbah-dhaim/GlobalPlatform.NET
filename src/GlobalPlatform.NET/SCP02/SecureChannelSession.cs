using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Exceptions;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Interfaces;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SCP02.Cryptography;
using GlobalPlatform.NET.SCP02.Interfaces;
using GlobalPlatform.NET.SCP02.Reference;
using Iso7816;
using DES = GlobalPlatform.NET.Cryptography.DES;
using TripleDES = GlobalPlatform.NET.Cryptography.TripleDES;

namespace GlobalPlatform.NET.SCP02
{
    public interface IScp02SessionBuilder
    {
        /// <summary>
        /// Uses i=15 to configure the SCP02 secure channel. 
        /// <para> Based on section E.1.1 of the v2.3 GlobalPlatform Card Specification. </para>
        /// </summary>
        /// <returns></returns>
        IScp02SecurityLevelPicker Option15();
    }

    public interface IScp02SecurityLevelPicker
    {
        IScp02KeyPicker ChangeSecurityLevelTo(SecurityLevel securityLevel);
    }

    public interface IScp02KeyPicker : IScp02EncryptionKeyPicker
    {
        /// <summary>
        /// Specifies the object containing the 3 secure channel keys that you wish to use for this
        /// secure channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyObject"></param>
        /// <param name="encryptionKeySelector"></param>
        /// <param name="macKeySelector"></param>
        /// <param name="dataEncryptionKeySelector"></param>
        /// <returns></returns>
        IScp02HostChallengePicker UsingKeysFrom<T>(T keyObject,
            Func<T, IEnumerable<byte>> encryptionKeySelector,
            Func<T, IEnumerable<byte>> macKeySelector,
            Func<T, IEnumerable<byte>> dataEncryptionKeySelector);
    }

    public interface IScp02EncryptionKeyPicker
    {
        /// <summary>
        /// Specifies the encryption key that you wish to use for this secure channel. 
        /// </summary>
        /// <param name="encryptionKey"></param>
        /// <returns></returns>
        IScp02MacKeyPicker UsingEncryptionKey(byte[] encryptionKey);
    }

    public interface IScp02MacKeyPicker
    {
        /// <summary>
        /// Specifies the MAC key that you wish to use for this secure channel. 
        /// </summary>
        /// <param name="macKey"></param>
        /// <returns></returns>
        IScp02DataEncryptionKeyPicker AndMacKey(byte[] macKey);
    }

    public interface IScp02DataEncryptionKeyPicker
    {
        /// <summary>
        /// Specifies the data encryption key that you wish to use for this secure channel. 
        /// </summary>
        /// <param name="dataEncryptionKey"></param>
        /// <returns></returns>
        IScp02HostChallengePicker AndDataEncryptionKey(byte[] dataEncryptionKey);
    }

    public interface IScp02HostChallengePicker
    {
        /// <summary>
        /// The host challenge used in the preceding INITIALIZE UPDATE command. 
        /// </summary>
        /// <param name="hostChallenge"></param>
        /// <returns></returns>
        IScp02InitializeUpdateResponsePicker WithHostChallenge(byte[] hostChallenge);
    }

    public interface IScp02InitializeUpdateResponsePicker
    {
        /// <summary>
        /// The response buffer to the proceeding INITIALIZE UPDATE command. 
        /// </summary>
        /// <param name="initializeUpdateResponse"></param>
        /// <returns></returns>
        ISecureChannelSessionEstablisher<IScp02SecureChannelSession> AndInitializeUpdateResponse(byte[] initializeUpdateResponse);
    }

    [Serializable]
    public class SecureChannelSession : IScp02SecureChannelSession,
        IScp02SessionBuilder,
        IScp02SecurityLevelPicker,
        IScp02KeyPicker,
        IScp02MacKeyPicker,
        IScp02DataEncryptionKeyPicker,
        IScp02HostChallengePicker,
        IScp02InitializeUpdateResponsePicker
    {
        private byte[] encryptionKey;
        private byte[] macKey;
        private byte[] dataEncryptionKey;
        private byte[] initializeUpdateResponse;

        public byte[] SequenceCounter => this.initializeUpdateResponse.Skip(12).Take(2).ToArray();

        public byte[] CardChallenge => this.initializeUpdateResponse.Skip(14).Take(6).ToArray();

        public byte[] HostChallenge { get; private set; }

        public byte[] CardCryptogram => this.initializeUpdateResponse.Skip(20).Take(8).ToArray();

        public bool IsEstablished { get; private set; }

        public SecurityLevel SecurityLevel { get; private set; }

        public byte[] CMac { get; private set; } = new byte[8];

        public byte[] CMacKey { get; private set; }

        public byte[] RMac { get; private set; } = new byte[8];

        public byte[] RMacKey { get; private set; }

        public byte[] EncryptionKey { get; private set; }

        public byte[] DataEncryptionKey { get; private set; }

        public byte[] HostCryptogram { get; private set; }

        public CommandApdu SecureApdu(CommandApdu apdu)
        {
            if (apdu.ExtendedMode)
            {
                throw new InvalidOperationException("Extended commands APDUs are not supported.");
            }

            ApduClass.ByteCoding classByteCoding;

            if (apdu.CLA.IsBitSet(7))
            {
                classByteCoding = ApduClass.ByteCoding.First;
            }
            else if (apdu.CLA.IsBitSet(6))
            {
                classByteCoding = ApduClass.ByteCoding.InterIndustry;
            }
            else
            {
                return apdu;
            }

            if (this.SecurityLevel.HasFlag(SecurityLevel.CMac) || apdu.INS == ApduInstruction.ExternalAuthenticate)
            {
                switch (classByteCoding)
                {
                    case ApduClass.ByteCoding.First:
                        apdu.CLA = apdu.CLA |= 0b00000100;
                        break;

                    case ApduClass.ByteCoding.InterIndustry:
                        apdu.CLA = apdu.CLA |= 0b00100000;
                        break;

                    default:
                        throw new InvalidOperationException("APDU does not use either First or Interindustry Class Byte coding.");
                }

                var mac = this.GenerateCmac(apdu);

                var data = new List<byte>();
                data.AddRange(apdu.CommandData);
                data.AddRange(mac);

                apdu.CommandData = data.ToArray();
            }

            if (this.SecurityLevel.HasFlag(SecurityLevel.CDecryption) && apdu.INS != ApduInstruction.ExternalAuthenticate)
            {
                var commandData = apdu.CommandData.Take(apdu.CommandData.Count() - 8).ToList();
                var mac = apdu.CommandData.TakeLast(8);

                byte[] padded = commandData.Pad().ToArray();

                byte[] encrypted = TripleDES.Encrypt(padded, this.EncryptionKey);

                var data = new List<byte>();
                data.AddRange(encrypted);
                data.AddRange(mac);

                apdu.CommandData = data.ToArray();
            }

            return apdu;
        }

        private byte[] GenerateCmac(CommandApdu apdu)
        {
            byte[] icv = this.CMac.All(x => x == 0x00) ? this.CMac : DES.Encrypt(this.CMac, this.CMacKey.Take(8).ToArray());

            var data = GetDataForCmac(apdu);

            byte[] mac = MAC.Algorithm3(data, this.CMacKey, icv);

            return this.CMac = mac;
        }

        /// <summary>
        /// Returns the parts of an APDU that are used in C-MAC generation. This method pre-emptively
        /// increases the length of Lc by 8 bytes, to accommodate the MAC being added to the
        /// CommandData field.
        /// <para> Based on section E.4.4 of the v2.3 GlobalPlatform Card Specification. </para>
        /// </summary>
        /// <param name="apdu"></param>
        /// <returns></returns>
        private static byte[] GetDataForCmac(CommandApdu apdu)
        {
            var apduData = new List<byte> { apdu.CLA, apdu.INS, apdu.P1, apdu.P2 };

            byte newLc = checked((byte)(apdu.Lc.FirstOrDefault() + 8));

            apduData.Add(newLc);
            apduData.AddRange(apdu.CommandData);

            byte[] padded = apduData.Pad().ToArray();

            return padded;
        }

        public IEnumerable<CommandApdu> SecureApdu(params CommandApdu[] apdus)
            => apdus.Select(this.SecureApdu);

        public IEnumerable<CommandApdu> SecureApdu(IEnumerable<CommandApdu> apdus)
            => this.SecureApdu(apdus.ToArray());

        public IScp02SecurityLevelPicker Option15()
        {
            return this;
        }

        public IScp02KeyPicker ChangeSecurityLevelTo(SecurityLevel securityLevel)
        {
            this.SecurityLevel = securityLevel;

            return this;
        }

        public IScp02HostChallengePicker UsingKeysFrom<T>(T keyObject,
            Func<T, IEnumerable<byte>> encryptionKeySelector,
            Func<T, IEnumerable<byte>> macKeySelector,
            Func<T, IEnumerable<byte>> dataEncryptionKeySelector)
        {
            var encryptionKey = encryptionKeySelector(keyObject).ToArray();
            var macKey = macKeySelector(keyObject).ToArray();
            var dataEncryptionKey = dataEncryptionKeySelector(keyObject).ToArray();

            Ensure.HasCount(encryptionKey, nameof(encryptionKey), 16);
            Ensure.HasCount(macKey, nameof(macKey), 16);
            Ensure.HasCount(dataEncryptionKey, nameof(dataEncryptionKey), 16);

            this.encryptionKey = encryptionKey;
            this.macKey = macKey;
            this.dataEncryptionKey = dataEncryptionKey;

            return this;
        }

        public IScp02MacKeyPicker UsingEncryptionKey(byte[] encryptionKey)
        {
            Ensure.HasCount(encryptionKey, nameof(encryptionKey), 16);

            this.encryptionKey = encryptionKey;

            return this;
        }

        public IScp02DataEncryptionKeyPicker AndMacKey(byte[] macKey)
        {
            Ensure.HasCount(macKey, nameof(macKey), 16);

            this.macKey = macKey;

            return this;
        }

        public IScp02HostChallengePicker AndDataEncryptionKey(byte[] dataEncryptionKey)
        {
            Ensure.HasCount(dataEncryptionKey, nameof(dataEncryptionKey), 16);

            this.dataEncryptionKey = dataEncryptionKey;

            return this;
        }

        public IScp02InitializeUpdateResponsePicker WithHostChallenge(byte[] hostChallenge)
        {
            Ensure.HasCount(hostChallenge, nameof(hostChallenge), 8);

            this.HostChallenge = hostChallenge;

            return this;
        }

        public ISecureChannelSessionEstablisher<IScp02SecureChannelSession> AndInitializeUpdateResponse(byte[] initializeUpdateResponse)
        {
            Ensure.HasCount(initializeUpdateResponse, nameof(initializeUpdateResponse), 30);

            if (!initializeUpdateResponse.TakeLast(2).SequenceEqual(new byte[] { 0x90, 0x00 }))
            {
                throw new ArgumentException("INITIALIZE UPDATE response status bytes do not indicate success.", nameof(initializeUpdateResponse));
            }

            this.initializeUpdateResponse = initializeUpdateResponse;

            return this;
        }

        public IScp02SecureChannelSession Establish()
        {
            this.GenerateSessionKeys();
            this.HostCryptogram = this.GenerateCryptogram(CryptogramType.Host);
            this.VerifyCardCryptogram();

            this.IsEstablished = true;

            return this;
        }

        public bool TryEstablish(out IScp02SecureChannelSession secureChannelSession)
        {
            try
            {
                this.Establish();
            }
            catch
            {
                // ignored
            }

            secureChannelSession = this;

            return this.IsEstablished;
        }

        private void GenerateSessionKeys()
        {
            this.EncryptionKey = this.GenerateSessionKey(SessionKeyType.SEnc, this.encryptionKey);
            this.CMacKey = this.GenerateSessionKey(SessionKeyType.CMac, this.macKey);
            this.RMacKey = this.GenerateSessionKey(SessionKeyType.RMac, this.macKey);
            this.DataEncryptionKey = this.GenerateSessionKey(SessionKeyType.Dek, this.dataEncryptionKey);
        }

        private byte[] GenerateDerivationData(SessionKeyType sessionKeyType)
        {
            var data = new List<byte> { 0x01, (byte)sessionKeyType };

            data.AddRange(this.SequenceCounter);
            data.AddRange(Enumerable.Repeat<byte>(0x00, 12));

            return data.ToArray();
        }

        private byte[] GenerateSessionKey(SessionKeyType sessionKeyType, byte[] staticKey)
            => TripleDES.Encrypt(this.GenerateDerivationData(sessionKeyType), staticKey);

        private void VerifyCardCryptogram()
        {
            var fromCard = this.CardCryptogram;
            var generated = this.GenerateCryptogram(CryptogramType.Card);

            if (!fromCard.SequenceEqual(generated))
            {
                throw new CardCryptogramException("Card cryptogram could not be verified.")
                {
                    ReceivedCryptogram = fromCard,
                    GeneratedCryptogram = generated
                };
            }
        }

        private enum CryptogramType
        {
            Card,
            Host
        }

        private byte[] GenerateCryptogram(CryptogramType cryptogramType)
        {
            var data = new List<byte>();

            switch (cryptogramType)
            {
                case CryptogramType.Card:
                    data.AddRange(this.HostChallenge);
                    data.AddRange(this.SequenceCounter);
                    data.AddRange(this.CardChallenge);
                    break;

                case CryptogramType.Host:
                    data.AddRange(this.SequenceCounter);
                    data.AddRange(this.CardChallenge);
                    data.AddRange(this.HostChallenge);
                    break;
            }

            data.Pad();

            byte[] cryptogram = MAC.Algorithm1(data.ToArray(), this.EncryptionKey);

            return cryptogram;
        }
    }
}
