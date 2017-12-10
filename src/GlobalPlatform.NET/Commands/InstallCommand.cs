using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using Iso7816;

namespace GlobalPlatform.NET.Commands
{
    public interface IInstallCommandForPicker : IInstallCommandForLoadPicker,
        IInstallCommandForInstallPicker,
        IInstallCommandForMakeSelectablePicker,
        IInstallCommandForInstallAndMakeSelectablePicker,
        IInstallCommandForExtraditionPicker,
        IInstallCommandForRegistryUpdatePicker,
        IInstallCommandForPersonalizationPicker
    {
    }

    public interface IInstallCommandForLoadPicker
    {
        IInstallCommandForLoadApplicationPicker ForLoad();
    }

    public interface IInstallCommandForLoadApplicationPicker
    {
        IInstallCommandForLoadSecurityDomainPicker Load(byte[] loadFileAID);
    }

    public interface IInstallCommandForLoadSecurityDomainPicker : IInstallCommandForLoadDataBlockHashPicker
    {
        IInstallCommandForLoadDataBlockHashPicker ToSecurityDomain(byte[] securityDomainAID);
    }

    public interface IInstallCommandForLoadDataBlockHashPicker : IInstallCommandForLoadParametersPicker, IInstallCommandForLoadTokenPicker
    {
        IInstallCommandForLoadParametersPicker WithDataBlockHash(byte[] hash);
    }

    public interface IInstallCommandForLoadParametersPicker : IInstallCommandForLoadBuilder
    {
        IInstallCommandForLoadTokenPicker WithParameters(byte[] parameters);
    }

    public interface IInstallCommandForLoadTokenPicker : IInstallCommandForInstallPicker, IApduBuilder
    {
        IInstallCommandForLoadBuilder WithToken(byte[] token);
    }

    public interface IInstallCommandForLoadBuilder : IInstallCommandForInstallAndMakeSelectablePicker, IApduBuilder
    {
    }

    public interface IInstallCommandForInstallPicker
    {
        IInstallCommandForInstallExecutableLoadFilePicker ForInstall();
    }

    public interface IInstallCommandForInstallExecutableLoadFilePicker
    {
        IInstallCommandForInstallExecutableModulePicker FromLoadFile(byte[] loadFileAID);
    }

    public interface IInstallCommandForInstallExecutableModulePicker
    {
        IInstallCommandForInstallApplicationPicker InstallModule(byte[] moduleAID);
    }

    public interface IInstallCommandForInstallApplicationPicker
    {
        IInstallCommandForInstallPrivilegesPicker As(byte[] applicationAID);
    }

    public interface IInstallCommandForInstallPrivilegesPicker
    {
        IInstallCommandForInstallParametersPicker WithPrivileges(Privileges privileges);

        IInstallCommandForInstallParametersPicker WithPrivileges(byte[] privileges);
    }

    public interface IInstallCommandForInstallParametersPicker : IInstallCommandForInstallTokenPicker
    {
        IInstallCommandForInstallTokenPicker WithParameters(byte[] parameters);
    }

    public interface IInstallCommandForInstallTokenPicker : IInstallCommandForInstallBuilder
    {
        IInstallCommandForInstallBuilder WithToken(byte[] token);
    }

    public interface IInstallCommandForInstallBuilder : IInstallCommandForMakeSelectablePicker, IApduBuilder
    {
    }

    public interface IInstallCommandForMakeSelectablePicker : IMultiApduBuilder

    {
        IInstallCommandForMakeSelectableApplicationPicker ForMakeSelectable();
    }

    public interface IInstallCommandForMakeSelectableApplicationPicker
    {
        IInstallCommandForMakeSelectablePrivilegesPicker ChangeApplication(byte[] applicationAID);
    }

    public interface IInstallCommandForMakeSelectablePrivilegesPicker
    {
        IInstallCommandForMakeSelectableParametersPicker WithPrivileges(Privileges privileges);

        IInstallCommandForMakeSelectableParametersPicker WithPrivileges(byte[] privileges);
    }

    public interface IInstallCommandForMakeSelectableParametersPicker : IInstallCommandForMakeSelectableTokenPicker
    {
        IInstallCommandForMakeSelectableTokenPicker WithParameters(byte[] parameters);
    }

