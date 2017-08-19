using FluentAssertions;
using GlobalPlatform.NET.SecureChannel.SCP02.Commands;
using GlobalPlatform.NET.SecureChannel.SCP02.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.SecureChannel.SCP02.CommandBuilderTests
{
    [TestClass]
    public class ExternalAuthenticateCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void ExternalAuthenticate()
        {
            const SecurityLevel securityLevel = SecurityLevel.CDecryption;
            byte[] hostCryptogram = new byte[8];

            var apdu = ExternalAuthenticateCommand.Build
                .WithSecurityLevel(securityLevel)
                .UsingHostCryptogram(hostCryptogram)
                .AsApdu();

            apdu.Buffer.ShouldBeEquivalentTo(new byte[] { 0x80, 0x82, (byte)securityLevel, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 });
        }
    }
}
