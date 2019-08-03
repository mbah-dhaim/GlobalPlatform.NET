using GlobalPlatform.NET.Exceptions;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Interfaces;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SCP02.Cryptography;
using GlobalPlatform.NET.SCP02.Interfaces;
using GlobalPlatform.NET.SCP02.Reference;
using Iso7816;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <returns>  </returns>
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
        /// <typeparam name="T">  </typeparam>
        /// <param name="keyObject">  </param>
        /// <param name="encryptionKeySelector">  </param>
        /// <param name="macKeySelector">  </param>
        /// <param name="dataEncryptionKeySelector">  </param>
        /// <returns>  </returns>
        IScp02HostChallengePicker UsingKeysFrom<T>(T keyObject,
            Func<T, IEnumerable<byte>> encryptionKeySelector,
            Func<T, IEnumerable<byte>> macKeySelector,
            Func<T, IEnumerable<byte>> dataEncryptionKeySelector);

        /// <summary>
        /// Specifies the object containing the 4 secure channel session keys that you wish to use
        /// for this secure channel.
        /// </summary>
        /// <typeparam name="T">  </typeparam>
        /// <param name="keyObject">  </param>
        /// <param name="encryptionKeySelector">  </param>
        /// <param name="cMacKeySelector">  </param>
        /// <param name="rMacKeySelector">  </param>
        /// <param name="dataEncryptionKeySelector">  </param>
        /// <returns>  </returns>
        IScp02HostChallengePicker UsingSessionKeysFrom<T>(T keyObject,
            Func<T, IEnumerable<byte>> encryptionKeySelector,
            Func<T, IEnumerable<byte>> cMacKeySelector,
            Func<T, IEnumerable<byte>> rMacKeySelector,
            Func<T, IEnumerable<byte>> dataEncryptionKeySelector);
    }

    public interface IScp02EncryptionKeyPicker
    {
        /// <summary>
        /// Specifies the encryption key that you wish to use for this secure channel.
        /// </summary>
        /// <param name="encryptionKey">  </param>
        /// <returns>  </returns>
        IScp02MacKeyPicker UsingEncryptionKey(byte[] encryptionKey);

        /// <summary>
        /// Specifies the encryption session key that you wish to use for this secure channel.
        /// </summary>
        /// <param name="encryptionKey">  </param>
        /// <returns>  </returns>
        IScp02CMacSessionKeyPicker UsingEncryptionSessionKey(byte[] encryptionKey);
    }

    public interface IScp02MacKeyPicker
    {
        /// <summary>
        /// Specifies the MAC key that you wish to use for this secure channel.
        /// </summary>
        /// <param name="macKey">  </param>
        /// <returns>  </returns>
        IScp02DataEncryptionKeyPicker AndMacKey(byte[] macKey);
    }

    public interface IScp02CMacSessionKeyPicker
    {
        /// <summary>
        /// Specifies the C-MAC session key that you wish to use for this secure channel.
        /// </summary>
        /// <param name="cMacKey">  </param>
        /// <returns>  </returns>
        IScp02RMacSessionKeyPicker AndCMacSessionKey(byte[] cMacKey);
    }

    public interface IScp02RMacSessionKeyPicker : IScp02DataEncryptionSessionKeyPicker
    {
        /// <summary>
        /// Specifies the R-MAC session key that you wish to use for this secure channel.
        /// </summary>
        /// <param name="rMacKey">  </param>
        /// <returns>  </returns>
        IScp02DataEncryptionSessionKeyPicker AndRMacSessionKey(byte[] rMacKey);
    }

    public interface IScp02DataEncryptionKeyPicker
    {
        /// <summary>
        /// Specifies the data encryption key that you wish to use for this secure channel.
        /// </summary>
        /// <param name="dataEncryptionKey">  </param>
        /// <returns>  </returns>
        IScp02HostChallengePicker AndDataEncryptionKey(byte[] dataEncryptionKey);
    }

    public interface IScp02DataEncryptionSessionKeyPicker
    {
        /// <summary>
        /// Specifies the data encryption session key that you wish to use for this secure channel.
        /// </summary>
        /// <param name="dataEncryptionKey">  </param>
        /// <returns>  </returns>
        IScp02HostChallengePicker AndDataEncryptionSessionKey(byte[] dataEncryptionKey);
    }

    public interface IScp02HostChallengePicker
    {
        /// <summary>
        /// The host challenge used in the preceding INITIALIZE UPDATE command.
        /// </summary>
        /// <param name="hostChallenge">  </param>
        /// <returns>  </returns>
        IScp02InitializeUpdateResponsePicker WithHostChallenge(byte[] hostChallenge);
    }

    public interface IScp02InitializeUpdateResponsePicker
    {
        /// <summary>
        /// The response buffer to the proceeding INITIALIZE UPDATE command.
        /// </summary>
        /// <param name="initializeUpdateResponse">  </param>
        /// <returns>  </returns>
        ISecureChannelSessionEstablisher<IScp02SecureChannelSession> AndInitializeUpdateResponse(byte[] initializeUpdateResponse);
    }

    [Serializable]
    public class SecureChannelSession : IScp02SecureChannelSession,
        IScp02SessionBuilder,
        IScp02SecurityLevelPicker,
        IScp02KeyPicker,
        IScp02MacKeyPicker,
        IScp02CMacSessionKeyPicker,
        IScp02RMacSessionKeyPicker,
        IScp02DataEncryptionKeyPicker,
        IScp02DataEncryptionSessionKeyPicker,
        IScp02HostChallengePicker,
        IScp02InitializeUpdateResponsePicker
    {
        private KeyMode keyMode;
        private byte[] encryptionKey;
        private byte[] macKey;
        private byte[] dataEncryptionKey;
        private byte[] initializeUpdateResponse;

        public byte[] SequenceCounter => initializeUpdateResponse.Skip(12).Take(2).ToArray();

        public byte[] CardChallenge => initializeUpdateResponse.Skip(14).Take(6).ToArray();

        public byte[] HostChallenge { get; private set; }

        public byte[] CardCryptogram => initializeUpdateResponse.Skip(20).Take(8).ToArray();

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

            if (SecurityLevel.HasFlag(SecurityLevel.CMac) || apdu.INS == ApduInstruction.ExternalAuthenticate)
            {
                byte cla = apdu.CLA;
                switch (classByteCoding)
                {
                    case ApduClass.ByteCoding.First:
                        //apdu.CLA = apdu.CLA |= 0b00000100;
                        cla = cla |= 0b00000100;
                        break;

                    case ApduClass.ByteCoding.InterIndustry:
                        //apdu.CLA = apdu.CLA |= 0b00100000;
                        cla = cla |= 0b00100000;
                        break;

                    default:
                        throw new InvalidOperationException("APDU does not use either First or Interindustry Class Byte coding.");
                }

                var mac = GenerateCmac(apdu);

                var data = new List<byte>();
                data.AddRange(apdu.CommandData);
                data.AddRange(mac);

                //apdu.CommandData = data.ToArray();
                //recreate apdu
                //apdu = CommandApdu.Case4S(cla, apdu.INS, apdu.P1, apdu.P2, data.ToArray(), apdu.Le.ToArray()[0]);
                apdu = CommandApdu.Case3S(cla, apdu.INS, apdu.P1, apdu.P2, data.ToArray());
            }

            if (SecurityLevel.HasFlag(SecurityLevel.CDecryption) && apdu.INS != ApduInstruction.ExternalAuthenticate)
            {
                var commandData = apdu.CommandData.Take(apdu.CommandData.Count() - 8).ToList();
                var mac = apdu.CommandData.TakeLast(8);

                byte[] padded = commandData.Pad().ToArray();

                byte[] encrypted = TripleDES.Encrypt(padded, EncryptionKey);

                var data = new List<byte>();
                data.AddRange(encrypted);
                data.AddRange(mac);

                //apdu.CommandData = data.ToArray();
                //recreate apdu
                //apdu = CommandApdu.Case4S(apdu.CLA, apdu.INS, apdu.P1, apdu.P2, data.ToArray(), apdu.Le.ToArray()[0]);
                apdu = CommandApdu.Case3S(apdu.CLA, apdu.INS, apdu.P1, apdu.P2, data.ToArray());
            }

            return apdu;
        }

        private byte[] GenerateCmac(CommandApdu apdu)
        {
            byte[] icv = CMac.All(x => x == 0x00) ? CMac : DES.Encrypt(CMac, CMacKey.Take(8).ToArray());

            var data = GetDataForCmac(apdu);

            byte[] mac = MAC.Algorithm3(data, CMacKey, icv);

            return CMac = mac;
        }

        /// <summary>
        /// Returns the parts of an APDU that are used in C-MAC generation. This method pre-emptively
        /// increases the length of Lc by 8 bytes, to accommodate the MAC being added to the
        /// CommandData field.
        /// <para> Based on section E.4.4 of the v2.3 GlobalPlatform Card Specification. </para>
        /// </summary>
        /// <param name="apdu">  </param>
        /// <returns>  </returns>
        private static byte[] GetDataForCmac(CommandApdu apdu)
        {
            var apduData = new List<byte> { apdu.CLA, apdu.INS, apdu.P1, apdu.P2 };

            byte newLc = checked((byte)(apdu.Lc.FirstOrDefault() + 8));

            apduData.Add(newLc);
            apduData.AddRange(apdu.CommandData);

            byte[] padded = apduData.Pad().ToArray();

            return padded;
        }

        public IEnumerable<CommandApdu> SecureApdu(params CommandApdu[] apdus) => apdus.Select(SecureApdu);

        public IEnumerable<CommandApdu> SecureApdu(IEnumerable<CommandApdu> apdus) => SecureApdu(apdus.ToArray());

        public IScp02SecurityLevelPicker Option15() => this;

        public IScp02KeyPicker ChangeSecurityLevelTo(SecurityLevel securityLevel)
        {
            SecurityLevel = securityLevel;

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

            keyMode = KeyMode.Static;

            this.encryptionKey = encryptionKey;
            this.macKey = macKey;
            this.dataEncryptionKey = dataEncryptionKey;

            return this;
        }

        public IScp02HostChallengePicker UsingSessionKeysFrom<T>(T keyObject,
            Func<T, IEnumerable<byte>> encryptionKeySelector,
            Func<T, IEnumerable<byte>> cMacKeySelector,
            Func<T, IEnumerable<byte>> rMacKeySelector,
            Func<T, IEnumerable<byte>> dataEncryptionKeySelector)
        {
            var encryptionKey = encryptionKeySelector(keyObject).ToArray();
            var cMacKey = cMacKeySelector(keyObject).ToArray();
            var rMacKey = rMacKeySelector(keyObject).ToArray();
            var dataEncryptionKey = dataEncryptionKeySelector(keyObject).ToArray();

            Ensure.HasCount(encryptionKey, nameof(encryptionKey), 16);
            Ensure.HasCount(cMacKey, nameof(cMacKey), 16);
            Ensure.HasCount(rMacKey, nameof(rMacKey), 16);
            Ensure.HasCount(dataEncryptionKey, nameof(dataEncryptionKey), 16);

            keyMode = KeyMode.Session;

            EncryptionKey = encryptionKey;
            CMacKey = cMacKey;
            RMacKey = rMacKey;
            DataEncryptionKey = dataEncryptionKey;

            return this;
        }

        public IScp02MacKeyPicker UsingEncryptionKey(byte[] encryptionKey)
        {
            Ensure.HasCount(encryptionKey, nameof(encryptionKey), 16);

            keyMode = KeyMode.Static;

            this.encryptionKey = encryptionKey;

            return this;
        }

        public IScp02CMacSessionKeyPicker UsingEncryptionSessionKey(byte[] encryptionKey)
        {
            Ensure.HasCount(encryptionKey, nameof(encryptionKey), 16);

            keyMode = KeyMode.Session;

            EncryptionKey = encryptionKey;

            return this;
        }

        public IScp02DataEncryptionKeyPicker AndMacKey(byte[] macKey)
        {
            Ensure.HasCount(macKey, nameof(macKey), 16);

            this.macKey = macKey;

            return this;
        }

        public IScp02RMacSessionKeyPicker AndCMacSessionKey(byte[] cMacKey)
        {
            Ensure.HasCount(cMacKey, nameof(cMacKey), 16);

            CMacKey = cMacKey;

            return this;
        }

        public IScp02DataEncryptionSessionKeyPicker AndRMacSessionKey(byte[] rMacKey)
        {
            Ensure.HasCount(rMacKey, nameof(rMacKey), 16);

            RMacKey = rMacKey;

            return this;
        }

        public IScp02HostChallengePicker AndDataEncryptionKey(byte[] dataEncryptionKey)
        {
            Ensure.HasCount(dataEncryptionKey, nameof(dataEncryptionKey), 16);

            this.dataEncryptionKey = dataEncryptionKey;

            return this;
        }

        public IScp02HostChallengePicker AndDataEncryptionSessionKey(byte[] dataEncryptionKey)
        {
            Ensure.HasCount(dataEncryptionKey, nameof(dataEncryptionKey), 16);

            DataEncryptionKey = dataEncryptionKey;

            return this;
        }

        public IScp02InitializeUpdateResponsePicker WithHostChallenge(byte[] hostChallenge)
        {
            Ensure.HasCount(hostChallenge, nameof(hostChallenge), 8);

            HostChallenge = hostChallenge;

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
            if (keyMode == KeyMode.Static)
            {
                GenerateSessionKeys();
            }

            HostCryptogram = GenerateCryptogram(CryptogramType.Host);
            VerifyCardCryptogram();

            IsEstablished = true;

            return this;
        }

        public bool TryEstablish(out IScp02SecureChannelSession secureChannelSession)
        {
            try
            {
                Establish();
            }
            catch
            {
                // ignored
            }

            secureChannelSession = this;

            return IsEstablished;
        }

        private void GenerateSessionKeys()
        {
            EncryptionKey = GenerateSessionKey(SessionKeyType.SEnc, encryptionKey);
            CMacKey = GenerateSessionKey(SessionKeyType.CMac, macKey);
            RMacKey = GenerateSessionKey(SessionKeyType.RMac, macKey);
            DataEncryptionKey = GenerateSessionKey(SessionKeyType.Dek, dataEncryptionKey);
        }

        private byte[] GenerateDerivationData(SessionKeyType sessionKeyType)
        {
            var data = new List<byte> { 0x01, (byte)sessionKeyType };

            data.AddRange(SequenceCounter);
            data.AddRange(Enumerable.Repeat<byte>(0x00, 12));

            return data.ToArray();
        }

        private byte[] GenerateSessionKey(SessionKeyType sessionKeyType, byte[] staticKey) => TripleDES.Encrypt(GenerateDerivationData(sessionKeyType), staticKey);

        private void VerifyCardCryptogram()
        {
            var fromCard = CardCryptogram;
            var generated = GenerateCryptogram(CryptogramType.Card);

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

        private enum KeyMode
        {
            Static,
            Session
        }

        private byte[] GenerateCryptogram(CryptogramType cryptogramType)
        {
            var data = new List<byte>();

            switch (cryptogramType)
            {
                case CryptogramType.Card:
                    data.AddRange(HostChallenge);
                    data.AddRange(SequenceCounter);
                    data.AddRange(CardChallenge);
                    break;

                case CryptogramType.Host:
                    data.AddRange(SequenceCounter);
                    data.AddRange(CardChallenge);
                    data.AddRange(HostChallenge);
                    break;
            }

            data.Pad();

            byte[] cryptogram = MAC.Algorithm1(data.ToArray(), EncryptionKey);

            return cryptogram;
        }
    }
}
