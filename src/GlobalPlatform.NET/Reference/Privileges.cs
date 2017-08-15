using GlobalPlatform.NET.Extensions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GlobalPlatform.NET.Reference
{
    public interface IPrivilegesByte1Builder
    {
        IPrivilegesByte2Builder WithByte1(params byte[] privileges);
    }

    public interface IPrivilegesByte2Builder : IPrivilegesBuilder
    {
        IPrivilegesByte3Builder WithByte2(params byte[] privileges);
    }

    public interface IPrivilegesByte3Builder : IPrivilegesBuilder
    {
        IPrivilegesBuilder WithByte3(params byte[] privileges);
    }

    public interface IPrivilegesBuilder
    {
        byte[] AsBytes();
    }

    public class Privileges : IPrivilegesByte1Builder,
        IPrivilegesByte2Builder,
        IPrivilegesByte3Builder
    {
        private byte byte1;
        private byte byte2;
        private byte byte3;

        private Privileges()
        {
        }

        public static IPrivilegesByte1Builder Build => new Privileges();

        public IPrivilegesByte2Builder WithByte1(params byte[] privileges)
        {
            privileges.ForEach(privilege => this.byte1 |= privilege);

            return this;
        }

        public IPrivilegesByte3Builder WithByte2(params byte[] privileges)
        {
            privileges.ForEach(privilege => this.byte2 |= privilege);

            return this;
        }

        public IPrivilegesBuilder WithByte3(params byte[] privileges)
        {
            privileges.ForEach(privilege => this.byte3 |= privilege);

            return this;
        }

        public byte[] AsBytes()
        {
            var bytes = new List<byte>
            {
                this.byte1
            };

            if (this.byte2 > 0)
            {
                bytes.Add(this.byte2);
            }
            if (this.byte3 > 0)
            {
                bytes.Add(this.byte3);
            }

            return bytes.ToArray();
        }

        public static byte[] SecurityDomain => Build.WithByte1(Byte1.SecurityDomain).AsBytes();

        public static class Byte1
        {
            public const byte SecurityDomain = 0b10000000;
            public const byte DapVerification = 0b11000000;
            public const byte MandatedVerification = 0b11000001;
            public const byte DelegatedManagement = 0b10100000;
            public const byte CardLock = 0b00010000;
            public const byte CardTerminate = 0b00001000;
            public const byte CardReset = 0b00000100;
            public const byte CvmManagement = 0b00000010;
        }

        public static class Byte2
        {
            public const byte TrustedPath = 0b10000000;
            public const byte AuthorizedManagement = 0b01000000;
            public const byte TokenManagement = 0b00100000;
            public const byte GlobalDelete = 0b00010000;
            public const byte GlobalLock = 0b00001000;
            public const byte GlobalRegistry = 0b00000100;
            public const byte FinalApplication = 0b00000010;
            public const byte GlobalService = 0b00000001;
        }

        public static class Byte3
        {
            public const byte ReceiptGeneration = 0b10000000;
            public const byte CipheredLoadFileBlock = 0b01000000;
            public const byte ContactlessActivation = 0b00100000;
            public const byte ContactlessSelfActivation = 0b00010000;
        }
    }
}
