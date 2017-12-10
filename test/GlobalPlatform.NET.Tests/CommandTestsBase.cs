using System.Linq;
using FluentAssertions;
using Iso7816;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests
{
    [TestClass]
    public abstract class CommandTestsBase
    {
        protected static readonly byte[] ExecutableLoadFileAID = Enumerable.Repeat<byte>(0xAA, 8).ToArray();
        protected static readonly byte[] ExecutableModuleAID = Enumerable.Repeat<byte>(0xBB, 8).ToArray();
        protected static readonly byte[] SecurityDomainAID = Enumerable.Repeat<byte>(0xEE, 8).ToArray();
        protected static readonly byte[] ApplicationAID = Enumerable.Repeat<byte>(0xFF, 8).ToArray();
        protected static readonly byte[] Token = Enumerable.Repeat<byte>(0x11, 4).ToArray();
        protected static readonly byte[] Hash = Enumerable.Repeat<byte>(0x22, 4).ToArray();
        protected static readonly byte[] Signature = Enumerable.Repeat<byte>(0x33, 4).ToArray();
        protected static readonly byte[] KeyData = Enumerable.Range(64, 16).Select(x => (byte)x).ToArray();
        protected static readonly byte[] InstallParameters = { 0xC9, 0x00 };
    }

    public static class ApduExtensions
    {
        /// <summary>
        /// Asserts that an APDU has the specified values. 
        /// </summary>
        /// <param name="apdu"></param>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public static void Assert(this CommandApdu apdu,
            byte cla,
            byte ins,
            byte p1,
            byte p2)
        {
            apdu.CLA.Should().Be(cla);
            apdu.INS.Should().Be(ins);
            apdu.P1.Should().Be(p1);
            apdu.P2.Should().Be(p2);
        }

        /// <summary>
        /// Asserts that an APDU has the specified values. 
        /// </summary>
        /// <param name="apdu"></param>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="le"></param>
        public static void Assert(this CommandApdu apdu,
            byte cla,
            byte ins,
            byte p1,
            byte p2,
            byte le)
        {
            apdu.CLA.Should().Be(cla);
            apdu.INS.Should().Be(ins);
            apdu.P1.Should().Be(p1);
            apdu.P2.Should().Be(p2);
            apdu.Le.ShouldBeEquivalentTo(new[] { le });
        }

        /// <summary>
        /// Asserts that an APDU has the specified values. 
        /// </summary>
        /// <param name="apdu"></param>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="commandData"></param>
        public static void Assert(this CommandApdu apdu,
            byte cla,
            byte ins,
            byte p1,
            byte p2,
            params byte[] commandData)
        {
            apdu.CLA.Should().Be(cla);
            apdu.INS.Should().Be(ins);
            apdu.P1.Should().Be(p1);
            apdu.P2.Should().Be(p2);
            apdu.Lc.First().Should().Be(checked((byte)apdu.CommandData.Count()));
            apdu.CommandData.ShouldBeEquivalentTo(commandData);
        }

        /// <summary>
        /// Asserts that an APDU has the specified values. 
        /// </summary>
        /// <param name="apdu"></param>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="commandData"></param>
        /// <param name="le"></param>
        public static void Assert(this CommandApdu apdu,
            byte cla,
            byte ins,
            byte p1,
            byte p2,
            byte[] commandData,
            byte le)
        {
            apdu.CLA.Should().Be(cla);
            apdu.INS.Should().Be(ins);
            apdu.P1.Should().Be(p1);
            apdu.P2.Should().Be(p2);
            apdu.Lc.First().Should().Be(checked((byte)apdu.CommandData.Count()));
            apdu.CommandData.ShouldBeEquivalentTo(commandData);
            apdu.Le.ShouldBeEquivalentTo(new[] { le });
        }
    }
}
