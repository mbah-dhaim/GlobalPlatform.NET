using FluentAssertions;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.Tools;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class LoadCommandTests : CommandTestsBase
    {
        [TestMethod]
        public void Load()
        {
            byte[] data = new byte[new Random().Next(1, 32767)];
            byte blockSize = (byte)new Random().Next(128, 240);

            var apdus = LoadCommand.Build
                .Load(data)
                .WithBlockSize(blockSize)
                .AsApdus()
                .ToList();

            var commandData = new List<byte> { (byte)Tag.LoadFileDataBlock, 0x82 };

            var loadFileDataBlockLength = BitConverter.GetBytes((ushort)data.Length);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(loadFileDataBlockLength);
            }

            commandData.AddRange(loadFileDataBlockLength);

            apdus.First().CommandData.Take(4).ShouldBeEquivalentTo(commandData);

            byte[] dataBlock = apdus.SelectMany(apdu => apdu.CommandData).ToArray();

            apdus.ForEach((apdu, index, isLast) =>
            {
                byte p1 = isLast ? (byte)0x80 : (byte)0x00;

                apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Load, p1, (byte)index);
                apdu.Lc.Should().Be((byte)(isLast ? dataBlock.Length % blockSize : blockSize));
            });
        }

        [TestMethod]
        public void Load_With_DAP_Block()
        {
            byte[] data = new byte[new Random().Next(1, 32767)];
            byte blockSize = (byte)new Random().Next(128, 240);

            var apdus = LoadCommand.Build
                .WithDapBlock(SecurityDomainAID, Signature)
                .Load(data)
                .WithBlockSize(blockSize)
                .AsApdus()
                .ToList();

            var tlvs = TLV.Parse(apdus.First().CommandData);

            tlvs.Count.Should().Be(2);
            tlvs.First().Tag.ShouldAllBeEquivalentTo((byte)Tag.DapBlock);
            tlvs.Single((byte)Tag.DapBlock).NestedTags.Count.Should().Be(2);
            tlvs.Single((byte)Tag.DapBlock).NestedTags.First().Tag.ShouldAllBeEquivalentTo((byte)Tag.SecurityDomainAID);
            tlvs.Single((byte)Tag.DapBlock).NestedTags.Last().Tag.ShouldAllBeEquivalentTo((byte)Tag.LoadFileDataBlockSignature);
            tlvs.Last().Tag.ShouldAllBeEquivalentTo((byte)Tag.LoadFileDataBlock);
            tlvs.Single((byte)Tag.LoadFileDataBlock).NestedTags.Count.Should().Be(0);

            byte[] dataBlock = apdus.SelectMany(apdu => apdu.CommandData).ToArray();

            apdus.ForEach((apdu, index, isLast) =>
            {
                byte p1 = isLast ? (byte)0x80 : (byte)0x00;

                apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.Load, p1, (byte)index);
                apdu.Lc.Should().Be((byte)(isLast ? dataBlock.Length % blockSize : blockSize));
            });
        }
    }
}

