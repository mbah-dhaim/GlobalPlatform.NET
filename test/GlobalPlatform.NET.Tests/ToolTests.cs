using FluentAssertions;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GlobalPlatform.NET.Tests
{
    [TestClass]
    public class ToolTests
    {
        [TestMethod]
        public void KeyCheckValue()
        {
            byte[] keyData = Enumerable.Range(64, 16).Select(x => (byte)x).ToArray();

            var keyCheckValue = Tools.KeyCheckValue.Generate(KeyTypeCoding.DES, keyData);

            keyCheckValue.ShouldAllBeEquivalentTo(new byte[] { 0x8B, 0xAF, 0x47 });
        }
    }
}
