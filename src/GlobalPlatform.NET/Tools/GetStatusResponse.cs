using GlobalPlatform.NET.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Tools
{
    public static class GetStatusResponse
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

        public static ApplicationData ParseIssuerSecurityDomain(IList<byte> apdu)
            => ParseApplications(apdu).Single();

        public static IEnumerable<ApplicationData> ParseApplications(params IList<byte>[] apdus)
            => TLV.Parse(apdus.SelectMany(x => x).ToList())
                .Where(Tag.GlobalPlatformRegistry)
                .Select(tlv => new ApplicationData
                {
                    AID = tlv.NestedTags.Single(Tag.AID).Value.ToArray(),
                    LifeCycleState = tlv.NestedTags.Single(Tag.LifeCycleState).Value.First(),
                    Privileges = tlv.NestedTags.Single(Tag.Privileges).Value.ToArray(),
                    ExecutableLoadFileAID = tlv.NestedTags.SingleOrDefault(Tag.ExecutableLoadFileAID)?.Value.ToArray(),
                    AssociatedSecurityDomainAID = tlv.NestedTags.SingleOrDefault(Tag.AssociatedSecurityDomainAID)?.Value.ToArray(),
                });

        public static IEnumerable<ExecutableLoadFileData> ParseExecutableLoadFiles(params IList<byte>[] apdus)
            => TLV.Parse(apdus.SelectMany(x => x).ToList())
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
        public byte[] AID { get; set; }

        public byte LifeCycleState { get; set; }

        public byte[] Privileges { get; set; }

        public byte[] ExecutableLoadFileAID { get; set; }

        public byte[] AssociatedSecurityDomainAID { get; set; }
    }

    public class ExecutableLoadFileData
    {
        public byte[] AID { get; set; }

        public byte LifeCycleState { get; set; }

        public byte[] ExecutableLoadFileVersionNumber { get; set; }

        public IReadOnlyCollection<byte[]> ExecutableModuleAIDs { get; set; }

        public byte[] AssociatedSecurityDomainAID { get; set; }
    }
}
