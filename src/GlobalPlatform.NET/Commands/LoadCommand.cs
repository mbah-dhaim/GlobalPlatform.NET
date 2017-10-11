using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Tools;

namespace GlobalPlatform.NET.Commands
{
    public enum Tag : byte
    {
        DapBlock = 0xE2,
        SecurityDomainAID = 0x4F,
        LoadFileDataBlockSignature = 0xC3,
        LoadFileDataBlock = 0xC4
    }

    public interface ILoadCommandBlockSizePicker : IMultiApduBuilder
    {
        IMultiApduBuilder WithBlockSize(byte blockSize);
    }

    public interface ILoadFileStructureBuilder
    {
        ILoadFileStructureBuilder WithDapBlock(byte[] securityDomainAID, byte[] signature);

        ILoadCommandBlockSizePicker Load(byte[] data);
    }

    /// <summary>
    /// The LOAD command is used for loading a Load File. The runtime environment internal handling
    /// or storage of the Load File is beyond the scope of this Specification.
    /// <para>
    /// Multiple LOAD commands may be used to transfer a Load File to the card. The Load File is
    /// divided into smaller components for transmission. Each LOAD command shall be numbered
    /// starting at '00'. The LOAD command numbering shall be strictly sequential and increments by
    /// one. The card shall be informed of the last block of the Load File.
    /// </para>
    /// <para>
    /// After receiving the last block of the Load File, the card shall execute the internal
    /// processes necessary for the Load File and any additional processes identified in the INSTALL
    /// [for load] command that preceded the LOAD commands.
    /// </para>
    /// <para> Based on section 11.6 of the v2.3 GlobalPlatform Card Specification. </para>
    /// </summary>
    public class LoadCommand : MultiCommandBase<LoadCommand, ILoadFileStructureBuilder>,
        ILoadFileStructureBuilder,
        ILoadCommandBlockSizePicker
    {
        private byte blockSize = 247;
        private byte[] data;
        private byte[] securityDomainAID = new byte[0];
        private byte[] signature = new byte[0];

        public override IEnumerable<CommandApdu> AsApdus()
        {
            var commandData = new List<byte>();

            if (this.securityDomainAID.Any())
            {
                var dapBlock = TLV.Build((byte) Tag.DapBlock,
                    TLV.Build((byte) Tag.SecurityDomainAID, this.securityDomainAID),
                    TLV.Build((byte) Tag.LoadFileDataBlockSignature, this.signature)
                );

                commandData.AddTLV(dapBlock);
            }

            commandData.AddTLV(TLV.Build((byte)Tag.LoadFileDataBlock, this.data));

            var chunks = commandData.Split(this.blockSize).ToList();

            return chunks.Select((block, index, isLast) => CommandApdu.Case4S(
                ApduClass.GlobalPlatform,
                ApduInstruction.Load,
                (byte)(isLast ? 0x80 : 0x00),
                (byte)index,
                block.ToArray(),
                0x00));
        }

        public IMultiApduBuilder WithBlockSize(byte blockSize)
        {
            if (blockSize < 1)
            {
                throw new ArgumentException("Block size must be at least 1.", nameof(blockSize));
            }

            this.blockSize = blockSize;

            return this;
        }

        public ILoadFileStructureBuilder WithDapBlock(byte[] securityDomainAID, byte[] signature)
        {
            Ensure.IsAID(securityDomainAID, nameof(securityDomainAID));
            Ensure.IsNotNullOrEmpty(signature, nameof(signature));

            this.securityDomainAID = securityDomainAID;
            this.signature = signature;

            return this;
        }

        public ILoadCommandBlockSizePicker Load(byte[] data)
        {
            Ensure.HasNoMoreThan(data, nameof(data), 65536);

            this.data = data;

            return this;
        }
    }
}
