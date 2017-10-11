using FluentAssertions;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GlobalPlatform.NET.Tests.ApduTests
{
    [TestClass]
    public class CommandApduTests
    {
        [TestMethod]
        public void CommandApdu_Should_Fail_To_Build_When_Data_Exceeds_255_Bytes()
        {
            Action action = () =>
            {
                CommandApdu.Case3S(ApduClass.Iso7816, ApduInstruction.Select, 0x00, 0x00, new byte[256]);
            };

            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void CommandApdu_Should_Build_Case1()
        {
            var apdu = CommandApdu.Case1(ApduClass.Iso7816, ApduInstruction.Select, 0x00, 0x00);

            apdu.Buffer.ShouldAllBeEquivalentTo(new byte[] { 0x00, 0xA4, 0x00, 0x00 });
            apdu.CLA.Should().Be(ApduClass.Iso7816);
            apdu.INS.Should().Be(ApduInstruction.Select);
            apdu.P1.Should().Be(0x00);
            apdu.P2.Should().Be(0x00);
            apdu.Lc.Should().Be(0x00);
            apdu.CommandData.Should().BeEmpty();
            apdu.Le.Should().BeEmpty();
        }

        [TestMethod]
        public void CommandApdu_Should_Build_Case2S()
        {
            var apdu = CommandApdu.Case2S(ApduClass.Iso7816, ApduInstruction.Select, 0x00, 0x00, 0x00);

            apdu.Buffer.ShouldAllBeEquivalentTo(new byte[] { 0x00, 0xA4, 0x00, 0x00, 0x00 });
            apdu.CLA.Should().Be(ApduClass.Iso7816);
            apdu.INS.Should().Be(ApduInstruction.Select);
            apdu.P1.Should().Be(0x00);
            apdu.P2.Should().Be(0x00);
            apdu.Lc.Should().Be(0x00);
            apdu.CommandData.Should().BeEmpty();
            apdu.Le.ShouldBeEquivalentTo(new byte[] { 0x00 });
        }

        [TestMethod]
        public void CommandApdu_Should_Build_Case3S()
        {
            var apdu = CommandApdu.Case3S(ApduClass.Iso7816, ApduInstruction.Select, 0x00, 0x00, new byte[16]);

            apdu.Buffer.ShouldAllBeEquivalentTo(new byte[] { 0x00, 0xA4, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            apdu.CLA.Should().Be(ApduClass.Iso7816);
            apdu.INS.Should().Be(ApduInstruction.Select);
            apdu.P1.Should().Be(0x00);
            apdu.P2.Should().Be(0x00);
            apdu.Lc.Should().Be(0x10);
            apdu.CommandData.ShouldAllBeEquivalentTo(new byte[16]);
            apdu.Le.Should().BeEmpty();
        }

        [TestMethod]
        public void CommandApdu_Should_Build_Case4S()
        {
            var apdu = CommandApdu.Case4S(ApduClass.Iso7816, ApduInstruction.Select, 0x00, 0x00, new byte[16], 0x00);

            apdu.Buffer.ShouldAllBeEquivalentTo(new byte[] { 0x00, 0xA4, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            apdu.CLA.Should().Be(ApduClass.Iso7816);
            apdu.INS.Should().Be(ApduInstruction.Select);
            apdu.P1.Should().Be(0x00);
            apdu.P2.Should().Be(0x00);
            apdu.Lc.Should().Be(0x10);
            apdu.CommandData.ShouldAllBeEquivalentTo(new byte[16]);
            apdu.Le.ShouldBeEquivalentTo(new byte[] { 0x00 });
        }

        [TestMethod]
        public void CommandApdu_Should_ToString()
        {
            var apdu = CommandApdu.Case1(ApduClass.Iso7816, ApduInstruction.Select, 0x00, 0x00);

            apdu.ToString().Should().Be("00-A4-00-00");
            apdu.ToString("").Should().Be("00A40000");
            apdu.ToString("", null).Should().Be("00A40000");
        }
    }
}
