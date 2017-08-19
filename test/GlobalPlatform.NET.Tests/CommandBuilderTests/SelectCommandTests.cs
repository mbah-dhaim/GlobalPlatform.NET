using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class SelectCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void Select_Issuer_Security_Domain()
        {
            var apdu = SelectCommand.Build
                .SelectIssuerSecurityDomain()
                .AsApdu();

            apdu.Assert(ApduClass.Iso7816, ApduInstruction.Select, 0x04, 0x00, 0x00);
        }

        [TestMethod]
        public void Select_First_Or_Only_Occurrence_Of_Application()
        {
            var apdu = SelectCommand.Build
                .SelectFirstOrOnlyOccurrence()
                .Of(ApplicationAID)
                .AsApdu();

            apdu.Assert(ApduClass.Iso7816, ApduInstruction.Select, 0x04, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
        }

        [TestMethod]
        public void Select_Next_Occurrence_Of_Application()
        {
            var apdu = SelectCommand.Build
                .SelectNextOccurrence()
                .Of(ApplicationAID)
                .AsApdu();

            apdu.Assert(ApduClass.Iso7816, ApduInstruction.Select, 0x04, 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
        }
    }
}
