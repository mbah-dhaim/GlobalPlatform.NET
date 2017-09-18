using GlobalPlatform.NET.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Tools
{
    /// <summary>
    /// Builds and parses data encoded using ASN.1 BER-TLV. 
    /// </summary>
    public static class TLV
    {
        private static byte[] EndOfContent = {0x00, 0x00};

        /// <summary>
        /// Encodes a block of data using ASN.1 BER-TLV. Length is encoded as definite. 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ICollection<byte> Build(byte tag, ICollection<byte> data)
        {
            var tlv = new List<byte> { tag };

            if (data.Count < 128)
            {
                tlv.Add(data.LengthChecked());
            }
            else
            {
                byte[] lengthBytes = BitConverter.GetBytes(data.Count);

                if (BitConverter.IsLittleEndian)
                {
                    lengthBytes = lengthBytes.Reverse().ToArray();
                }

                while (lengthBytes.First() == 0x00)
                {
                    lengthBytes = lengthBytes.Skip(1).ToArray();
                }

                tlv.Add((byte)(lengthBytes.Length ^ 0b10000000));

                tlv.AddRange(lengthBytes);
            }

            tlv.AddRange(data);

            return tlv;
        }

        /// <summary>
        /// Parses a block of data encoded using ASN.1 BER-TLV. Returns the value, disregarding the tag and length bytes.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static ICollection<byte> Parse(ICollection<byte> data, byte tag)
        {
            int pos = 0;

            byte Seek(int count = 1) => SeekX(count).First();
            byte Next() => NextX().First();

            IEnumerable<byte> SeekX(int count = 1)
            {
                return data.Skip(pos).Take(count);
            }

            IEnumerable<byte> NextX(int count = 1)
            {
                var bytes = data.Skip(pos).Take(count);

                pos += count;

                return bytes;
            }

            bool IsBitHigh(byte b, byte bit)
            {
                if (bit > 8)
                {
                    throw new ArgumentException("Bit index cannot be higher than 8", nameof(bit));
                }

                return (b & (1 << bit - 1)) != 0;
            }

            byte GetBits7Thru1(byte value) => (byte)(value & 0b01111111);

            int GetLength()
            {
                byte byte1 = Next();

                if (IsBitHigh(byte1, 8))
                {
                    byte numLengthBytes = GetBits7Thru1(byte1);

                    switch (numLengthBytes)
                    {
                        case 0:
                            if (!data.TakeLast(EndOfContent.Length).SequenceEqual(EndOfContent))
                            {
                                throw new InvalidOperationException("Length octets suggest indefinite encoding but data does not end with 2 low bytes.");
                            }
                            else
                            {
                                return data.Count - pos - EndOfContent.Length;
                            }

                        case 127:
                            throw new InvalidOperationException("Length octets denoting 'reserved' (0b11111111) are not supported.");

                        default:
                            var bytes = NextX(numLengthBytes).ToArray();

                            if (bytes.Length < 4)
                            {
                                bytes = bytes.Concat(Enumerable.Repeat((byte)0x00, 4 - bytes.Length)).ToArray();
                            }

                            return BitConverter.ToInt16(bytes, 0);
                    }
                }

                return GetBits7Thru1(byte1);
            }

            if (Next() != tag)
            {
                throw new InvalidOperationException("First byte of supplied data was not equal to the specified tag.");
            }

            return NextX(GetLength()).ToList();
        }

        /// <summary>
        /// Recursively parses a block of data encoded using ASN.1 BER-TLV, returning the innermost tag value, disregarding the tag and length bytes.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static ICollection<byte> Parse(ICollection<byte> data, params byte[] tags) =>
            tags.Aggregate(data, Parse);
    }
}
