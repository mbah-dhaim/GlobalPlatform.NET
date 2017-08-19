using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using System;
using System.Linq;
using System.Security.Cryptography;
using TripleDES = GlobalPlatform.NET.SecureChannel.Cryptography.TripleDES;

namespace GlobalPlatform.NET.Tools
{
    public static class KeyCheckValue
    {
        public static byte[] Generate(KeyTypeCoding keyType, byte[] key)
        {
            byte[] data;

            switch (keyType)
            {
                case KeyTypeCoding.DES:
                    data = Enumerable.Repeat<byte>(0x00, 8).ToArray();
                    break;

                case KeyTypeCoding.AES:
                    data = Enumerable.Repeat<byte>(0x01, 16).ToArray();
                    break;

                default:
                    throw new NotSupportedException("Unsupported key type.");
            }

            return TripleDES.Encrypt(data, key, CipherMode.ECB).Take(3).ToArray();
        }
    }
}
