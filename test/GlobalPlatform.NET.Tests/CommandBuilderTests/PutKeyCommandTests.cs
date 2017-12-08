using System.Linq;
using System.Security.Cryptography;
using FluentAssertions;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TripleDES = GlobalPlatform.NET.SecureChannel.Cryptography.TripleDES;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class PutKeyCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void PutKey()
        {
            const byte keyVersion = 0x7F;
            const byte keyIdentifier = 0x01;
            byte[] encryptionkey = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            var apdu = PutKeyCommand.Build
                .WithKeyVersion(keyVersion)
                .WithKeyIdentifier(keyIdentifier)
                .UsingEncryptionKey(encryptionkey)
                .PutFirstKey(KeyTypeCoding.DES, KeyData)
                .PutSecondKey(KeyTypeCoding.DES, KeyData)
                .PutThirdKey(KeyTypeCoding.DES, KeyData)
                .AsApdu();

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.PutKey, keyVersion, keyIdentifier, 0x00);

            apdu.Lc.ShouldAllBeEquivalentTo(1 + 3 * 22);
            apdu.CommandData.First().Should().Be(keyVersion);
            apdu.CommandData.Skip(1).Split(22).ForEach(block =>
            {
                block.First().Should().Be(0x80);
                block.Skip(1).First().Should().Be(0x10);
                block.Skip(2).Take(16).ShouldAllBeEquivalentTo(TripleDES.Encrypt(KeyData, encryptionkey, CipherMode.ECB));
                block.Skip(18).First().Should().Be(0x03);
                block.Skip(19).ShouldAllBeEquivalentTo(KeyCheckValue.Generate(KeyTypeCoding.DES, KeyData));
            });
        }
    }
}
