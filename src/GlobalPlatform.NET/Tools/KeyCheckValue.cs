using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using GlobalPlatform.NET.Reference;
using TripleDES = GlobalPlatform.NET.Cryptography.TripleDES;

namespace GlobalPlatform.NET.Tools
{
    public static class KeyCheckValue
    {
        /// <summary>
        /// Generates a key check value for the specified key. 
        /// <para> Based on section B.6 of the v2.3 GlobalPlatform Card Specification. </para>
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Generate(KeyTypeCoding keyType, byte[] key)
        {
            IEnumerable<byte> data;

            switch (keyType)
            {
                case KeyTypeCoding.DES:
                    data = Enumerable.Repeat<byte>(0x00, 8);
                    break;

                case KeyTypeCoding.AES:
                    data = Enumerable.Repeat<byte>(0x01, 16);
                    break;

                default:
                    throw new NotSupportedException("Unsupported key type.");
            }

            return TripleDES.Encrypt(data.ToArray(), key, CipherMode.ECB).Take(3).ToArray();
        }
    }
}
