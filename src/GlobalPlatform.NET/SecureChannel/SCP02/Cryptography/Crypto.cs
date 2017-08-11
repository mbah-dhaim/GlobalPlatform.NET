using GlobalPlatform.NET.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GlobalPlatform.NET.SecureChannel.SCP02.Cryptography
{
    internal static class Crypto
    {
        internal static class SecureRandom
        {
            public static byte[] GetBytes(int length)
            {
                var data = new byte[length];

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(data);
                }

                return data;
            }
        }

        internal static class Des
        {
            /// <summary>
            /// Encrypts data using DES. 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static byte[] Encrypt(byte[] data, byte[] key)
                => Encrypt(data, key, new byte[8], CipherMode.CBC);

            /// <summary>
            /// Encrypts data using DES. 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="cipherMode"></param>
            /// <returns></returns>
            public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv, CipherMode cipherMode)
            {
                using (var des = TripleDES.Create())
                {
                    des.Mode = cipherMode;
                    des.Padding = PaddingMode.None;
                    des.IV = iv;
                    des.Key = key.Concat(key).ToArray();

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                        }

                        return ms.ToArray();
                    }
                }
            }
        }

        internal static class TripleDes
        {
            /// <summary>
            /// Encrypts data using TripleDES. Uses CBC as the cipher mode. 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static byte[] Encrypt(byte[] data, byte[] key)
                => Encrypt(data, key, CipherMode.CBC);

            /// <summary>
            /// Encrypts data using TripleDES. 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <param name="cipherMode"></param>
            /// <returns></returns>
            public static byte[] Encrypt(byte[] data, byte[] key, CipherMode cipherMode)
                => Encrypt(data, key, new byte[8], cipherMode);

            /// <summary>
            /// Encrypts data using TripleDES. 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="cipherMode"></param>
            /// <returns></returns>
            public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv, CipherMode cipherMode)
            {
                using (var des = TripleDES.Create())
                {
                    des.Mode = cipherMode;
                    des.Padding = PaddingMode.None;
                    des.IV = iv;
                    des.Key = key;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                        }

                        return ms.ToArray();
                    }
                }
            }
        }

        internal static class Mac
        {
            /// <summary>
            /// <para> Based on section B.1.2.1 of the v2.3 GlobalPlatform Card Specification. </para>
            /// </summary>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static byte[] FullTripleDes(byte[] data, byte[] key)
            {
                byte[] ciphertext = TripleDes.Encrypt(data, key);

                return ciphertext.Skip(ciphertext.Length - 8).Take(8).ToArray();
            }

            /// <summary>
            /// <para> Based on section B.1.2.2 of the v2.3 GlobalPlatform Card Specification. </para>
            /// </summary>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <param name="icv"></param>
            /// <returns></returns>
            public static byte[] SingleDesPlusFinalTripleDes(byte[] data, byte[] key, byte[] icv)
            {
                if (data.Length % 8 != 0)
                {
                    throw new ArgumentException("Data must be padded to 8-byte blocks.", nameof(data));
                }

                if (key.Length != 16)
                {
                    throw new ArgumentException("The key must be exactly 16 bytes.", nameof(key));
                }

                if (icv.Length != 8)
                {
                    throw new ArgumentException("The ICV must be exactly 8 bytes.", nameof(icv));
                }

                int numBlocks = data.Length / 8;

                byte[] iv;

                if (numBlocks > 1)
                {
                    byte[] firstBlocks = data.Take((numBlocks - 1) * 8).ToArray();

                    byte[] encFirstBlocks = Des.Encrypt(firstBlocks, key.Take(8).ToArray(), icv, CipherMode.CBC);

                    iv = encFirstBlocks.TakeLast(8).ToArray();
                }
                else
                {
                    iv = icv;
                }

                byte[] lastBlock = data.TakeLast(8).ToArray();
                byte[] encLastBlock = TripleDes.Encrypt(lastBlock, key, iv, CipherMode.CBC);
                byte[] mac = encLastBlock.TakeLast(8).ToArray();

                return mac;
            }
        }
    }
}
