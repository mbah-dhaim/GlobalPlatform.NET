using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Extensions
{
    internal static class ByteExtensions
    {
        /// <summary>
        /// Adds a range of bytes to the collection, prefixed by a single byte denoting the range's length. 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static byte AddRangeWithLength(this ICollection<byte> bytes, byte[] range)
        {
            byte length = range.LengthChecked();

            bytes.Add(length);
            bytes.AddRange(range);

            return length;
        }

        /// <summary>
        /// Adds a tag to the collection, followed by the length of the data, followed by the data itself. 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="tag"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte AddTag(this ICollection<byte> bytes, byte tag, params byte[] data)
        {
            bytes.Add(tag);

            byte length = data.LengthChecked();

            bytes.Add(length);
            bytes.AddRange(data);

            return length;
        }

        /// <summary>
        /// Returns the length of the array, as a checked byte. 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte LengthChecked(this IEnumerable<byte> array) => checked((byte)array.Count());

        /// <summary>
        /// Pads a byte array using ISO/IEC 7816-4. 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static IList<byte> Pad(this IList<byte> bytes)
        {
            bytes.Add(0x80);

            if (bytes.Count % 8 != 0)
            {
                bytes.AddRange(Enumerable.Repeat<byte>(0x00, 8 - bytes.Count % 8));
            }

            return bytes;
        }
    }
}
