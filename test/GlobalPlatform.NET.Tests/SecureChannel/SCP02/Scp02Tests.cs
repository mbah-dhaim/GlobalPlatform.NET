using FluentAssertions;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SecureChannel;
using GlobalPlatform.NET.SecureChannel.SCP02.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.SecureChannel.SCP02
{
    [TestClass]
    public class Scp02Tests
    {
        [TestMethod]
        public void SecureChannel_Scp02_Option15_Establish_Session()
        {
            byte[] key1, key2, key3;
            key1 = key2 = key3 = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F };
            byte[] hostChallenge = { 0xED, 0x29, 0x3C, 0x60, 0xB5, 0x0D, 0xF4, 0x20 };

            byte[] initializeUpdateResponse = { 0x00, 0x00, 0x74, 0x74, 0x6E, 0x6E, 0x6E, 0x62, 0x62, 0x62, 0xFF, 0x02, 0x00, 0x00, 0x3D, 0x02, 0x9C, 0x31, 0xC7, 0x89, 0xBD, 0x81, 0xD9, 0x37, 0x9C, 0x00, 0xD2, 0x8F, 0x90, 0x00 };

            var secureChannelSession = SecureChannelSession.Build
                .UsingScp02()
                .UsingOption15()
                .UsingSecurityLevel(SecurityLevel.CMac)
                .UsingEncryptionKey(key1)
                .UsingMacKey(key2)
                .UsingDataEncryptionKey(key3)
                .UsingHostChallenge(hostChallenge)
                .UsingInitializeUpdateResponse(initializeUpdateResponse)
                .Establish();

            secureChannelSession.EncryptionKey.ShouldBeEquivalentTo(new byte[] { 0x01, 0x0B, 0x03, 0x71, 0xD7, 0x83, 0x77, 0xB8, 0x01, 0xF2, 0xD6, 0x2A, 0xFC, 0x67, 0x1D, 0x95 });
            secureChannelSession.CMacKey.ShouldBeEquivalentTo(new byte[] { 0xD1, 0xC2, 0x8C, 0x60, 0x16, 0x52, 0xA4, 0x77, 0x0D, 0x67, 0xAD, 0x82, 0xD2, 0xD2, 0xE1, 0xC4 });
            secureChannelSession.RMacKey.ShouldBeEquivalentTo(new byte[] { 0xFF, 0xAE, 0xC7, 0xEC, 0x7F, 0xAD, 0x69, 0xF9, 0xFB, 0xFF, 0x09, 0x3B, 0xF2, 0xF7, 0x9C, 0x45 });
            secureChannelSession.DataEncryptionKey.ShouldBeEquivalentTo(new byte[] { 0xE1, 0x19, 0x87, 0xEE, 0x33, 0x1B, 0x41, 0x7A, 0x5D, 0x67, 0xD7, 0x60, 0x69, 0x2F, 0x89, 0xD4 });
            secureChannelSession.HostCryptogram.ShouldBeEquivalentTo(new byte[] { 0xE4, 0x47, 0x69, 0xBB, 0xAA, 0xF7, 0x5A, 0x6A });
        }

        [TestMethod]
        public void SecureChannel_Scp02_Option15_Secure_APDU_Cmac()
        {
            byte[] key1, key2, key3;
            key1 = key2 = key3 = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F };
            byte[] hostChallenge = { 0xED, 0x29, 0x3C, 0x60, 0xB5, 0x0D, 0xF4, 0x20 };

            byte[] initializeUpdateResponse = { 0x00, 0x00, 0x74, 0x74, 0x6E, 0x6E, 0x6E, 0x62, 0x62, 0x62, 0xFF, 0x02, 0x00, 0x00, 0x3D, 0x02, 0x9C, 0x31, 0xC7, 0x89, 0xBD, 0x81, 0xD9, 0x37, 0x9C, 0x00, 0xD2, 0x8F, 0x90, 0x00 };

            var secureChannelSession = SecureChannelSession.Build
                .UsingScp02()
                .UsingOption15()
                .UsingSecurityLevel(SecurityLevel.CMac)
                .UsingEncryptionKey(key1)
                .UsingMacKey(key2)
                .UsingDataEncryptionKey(key3)
                .UsingHostChallenge(hostChallenge)
                .UsingInitializeUpdateResponse(initializeUpdateResponse)
                .Establish();

            var apdu = GetStatusCommand.Build
                .GetStatusOf(GetStatusScope.IssuerSecurityDomain)
                .AsApdu();

            apdu.Lc.Should().Be(2);

            var securedApdu = secureChannelSession.SecureApdu(apdu);

            securedApdu.CLA.Should().Be(ApduClass.SecureMessaging);
            securedApdu.Lc.Should().Be(10);
        }

        [TestMethod]
        public void SecureChannel_Scp02_Option15_Secure_APDU_Command_Encryption()
        {
            byte[] key1, key2, key3;
            key1 = key2 = key3 = new byte[] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F };
            byte[] hostChallenge = { 0xED, 0x29, 0x3C, 0x60, 0xB5, 0x0D, 0xF4, 0x20 };

            byte[] initializeUpdateResponse = { 0x00, 0x00, 0x74, 0x74, 0x6E, 0x6E, 0x6E, 0x62, 0x62, 0x62, 0xFF, 0x02, 0x00, 0x00, 0x3D, 0x02, 0x9C, 0x31, 0xC7, 0x89, 0xBD, 0x81, 0xD9, 0x37, 0x9C, 0x00, 0xD2, 0x8F, 0x90, 0x00 };

            var secureChannelSession = SecureChannelSession.Build
                .UsingScp02()
                .UsingOption15()
                .UsingSecurityLevel(SecurityLevel.CDecryption)
                .UsingEncryptionKey(key1)
                .UsingMacKey(key2)
                .UsingDataEncryptionKey(key3)
                .UsingHostChallenge(hostChallenge)
                .UsingInitializeUpdateResponse(initializeUpdateResponse)
                .Establish();

            var apdu = GetStatusCommand.Build
                .GetStatusOf(GetStatusScope.IssuerSecurityDomain)
                .AsApdu();

            apdu.Lc.Should().Be(2);

            var securedApdu = secureChannelSession.SecureApdu(apdu);

            securedApdu.CLA.Should().Be(ApduClass.SecureMessaging);
            securedApdu.Lc.Should().Be(16);
        }
    }
}
