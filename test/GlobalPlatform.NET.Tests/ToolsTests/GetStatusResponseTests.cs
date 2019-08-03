using FluentAssertions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GlobalPlatform.NET.Tests.ToolsTests
{
    [TestClass]
    public class GetStatusResponseTests
    {
        [TestMethod]
        public void GetStatusResponse_Should_Parse_For_ISD_Scope()
        {
            byte[] response =
            {
                0xE3, 0x11, 0x4F, 0x08, 0xA0, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x9F, 0x70, 0x01, 0x01, 0xC5,
                0x01, 0x9E
            };

            var status = GetStatusResponse.Analyze
                .WithScopeOf()
                .IssuerSecurityDomain()
                .Parse(response);

            status.AID.Should().BeEquivalentTo(new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00 },
                o => o.WithStrictOrderingFor(x => x));
            status.LifeCycleState.Should().Be((byte)CardLifeCycleCoding.OpReady);
            status.Privileges.Should().BeEquivalentTo(0x9E);
            status.ExecutableLoadFileAID.Should().BeNull();
            status.AssociatedSecurityDomainAID.Should().BeNull();
        }

        [TestMethod]
        public void GetStatusResponse_Should_Parse_For_Applications_Scope()
        {
            byte[] response =
            {
                0xE3, 0x0E, 0x4F, 0x05, 0xA0, 0x00, 0x00, 0x00, 0x01, 0x9F, 0x70, 0x01, 0x07, 0xC5, 0x01, 0x00,
                0xE3, 0x0E, 0x4F, 0x05, 0xA0, 0x00, 0x00, 0x00, 0x81, 0x9F, 0x70, 0x01, 0x07, 0xC5, 0x01, 0x80
            };

            var status = GetStatusResponse.Analyze
                .WithScopeOf()
                .Applications()
                .Parse(response)
                .ToList();

            status.Count.Should().Be(2);

            status.First().AID.Should().BeEquivalentTo(
                new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x01 },
                o => o.WithStrictOrderingFor(x => x));
            status.First().LifeCycleState.Should().Be((byte)ApplicationLifeCycleCoding.Selectable);
            status.First().Privileges.Should().BeEquivalentTo(Privileges.Empty);
            status.First().ExecutableLoadFileAID.Should().BeNull();
            status.First().AssociatedSecurityDomainAID.Should().BeNull();

            status.Last().AID.Should().BeEquivalentTo(
                new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x81 },
                o => o.WithStrictOrderingFor(x => x));
            status.Last().LifeCycleState.Should().Be((byte)ApplicationLifeCycleCoding.Selectable);
            status.Last().Privileges.Should().BeEquivalentTo(Privileges.SecurityDomain);
            status.Last().ExecutableLoadFileAID.Should().BeNull();
            status.Last().AssociatedSecurityDomainAID.Should().BeNull();
        }

        [TestMethod]
        public void GetStatusResponse_Should_Parse_For_ExecutableLoadFiles_Scope()
        {
            byte[] response =
            {
                0xE3, 0x17,
                0x4F, 0x07, 0xA0, 0x00, 0x00, 0x00, 0x03, 0x53, 0x50,
                0x9F, 0x70, 0x01, 0x01,
                0x84, 0x08, 0xA0, 0x00, 0x00, 0x00, 0x03, 0x53, 0x50, 0x41
            };

            var status = GetStatusResponse.Analyze
                .WithScopeOf()
                .ExecutableLoadFiles()
                .Parse(response)
                .ToList();

            status.Count.Should().Be(1);

            status.First().AID.Should().BeEquivalentTo(
                new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03, 0x53, 0x50 },
                o => o.WithStrictOrderingFor(x => x));
            status.First().LifeCycleState.Should().Be((byte)ExecutableLoadFileLifeCycleCoding.Installed);
            status.First().ExecutableLoadFileVersionNumber.Should().BeNull();
            status.First().ExecutableModuleAIDs.Count.Should().Be(1);
            status.First().ExecutableModuleAIDs.First().Should().BeEquivalentTo(
                new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x03, 0x53, 0x50, 0x41 },
                o => o.WithStrictOrderingFor(x => x));
            status.First().AssociatedSecurityDomainAID.Should().BeNull();
        }
    }
}
