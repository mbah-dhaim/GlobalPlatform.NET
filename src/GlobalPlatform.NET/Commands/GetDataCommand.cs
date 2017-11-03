using System;
using System.Linq;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    public enum DataObject : ushort
    {
        IssuerIdentificationNumber = 0x0042,
        CardImageNumber = 0x0045,
        CardData = 0x0066,
        KeyInformationTemplate = 0x00E0,
        CardCapabilityInformation = 0x0067,
        CurrentSecurityLevel = 0x00D3,
        ApplicationList = 0x2F00,
        ExtendedCardResourcesInformation = 0xFF21,
        SecurityDomainManagerUrl = 0x5F50
    }

    public interface IGetDataObjectPicker : IGetDataP1Picker
    {
        IGetDataTagListPicker FromDataObject(DataObject dataObject);
    }

    public interface IGetDataP1Picker
    {
        IGetDataP2Picker UsingP1(byte p1);
    }

    public interface IGetDataP2Picker
    {
        IGetDataTagListPicker UsingP2(byte p2);
    }

    public interface IGetDataTagListPicker : IApduBuilder
    {
        IApduBuilder WithTagList(params byte[] tags);
    }

    /// <summary>
    /// The GET DATA command is used to retrieve either a single data object, which may be
    /// constructed, or a set of data objects. Reference control parameters P1 and P2 coding is used
    /// to define the specific data object tag. The data object may contain information pertaining to
    /// a key.
    /// <para> Based on section 11.3 of the v2.3 GlobalPlatform Card Specification. </para>
    /// </summary>
    public class GetDataCommand : CommandBase<GetDataCommand, IGetDataObjectPicker>,
        IGetDataObjectPicker,
        IGetDataP2Picker,
        IGetDataTagListPicker
    {
        private byte[] tagList = new byte[0];

        public IGetDataTagListPicker FromDataObject(DataObject dataObject)
        {
            var bytes = BitConverter.GetBytes((ushort)dataObject);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            this.P1 = bytes.First();
            this.P2 = bytes.Last();

            return this;
        }

        public IGetDataP2Picker UsingP1(byte p1)
        {
            this.P1 = p1;

            return this;
        }

        public IGetDataTagListPicker UsingP2(byte p2)
        {
            this.P2 = p2;

            return this;
        }

        public IApduBuilder WithTagList(params byte[] tagList)
        {
            Ensure.IsNotNull(tagList, nameof(tagList));

            this.tagList = tagList;

            return this;
        }

        public override CommandApdu AsApdu()
            => CommandApdu.Case4S(ApduClass.GlobalPlatform, ApduInstruction.GetData, this.P1, this.P2, this.tagList, 0x00);
    }
}
