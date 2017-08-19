using FluentAssertions;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using GlobalPlatform.NET.Reference;

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

            byte[] dataBlock = apdus.SelectMany(apdu => apdu.CommandData).ToArray();

            apdus.ForEach((apdu, index, isLast) =>
            {
                byte p1 = isLast ? (byte)0x80 : (byte)0x00;

                apdu.Assert(ApduInstruction.Load, p1, (byte)index);
                apdu.Lc.Should().Be((byte)(isLast ? dataBlock.Length % blockSize : blockSize));
            });
        }

        [TestMethod]
        public void Load_With_DAP_Block()
        {
            byte[] data = new byte[new Random().Next(1, 32767)];
            byte blockSize = (byte)new Random().Next(128, 240);

            var apdus = LoadCommand.Build
                .WithDapBlock(ApplicationAID, Enumerable.Range(8, 8).Select(x => (byte)x).ToArray())
                .Load(data)
                .WithBlockSize(blockSize)
                .AsApdus()
                .ToList();

            byte[] dataBlock = apdus.SelectMany(apdu => apdu.CommandData).ToArray();

            apdus.ForEach((apdu, index, isLast) =>
            {
                byte p1 = isLast ? (byte)0x80 : (byte)0x00;

                apdu.Assert(ApduInstruction.Load, p1, (byte)index);
                apdu.Lc.Should().Be((byte)(isLast ? dataBlock.Length % blockSize : blockSize));
            });
        }
    }
}
