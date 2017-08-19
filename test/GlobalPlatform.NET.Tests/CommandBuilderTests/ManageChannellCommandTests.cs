using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class ManageChannellCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void ManageChannel_Open()
        {
            var apdu = ManageChannelCommand.Build
                .OpenChannel()
                .AsApdu();

            apdu.Assert(ApduInstruction.ManageChannel, 0x00, 0x00, 0x01);
        }

        [TestMethod]
        public void ManageChannel_Close()
        {
            var apdu = ManageChannelCommand.Build
                .CloseChannel()
                .WithIdentifier(0x01)
                .AsApdu();

            apdu.Assert(ApduInstruction.ManageChannel, 0x80, 0x01);
        }
    }
}
