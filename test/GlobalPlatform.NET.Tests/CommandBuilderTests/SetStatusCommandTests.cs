using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class SetStatusCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void SetStatus_Of_Issuer_Security_Domain()
        {
            var apdu = SetStatusCommand.Build
                .SetIssuerSecurityDomainStatus()
                .To(CardLifeCycleCoding.Initialized)
                .AsApdu();

            apdu.Assert(ApduInstruction.SetStatus, 0x80, 0x07);
        }

        [TestMethod]
        public void SetStatus_Of_Security_Domain()
        {
            var apdu = SetStatusCommand.Build
                .SetSecurityDomainStatus()
                .To(SecurityDomainLifeCycleCoding.Personalized)
                .For(ApplicationAID)
                .AsApdu();

            apdu.Assert(ApduInstruction.SetStatus, 0x40, 0x0F, ApplicationAID, new byte[0]);
        }

        [TestMethod]
        public void SetStatus_Of_Application()
        {
            var apdu = SetStatusCommand.Build
                .SetApplicationStatus()
                .To(ApplicationLifeCycleCoding.Selectable)
                .For(ApplicationAID)
                .AsApdu();

            apdu.Assert(ApduInstruction.SetStatus, 0x60, 0x07, ApplicationAID, new byte[0]);
        }
    }
}
