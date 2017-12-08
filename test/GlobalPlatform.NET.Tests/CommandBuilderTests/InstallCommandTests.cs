using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class InstallCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void Install_For_Load_To_Implicit_Destination()
        {
            var apdu = InstallCommand.Build
                .ForLoad()
                .Load(ExecutableLoadFileAID)
                .WithDataBlockHash(Hash)
                .WithParameters(InstallParameters)
                .WithToken(Token)
                .AsApdu();

            var commandData = new List<byte>();
            commandData.AddRangeWithLength(ExecutableLoadFileAID);
            commandData.Add(0x00);
            commandData.AddRangeWithLength(Hash);
            commandData.AddRangeWithLength(InstallParameters);
            commandData.AddRangeWithLength(Token);

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x02, 0x00, commandData.ToArray(), 0x00);
        }

        [TestMethod]
        public void Install_For_Load_To_Explicit_Destination()
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

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x02, 0x00, commandData.ToArray(), 0x00);
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

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x04, 0x00, commandData.ToArray(), 0x00);
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

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x08, 0x00, commandData.ToArray(), 0x00);
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

                .ForInstallAndMakeSelectable()
                .FromLoadFile(ExecutableLoadFileAID)
                .InstallModule(ExecutableModuleAID)
                .As(ApplicationAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)

                .AsApdus()
                .ToList();

            apdus.Count.Should().Be(2);
            apdus.Skip(0).First().Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x0E, 0x01);
            apdus.Skip(1).First().Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x0E, 0x03);
        }

        [TestMethod]
        public void Install_For_Install_For_Make_Selectable()
        {
            var apdu = InstallCommand.Build
                .ForInstallAndMakeSelectable()
                .FromLoadFile(ExecutableLoadFileAID)
                .InstallModule(ExecutableModuleAID)
                .As(ApplicationAID)
                .WithPrivileges(Privileges.Empty)
                .WithParameters(InstallParameters)
                .WithToken(Token)
                .AsApdu();

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x0C, 0x00);
        }

        [TestMethod]
        public void Install_For_Extradition()
        {
            var apdu = InstallCommand.Build
                .ForExtradition()
                .Extradite(ApplicationAID)
                .To(SecurityDomainAID)
                .WithParameters(InstallParameters)
                .WithToken(Token)
                .AsApdu();

            var commandData = new List<byte>();
            commandData.AddRangeWithLength(SecurityDomainAID);
            commandData.Add(0x00);
            commandData.AddRangeWithLength(ApplicationAID);
            commandData.Add(0x00);
            commandData.AddRangeWithLength(InstallParameters);
            commandData.AddRangeWithLength(Token);

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x10, 0x00, commandData.ToArray(), 0x00);
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

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x40, 0x00, commandData.ToArray(), 0x00);
        }

        [TestMethod]
        public void Install_For_Personalization()
        {
            var apdu = InstallCommand.Build
                .ForPersonalization()
                .Personalize(ApplicationAID)
                .AsApdu();

            var commandData = new List<byte> { 0x00, 0x00 };
            commandData.AddRangeWithLength(ApplicationAID);
            commandData.Add(0x00);
            commandData.Add(0x00);
            commandData.Add(0x00);

            apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Install, 0x20, 0x00, commandData.ToArray(), 0x00);
        }
    }
}
