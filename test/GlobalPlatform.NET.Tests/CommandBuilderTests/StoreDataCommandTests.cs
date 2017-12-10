using System;
using System.Linq;
using FluentAssertions;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests.CommandBuilderTests
{
    [TestClass]
    public class StoreDataCommandTests
    {
        [TestMethod]
        public void StoreData()
        {
            byte p1 = 0x10;
            byte[] data = new byte[new Random().Next(256, 510)];
            byte blockSize = (byte)new Random().Next(128, 240);

            var apdus = StoreDataCommand.Build
                .WithP1(p1)
                .StoreData(data)
                .WithBlockSize(blockSize)
                .AsApdus()
                .ToList();

            apdus.ForEach((apdu, index, isLast) =>
            {
                byte expectedP1 = isLast ? p1 |= 0x80 : p1;

                apdu.Assert(ApduClass.GlobalPlatform, ApduInstruction.StoreData, expectedP1, (byte)index);
                apdu.CommandData.All(x => x == 0x00).Should().BeTrue();
            });
        }
    }
}
