using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SecureChannel.SCP02.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace GlobalPlatform.NET.Commands
{
    public interface IPutKeyVersionPicker
    {
        IPutKeyIdentifierPicker WithKeyVersion(byte version);
    }

    public interface IPutKeyIdentifierPicker
    {
        IPutKeyEncryptionKeyPicker WithKeyIdentifier(byte identifier);
    }

    public interface IPutKeyEncryptionKeyPicker
    {
        IPutKeyFirstKeyPicker UsingEncryptionKey(byte[] encryptionKey);
    }

    public interface IPutKeyFirstKeyPicker
    {
        IPutKeySecondKeyPicker PutFirstKey(KeyTypeCoding keyType, byte[] key);
    }

    public interface IPutKeySecondKeyPicker : IApduBuilder
    {
        IPutKeyThirdKeyPicker PutSecondKey(KeyTypeCoding keyType, byte[] key);
    }

    public interface IPutKeyThirdKeyPicker : IApduBuilder
    {
        IApduBuilder PutThirdKey(KeyTypeCoding keyType, byte[] key);
    }

    /// <summary>
    /// The PUT KEY command is used to either: 
    /// <para>
    /// Replace an existing key with a new key: The new key has the same or a different Key Version
    /// Number but the same Key Identifier as the key being replaced;
    /// </para>
    /// <para>
    /// Replace multiple existing keys with new keys: The new keys have the same or a different Key
    /// Version Number (identical for all new keys) but the same Key Identifiers as the keys being replaced;
    /// </para>
    /// <para>
    /// Add a single new key: The new key has a different combination Key Identifier / Key Version
    /// Number than that of the existing keys;
    /// </para>
    /// <para>
    /// Add multiple new keys: The new keys have different combinations of Key Identifiers / Key
    /// Version Number (identical to all new keys) than that of the existing keys;
    /// </para>
    /// <para>
    /// When the key management operation requires multiple PUT KEY commands, chaining of the
    /// multiple PUT KEY commands is recommended to ensure integrity of the operation.
    /// </para>
    /// </summary>
    public class PutKeyCommand : CommandBase<PutKeyCommand, IPutKeyVersionPicker>,
        IPutKeyVersionPicker,
        IPutKeyIdentifierPicker,
        IPutKeyEncryptionKeyPicker,
        IPutKeyFirstKeyPicker,
        IPutKeySecondKeyPicker,
        IPutKeyThirdKeyPicker
    {
        private byte keyVersion;
        private byte keyIdentifier;
        private byte[] encryptionKey;
        private (KeyTypeCoding KeyType, byte[] Value) key1;
        private (KeyTypeCoding KeyType, byte[] Value) key2;
        private (KeyTypeCoding KeyType, byte[] Value) key3;

        public IPutKeyIdentifierPicker WithKeyVersion(byte keyVersion)
        {
            this.keyVersion = keyVersion;

            return this;
        }

        public IPutKeyEncryptionKeyPicker WithKeyIdentifier(byte keyIdentifier)
        {
            this.keyIdentifier = keyIdentifier;

            return this;
        }

        public IPutKeyFirstKeyPicker UsingEncryptionKey(byte[] encryptionKey)
        {
            this.encryptionKey = encryptionKey;

            return this;
        }

        public IPutKeySecondKeyPicker PutFirstKey(KeyTypeCoding keyType, byte[] key)
        {
            this.key1 = (keyType, key);

            return this;
        }

        public IPutKeyThirdKeyPicker PutSecondKey(KeyTypeCoding keyType, byte[] key)
        {
            this.key2 = (keyType, key);

            return this;
        }

        public IApduBuilder PutThirdKey(KeyTypeCoding keyType, byte[] key)
        {
            this.key3 = (keyType, key);

            return this;
        }

        public override Apdu AsApdu()
        {
            var apdu = Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.PutKey, this.keyVersion, this.keyIdentifier, 0x00);

            var data = new List<byte> { this.keyVersion };

            data.Add((byte)this.key1.KeyType);
            data.AddRangeWithLength(Crypto.TripleDes.Encrypt(this.key1.Value, this.encryptionKey, CipherMode.ECB));
            data.Add(0x00);

            if (this.key2.Value.Any())
            {
                data.Add((byte)this.key2.KeyType);
                data.AddRangeWithLength(Crypto.TripleDes.Encrypt(this.key2.Value, this.encryptionKey, CipherMode.ECB));
                data.Add(0x00);
            }

            if (this.key3.Value.Any())
            {
                data.Add((byte)this.key3.KeyType);
                data.AddRangeWithLength(Crypto.TripleDes.Encrypt(this.key3.Value, this.encryptionKey, CipherMode.ECB));
                data.Add(0x00);
            }

            apdu.CommandData = data.ToArray();

            return apdu;
        }
    }
}
