using FluentAssertions;
using GlobalPlatform.NET.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Tests.ToolsTests
{
    [TestClass]
    public class TlvTests
    {
        [TestMethod]
        public void TLV_Should_Build_Short()
        {
            var tlv = TLV.Build(0xFF, new byte[2]);

            tlv.ShouldAllBeEquivalentTo(new byte[] { 0xFF, 0x02, 0x00, 0x00 });
        }

        [TestMethod]
        public void TLV_Should_Build_Long()
        {
            var tlv = TLV.Build(0xFF, new byte[50000]);

            var data = new List<byte>
            {
                0xFF,
                0x82,
                0xC3,
                0x50
            };
            data.AddRange(new byte[50000]);

            tlv.ShouldAllBeEquivalentTo(data);
        }

        [TestMethod]
        public void TLV_Should_Parse_Tags_With_Definite_Short_Length()
        {
            var tag = TLV.Build(0xFF, Enumerable.Repeat((byte)0xDD, 2).ToList());

            var data = TLV.Parse(tag, 0xFF);

            data.ShouldAllBeEquivalentTo(new byte[] { 0xDD, 0xDD });
        }

        [TestMethod]
        public void TLV_Should_Parse_Tags_With_Definite_Long_Length()
        {
            var tag = new byte[] { 0xFF, 0x81, 0x02, 0xDD, 0xDD };

            var data = TLV.Parse(tag, 0xFF);

            data.ShouldAllBeEquivalentTo(new byte[] { 0xDD, 0xDD });
        }

        [TestMethod]
        public void TLV_Should_Recursively_Parse_Tags_With_Definite_Short_Length()
        {
            var innerTag = TLV.Build(0xEE, Enumerable.Repeat((byte)0xDD, 2).ToList());

            var outerTag = TLV.Build(0xFF, innerTag);

            var data = TLV.Parse(outerTag, 0xFF, 0xEE);

            data.ShouldAllBeEquivalentTo(new byte[] { 0xDD, 0xDD });
        }

        [TestMethod]
        public void TLV_Should_Recursively_Parse_Tags_With_Definite_Long_Length()
        {
            var tag = new byte[] { 0xFF, 0x81, 0x05, 0xEE, 0x81, 0x02, 0xDD, 0xDD };

            var data = TLV.Parse(tag, 0xFF, 0xEE);

            data.ShouldAllBeEquivalentTo(new byte[] { 0xDD, 0xDD });
        }

        [TestMethod]
        public void TLV_Should_Parse_Tags_With_Indefinite_Length()
        {
            var tag = new byte[] { 0xFF, 0x80, 0x00, 0x00, 0xDD, 0x00, 0x00 };

            var data = TLV.Parse(tag, 0xFF);

            data.ShouldAllBeEquivalentTo(new byte[] { 0x00, 0x00, 0xDD });
        }

        [TestMethod]
        public void TLV_Should_Recursively_Parse_Tags_With_Indefinite_Length()
        {
            var tag = new byte[] { 0xFF, 0x80, 0xEE, 0x80, 0x00, 0x00, 0xDD, 0x00, 0x00, 0x00, 0x00 };

            var data = TLV.Parse(tag, 0xFF, 0xEE);

            data.ShouldAllBeEquivalentTo(new byte[] { 0x00, 0x00, 0xDD });
        }
    }
}
