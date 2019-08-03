using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.Tools;
using Iso7816;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TripleDES = GlobalPlatform.NET.Cryptography.TripleDES;

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

            key1 = (keyType, key);

            return this;
        }

        public IPutKeyThirdKeyPicker PutSecondKey(KeyTypeCoding keyType, byte[] key)
        {
            Ensure.HasCount(key, nameof(key), 16);

            key2 = (keyType, key);

            return this;
        }

        public IApduBuilder PutThirdKey(KeyTypeCoding keyType, byte[] key)
        {
            Ensure.HasCount(key, nameof(key), 16);

            key3 = (keyType, key);

            return this;
        }

        public override CommandApdu AsApdu()
        {

            var data = new List<byte> { keyVersion };

            data.Add((byte)key1.KeyType);
            data.AddRangeWithLength(TripleDES.Encrypt(key1.Value, encryptionKey, CipherMode.ECB));
            data.AddRangeWithLength(KeyCheckValue.Generate(key1.KeyType, key1.Value));

            if (key2.Value.Any())
            {
                data.Add((byte)key2.KeyType);
                data.AddRangeWithLength(TripleDES.Encrypt(key2.Value, encryptionKey, CipherMode.ECB));
                data.AddRangeWithLength(KeyCheckValue.Generate(key2.KeyType, key2.Value));
            }

            if (key3.Value.Any())
            {
                data.Add((byte)key3.KeyType);
                data.AddRangeWithLength(TripleDES.Encrypt(key3.Value, encryptionKey, CipherMode.ECB));
                data.AddRangeWithLength(KeyCheckValue.Generate(key3.KeyType, key3.Value));
            }

            return CommandApdu.Case4S(ApduClass.GlobalPlatform, ApduInstruction.PutKey, keyVersion, keyIdentifier, data.ToArray(), 0x00);
        }
    }
}
