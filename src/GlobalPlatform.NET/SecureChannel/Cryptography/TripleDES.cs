using System.IO;
using System.Security.Cryptography;

namespace GlobalPlatform.NET.SecureChannel.Cryptography
{
    internal static class TripleDES
    {
        public static byte[] Encrypt(byte[] data, byte[] key)
            => Encrypt(data, key, CipherMode.CBC);

        public static byte[] Encrypt(byte[] data, byte[] key, CipherMode cipherMode)
            => Encrypt(data, key, new byte[8], cipherMode);

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv, CipherMode cipherMode)
        {
            using (var des = System.Security.Cryptography.TripleDES.Create())
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
}
