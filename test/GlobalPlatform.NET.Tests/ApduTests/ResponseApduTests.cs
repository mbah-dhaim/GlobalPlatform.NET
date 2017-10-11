using System;
using FluentAssertions;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.ApduTests
{
    [TestClass]
    public class ResponseApduTests
    {
        [TestMethod]
        public void ResponseApdu_Should_Fail_To_Build_When_Buffer_Exceeds_256_Plus_2_Bytes()
        {
            Action action = () =>
            {
                ResponseApdu.Build(new byte[259]);
            };

            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void ResponseApdu_Should_Build()
        {
            var apdu = ResponseApdu.Build(new byte[] { 0x00, 0x90, 0x00 });

            apdu.Buffer.ShouldAllBeEquivalentTo(new byte[] { 0x00, 0x90, 0x00 });
            apdu.Data.ShouldAllBeEquivalentTo(new byte[] { 0x00 });
            apdu.SW1.Should().Be(0x90);
            apdu.SW2.Should().Be(0x00);
            apdu.Status.Should().Be(ApduStatus.NoFurtherQualification);
        }

        [TestMethod]
        public void ResponseApdu_Should_ToString()
        {
            var apdu = ResponseApdu.Build(new byte[] { 0x00, 0x90, 0x00 });

            apdu.ToString().Should().Be("00-90-00");
            apdu.ToString("").Should().Be("009000");
            apdu.ToString("", null).Should().Be("009000");
        }

        [TestMethod]
        public void ResponseApdu_Status_Should_Ignore_SW2_When_Appropriate()
        {
            var apdu = ResponseApdu.Build(new byte[] { 0x61, 0x10 });

            apdu.Status.Should().Be(ApduStatus.DataAvailable);
        }

        [TestMethod]
        public void ResponseApdu_GetStatus_Should_Report_Info_When_Appropriate()
        {
            var apdu = ResponseApdu.Build(new byte[] { 0x61, 0x10 });

            apdu.GetStatus(out byte info).Should().Be(ApduStatus.DataAvailable);
            info.Should().Be(0x10);
        }
    }
}