    public interface IInstallCommandForMakeSelectableTokenPicker : IMultiApduBuilder
    {
        IInstallCommandForMakeSelectableBuilder WithToken(byte[] token);
    }

    public interface IInstallCommandForMakeSelectableBuilder : IApduBuilder, IMultiApduBuilder
    {
    }

    public interface IInstallCommandForInstallAndMakeSelectablePicker
    {
        IInstallCommandForInstallAndMakeSelectableExecutableLoadFilePicker ForInstallAndMakeSelectable();
    }

    public interface IInstallCommandForInstallAndMakeSelectableExecutableLoadFilePicker
    {
        IInstallCommandForInstallAndMakeSelectableExecutableModulePicker FromLoadFile(byte[] loadFileAID);
    }

    public interface IInstallCommandForInstallAndMakeSelectableExecutableModulePicker
    {
        IInstallCommandForInstallAndMakeSelectableApplicationPicker InstallModule(byte[] moduleAID);
    }

    public interface IInstallCommandForInstallAndMakeSelectableApplicationPicker
    {
        IInstallCommandForInstallAndMakeSelectablePrivilegesPicker As(byte[] applicationAID);
    }

    public interface IInstallCommandForInstallAndMakeSelectablePrivilegesPicker
    {
        IInstallCommandForInstallAndMakeSelectableParametersPicker WithPrivileges(Privileges privileges);

        IInstallCommandForInstallAndMakeSelectableParametersPicker WithPrivileges(byte[] privileges);
    }

    public interface IInstallCommandForInstallAndMakeSelectableParametersPicker : IInstallCommandForInstallAndMakeSelectableTokenPicker
    {
        IInstallCommandForInstallAndMakeSelectableTokenPicker WithParameters(byte[] parameters);
    }

    public interface IInstallCommandForInstallAndMakeSelectableTokenPicker : IInstallCommandForInstallAndMakeSelectableBuilder
    {
        IInstallCommandForInstallAndMakeSelectableBuilder WithToken(byte[] token);
    }

    public interface IInstallCommandForInstallAndMakeSelectableBuilder : IApduBuilder, IMultiApduBuilder
    {
    }

    public interface IInstallCommandForExtraditionPicker
    {
        IInstallCommandForExtraditionApplicationPicker ForExtradition();
    }

    public interface IInstallCommandForExtraditionApplicationPicker
    {
        IInstallCommandForExtraditionSecurityDomainPicker Extradite(byte[] applicationAID);
    }

    public interface IInstallCommandForExtraditionSecurityDomainPicker
    {
        IInstallCommandForExtraditionParametersPicker To(byte[] securityDomainAID);
    }

    public interface IInstallCommandForExtraditionParametersPicker : IInstallCommandForExtraditionTokenPicker
    {
        IInstallCommandForExtraditionTokenPicker WithParameters(byte[] parameters);
    }

    public interface IInstallCommandForExtraditionTokenPicker : IInstallCommandForExtraditionBuilder
    {
        IInstallCommandForExtraditionBuilder WithToken(byte[] token);
    }

    public interface IInstallCommandForExtraditionBuilder : IApduBuilder, IMultiApduBuilder
    {
    }

    public interface IInstallCommandForRegistryUpdatePicker
    {
        IInstallCommandForRegistryUpdateApplicationPicker ForRegistryUpdate();
    }

    public interface IInstallCommandForRegistryUpdateApplicationPicker
    {
        IInstallCommandForRegistryUpdateSecurityDomainPicker Extradite(byte[] applicationAID);
    }

    public interface IInstallCommandForRegistryUpdateSecurityDomainPicker
    {
        IInstallCommandForRegistryUpdatePrivilegesPicker To(byte[] securityDomainAID);
    }

    public interface IInstallCommandForRegistryUpdatePrivilegesPicker
    {
        IInstallCommandForRegistryUpdateParametersPicker WithPrivileges(Privileges privileges);

        IInstallCommandForRegistryUpdateParametersPicker WithPrivileges(byte[] privileges);
    }

