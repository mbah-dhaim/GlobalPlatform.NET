using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class GetStatusCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void GetStatus_Of_Issuer_Security_Domain()
        {
            var apdu = GetStatusCommand.Build
                .GetStatusOf(GetStatusScope.IssuerSecurityDomain)
                .AsApdu();

            apdu.Assert(ApduInstruction.GetStatus, 0x80, 0x00, 0x4F, 0x00);
        }

        [TestMethod]
        public void GetStatus_Of_Executable_Load_Files_And_Modules()
        {
            var apdu = GetStatusCommand.Build
                .GetStatusOf(GetStatusScope.ExecutableLoadFilesAndModules)
                .WithFilter(new byte[] { 0xA0, 0x00 })
                .AsApdu();

            apdu.Assert(ApduInstruction.GetStatus, 0x10, 0x00, 0x4F, 0x02, 0xA0, 0x00);
        }
    }
}
