using GlobalPlatform.NET.Reference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET
{
    [Serializable]
    public class ResponseApdu : IEquatable<ResponseApdu>, IEquatable<IEnumerable<byte>>, IFormattable
    {
        private ResponseApdu()
        {
        }

        public IEnumerable<byte> Data => this.Buffer.Take(this.Buffer.Length - 2);

        public byte SW1 => this.Buffer.Reverse().Skip(1).First();

        public byte SW2 => this.Buffer.Reverse().First();

        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Returns the processing status of the Response APDU, by combining SW1+SW2 in to a status word. 
        /// </summary>
        public ApduStatus Status
        {
            get
            {
                switch (this.SW1)
                {
                    case 0x61:
                        return ApduStatus.DataAvailable;

                    case 0x6C:
                        return ApduStatus.WrongLengthInLe;

                    default:
                        var bytes = new[] { this.SW1, this.SW2 };

                        if (BitConverter.IsLittleEndian)
                        {
                            bytes = bytes.Reverse().ToArray();
                        }

                        ushort status = BitConverter.ToUInt16(bytes, 0);

                        return (ApduStatus)status;
                }
            }
        }

        /// <summary>
        /// Returns the processing status of the Response APDU, by combining SW1+SW2 in to a status
        /// word. Where SW2 returns further information, this is provided as an out parameter.
        /// </summary>
        public ApduStatus GetStatus(out byte info)
        {
            switch (this.Status)
            {
                case ApduStatus.DataAvailable:
                    info = this.SW2;
                    break;

                case ApduStatus.WrongLengthInLe:
                    info = this.SW2;
                    break;

                default:
                    info = Byte.MinValue;
                    break;
            }

            return this.Status;
        }

        /// <summary>
        /// Builds an ISO 7816-4 Response APDU. 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static ResponseApdu Build(IEnumerable<byte> buffer)
        {
            var buff = buffer.ToArray();

            if (buff.Length < 2)
            {
                throw new ArgumentException("A response must be at least 2 bytes", nameof(buffer));
            }

            if (buff.Length > 258)
            {
                throw new ArgumentException("A response must be no more than 256 bytes", nameof(buffer));
            }

            return new ResponseApdu
            {
                Buffer = buff
            };
        }

        public bool Equals(ResponseApdu other) => this.Equals(other.Buffer);

        public bool Equals(IEnumerable<byte> other) => this.Buffer.SequenceEqual(other);

        public override string ToString() => BitConverter.ToString(this.Buffer.ToArray());

        public string ToString(string separator)
            => this.ToString().Replace("-", separator);

        public string ToString(string separator, IFormatProvider formatProvider)
            => this.ToString(separator);

        public static implicit operator byte[] (ResponseApdu apdu) => apdu.Buffer;

        public static explicit operator ResponseApdu(byte[] buffer) => Build(buffer);
    }
}