    public interface IInstallCommandForRegistryUpdateParametersPicker : IInstallCommandForRegistryUpdateTokenPicker
    {
        IInstallCommandForRegistryUpdateTokenPicker WithParameters(byte[] parameters);
    }

    public interface IInstallCommandForRegistryUpdateTokenPicker : IApduBuilder
    {
        IApduBuilder WithToken(byte[] token);
    }

    public interface IInstallCommandForPersonalizationPicker
    {
        IInstallCommandForPersonalizationApplicationPicker ForPersonalization();
    }

    public interface IInstallCommandForPersonalizationApplicationPicker
    {
        IApduBuilder Personalize(byte[] applicationAID);
    }

    /// <summary>
    /// The INSTALL command is issued to a Security Domain to initiate or perform the various steps
    /// required for Card Content management.
    /// <para> Based on section 11.5 of the v2.3 GlobalPlatform Card Specification. </para>
    /// </summary>
    public class InstallCommand : MultiCommandBase<InstallCommand, IInstallCommandForPicker>,
        IInstallCommandForPicker,
        IInstallCommandForLoadApplicationPicker,
        IInstallCommandForLoadSecurityDomainPicker,
        IInstallCommandForInstallExecutableLoadFilePicker,
        IInstallCommandForInstallExecutableModulePicker,
        IInstallCommandForInstallApplicationPicker,
        IInstallCommandForInstallPrivilegesPicker,
        IInstallCommandForInstallParametersPicker,
        IInstallCommandForMakeSelectableApplicationPicker,
        IInstallCommandForMakeSelectablePrivilegesPicker,
        IInstallCommandForMakeSelectableParametersPicker,
        IInstallCommandForMakeSelectableBuilder,
        IInstallCommandForInstallAndMakeSelectableExecutableLoadFilePicker,
        IInstallCommandForInstallAndMakeSelectableExecutableModulePicker,
        IInstallCommandForInstallAndMakeSelectableApplicationPicker,
        IInstallCommandForInstallAndMakeSelectablePrivilegesPicker,
        IInstallCommandForInstallAndMakeSelectableParametersPicker,
        IInstallCommandForExtraditionApplicationPicker,
        IInstallCommandForExtraditionSecurityDomainPicker,
        IInstallCommandForExtraditionParametersPicker,
        IInstallCommandForRegistryUpdateApplicationPicker,
        IInstallCommandForRegistryUpdateSecurityDomainPicker,
        IInstallCommandForRegistryUpdatePrivilegesPicker,
        IInstallCommandForRegistryUpdateParametersPicker,
        IInstallCommandForPersonalizationApplicationPicker
    {
        private const byte forLoad = 0b00000010;
        private const byte forInstall = 0b00000100;
        private const byte forMakeSelectable = 0b00001000;
        private const byte forInstallAndMakeSelectable = 0b00001100;
        private const byte forExtradition = 0b00010000;
        private const byte forRegistryUpdate = 0b01000000;
        private const byte forPersonalization = 0b00100000;

        private byte[] forLoadLoadFileAID;
        private byte[] forLoadSecurityDomainAID = new byte[0];
        private byte[] forLoadDataBlockHash = new byte[0];
        private byte[] forLoadParameters = new byte[0];
        private byte[] forLoadToken = new byte[0];
        private byte[] forInstallExecutableLoadFileAID;
        private byte[] forInstallExecutableModuleAID;
        private byte[] forInstallApplicationAID;
        private byte[] forInstallPrivileges;
        private byte[] forInstallParameters = new byte[0];
        private byte[] forInstallToken = new byte[0];
        private byte[] forMakeSelectableApplicationAID;
        private byte[] forMakeSelectablePrivileges;
        private byte[] forMakeSelectableParameters = new byte[0];
        private byte[] forMakeSelectableToken = new byte[0];
        private byte[] forExtraditionApplicationAID;
        private byte[] forExtraditionSecurityDomainAID;
        private byte[] forExtraditionParameters = new byte[0];
        private byte[] forExtraditionToken = new byte[0];
        private byte[] forRegistryUpdateApplicationAID;
        private byte[] forRegistryUpdateSecurityDomainAID;
        private byte[] forRegistryUpdatePrivileges;
        private byte[] forRegistryUpdateParameters = new byte[0];
        private byte[] forRegistryUpdateToken = new byte[0];
        private byte[] forPersonalizationApplicationAID;

        public IInstallCommandForLoadApplicationPicker ForLoad()
        {
            this.P1 |= forLoad;

            return this;
        }

        public IInstallCommandForLoadSecurityDomainPicker Load(byte[] loadFileAID)
        {
            Ensure.IsAID(loadFileAID, nameof(loadFileAID));

            this.forLoadLoadFileAID = loadFileAID;

            return this;
        }

        public IInstallCommandForLoadDataBlockHashPicker ToSecurityDomain(byte[] securityDomainAID)
        {
            Ensure.IsNotNull(securityDomainAID, nameof(securityDomainAID));

            if (securityDomainAID.Any())
            {
                Ensure.IsAID(securityDomainAID, nameof(securityDomainAID));
            }

            this.forLoadSecurityDomainAID = securityDomainAID;

            return this;
        }

        public IInstallCommandForLoadParametersPicker WithDataBlockHash(byte[] hash)
        {
            Ensure.HasCount(hash, nameof(hash), 0, 0x7F);

            this.forLoadDataBlockHash = hash;

            return this;
        }

        public IInstallCommandForLoadTokenPicker WithParameters(byte[] parameters)
        {
            Ensure.IsNotNull(parameters, nameof(parameters));

            this.forLoadParameters = parameters;

            return this;
        }

        public IInstallCommandForLoadBuilder WithToken(byte[] token)
        {
            Ensure.IsNotNull(token, nameof(token));

            this.forLoadToken = token;

            return this;
        }

        public IInstallCommandForInstallAndMakeSelectableExecutableLoadFilePicker ForInstallAndMakeSelectable()
        {
            this.P1 |= forInstall;
            this.P1 |= forMakeSelectable;

            return this;
        }

        IInstallCommandForInstallAndMakeSelectableExecutableModulePicker IInstallCommandForInstallAndMakeSelectableExecutableLoadFilePicker.FromLoadFile(byte[] loadFileAID)
        {
            Ensure.IsAID(loadFileAID, nameof(loadFileAID));

            this.forInstallExecutableLoadFileAID = loadFileAID;

            return this;
        }

        IInstallCommandForInstallAndMakeSelectableApplicationPicker IInstallCommandForInstallAndMakeSelectableExecutableModulePicker.InstallModule(byte[] moduleAID)
        {
            Ensure.IsAID(moduleAID, nameof(moduleAID));

            this.forInstallExecutableModuleAID = moduleAID;

            return this;
        }

        IInstallCommandForInstallAndMakeSelectablePrivilegesPicker IInstallCommandForInstallAndMakeSelectableApplicationPicker.As(byte[] applicationAID)
        {
            Ensure.IsAID(applicationAID, nameof(applicationAID));

            this.forInstallApplicationAID = applicationAID;

            return this;
        }

        IInstallCommandForInstallAndMakeSelectableParametersPicker IInstallCommandForInstallAndMakeSelectablePrivilegesPicker.WithPrivileges(Privileges privileges)
            => (this as IInstallCommandForInstallAndMakeSelectablePrivilegesPicker).WithPrivileges(privileges.AsBytes());

        IInstallCommandForInstallAndMakeSelectableParametersPicker IInstallCommandForInstallAndMakeSelectablePrivilegesPicker.WithPrivileges(byte[] privileges)
        {
            Ensure.HasCount(privileges, nameof(privileges), 1, 3);

            this.forInstallPrivileges = privileges;

            return this;
        }

        IInstallCommandForInstallAndMakeSelectableTokenPicker IInstallCommandForInstallAndMakeSelectableParametersPicker.WithParameters(byte[] parameters)
        {
            Ensure.HasAtLeast(parameters, nameof(parameters), 2);

            this.forInstallParameters = parameters;

            return this;
        }

        IInstallCommandForInstallAndMakeSelectableBuilder IInstallCommandForInstallAndMakeSelectableTokenPicker.WithToken(byte[] token)
        {
            Ensure.IsNotNull(token, nameof(token));

            this.forInstallToken = token;

            return this;
        }

        public IInstallCommandForInstallExecutableLoadFilePicker ForInstall()
        {
            this.P1 |= forInstall;

            return this;
        }

        public IInstallCommandForInstallExecutableModulePicker FromLoadFile(byte[] loadFileAID)
        {
            Ensure.IsAID(loadFileAID, nameof(loadFileAID));

            this.forInstallExecutableLoadFileAID = loadFileAID;

            return this;
        }

        public IInstallCommandForInstallApplicationPicker InstallModule(byte[] moduleAID)
        {
            Ensure.IsAID(moduleAID, nameof(moduleAID));

            this.forInstallExecutableModuleAID = moduleAID;

            return this;
        }

        public IInstallCommandForInstallPrivilegesPicker As(byte[] applicationAID)
        {
            Ensure.IsAID(applicationAID, nameof(applicationAID));

            this.forInstallApplicationAID = applicationAID;

            return this;
        }

        public IInstallCommandForInstallParametersPicker WithPrivileges(Privileges privileges)
            => (this as IInstallCommandForInstallPrivilegesPicker).WithPrivileges(privileges.AsBytes());

        public IInstallCommandForInstallParametersPicker WithPrivileges(byte[] privileges)
        {
            Ensure.HasCount(privileges, nameof(privileges), 1, 3);

            this.forInstallPrivileges = privileges;

            return this;
        }

        IInstallCommandForInstallTokenPicker IInstallCommandForInstallParametersPicker.WithParameters(byte[] parameters)
        {
            Ensure.HasAtLeast(parameters, nameof(parameters), 2);

            this.forInstallParameters = parameters;

            return this;
        }

        IInstallCommandForInstallBuilder IInstallCommandForInstallTokenPicker.WithToken(byte[] token)
        {
            Ensure.IsNotNull(token, nameof(token));

            this.forInstallToken = token;

            return this;
        }

        public IInstallCommandForMakeSelectableApplicationPicker ForMakeSelectable()
        {
            this.P1 |= forMakeSelectable;

            return this;
        }

        public IInstallCommandForMakeSelectablePrivilegesPicker ChangeApplication(byte[] aid)
        {
            Ensure.IsAID(aid, nameof(aid));

            this.forMakeSelectableApplicationAID = aid;

            return this;
        }

        IInstallCommandForMakeSelectableParametersPicker IInstallCommandForMakeSelectablePrivilegesPicker.WithPrivileges(Privileges privileges)
            => (this as IInstallCommandForMakeSelectablePrivilegesPicker).WithPrivileges(privileges.AsBytes());

        IInstallCommandForMakeSelectableParametersPicker IInstallCommandForMakeSelectablePrivilegesPicker.WithPrivileges(byte[] privileges)
        {
            Ensure.HasCount(privileges, nameof(privileges), 1, 3);

            this.forMakeSelectablePrivileges = privileges;

            return this;
        }

        IInstallCommandForMakeSelectableTokenPicker IInstallCommandForMakeSelectableParametersPicker.WithParameters(byte[] parameters)
        {
            Ensure.IsNotNull(parameters, nameof(parameters));

            this.forMakeSelectableParameters = parameters;

            return this;
        }

        IInstallCommandForMakeSelectableBuilder IInstallCommandForMakeSelectableTokenPicker.WithToken(byte[] token)
        {
            Ensure.IsNotNull(token, nameof(token));

            this.forMakeSelectableToken = token;

            return this;
        }

        public IInstallCommandForExtraditionApplicationPicker ForExtradition()
        {
            this.P1 |= forExtradition;

            return this;
        }

        public IInstallCommandForExtraditionSecurityDomainPicker Extradite(byte[] applicationAid)
        {
            Ensure.IsAID(applicationAid, nameof(applicationAid));

            this.forExtraditionApplicationAID = applicationAid;

            return this;
        }

        public IInstallCommandForExtraditionParametersPicker To(byte[] securityDomainAid)
        {
            Ensure.IsAID(securityDomainAid, nameof(securityDomainAid));

            this.forExtraditionSecurityDomainAID = securityDomainAid;

            return this;
        }

        IInstallCommandForExtraditionTokenPicker IInstallCommandForExtraditionParametersPicker.WithParameters(byte[] parameters)
        {
            Ensure.IsNotNull(parameters, nameof(parameters));

            this.forExtraditionParameters = parameters;

            return this;
        }

        IInstallCommandForExtraditionBuilder IInstallCommandForExtraditionTokenPicker.WithToken(byte[] token)
        {
            Ensure.IsNotNull(token, nameof(token));

            this.forExtraditionToken = token;

            return this;
        }

        public IInstallCommandForRegistryUpdateApplicationPicker ForRegistryUpdate()
        {
            this.P1 |= forRegistryUpdate;

            return this;
        }

        IInstallCommandForRegistryUpdateSecurityDomainPicker IInstallCommandForRegistryUpdateApplicationPicker.Extradite(byte[] applicationAID)
        {
            Ensure.IsNotNull(applicationAID, nameof(applicationAID));

            if (applicationAID.Any())
            {
                Ensure.IsAID(applicationAID, nameof(applicationAID));
            }

            this.forRegistryUpdateApplicationAID = applicationAID;

            return this;
        }

        IInstallCommandForRegistryUpdatePrivilegesPicker IInstallCommandForRegistryUpdateSecurityDomainPicker.To(byte[] securityDomainAID)
        {
            Ensure.IsNotNull(securityDomainAID, nameof(securityDomainAID));

            if (securityDomainAID.Any())
            {
                Ensure.IsAID(securityDomainAID, nameof(securityDomainAID));
            }

            this.forRegistryUpdateSecurityDomainAID = securityDomainAID;

            return this;
        }

        IInstallCommandForRegistryUpdateParametersPicker IInstallCommandForRegistryUpdatePrivilegesPicker.WithPrivileges(Privileges privileges)
            => (this as IInstallCommandForRegistryUpdatePrivilegesPicker).WithPrivileges(privileges.AsBytes());

        IInstallCommandForRegistryUpdateParametersPicker IInstallCommandForRegistryUpdatePrivilegesPicker.WithPrivileges(byte[] privileges)
        {
            Ensure.HasCount(privileges, nameof(privileges), 0, 1, 3);

            this.forRegistryUpdatePrivileges = privileges;

            return this;
        }

        IInstallCommandForRegistryUpdateTokenPicker IInstallCommandForRegistryUpdateParametersPicker.WithParameters(byte[] parameters)
        {
            Ensure.IsNotNull(parameters, nameof(parameters));

            this.forRegistryUpdateParameters = parameters;

            return this;
        }

        IApduBuilder IInstallCommandForRegistryUpdateTokenPicker.WithToken(byte[] token)
        {
            Ensure.IsNotNull(token, nameof(token));

            this.forRegistryUpdateToken = token;

            return this;
        }

        public IInstallCommandForPersonalizationApplicationPicker ForPersonalization()
        {
            this.P1 |= forPersonalization;

            return this;
        }

        public IApduBuilder Personalize(byte[] applicationAID)
        {
            Ensure.IsAID(applicationAID, nameof(applicationAID));

            this.forPersonalizationApplicationAID = applicationAID;

            return this;
        }

        public CommandApdu AsApdu()
        {
            switch (this.P1)
            {
                case forLoad:
                    return this.BuildForLoad();

                case forInstall:
                    return this.BuildForInstall();

                case forMakeSelectable:
                    return this.BuildForMakeSelectable();

                case forInstallAndMakeSelectable:
                    return this.BuildForInstallAndMakeSelectable();

                case forExtradition:
                    return this.BuildForExtradition();

                case forRegistryUpdate:
                    return this.BuildForRegistryUpdate();

                case forPersonalization:
                    return this.BuildForPersonalization();

                default:
                    throw new InvalidOperationException("The current value of P1 suggests multiple commands must be generated.");
            }
        }

        public new byte[] AsBytes() => this.AsApdu().Buffer.ToArray();

        public override IEnumerable<CommandApdu> AsApdus()
        {
            if (this.HasOption(forLoad))
            {
                yield return this.BuildForLoad();
            }

            if (this.HasOption(forInstall) && this.HasOption(forMakeSelectable))
            {
                yield return this.BuildForInstallAndMakeSelectable();
            }
            else
            {
                if (this.HasOption(forInstall))
                {
                    yield return this.BuildForInstall();
                }

                if (this.HasOption(forMakeSelectable))
                {
                    yield return this.BuildForMakeSelectable();
                }
            }

            if (this.HasOption(forExtradition))
            {
                yield return this.BuildForExtradition();
            }

            if (this.HasOption(forRegistryUpdate))
            {
                yield return this.BuildForRegistryUpdate();
            }

            if (this.HasOption(forPersonalization))
            {
                yield return this.BuildForPersonalization();
            }
        }

        private CommandApdu BuildForLoad()
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forLoadLoadFileAID);
            commandData.AddRangeWithLength(this.forLoadSecurityDomainAID);
            commandData.AddRangeWithLength(this.forLoadDataBlockHash);
            commandData.AddRangeWithLength(this.forLoadParameters);
            commandData.AddRangeWithLength(this.forLoadToken);

