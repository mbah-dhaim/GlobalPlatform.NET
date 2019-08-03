using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.Tools;
using Iso7816;
using System.Collections.Generic;

namespace GlobalPlatform.NET.Commands
{
    public enum GetStatusScope : byte
    {
        IssuerSecurityDomain = 0b10000000,
        Applications = 0b01000000,
        ExecutableLoadFiles = 0b00100000,
        ExecutableLoadFilesAndModules = 0b00010000
    }

    public interface IGetStatusScopePicker
    {
        IGetStatusApplicationFilter GetStatusOf(GetStatusScope scope);
    }

    public interface IGetStatusApplicationFilter : IGetStatusOccurrencePicker
    {
        IGetStatusOccurrencePicker WithFilter(byte[] applicationFilter);
    }

    public interface IGetStatusOccurrencePicker : IApduBuilder
    {
        IApduBuilder ReturnFirstOrAllOccurrences();

        IApduBuilder ReturnNextOccurrence();
    }

    /// <summary>
    /// The GET STATUS command is used to retrieve Issuer Security Domain, Executable Load File,
    /// Executable Module, Application or Security Domain Life Cycle status information according to
    /// a given match/search criteria.
    /// <para> Based on section 11.4 of the v2.3 GlobalPlatform Card Specification. </para>
    /// </summary>
    public class GetStatusCommand : CommandBase<GetStatusCommand, IGetStatusScopePicker>,
        IGetStatusScopePicker,
        IGetStatusApplicationFilter
    {
        private byte[] applicationFilter = new byte[0];

        public enum Tag : byte
        {
            ApplicationAID = 0x4F
        }

        public IGetStatusApplicationFilter GetStatusOf(GetStatusScope scope)
        {
            P1 = (byte)scope;

            return this;
        }

        public IGetStatusOccurrencePicker WithFilter(byte[] applicationFilter)
        {
            Ensure.IsNotNull(applicationFilter, nameof(applicationFilter));

            this.applicationFilter = applicationFilter;

            return this;
        }

        public IApduBuilder ReturnFirstOrAllOccurrences()
        {
            return this;
        }

        public IApduBuilder ReturnNextOccurrence()
        {
            P2 |= 0b00000001;

            return this;
        }

        public override CommandApdu AsApdu()
        {
            var data = new List<byte>();
            data.AddTLV(TLV.Build((byte)Tag.ApplicationAID, applicationFilter));
            var apdu = CommandApdu.Case4S(ApduClass.GlobalPlatform, ApduInstruction.GetStatus, P1, P2 |= 0b00000010, data.ToArray(), 0x00);
            return apdu;
        }
    }
}
