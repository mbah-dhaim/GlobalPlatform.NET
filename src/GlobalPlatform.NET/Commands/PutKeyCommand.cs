using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.Tools;
using Iso7816;
using TripleDES = GlobalPlatform.NET.SecureChannel.Cryptography.TripleDES;

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
            if (keyVersion < 1 || keyVersion > 0x7F)
            {
                throw new ArgumentException("Version number must be between 1-7F (inclusive).", nameof(keyVersion));
            }

            this.keyVersion = keyVersion;

            return this;
        }

        public IPutKeyEncryptionKeyPicker WithKeyIdentifier(byte keyIdentifier)
        {
            if (keyIdentifier < 1 || keyIdentifier > 0x7F)
            {
                throw new ArgumentException("Identifier must be between 1-7F (inclusive).", nameof(keyIdentifier));
            }

            this.keyIdentifier = keyIdentifier;

            return this;
        }

        public IPutKeyFirstKeyPicker UsingEncryptionKey(byte[] encryptionKey)
        {
            Ensure.HasCount(encryptionKey, nameof(encryptionKey), 16);

            this.encryptionKey = encryptionKey;

            return this;
        }

        public IPutKeySecondKeyPicker PutFirstKey(KeyTypeCoding keyType, byte[] key)
        {
            Ensure.HasCount(key, nameof(key), 16);

            this.key1 = (keyType, key);

            return this;
        }

        public IPutKeyThirdKeyPicker PutSecondKey(KeyTypeCoding keyType, byte[] key)
        {
            Ensure.HasCount(key, nameof(key), 16);

            this.key2 = (keyType, key);

            return this;
        }

        public IApduBuilder PutThirdKey(KeyTypeCoding keyType, byte[] key)
        {
            Ensure.HasCount(key, nameof(key), 16);

            this.key3 = (keyType, key);

            return this;
        }

        public override CommandApdu AsApdu()
        {
            var apdu = CommandApdu.Case2S(ApduClass.GlobalPlatform, ApduInstruction.PutKey, this.keyVersion, this.keyIdentifier, 0x00);

            var data = new List<byte> { this.keyVersion };

            data.Add((byte)this.key1.KeyType);
            data.AddRangeWithLength(TripleDES.Encrypt(this.key1.Value, this.encryptionKey, CipherMode.ECB));
            data.AddRangeWithLength(KeyCheckValue.Generate(this.key1.KeyType, this.key1.Value));

            if (this.key2.Value.Any())
            {
                data.Add((byte)this.key2.KeyType);
                data.AddRangeWithLength(TripleDES.Encrypt(this.key2.Value, this.encryptionKey, CipherMode.ECB));
                data.AddRangeWithLength(KeyCheckValue.Generate(this.key2.KeyType, this.key2.Value));
            }

            if (this.key3.Value.Any())
            {
                data.Add((byte)this.key3.KeyType);
                data.AddRangeWithLength(TripleDES.Encrypt(this.key3.Value, this.encryptionKey, CipherMode.ECB));
                data.AddRangeWithLength(KeyCheckValue.Generate(this.key3.KeyType, this.key3.Value));
            }

            apdu.CommandData = data.ToArray();

            return apdu;
        }
    }
}
