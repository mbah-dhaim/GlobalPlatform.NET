using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class GetDataCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void GetData_List_Applications()
        {
            var apdu = GetDataCommand.Build
                .GetDataFrom(DataObject.ListApplications)
                .WithTagList(0x5C, 0x00)
                .AsApdu();

            apdu.Assert(ApduInstruction.GetData, 0x2F, 0x00, 0x5C, 0x00);
        }

        [TestMethod]
        public void GetData_Key_Information_Template()
        {
            var apdu = GetDataCommand.Build
                .GetDataFrom(DataObject.KeyInformationTemplate)
                .AsApdu();

            apdu.Assert(ApduInstruction.GetData, 0x00, 0xE0, 0x00);
        }
    }
}
