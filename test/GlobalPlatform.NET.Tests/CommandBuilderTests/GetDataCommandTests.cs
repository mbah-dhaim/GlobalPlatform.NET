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
                .FromDataObject(DataObject.ApplicationList)
                .WithTagList(0x5C, 0x00)
                .AsApdu();

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.GetData, 0x2F, 0x00, 0x5C, 0x00);
        }

        [TestMethod]
        public void GetData_Key_Information_Template()
        {
            var apdu = GetDataCommand.Build
                .FromDataObject(DataObject.KeyInformationTemplate)
                .AsApdu();

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.GetData, 0x00, 0xE0, 0x00);
        }

        [TestMethod]
        public void GetData_Custom_P1_P2()
        {
            var apdu = GetDataCommand.Build
                .UsingP1(0x9F)
                .UsingP2(0x7F)
                .AsApdu();

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.GetData, 0x9F, 0x7F, 0x00);
        }
    }
}
