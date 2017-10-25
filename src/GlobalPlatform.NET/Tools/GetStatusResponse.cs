using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Tools
{
    public interface IGetStatusResponseOperationPicker
    {
        IGetStatusResponseScopePicker WithScopeOf();
    }

    public interface IGetStatusResponseScopePicker
    {
        IGetStatusResponseIssuerSecurityDomainParser IssuerSecurityDomain();

        IGetStatusResponseApplicationsParser Applications();

        IGetStatusResponseExecutableLoadFilesParser ExecutableLoadFiles();
    }

    public interface IGetStatusResponseIssuerSecurityDomainParser
    {
        ApplicationData Parse(IEnumerable<byte> apdu);
    }

    public interface IGetStatusResponseApplicationsParser
    {
        IEnumerable<ApplicationData> Parse(params IEnumerable<byte>[] apdus);
    }

    public interface IGetStatusResponseExecutableLoadFilesParser
    {
        IEnumerable<ExecutableLoadFileData> Parse(params IEnumerable<byte>[] apdus);
    }

    /// <summary>
    /// The GET STATUS command is used to retrieve Issuer Security Domain, Executable Load File,
    /// Executable Module, Application or Security Domain Life Cycle status information according to
    /// a given match/search criteria.
    /// <para>
    /// Here you can analyze the responses to return structured data objects, for further interrogation.
    /// </para>
    /// <para> Based on section 11.4.3 of the v2.3 GlobalPlatform Card Specification. </para>
    /// </summary>
    public class GetStatusResponse : IGetStatusResponseOperationPicker,
        IGetStatusResponseScopePicker,
        IGetStatusResponseIssuerSecurityDomainParser,
        IGetStatusResponseApplicationsParser,
        IGetStatusResponseExecutableLoadFilesParser
    {
        private static class Tag
        {
            public const byte AID = 0x4F;
            public const byte ExecutableModuleAID = 0x84;
            public static readonly byte[] LifeCycleState = { 0x9F, 0x70 };
            public const byte Privileges = 0xC5;
            public const byte ExecutableLoadFileAID = 0xC4;
            public const byte AssociatedSecurityDomainAID = 0xCC;
            public const byte ExecutableLoadFileVersionNumber = 0xCE;
            public const byte GlobalPlatformRegistry = 0xE3;
        }

        /// <summary>
        /// Starts analyzing the response. 
        /// </summary>
        public static IGetStatusResponseOperationPicker Analyze => new GetStatusResponse();

        public IGetStatusResponseScopePicker WithScopeOf() => this;

        public IGetStatusResponseIssuerSecurityDomainParser IssuerSecurityDomain() => this;

        public IGetStatusResponseApplicationsParser Applications() => this;

        public IGetStatusResponseExecutableLoadFilesParser ExecutableLoadFiles() => this;

        ApplicationData IGetStatusResponseIssuerSecurityDomainParser.Parse(IEnumerable<byte> apdu)
            => ((IGetStatusResponseApplicationsParser)this).Parse(apdu).Single();

        IEnumerable<ApplicationData> IGetStatusResponseApplicationsParser.Parse(params IEnumerable<byte>[] apdus)
            => TLV.Parse(apdus.SelectMany(x => x))
                .Where(Tag.GlobalPlatformRegistry)
                .Select(tlv => new ApplicationData
                {
                    AID = tlv.NestedTags.Single(Tag.AID).Value.ToArray(),
                    LifeCycleState = tlv.NestedTags.Single(Tag.LifeCycleState).Value.First(),
                    Privileges = tlv.NestedTags.Single(Tag.Privileges).Value.ToArray(),
                    ExecutableLoadFileAID = tlv.NestedTags.SingleOrDefault(Tag.ExecutableLoadFileAID)?.Value.ToArray(),
                    AssociatedSecurityDomainAID = tlv.NestedTags.SingleOrDefault(Tag.AssociatedSecurityDomainAID)?.Value.ToArray(),
                });

        IEnumerable<ExecutableLoadFileData> IGetStatusResponseExecutableLoadFilesParser.Parse(params IEnumerable<byte>[] apdus)
            => TLV.Parse(apdus.SelectMany(x => x))
                .Select(tlv => new ExecutableLoadFileData
                {
                    AID = tlv.NestedTags.Single(Tag.AID).Value.ToArray(),
                    LifeCycleState = tlv.NestedTags.Single(Tag.LifeCycleState).Value.First(),
                    ExecutableLoadFileVersionNumber = tlv.NestedTags.SingleOrDefault(Tag.ExecutableLoadFileVersionNumber)?.Value.ToArray(),
                    ExecutableModuleAIDs = tlv.NestedTags.Where(Tag.ExecutableModuleAID).Select(x => x.Value.ToArray()).ToList(),
                    AssociatedSecurityDomainAID = tlv.NestedTags.SingleOrDefault(Tag.AssociatedSecurityDomainAID)?.Value.ToArray(),
                });
    }

    public class ApplicationData
    {
        public byte[] AID { get; internal set; }

        public byte LifeCycleState { get; internal set; }

        public byte[] Privileges { get; internal set; }

        public byte[] ExecutableLoadFileAID { get; internal set; }

        public byte[] AssociatedSecurityDomainAID { get; internal set; }
    }

    public class ExecutableLoadFileData
    {
        public byte[] AID { get; internal set; }

        public byte LifeCycleState { get; internal set; }

        public byte[] ExecutableLoadFileVersionNumber { get; internal set; }

        public IReadOnlyCollection<byte[]> ExecutableModuleAIDs { get; internal set; }

        public byte[] AssociatedSecurityDomainAID { get; internal set; }
    }
}
