using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET
{
    [Serializable]
    public class CommandApdu : IEquatable<CommandApdu>, IEquatable<IEnumerable<byte>>, IFormattable
    {
        private CommandApdu()
        {
        }

        public byte[] Buffer
        {
            get
            {
                var buffer = new List<byte>
                {
                    (byte)this.CLA,
                    (byte)this.INS,
                    this.P1,
                    this.P2,
                };

                if (this.CommandData.Any())
                {
                    buffer.Add(this.Lc);
                    buffer.AddRange(this.CommandData);
                }

                buffer.AddRange(this.Le);

                return buffer.ToArray();
            }
        }

        public ApduClass CLA { get; set; }

        public ApduInstruction INS { get; private set; }

        public byte P1 { get; private set; }

        public byte P2 { get; private set; }

        public byte Lc => this.commandData.LengthChecked();

        private byte[] commandData = new byte[0];

        public byte[] CommandData
        {
            get => this.commandData;
            set
            {
                var bytes = value.ToArray();

                if (bytes.Length > 255)
                {
                    throw new ArgumentException("Data exceeds 255 bytes.", nameof(value));
                }

                this.commandData = bytes;
            }
        }

        private byte[] le = new byte[0];

        public byte[] Le
        {
            get => this.le;
            set
            {
                if (this.le.Length == 0)
                {
                    throw new ArgumentException("Le is not present.", nameof(value));
                }

                var bytes = value.ToArray();

                if (bytes.Length != 1)
                {
                    throw new ArgumentException("Le must be exactly 1 byte.", nameof(value));
                }

                this.le = bytes;
            }
        }

        /// <summary>
        /// Builds an ISO 7816-4 Case 1 Command APDU. Lc byte is not present, and thus no command
        /// data is present. Le byte is not present.
        /// </summary>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static CommandApdu Case1(ApduClass cla, ApduInstruction ins, byte p1, byte p2) => new CommandApdu
        {
            CLA = cla,
            INS = ins,
            P1 = p1,
            P2 = p2
        };

        /// <summary>
        /// Builds an ISO 7816-4 Case 2S Command APDU. Lc byte is not present, and thus no command
        /// data is present. Le is encoded on 1 byte.
        /// </summary>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="le"></param>
        /// <returns></returns>
        public static CommandApdu Case2S(ApduClass cla, ApduInstruction ins, byte p1, byte p2, byte le)
        {
            var apdu = Case1(cla, ins, p1, p2);

            apdu.le = new[] { le };

            return apdu;
        }

        /// <summary>
        /// Builds an ISO 7816-4 Case 3S Command APDU. Lc is encoded on 1 byte. Le byte is not present. 
        /// </summary>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CommandApdu Case3S(ApduClass cla, ApduInstruction ins, byte p1, byte p2, byte[] data)
        {
            var apdu = Case1(cla, ins, p1, p2);

            apdu.CommandData = data;

            return apdu;
        }

        /// <summary>
        /// Builds an ISO 7816-4 Case 4S Command APDU. Lc is encoded on 1 byte. Le is encoded on 1 byte. 
        /// </summary>
        /// <param name="cla"></param>
        /// <param name="ins"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="data"></param>
        /// <param name="le"></param>
        /// <returns></returns>
        public static CommandApdu Case4S(ApduClass cla, ApduInstruction ins, byte p1, byte p2, byte[] data, byte le)
        {
            var apdu = Case3S(cla, ins, p1, p2, data);

            apdu.le = new[] { le };

            return apdu;
        }

        public bool Equals(CommandApdu other) => this.Equals(other.Buffer);

        public bool Equals(IEnumerable<byte> other) => this.Buffer.SequenceEqual(other);

        public override string ToString() => BitConverter.ToString(this.Buffer);

        public string ToString(string separator)
            => this.ToString().Replace("-", separator);

        public string ToString(string separator, IFormatProvider formatProvider)
            => this.ToString(separator);

        public static implicit operator byte[] (CommandApdu apdu) => apdu.Buffer;
    }
}
