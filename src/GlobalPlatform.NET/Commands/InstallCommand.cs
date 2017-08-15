using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Commands
{
    public interface IInstallCommandForLoadPicker : IInstallCommandForInstallPicker
    {
        IInstallCommandForLoadAidPicker ForLoad();
    }

    public interface IInstallCommandForLoadAidPicker
    {
        IInstallCommandForLoadSecurityDomainAidPicker Load(byte[] applicationAID);
    }

    public interface IInstallCommandForLoadSecurityDomainAidPicker
    {
        IInstallCommandForLoadDataBlockHashPicker ToSecurityDomain(byte[] securityDomainAID);
    }

    public interface IInstallCommandForLoadDataBlockHashPicker : IInstallCommandForInstallPicker
    {
        IInstallCommandForLoadParametersPicker WithDataBlockHash(byte[] hash);
    }

    public interface IInstallCommandForLoadParametersPicker : IInstallCommandForInstallPicker
    {
        IInstallCommandForLoadTokenPicker WithParameters(byte[] parameters);
    }

    public interface IInstallCommandForLoadTokenPicker : IInstallCommandForInstallPicker, IApduBuilder
    {
        IInstallCommandForInstallPicker WithToken(byte[] token);
    }

    public interface IInstallCommandForInstallPicker : IInstallCommandForMakeSelectablePicker
    {
        IInstallCommandForInstallExecutableLoadFilePicker ForInstall();
    }

    public interface IInstallCommandForInstallExecutableLoadFilePicker
    {
        IInstallCommandForInstallExecutableModulePicker InstallFromLoadFile(byte[] loadFileAID);
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

    public interface IInstallCommandForInstallTokenPicker
    {
        IInstallCommandForMakeSelectablePicker WithToken(byte[] token);
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
        IInstallCommandForMakeSelectablePicker WithToken(byte[] token);
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

    public interface IInstallCommandForExtraditionTokenPicker : IApduBuilder
    {
        IMultiApduBuilder WithToken(byte[] token);
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
        IMultiApduBuilder WithToken(byte[] token);
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
    public class InstallCommand : MultiCommandBase<InstallCommand, IInstallCommandForLoadPicker>,
        IInstallCommandForLoadPicker,
        IInstallCommandForExtraditionPicker,
        IInstallCommandForRegistryUpdatePicker,
        IInstallCommandForPersonalizationPicker,
        IInstallCommandForLoadAidPicker,
        IInstallCommandForLoadSecurityDomainAidPicker,
        IInstallCommandForLoadDataBlockHashPicker,
        IInstallCommandForLoadParametersPicker,
        IInstallCommandForLoadTokenPicker,
        IInstallCommandForInstallExecutableLoadFilePicker,
        IInstallCommandForInstallExecutableModulePicker,
        IInstallCommandForInstallApplicationPicker,
        IInstallCommandForInstallPrivilegesPicker,
        IInstallCommandForInstallParametersPicker,
        IInstallCommandForMakeSelectableApplicationPicker,
        IInstallCommandForMakeSelectablePrivilegesPicker,
        IInstallCommandForMakeSelectableParametersPicker,
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
        private const byte forExtradition = 0b00010000;
        private const byte forRegistryUpdate = 0b01000000;
        private const byte forPersonalization = 0b00100000;

        private byte[] forLoadAID;
        private byte[] forLoadSecurityDomainAID;
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

        public IInstallCommandForLoadAidPicker ForLoad()
        {
            this.P1 |= forLoad;

            return this;
        }

        public IInstallCommandForLoadSecurityDomainAidPicker Load(byte[] aid)
        {
            Ensure.IsAID(aid, nameof(aid));

            this.forLoadAID = aid;

            return this;
        }

        public IInstallCommandForLoadDataBlockHashPicker ToSecurityDomain(byte[] aid)
        {
            Ensure.IsAID(aid, nameof(aid));

            this.forLoadSecurityDomainAID = aid;

            return this;
        }

        public IInstallCommandForLoadParametersPicker WithDataBlockHash(byte[] hash)
        {
            Ensure.IsNotNullOrEmpty(hash, nameof(hash));

            this.forLoadDataBlockHash = hash;

            return this;
        }

        public IInstallCommandForLoadTokenPicker WithParameters(byte[] parameters)
        {
            Ensure.IsNotNullOrEmpty(parameters, nameof(parameters));

            this.forLoadParameters = parameters;

            return this;
        }

        public IInstallCommandForInstallPicker WithToken(byte[] token)
        {
            Ensure.IsNotNullOrEmpty(token, nameof(token));

            this.forLoadToken = token;

            return this;
        }

        public IInstallCommandForInstallExecutableLoadFilePicker ForInstall()
        {
            this.P1 |= forInstall;

            return this;
        }

        public IInstallCommandForInstallExecutableModulePicker InstallFromLoadFile(byte[] loadFileAID)
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
            Ensure.IsNotNullOrEmpty(privileges, nameof(privileges));

            this.forInstallPrivileges = privileges;

            return this;
        }

        IInstallCommandForInstallTokenPicker IInstallCommandForInstallParametersPicker.WithParameters(byte[] parameters)
        {
            Ensure.IsNotNullOrEmpty(parameters, nameof(parameters));

            this.forInstallParameters = parameters;

            return this;
        }

        IInstallCommandForMakeSelectablePicker IInstallCommandForInstallTokenPicker.WithToken(byte[] token)
        {
            Ensure.IsNotNullOrEmpty(token, nameof(token));

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
            Ensure.IsNotNullOrEmpty(privileges, nameof(privileges));

            this.forMakeSelectablePrivileges = privileges;

            return this;
        }

        IInstallCommandForMakeSelectableTokenPicker IInstallCommandForMakeSelectableParametersPicker.WithParameters(byte[] parameters)
        {
            Ensure.IsNotNullOrEmpty(parameters, nameof(parameters));

            this.forMakeSelectableParameters = parameters;

            return this;
        }

        IInstallCommandForMakeSelectablePicker IInstallCommandForMakeSelectableTokenPicker.WithToken(byte[] token)
        {
            Ensure.IsNotNullOrEmpty(token, nameof(token));

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
            Ensure.IsNotNullOrEmpty(parameters, nameof(parameters));

            this.forExtraditionParameters = parameters;

            return this;
        }

        IMultiApduBuilder IInstallCommandForExtraditionTokenPicker.WithToken(byte[] token)
        {
            Ensure.IsNotNullOrEmpty(token, nameof(token));

            this.forExtraditionToken = token;

            return this;
        }

        public IInstallCommandForRegistryUpdateApplicationPicker ForRegistryUpdate()
        {
            this.P1 |= forRegistryUpdate;

            return this;
        }

        IInstallCommandForRegistryUpdateSecurityDomainPicker IInstallCommandForRegistryUpdateApplicationPicker.Extradite(byte[] applicationAid)
        {
            Ensure.IsAID(applicationAid, nameof(applicationAid));

            this.forRegistryUpdateApplicationAID = applicationAid;

            return this;
        }

        IInstallCommandForRegistryUpdatePrivilegesPicker IInstallCommandForRegistryUpdateSecurityDomainPicker.To(byte[] securityDomainAid)
        {
            Ensure.IsAID(securityDomainAid, nameof(securityDomainAid));

            this.forRegistryUpdateSecurityDomainAID = securityDomainAid;

            return this;
        }

        IInstallCommandForRegistryUpdateParametersPicker IInstallCommandForRegistryUpdatePrivilegesPicker.WithPrivileges(Privileges privileges)
            => (this as IInstallCommandForRegistryUpdatePrivilegesPicker).WithPrivileges(privileges.AsBytes());

        IInstallCommandForRegistryUpdateParametersPicker IInstallCommandForRegistryUpdatePrivilegesPicker.WithPrivileges(byte[] privileges)
        {
            Ensure.IsNotNullOrEmpty(privileges, nameof(privileges));

            this.forRegistryUpdatePrivileges = privileges;

            return this;
        }

        IInstallCommandForRegistryUpdateTokenPicker IInstallCommandForRegistryUpdateParametersPicker.WithParameters(byte[] parameters)
        {
            Ensure.IsNotNullOrEmpty(parameters, nameof(parameters));

            this.forRegistryUpdateParameters = parameters;

            return this;
        }

        IMultiApduBuilder IInstallCommandForRegistryUpdateTokenPicker.WithToken(byte[] token)
        {
            Ensure.IsNotNullOrEmpty(token, nameof(token));

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

        public Apdu AsApdu()
        {
            switch (this.P1)
            {
                case forLoad:
                    return this.BuildForLoad();

                case forInstall:
                    return this.BuildForInstall();

                case forMakeSelectable:
                    return this.BuildForMakeSelectable();

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

        public byte[] AsBytes() => this.AsApdu().Buffer;

        public override IEnumerable<Apdu> AsApdus()
        {
            bool HasOption(byte option)
            {
                return (this.P1 & option) == option;
            }

            if (HasOption(forLoad))
            {
                bool moreCommands = HasOption(forInstall);

                yield return this.BuildForLoad(moreCommands);
            }

            if (HasOption(forInstall))
            {
                bool moreCommands = HasOption(forMakeSelectable);

                yield return this.BuildForInstall(moreCommands);
            }

            if (HasOption(forMakeSelectable))
            {
                yield return this.BuildForMakeSelectable();
            }
        }

        private Apdu BuildForLoad(bool moreCommands = false)
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forLoadAID);
            commandData.AddRangeWithLength(this.forLoadSecurityDomainAID);
            commandData.AddRangeWithLength(this.forLoadDataBlockHash);
            commandData.AddRangeWithLength(this.forLoadParameters);
            commandData.AddRangeWithLength(this.forLoadToken);

            return this.Build(commandData, moreCommands);
        }

        private Apdu BuildForInstall(bool moreCommands = false)
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forInstallExecutableLoadFileAID);
            commandData.AddRangeWithLength(this.forInstallExecutableModuleAID);
            commandData.AddRangeWithLength(this.forInstallApplicationAID);
            commandData.AddRangeWithLength(this.forInstallPrivileges);
            commandData.AddRangeWithLength(this.forInstallParameters);
            commandData.AddRangeWithLength(this.forInstallToken);

            return this.Build(commandData, moreCommands);
        }

        private Apdu BuildForMakeSelectable()
        {
            var commandData = new List<byte> { 0x00, 0x00 };

            commandData.AddRangeWithLength(this.forMakeSelectableApplicationAID);
            commandData.AddRangeWithLength(this.forMakeSelectablePrivileges);
            commandData.AddRangeWithLength(this.forMakeSelectableParameters);
            commandData.AddRangeWithLength(this.forMakeSelectableToken);

            return this.Build(commandData);
        }

        private Apdu BuildForExtradition()
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forExtraditionSecurityDomainAID);
            commandData.AddRangeWithLength(this.forExtraditionApplicationAID);
            commandData.AddRangeWithLength(this.forExtraditionParameters);
            commandData.AddRangeWithLength(this.forExtraditionToken);

            return this.Build(commandData);
        }

        private Apdu BuildForRegistryUpdate()
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forRegistryUpdateSecurityDomainAID);
            commandData.AddRangeWithLength(this.forRegistryUpdateApplicationAID);
            commandData.AddRangeWithLength(this.forRegistryUpdatePrivileges);
            commandData.AddRangeWithLength(this.forRegistryUpdateParameters);
            commandData.AddRangeWithLength(this.forRegistryUpdateToken);

            return this.Build(commandData);
        }

        private Apdu BuildForPersonalization()
        {
            var commandData = new List<byte>();

            commandData.AddRangeWithLength(this.forPersonalizationApplicationAID);

            return this.Build(commandData);
        }

        private Apdu Build(IEnumerable<byte> commandData, bool moreCommands = false)
        {
            byte p2 = 0x00;

            if (moreCommands)
            {
                p2 |= 0b10000000;
            }

            return Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.Install, this.P1, p2, commandData.ToArray());
        }
    }
}
