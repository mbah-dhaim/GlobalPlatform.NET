using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SecureChannel.SCP02.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.SecureChannel.SCP02.CommandBuilderTests
{
    [TestClass]
    public class InitializeUpdateCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void InitializeUpdate()
        {
            const byte keyVersion = 0x01;
            byte[] hostChallenge;

            var apdu = InitializeUpdateCommand.Build
                .WithKeyVersion(keyVersion)
                .WithHostChallenge(out hostChallenge)
                .AsApdu();

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.InitializeUpdate, keyVersion, 0x00, hostChallenge, 0x00);
        }

        [TestMethod]
        public void InitializeUpdate_Using_Custom_Host_Challenge()
        {
            const byte keyVersion = 0x01;
            byte[] hostChallenge = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            var apdu = InitializeUpdateCommand.Build
                .WithKeyVersion(keyVersion)
                .WithHostChallenge(hostChallenge)
                .AsApdu();

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.InitializeUpdate, keyVersion, 0x00, hostChallenge, 0x00);
        }
    }
}
