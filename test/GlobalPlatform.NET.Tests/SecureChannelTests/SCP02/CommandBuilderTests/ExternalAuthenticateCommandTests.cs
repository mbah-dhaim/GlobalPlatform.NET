using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SCP02.Commands;
using GlobalPlatform.NET.SCP02.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.SecureChannelTests.SCP02.CommandBuilderTests
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

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.ExternalAuthenticate, (byte)securityLevel, 0x00, hostCryptogram);
        }
    }
}
