using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class DeleteCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void Delete_Card_Content()
        {
            var apdu = DeleteCommand.Build
                .DeleteCardContent()
                .WithAID(ApplicationAID)
                .AsApdu();

            apdu.Assert(ApduInstruction.Delete, 0x00, 0x00, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
        }

        [TestMethod]
        public void Delete_Card_Content_And_Related_Objects()
        {
            var apdu = DeleteCommand.Build
                .DeleteCardContent()
                .WithAID(ApplicationAID)
                .AndRelatedObjects()
                .AsApdu();

            apdu.Assert(ApduInstruction.Delete, 0x00, 0x80, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
        }

        [TestMethod]
        public void Delete_Card_Content_And_Related_Objects_Using_Token()
        {
            var apdu = DeleteCommand.Build
                .DeleteCardContent()
                .WithAID(ApplicationAID)
                .AndRelatedObjects()
                .UsingToken(Token)
                .AsApdu();

            apdu.Assert(ApduInstruction.Delete, 0x00, 0x80, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x9E, 0x04, 0xEE, 0xEE, 0xEE, 0xEE);
        }

        [TestMethod]
        public void Delete_Key_Version_Number()
        {
            var apdu = DeleteCommand.Build
                .DeleteKey()
                .WithVersionNumber(0x6F)
                .AsApdu();

            apdu.Assert(ApduInstruction.Delete, 0x00, 0x00, 0xD2, 0x01, 0x6F);
        }

        [TestMethod]
        public void Delete_Key_Identifier()
        {
            var apdu = DeleteCommand.Build
                .DeleteKey()
                .WithIdentifier(0x0F)
                .AsApdu();

            apdu.Assert(ApduInstruction.Delete, 0x00, 0x00, 0xD0, 0x01, 0x0F);
        }

        [TestMethod]
        public void Delete_Key_Version_Number_And_Identifier()
        {
            var apdu = DeleteCommand.Build
                .DeleteKey()
                .WithVersionNumber(0x6F)
                .WithIdentifier(0x0F)
                .AsApdu();

            apdu.Assert(ApduInstruction.Delete, 0x00, 0x00, 0xD0, 0x01, 0x0F, 0xD2, 0x01, 0x6F);
        }
    }
}
