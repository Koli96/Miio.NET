using System;
using System.Linq;

namespace Miio.Utilities
{
    public static class ByteUtilities
    {
        /// <summary>
        /// Converts hexadecimal number represented by string to array of bytes
        /// </summary>
        /// <param name="hex">Hexadecimal number</param>
        /// <returns>Number as array of bytes</returns>
        public static byte[] ConvertToBytes(string hex)
        {
            if(string.IsNullOrWhiteSpace(hex))
            {
                return Array.Empty<byte>();
            }
            return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
        }

        /// <summary>
        /// Converts hexadecimal number represented by unsigned integer to array of bytes
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <param name="bigEndian">Should be represented by big endian</param>
        /// <returns>Number as array of bytes</returns>
        public static byte[] ConvertToBytes(uint number, bool bigEndian = true)
        {
            var converted = BitConverter.GetBytes(number);
            if(BitConverter.IsLittleEndian && !bigEndian)
            {
                return converted;
            }
            else if(BitConverter.IsLittleEndian && bigEndian)
            {
                return converted.Reverse().ToArray();
            }
            else if(!BitConverter.IsLittleEndian && !bigEndian)
            {
                return converted.Reverse().ToArray();
            }
            else
            {
                return converted;
            }
        }

        /// <summary>
        /// Converts hexadecimal number represented by unsigned integer to array of bytes
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <param name="bigEndian">Should be represented by big endian</param>
        /// <returns>Number as array of bytes</returns>
        public static byte[] ConvertToBytes(ushort number, bool bigEndian = true)
        {
            var converted = BitConverter.GetBytes(number);
            if(BitConverter.IsLittleEndian && !bigEndian)
            {
                return converted;
            }
            else if(BitConverter.IsLittleEndian && bigEndian)
            {
                return converted.Reverse().ToArray();
            }
            else if(!BitConverter.IsLittleEndian && !bigEndian)
            {
                return converted.Reverse().ToArray();
            }
            else
            {
                return converted;
            }
        }

        /// <summary>
        /// Converts hexadecimal number represented by array of bytes to number as string (hex)
        /// </summary>
        /// <param name="bytes">Number as array of bytes</param>
        /// <returns>Number as hexadecimal string</returns>
        public static string ConvertToString(byte[] bytes)
        {
            return string.Join(null, bytes.Select(x => x.ToString("X2")));
        }
    }
}
