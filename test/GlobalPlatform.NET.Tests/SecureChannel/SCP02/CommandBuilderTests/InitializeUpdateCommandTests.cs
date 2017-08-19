using FluentAssertions;
using GlobalPlatform.NET.SecureChannel.SCP02.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

            apdu.Buffer.Take(4).Should().BeEquivalentTo(new byte[] { 0x80, 0x50, keyVersion, 0x00 });
            apdu.Lc.Should().Be(0x08);
            apdu.CommandData.Should().BeEquivalentTo(hostChallenge);
            apdu.Le.First().Should().Be(0x00);
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

            apdu.Buffer.Take(4).Should().BeEquivalentTo(new byte[] { 0x80, 0x50, keyVersion, 0x00 });
            apdu.Lc.Should().Be(0x08);
            apdu.CommandData.Should().BeEquivalentTo(hostChallenge);
            apdu.Le.First().Should().Be(0x00);
        }
    }
}
