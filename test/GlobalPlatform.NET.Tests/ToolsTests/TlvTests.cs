using FluentAssertions;
using GlobalPlatform.NET.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            var tlv = TLV.Build(0x4F, 0xDD, 0xDD);

            tlv.Tag.Should().BeEquivalentTo(0x4F);
            tlv.Length.Should().Be(2);
            tlv.Value.Should().BeEquivalentTo(new byte[] { 0xDD, 0xDD });
            tlv.Data.Should().BeEquivalentTo(new byte[] { 0x4F, 0x02, 0xDD, 0xDD }, o => o.WithStrictOrderingFor(x => x));
        }

        [TestMethod]
        public void TLV_Should_Build_Long()
        {
            var tlv = TLV.Build(0x4F, new byte[1000]);

            var data = new List<byte>
            {
                0x4F,
                0x82,
                0x03,
                0xE8
            };
            data.AddRange(new byte[1000]);

            tlv.Tag.Should().BeEquivalentTo(0x4F);
            tlv.Length.Should().Be(1000);
            tlv.Value.Should().BeEquivalentTo(new byte[1000]);
            tlv.Data.Should().BeEquivalentTo(data, o => o.WithStrictOrderingFor(x => x));
        }

        [TestMethod]
        public void TLV_Should_Parse_Tags_With_Definite_Short_Length()
        {
            var tlv = TLV.Build(0x4F, 0xDD, 0xDD);

            var tags = TLV.Parse(tlv.Data);

            tags.Count.Should().Be(1);
            tags.First().Tag.Should().BeEquivalentTo(0x4F);
            tags.First().Length.Should().Be(2);
            tags.First().Value.Should().BeEquivalentTo(new byte[] { 0xDD, 0xDD });
        }

        [TestMethod]
        public void TLV_Should_Parse_Tags_With_Definite_Long_Length()
        {
            var data = new byte[] { 0x4F, 0x81, 0x02, 0xDD, 0xDD };

            var tlvs = TLV.Parse(data);

            tlvs.Count.Should().Be(1);
            tlvs.First().Tag.Should().BeEquivalentTo(0x4F);
            tlvs.First().Length.Should().Be(2);
            tlvs.First().Value.Should().BeEquivalentTo(new byte[] { 0xDD, 0xDD });
        }

        [TestMethod]
        public void TLV_Should_Recursively_Parse_Tags_With_Definite_Short_Length()
        {
            var data = new byte[] { 0xE3, 0x05, 0x4F, 0x02, 0xDD, 0xDD };

            var tlvs = TLV.Parse(data);

            tlvs.Count.Should().Be(1);
            tlvs.First().Tag.Should().BeEquivalentTo(0xE3);
            tlvs.First().Length.Should().Be(4);
            tlvs.First().NestedTags.Count.Should().Be(1);
            tlvs.First().NestedTags.First().Tag.Should().BeEquivalentTo(0x4F);
            tlvs.First().NestedTags.First().Length.Should().Be(2);
            tlvs.First().NestedTags.First().Value.Should().BeEquivalentTo(new byte[] { 0xDD, 0xDD });
        }

        [TestMethod]
        public void TLV_Should_Recursively_Parse_Tags_With_Definite_Long_Length()
        {
            var data = new byte[] { 0xE3, 0x81, 0x05, 0x4F, 0x81, 0x02, 0xDD, 0xDD };

            var tlvs = TLV.Parse(data);

            tlvs.Count.Should().Be(1);
            tlvs.First().Tag.Should().BeEquivalentTo(0xE3);
            tlvs.First().Length.Should().Be(4);
            tlvs.First().NestedTags.Count.Should().Be(1);
            tlvs.First().NestedTags.First().Tag.Should().BeEquivalentTo(0x4F);
            tlvs.First().NestedTags.First().Length.Should().Be(2);
            tlvs.First().NestedTags.First().Value.Should().BeEquivalentTo(new byte[] { 0xDD, 0xDD });
        }

        [TestMethod]
        public void TLV_Should_Not_Build_Primitive_Containing_Encodings()
        {
            Action action = () =>
            {
                TLV.Build(0x4F, TLV.Build(0x4F));
            };

            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void TLV_Should_Not_Parse_Tags_With_Indefinite_Length()
        {
            var data = new byte[] { 0x4F, 0x80, 0x00, 0x00, 0xDD, 0x00, 0x00 };

            Action action = () =>
            {
                TLV.Parse(data);
            };

            action.Should().Throw<NotSupportedException>();
        }
    }
}
