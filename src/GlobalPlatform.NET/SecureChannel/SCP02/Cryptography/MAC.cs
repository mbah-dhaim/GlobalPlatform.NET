using GlobalPlatform.NET.Extensions;
using System;
using System.Linq;
using System.Security.Cryptography;
using DES = GlobalPlatform.NET.SecureChannel.Cryptography.DES;
using TripleDES = GlobalPlatform.NET.SecureChannel.Cryptography.TripleDES;

namespace GlobalPlatform.NET.SecureChannel.SCP02.Cryptography
{
    internal static class MAC
    {
        /// <summary>
        /// <para> Based on section B.1.2.1 of the v2.3 GlobalPlatform Card Specification. </para>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Algorithm1(byte[] data, byte[] key)
        {
            byte[] ciphertext = TripleDES.Encrypt(data, key);

            return ciphertext.Skip(ciphertext.Length - 8).Take(8).ToArray();
        }

        /// <summary>
        /// <para> Based on section B.1.2.2 of the v2.3 GlobalPlatform Card Specification. </para>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="icv"></param>
        /// <returns></returns>
        public static byte[] Algorithm3(byte[] data, byte[] key, byte[] icv)
        {
            if (data.Length % 8 != 0)
            {
                throw new ArgumentException("Data must be padded to 8-byte blocks.", nameof(data));
            }

            Ensure.HasCount(key, nameof(key), 16);
            Ensure.HasCount(icv, nameof(icv), 8);

            int numBlocks = data.Length / 8;

            byte[] iv;

            if (numBlocks > 1)
            {
                byte[] firstBlocks = data.Take((numBlocks - 1) * 8).ToArray();

                byte[] encFirstBlocks = DES.Encrypt(firstBlocks, key.Take(8).ToArray(), icv, CipherMode.CBC);

                iv = encFirstBlocks.TakeLast(8).ToArray();
            }
            else
            {
                iv = icv;
            }

            byte[] lastBlock = data.TakeLast(8).ToArray();
            byte[] encLastBlock = TripleDES.Encrypt(lastBlock, key, iv, CipherMode.CBC);
            byte[] mac = encLastBlock.TakeLast(8).ToArray();

            return mac;
        }
    }
}
