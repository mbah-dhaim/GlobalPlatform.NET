using System.Linq;
using FluentAssertions;
using GlobalPlatform.NET.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DES = GlobalPlatform.NET.Cryptography.DES;
using TripleDES = GlobalPlatform.NET.Cryptography.TripleDES;

namespace GlobalPlatform.NET.Tests
{
    [TestClass]
    public class EncryptionTests
    {
        [TestMethod]
        public void Des()
        {
            byte[] plaintext = { 0x67, 0x5a, 0x69, 0x67, 0x5e, 0x5a, 0x6b, 0x5a };
            byte[] key = { 0x5b, 0x5a, 0x57, 0x67, 0x6a, 0x56, 0x67, 0x6e };
            byte[] ciphertext = { 0x97, 0x4a, 0xff, 0xbf, 0x86, 0x02, 0x2d, 0x1f };

            DES.Encrypt(plaintext, key).Should().BeEquivalentTo(ciphertext);
        }

        [TestMethod]
        public void TripleDes()
        {
            byte[] plaintext = { 0x98, 0x26, 0x62, 0x60, 0x55, 0x53, 0x24, 0x4D };
            byte[] key = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17 };
            byte[] ciphertext = { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 };

            TripleDES.Encrypt(plaintext, key).Should().BeEquivalentTo(ciphertext);
        }

        [TestMethod]
        public void Padding()
        {
            var bytes = Enumerable.Repeat<byte>(0, 7).ToList();

            var padded = bytes.Pad();

            padded.Count.Should().Be(8);

            bytes = Enumerable.Repeat<byte>(0, 8).ToList();

            padded = bytes.Pad();

            padded.Count.Should().Be(16);

            bytes = Enumerable.Repeat<byte>(0, 9).ToList();

            padded = bytes.Pad();

            padded.Count.Should().Be(16);
        }
    }
}
