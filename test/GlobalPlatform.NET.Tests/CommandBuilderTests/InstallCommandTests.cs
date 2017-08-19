using FluentAssertions;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class InstallCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void Install_For_Load()
        {
            var apdu = InstallCommand.Build
                .ForLoad()
                .Load(ExecutableLoadFileAID)
                .ToSecurityDomain(SecurityDomainAID)
                .WithDataBlockHash(Hash)
                .WithParameters(InstallParameters)
                .WithToken(Token)
                .AsApdu();

            var commandData = new List<byte>();
            commandData.AddRangeWithLength(ExecutableLoadFileAID);
            commandData.AddRangeWithLength(SecurityDomainAID);
            commandData.AddRangeWithLength(Hash);
            commandData.AddRangeWithLength(InstallParameters);
            commandData.AddRangeWithLength(Token);

            apdu.Assert(ApduInstruction.Install, 0x02, 0x00, commandData.ToArray());
        }

        [TestMethod]
        public void Install_For_Install()
        {
            var apdu = InstallCommand.Build
                .ForInstall()
                .FromLoadFile(ExecutableLoadFileAID)
                .InstallModule(ExecutableModuleAID)
                .As(ApplicationAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)
                .AsApdu();

            var commandData = new List<byte>();
            commandData.AddRangeWithLength(ExecutableLoadFileAID);
            commandData.AddRangeWithLength(ExecutableModuleAID);
            commandData.AddRangeWithLength(ApplicationAID);
            commandData.AddRangeWithLength(Privileges.Empty);
            commandData.AddRangeWithLength(InstallParameters);
            commandData.AddRangeWithLength(Token);

            apdu.Assert(ApduInstruction.Install, 0x04, 0x00, commandData.ToArray());
        }

        [TestMethod]
        public void Install_For_Load_For_Install()
        {
            var apdus = InstallCommand.Build
                .ForLoad()
                .Load(ExecutableLoadFileAID)
                .ToSecurityDomain(SecurityDomainAID)
                .WithDataBlockHash(Hash)
                .WithParameters(InstallParameters)
                .WithToken(Token)

                .ForInstall()
                .FromLoadFile(ExecutableLoadFileAID)
                .InstallModule(ExecutableModuleAID)
                .As(ApplicationAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)

                .AsApdus()
                .ToList();

            apdus.Count.Should().Be(2);
            apdus.Skip(0).First().Assert(ApduInstruction.Install, 0x02, 0x80);
            apdus.Skip(1).First().Assert(ApduInstruction.Install, 0x04, 0x00);
        }

        [TestMethod]
        public void Install_For_MakeSelectable()
        {
            var apdu = InstallCommand.Build
                .ForMakeSelectable()
                .ChangeApplication(ApplicationAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)
                .AsApdu();

            var commandData = new List<byte> { 0x00, 0x00 };
            commandData.AddRangeWithLength(ApplicationAID);
            commandData.AddRangeWithLength(Privileges.Empty);
            commandData.AddRangeWithLength(InstallParameters);
            commandData.AddRangeWithLength(Token);

            apdu.Assert(ApduInstruction.Install, 0x08, 0x00, commandData.ToArray());
        }

        [TestMethod]
        public void Install_For_Load_For_Install_For_Make_Selectable()
        {
            var apdus = InstallCommand.Build
                .ForLoad()
                .Load(ExecutableLoadFileAID)
                .ToSecurityDomain(SecurityDomainAID)
                .WithDataBlockHash(Hash)
                .WithParameters(InstallParameters)
                .WithToken(Token)

                .ForInstall()
                .FromLoadFile(ExecutableLoadFileAID)
                .InstallModule(ExecutableModuleAID)
                .As(ApplicationAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)

                .ForMakeSelectable()
                .ChangeApplication(ApplicationAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)

                .AsApdus()
                .ToList();

            apdus.Count.Should().Be(3);
            apdus.Skip(0).First().Assert(ApduInstruction.Install, 0x02, 0x80);
            apdus.Skip(1).First().Assert(ApduInstruction.Install, 0x04, 0x80);
            apdus.Skip(2).First().Assert(ApduInstruction.Install, 0x08, 0x00);
        }

        [TestMethod]
        public void Install_For_Install_For_Make_Selectable()
        {
            var apdus = InstallCommand.Build
                .ForInstall()
                .FromLoadFile(ExecutableLoadFileAID)
                .InstallModule(ExecutableModuleAID)
                .As(ApplicationAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)

                .ForMakeSelectable()
                .ChangeApplication(ApplicationAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)

                .AsApdus()
                .ToList();

            apdus.Count.Should().Be(2);
            apdus.Skip(0).First().Assert(ApduInstruction.Install, 0x04, 0x80);
            apdus.Skip(1).First().Assert(ApduInstruction.Install, 0x08, 0x00);
        }

        [TestMethod]
        public void Install_For_Extradition()
        {
            var apdu = InstallCommand.Build
                .ForExtradition()
                .Extradite(ApplicationAID)
                .To(SecurityDomainAID)
                .WithParameters(InstallParameters)
                .AsApdu();

            var commandData = new List<byte>();
            commandData.AddRangeWithLength(SecurityDomainAID);
            commandData.AddRangeWithLength(ApplicationAID);
            commandData.Add(0x00);
            commandData.AddRangeWithLength(InstallParameters);

            apdu.Assert(ApduInstruction.Install, 0x10, 0x00, commandData.ToArray());
        }

        [TestMethod]
        public void Install_For_Registry_Update()
        {
            var apdu = InstallCommand.Build
                .ForRegistryUpdate()
                .Extradite(ApplicationAID)
                .To(SecurityDomainAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)
                .AsApdu();

            var commandData = new List<byte>();
            commandData.AddRangeWithLength(SecurityDomainAID);
            commandData.AddRangeWithLength(ApplicationAID);
            commandData.AddRangeWithLength(Privileges.Empty);
            commandData.AddRangeWithLength(InstallParameters);
            commandData.AddRangeWithLength(Token);

            apdu.Assert(ApduInstruction.Install, 0x40, 0x00, commandData.ToArray());
        }

        [TestMethod]
        public void Install_For_Personalization()
        {
            var apdu = InstallCommand.Build
                .ForPersonalization()
                .Personalize(ApplicationAID)
                .AsApdu();

            var commandData = new List<byte>();
            commandData.AddRangeWithLength(ApplicationAID);

            apdu.Assert(ApduInstruction.Install, 0x20, 0x00, commandData.ToArray());
        }
    }
}