            byte p2 = (byte)(this.HasOption(forInstall) && this.HasOption(forMakeSelectable) ? 0x01 : 0x00);

            return this.Build(commandData, p2);
        }

        private CommandApdu BuildForInstall()
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forInstallExecutableLoadFileAID);
            commandData.AddRangeWithLength(this.forInstallExecutableModuleAID);
            commandData.AddRangeWithLength(this.forInstallApplicationAID);
            commandData.AddRangeWithLength(this.forInstallPrivileges);
            commandData.AddRangeWithLength(this.forInstallParameters);
            commandData.AddRangeWithLength(this.forInstallToken);

            return this.Build(commandData);
        }

        private CommandApdu BuildForMakeSelectable()
        {
            var commandData = new List<byte> { 0x00, 0x00 };

            commandData.AddRangeWithLength(this.forMakeSelectableApplicationAID);
            commandData.AddRangeWithLength(this.forMakeSelectablePrivileges);
            commandData.AddRangeWithLength(this.forMakeSelectableParameters);
            commandData.AddRangeWithLength(this.forMakeSelectableToken);

            return this.Build(commandData);
        }

        private CommandApdu BuildForInstallAndMakeSelectable()
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forInstallExecutableLoadFileAID);
            commandData.AddRangeWithLength(this.forInstallExecutableModuleAID);
            commandData.AddRangeWithLength(this.forInstallApplicationAID);
            commandData.AddRangeWithLength(this.forInstallPrivileges);
            commandData.AddRangeWithLength(this.forInstallParameters);
            commandData.AddRangeWithLength(this.forInstallToken);

            byte p2 = (byte)(this.HasOption(forLoad) ? 0x03 : 0x00);

            return this.Build(commandData, p2);
        }

        private CommandApdu BuildForExtradition()
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forExtraditionSecurityDomainAID);
            commandData.Add(0x00);
            commandData.AddRangeWithLength(this.forExtraditionApplicationAID);
            commandData.Add(0x00);
            commandData.AddRangeWithLength(this.forExtraditionParameters);
            commandData.AddRangeWithLength(this.forExtraditionToken);

            return this.Build(commandData);
        }

        private CommandApdu BuildForRegistryUpdate()
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forRegistryUpdateSecurityDomainAID);
            commandData.AddRangeWithLength(this.forRegistryUpdateApplicationAID);
            commandData.AddRangeWithLength(this.forRegistryUpdatePrivileges);
            commandData.AddRangeWithLength(this.forRegistryUpdateParameters);
            commandData.AddRangeWithLength(this.forRegistryUpdateToken);

            return this.Build(commandData);
        }

        private CommandApdu BuildForPersonalization()
        {
            var commandData = new List<byte> { 0x00, 0x00 };

            commandData.AddRangeWithLength(this.forPersonalizationApplicationAID);
            commandData.Add(0x00);
            commandData.Add(0x00);
            commandData.Add(0x00);

            return this.Build(commandData);
        }

        private new CommandApdu Build(IEnumerable<byte> commandData, byte p2 = 0x00)
        {
            return CommandApdu.Case4S(ApduClass.GlobalPlatform, ApduInstruction.Install, this.P1, p2, commandData.ToArray(), 0x00);
        }

        private bool HasOption(byte option)
        {
            return (this.P1 & option) == option;
        }
    }
}
