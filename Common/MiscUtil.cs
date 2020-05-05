using Microsoft.Win32;
using MiscUtil.App;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Timers;

namespace MiscUtil.Conversion
{
    /// <summary>
    /// Equivalent of System.BitConverter, but with either endianness.
    /// </summary>
    public abstract class EndianBitConverter
    {
        #region Endianness of this converter
        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        /// <remarks>
        /// Different computer architectures store data using different byte orders. "Big-endian"
        /// means the most significant byte is on the left end of a word. "Little-endian" means the
        /// most significant byte is on the right end of a word.
        /// </remarks>
        /// <returns>true if this converter is little-endian, false otherwise.</returns>
        public abstract bool IsLittleEndian();

        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        public abstract Endianness Endianness { get; }
        #endregion

        #region Factory properties
        static LittleEndianBitConverter little = new LittleEndianBitConverter();
        /// <summary>
        /// Returns a little-endian bit converter instance. The same instance is
        /// always returned.
        /// </summary>
        public static LittleEndianBitConverter Little
        {
            get { return little; }
        }

        static BigEndianBitConverter big = new BigEndianBitConverter();
        /// <summary>
        /// Returns a big-endian bit converter instance. The same instance is
        /// always returned.
        /// </summary>
        public static BigEndianBitConverter Big
        {
            get { return big; }
        }
        #endregion

        #region Double/primitive conversions
        /// <summary>
        /// Converts the specified double-precision floating point number to a
        /// 64-bit signed integer. Note: the endianness of this converter does not
        /// affect the returned value.
        /// </summary>
        /// <param name="value">The number to convert. </param>
        /// <returns>A 64-bit signed integer whose value is equivalent to value.</returns>
        public long DoubleToInt64Bits(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        /// <summary>
        /// Converts the specified 64-bit signed integer to a double-precision
        /// floating point number. Note: the endianness of this converter does not
        /// affect the returned value.
        /// </summary>
        /// <param name="value">The number to convert. </param>
        /// <returns>A double-precision floating point number whose value is equivalent to value.</returns>
        public double Int64BitsToDouble(long value)
        {
            return BitConverter.Int64BitsToDouble(value);
        }

        /// <summary>
        /// Converts the specified single-precision floating point number to a
        /// 32-bit signed integer. Note: the endianness of this converter does not
        /// affect the returned value.
        /// </summary>
        /// <param name="value">The number to convert. </param>
        /// <returns>A 32-bit signed integer whose value is equivalent to value.</returns>
        public int SingleToInt32Bits(float value)
        {
            return new Int32SingleUnion(value).AsInt32;
        }

        /// <summary>
        /// Converts the specified 32-bit signed integer to a single-precision floating point
        /// number. Note: the endianness of this converter does not
        /// affect the returned value.
        /// </summary>
        /// <param name="value">The number to convert. </param>
        /// <returns>A single-precision floating point number whose value is equivalent to value.</returns>
        public float Int32BitsToSingle(int value)
        {
            return new Int32SingleUnion(value).AsSingle;
        }
        #endregion

        #region To(PrimitiveType) conversions
        /// <summary>
        /// Returns a Boolean value converted from one byte at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>true if the byte at startIndex in value is nonzero; otherwise, false.</returns>
        public bool ToBoolean(byte[] value, int startIndex)
        {
            CheckByteArgument(value, startIndex, 1);
            return BitConverter.ToBoolean(value, startIndex);
        }

        /// <summary>
        /// Returns a Unicode character converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A character formed by two bytes beginning at startIndex.</returns>
        public char ToChar(byte[] value, int startIndex)
        {
            return unchecked((char)(CheckedFromBytes(value, startIndex, 2)));
        }

        /// <summary>
        /// Returns a double-precision floating point number converted from eight bytes
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A double precision floating point number formed by eight bytes beginning at startIndex.</returns>
        public double ToDouble(byte[] value, int startIndex)
        {
            return Int64BitsToDouble(ToInt64(value, startIndex));
        }

        /// <summary>
        /// Returns a single-precision floating point number converted from four bytes
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A single precision floating point number formed by four bytes beginning at startIndex.</returns>
        public float ToSingle(byte[] value, int startIndex)
        {
            return Int32BitsToSingle(ToInt32(value, startIndex));
        }

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
        public short ToInt16(byte[] value, int startIndex)
        {
            return unchecked((short)(CheckedFromBytes(value, startIndex, 2)));
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 32-bit signed integer formed by four bytes beginning at startIndex.</returns>
        public int ToInt32(byte[] value, int startIndex)
        {
            return unchecked((int)(CheckedFromBytes(value, startIndex, 4)));
        }

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 64-bit signed integer formed by eight bytes beginning at startIndex.</returns>
        public long ToInt64(byte[] value, int startIndex)
        {
            return CheckedFromBytes(value, startIndex, 8);
        }

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit unsigned integer formed by two bytes beginning at startIndex.</returns>
        public ushort ToUInt16(byte[] value, int startIndex)
        {
            return unchecked((ushort)(CheckedFromBytes(value, startIndex, 2)));
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 32-bit unsigned integer formed by four bytes beginning at startIndex.</returns>
        public uint ToUInt32(byte[] value, int startIndex)
        {
            return unchecked((uint)(CheckedFromBytes(value, startIndex, 4)));
        }

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 64-bit unsigned integer formed by eight bytes beginning at startIndex.</returns>
        public ulong ToUInt64(byte[] value, int startIndex)
        {
            return unchecked((ulong)(CheckedFromBytes(value, startIndex, 8)));
        }

        /// <summary>
        /// Checks the given argument for validity.
        /// </summary>
        /// <param name="value">The byte array passed in</param>
        /// <param name="startIndex">The start index passed in</param>
        /// <param name="bytesRequired">The number of bytes required</param>
        /// <exception cref="ArgumentNullException">value is a null reference</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// startIndex is less than zero or greater than the length of value minus bytesRequired.
        /// </exception>
        static void CheckByteArgument(byte[] value, int startIndex, int bytesRequired)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (startIndex < 0 || startIndex > value.Length - bytesRequired)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
        }

        /// <summary>
        /// Checks the arguments for validity before calling FromBytes
        /// (which can therefore assume the arguments are valid).
        /// </summary>
        /// <param name="value">The bytes to convert after checking</param>
        /// <param name="startIndex">The index of the first byte to convert</param>
        /// <param name="bytesToConvert">The number of bytes to convert</param>
        /// <returns></returns>
        long CheckedFromBytes(byte[] value, int startIndex, int bytesToConvert)
        {
            CheckByteArgument(value, startIndex, bytesToConvert);
            return FromBytes(value, startIndex, bytesToConvert);
        }

        /// <summary>
        /// Convert the given number of bytes from the given array, from the given start
        /// position, into a long, using the bytes as the least significant part of the long.
        /// By the time this is called, the arguments have been checked for validity.
        /// </summary>
        /// <param name="value">The bytes to convert</param>
        /// <param name="startIndex">The index of the first byte to convert</param>
        /// <param name="bytesToConvert">The number of bytes to use in the conversion</param>
        /// <returns>The converted number</returns>
        protected abstract long FromBytes(byte[] value, int startIndex, int bytesToConvert);
        #endregion

        #region ToString conversions
        /// <summary>
        /// Returns a String converted from the elements of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <remarks>All the elements of value are converted.</remarks>
        /// <returns>
        /// A String of hexadecimal pairs separated by hyphens, where each pair
        /// represents the corresponding element in value; for example, "7F-2C-4A".
        /// </returns>
        public static string ToString(byte[] value)
        {
            return BitConverter.ToString(value);
        }

        /// <summary>
        /// Returns a String converted from the elements of a byte array starting at a specified array position.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <remarks>The elements from array position startIndex to the end of the array are converted.</remarks>
        /// <returns>
        /// A String of hexadecimal pairs separated by hyphens, where each pair
        /// represents the corresponding element in value; for example, "7F-2C-4A".
        /// </returns>
        public static string ToString(byte[] value, int startIndex)
        {
            return BitConverter.ToString(value, startIndex);
        }

        /// <summary>
        /// Returns a String converted from a specified number of bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <param name="length">The number of bytes to convert.</param>
        /// <remarks>The length elements from array position startIndex are converted.</remarks>
        /// <returns>
        /// A String of hexadecimal pairs separated by hyphens, where each pair
        /// represents the corresponding element in value; for example, "7F-2C-4A".
        /// </returns>
        public static string ToString(byte[] value, int startIndex, int length)
        {
            return BitConverter.ToString(value, startIndex, length);
        }
        #endregion

        #region    Decimal conversions
        /// <summary>
        /// Returns a decimal value converted from sixteen bytes
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A decimal  formed by sixteen bytes beginning at startIndex.</returns>
        public decimal ToDecimal(byte[] value, int startIndex)
        {
            // HACK: This always assumes four parts, each in their own endianness,
            // starting with the first part at the start of the byte array.
            // On the other hand, there's no real format specified...
            int[] parts = new int[4];
            for (int i = 0; i < 4; i++)
            {
                parts[i] = ToInt32(value, startIndex + i * 4);
            }
            return new Decimal(parts);
        }

        /// <summary>
        /// Returns the specified decimal value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 16.</returns>
        public byte[] GetBytes(decimal value)
        {
            byte[] bytes = new byte[16];
            int[] parts = decimal.GetBits(value);
            for (int i = 0; i < 4; i++)
            {
                CopyBytesImpl(parts[i], 4, bytes, i * 4);
            }
            return bytes;
        }

        /// <summary>
        /// Copies the specified decimal value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">A character to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(decimal value, byte[] buffer, int index)
        {
            int[] parts = decimal.GetBits(value);
            for (int i = 0; i < 4; i++)
            {
                CopyBytesImpl(parts[i], 4, buffer, i * 4 + index);
            }
        }
        #endregion

        #region GetBytes conversions
        /// <summary>
        /// Returns an array with the given number of bytes formed
        /// from the least significant bytes of the specified value.
        /// This is used to implement the other GetBytes methods.
        /// </summary>
        /// <param name="value">The value to get bytes for</param>
        /// <param name="bytes">The number of significant bytes to return</param>
        byte[] GetBytes(long value, int bytes)
        {
            byte[] buffer = new byte[bytes];
            CopyBytes(value, bytes, buffer, 0);
            return buffer;
        }

        /// <summary>
        /// Returns the specified Boolean value as an array of bytes.
        /// </summary>
        /// <param name="value">A Boolean value.</param>
        /// <returns>An array of bytes with length 1.</returns>
        public byte[] GetBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// Returns the specified Unicode character value as an array of bytes.
        /// </summary>
        /// <param name="value">A character to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        public byte[] GetBytes(char value)
        {
            return GetBytes(value, 2);
        }

        /// <summary>
        /// Returns the specified double-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        public byte[] GetBytes(double value)
        {
            return GetBytes(DoubleToInt64Bits(value), 8);
        }

        /// <summary>
        /// Returns the specified 16-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        public byte[] GetBytes(short value)
        {
            return GetBytes(value, 2);
        }

        /// <summary>
        /// Returns the specified 32-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        public byte[] GetBytes(int value)
        {
            return GetBytes(value, 4);
        }

        /// <summary>
        /// Returns the specified 64-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        public byte[] GetBytes(long value)
        {
            return GetBytes(value, 8);
        }

        /// <summary>
        /// Returns the specified single-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        public byte[] GetBytes(float value)
        {
            return GetBytes(SingleToInt32Bits(value), 4);
        }

        /// <summary>
        /// Returns the specified 16-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        public byte[] GetBytes(ushort value)
        {
            return GetBytes(value, 2);
        }

        /// <summary>
        /// Returns the specified 32-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        public byte[] GetBytes(uint value)
        {
            return GetBytes(value, 4);
        }

        /// <summary>
        /// Returns the specified 64-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        public byte[] GetBytes(ulong value)
        {
            return GetBytes(unchecked((long)value), 8);
        }

        #endregion

        #region CopyBytes conversions
        /// <summary>
        /// Copies the given number of bytes from the least-specific
        /// end of the specified value into the specified byte array, beginning
        /// at the specified index.
        /// This is used to implement the other CopyBytes methods.
        /// </summary>
        /// <param name="value">The value to copy bytes for</param>
        /// <param name="bytes">The number of significant bytes to copy</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        void CopyBytes(long value, int bytes, byte[] buffer, int index)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", "Byte array must not be null");
            }
            if (buffer.Length < index + bytes)
            {
                throw new ArgumentOutOfRangeException("Buffer not big enough for value");
            }
            CopyBytesImpl(value, bytes, buffer, index);
        }

        /// <summary>
        /// Copies the given number of bytes from the least-specific
        /// end of the specified value into the specified byte array, beginning
        /// at the specified index.
        /// This must be implemented in concrete derived classes, but the implementation
        /// may assume that the value will fit into the buffer.
        /// </summary>
        /// <param name="value">The value to copy bytes for</param>
        /// <param name="bytes">The number of significant bytes to copy</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        protected abstract void CopyBytesImpl(long value, int bytes, byte[] buffer, int index);

        /// <summary>
        /// Copies the specified Boolean value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">A Boolean value.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(bool value, byte[] buffer, int index)
        {
            CopyBytes(value ? 1 : 0, 1, buffer, index);
        }

        /// <summary>
        /// Copies the specified Unicode character value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">A character to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(char value, byte[] buffer, int index)
        {
            CopyBytes(value, 2, buffer, index);
        }

        /// <summary>
        /// Copies the specified double-precision floating point value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(double value, byte[] buffer, int index)
        {
            CopyBytes(DoubleToInt64Bits(value), 8, buffer, index);
        }

        /// <summary>
        /// Copies the specified 16-bit signed integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(short value, byte[] buffer, int index)
        {
            CopyBytes(value, 2, buffer, index);
        }

        /// <summary>
        /// Copies the specified 32-bit signed integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(int value, byte[] buffer, int index)
        {
            CopyBytes(value, 4, buffer, index);
        }

        /// <summary>
        /// Copies the specified 64-bit signed integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(long value, byte[] buffer, int index)
        {
            CopyBytes(value, 8, buffer, index);
        }

        /// <summary>
        /// Copies the specified single-precision floating point value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(float value, byte[] buffer, int index)
        {
            CopyBytes(SingleToInt32Bits(value), 4, buffer, index);
        }

        /// <summary>
        /// Copies the specified 16-bit unsigned integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(ushort value, byte[] buffer, int index)
        {
            CopyBytes(value, 2, buffer, index);
        }

        /// <summary>
        /// Copies the specified 32-bit unsigned integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(uint value, byte[] buffer, int index)
        {
            CopyBytes(value, 4, buffer, index);
        }

        /// <summary>
        /// Copies the specified 64-bit unsigned integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void CopyBytes(ulong value, byte[] buffer, int index)
        {
            CopyBytes(unchecked((long)value), 8, buffer, index);
        }

        #endregion

        #region Private struct used for Single/Int32 conversions
        /// <summary>
        /// Union used solely for the equivalent of DoubleToInt64Bits and vice versa.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        struct Int32SingleUnion
        {
            /// <summary>
            /// Int32 version of the value.
            /// </summary>
            [FieldOffset(0)]
            int i;
            /// <summary>
            /// Single version of the value.
            /// </summary>
            [FieldOffset(0)]
            float f;

            /// <summary>
            /// Creates an instance representing the given integer.
            /// </summary>
            /// <param name="i">The integer value of the new instance.</param>
            internal Int32SingleUnion(int i)
            {
                this.f = 0; // Just to keep the compiler happy
                this.i = i;
            }

            /// <summary>
            /// Creates an instance representing the given floating point number.
            /// </summary>
            /// <param name="f">The floating point value of the new instance.</param>
            internal Int32SingleUnion(float f)
            {
                this.i = 0; // Just to keep the compiler happy
                this.f = f;
            }

            /// <summary>
            /// Returns the value of the instance as an integer.
            /// </summary>
            internal int AsInt32
            {
                get { return i; }
            }

            /// <summary>
            /// Returns the value of the instance as a floating point number.
            /// </summary>
            internal float AsSingle
            {
                get { return f; }
            }
        }
        #endregion
    }

    /// <summary>
    /// Endianness of a converter
    /// </summary>
    public enum Endianness
    {
        /// <summary>
        /// Little endian - least significant byte first
        /// </summary>
        LittleEndian,
        /// <summary>
        /// Big endian - most significant byte first
        /// </summary>
        BigEndian
    }

    /// <summary>
    /// Implementation of EndianBitConverter which converts to/from big-endian
    /// byte arrays.
    /// </summary>
    public sealed class BigEndianBitConverter : EndianBitConverter
    {
        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        /// <remarks>
        /// Different computer architectures store data using different byte orders. "Big-endian"
        /// means the most significant byte is on the left end of a word. "Little-endian" means the
        /// most significant byte is on the right end of a word.
        /// </remarks>
        /// <returns>true if this converter is little-endian, false otherwise.</returns>
        public sealed override bool IsLittleEndian()
        {
            return false;
        }

        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        public sealed override Endianness Endianness
        {
            get { return Endianness.BigEndian; }
        }

        /// <summary>
        /// Copies the specified number of bytes from value to buffer, starting at index.
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <param name="bytes">The number of bytes to copy</param>
        /// <param name="buffer">The buffer to copy the bytes into</param>
        /// <param name="index">The index to start at</param>
        protected override void CopyBytesImpl(long value, int bytes, byte[] buffer, int index)
        {
            int endOffset = index + bytes - 1;
            for (int i = 0; i < bytes; i++)
            {
                buffer[endOffset - i] = unchecked((byte)(value & 0xff));
                value = value >> 8;
            }
        }

        /// <summary>
        /// Returns a value built from the specified number of bytes from the given buffer,
        /// starting at index.
        /// </summary>
        /// <param name="buffer">The data in byte array format</param>
        /// <param name="startIndex">The first index to use</param>
        /// <param name="bytesToConvert">The number of bytes to use</param>
        /// <returns>The value built from the given bytes</returns>
        protected override long FromBytes(byte[] buffer, int startIndex, int bytesToConvert)
        {
            long ret = 0;
            for (int i = 0; i < bytesToConvert; i++)
            {
                ret = unchecked((ret << 8) | buffer[startIndex + i]);
            }
            return ret;
        }
    }

    /// <summary>
    /// Implementation of EndianBitConverter which converts to/from little-endian
    /// byte arrays.
    /// </summary>
    public sealed class LittleEndianBitConverter : EndianBitConverter
    {
        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        /// <remarks>
        /// Different computer architectures store data using different byte orders. "Big-endian"
        /// means the most significant byte is on the left end of a word. "Little-endian" means the
        /// most significant byte is on the right end of a word.
        /// </remarks>
        /// <returns>true if this converter is little-endian, false otherwise.</returns>
        public sealed override bool IsLittleEndian()
        {
            return true;
        }

        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        public sealed override Endianness Endianness
        {
            get { return Endianness.LittleEndian; }
        }

        /// <summary>
        /// Copies the specified number of bytes from value to buffer, starting at index.
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <param name="bytes">The number of bytes to copy</param>
        /// <param name="buffer">The buffer to copy the bytes into</param>
        /// <param name="index">The index to start at</param>
        protected override void CopyBytesImpl(long value, int bytes, byte[] buffer, int index)
        {
            for (int i = 0; i < bytes; i++)
            {
                buffer[i + index] = unchecked((byte)(value & 0xff));
                value = value >> 8;
            }
        }

        /// <summary>
        /// Returns a value built from the specified number of bytes from the given buffer,
        /// starting at index.
        /// </summary>
        /// <param name="buffer">The data in byte array format</param>
        /// <param name="startIndex">The first index to use</param>
        /// <param name="bytesToConvert">The number of bytes to use</param>
        /// <returns>The value built from the given bytes</returns>
        protected override long FromBytes(byte[] buffer, int startIndex, int bytesToConvert)
        {
            long ret = 0;
            for (int i = 0; i < bytesToConvert; i++)
            {
                ret = unchecked((ret << 8) | buffer[startIndex + bytesToConvert - 1 - i]);
            }
            return ret;
        }
    }

    public sealed class MiscConverter
    {
        public static byte GetNMEACheckSum(byte b1, byte b2)
        {
            return (byte)(GetHexVal((char)b1) * 0x10 + GetHexVal((char)b2));
        }

        //---------------------------------------------------------
        // GetBitData: get data from big-endian bits in U08 array
        //
        // Input:   buff - buffer array
        //          pos  - bit position from start of data (bits)
        //          len  - bit length (bits) (len<=32)
        //          dataLen - length of output data

        // Output:  data - output data pointr
        // Return:  NONE
        //---------------------------------------------------------
        public static void GetBitData(byte[] buff, int start, int pos, int len, byte[] data)
        {
            int i, o;
            if (len <= 0 || data.Length * 8 < len)
            {
                return;
            }

            int oPtr = 0;
            Array.Clear(data, 0, data.Length);
            for (i = (pos + len - 1), o = 0; i >= pos; --i, ++o)
            {
                byte imask = (byte)(1 << (7 - i % 8));
                byte omask = (byte)(1 << (o % 8));
                if ((buff[start + i / 8] & imask) != 0)
                {
                    data[oPtr] |= omask;
                }
                else
                {
                    data[oPtr] &= (byte)(~omask);
                }
                if (o % 8 == 7)
                {
                    ++oPtr;
                }
            }
        }

        public static byte[] ConvertInt38Sign(byte[] d)
        {
            if ((d[4] & 0x20) != 0)
            {
                d[5] = 0xFF;
                d[6] = 0xFF;
                d[7] = 0xFF;
                d[4] |= 0xF0;
            }
            return d;
        }

        public static int GetBitFlagCounts(byte[] buff)
        {
            int count = 0;
            for (int i = 0; i < buff.Length * 8; ++i)
            {
                if (((buff[i / 8] >> (8 - (i % 8) - 1)) & 0x1) != 0)
                {
                    ++count;
                }
            }
            return count;
        }

        public static int GetNoneZeroBitPosition(byte[] buff, int order)
        {
            int count = 0;
            for (int i = 0; i < buff.Length * 8; ++i)
            {
                if (((buff[buff.Length - i / 8 - 1] >> (8 - (i % 8) - 1)) & 0x1) != 0)
                {
                    if (count++ == order)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static void ConvertDegToDegMin(double dd, ref int d, ref double m)
        {
            d = (int)dd;
            m = (dd - d) * 60.0;
        }

        public static void ConvertDegToDegMinSec(double dd, ref int d, ref int m, ref double s)
        {
            d = (int)dd;
            m = (int)((dd - d) * 60.0);
            s = ((dd - d) * 60.0 - m) * 60.0;
        }

        public static double ConvertDegMinToDeg(int d, double m)
        {
            return d + m / 60.0;
        }

        public static double ConvertDegMinSecToDeg(int d, int m, double s)
        {
            return d + m / 60.0 + s / 3600.0;
        }

        public static string GetSerialNumberString(byte[] s, int len)
        {
            if (len == 8)
            {
                return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}",
                    s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7]);
            }
            else if(len == 12)
            {
                return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                    s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7], s[8], s[9], s[10], s[11]);
            }
            else if(len == 16)
            {
                return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}{12:X2}{13:X2}{14:X2}{15:X2}",
                    s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7], s[8], s[9], s[10], s[11], s[12], s[13], s[14], s[15]);
            }
            return "";
        }

        public static byte[] StringToByteArrayNoSpace(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                throw new Exception("The binary key cannot have an odd number of digits");
            }

            byte[] arr = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static byte[] StringToByteArrayWithSpace(string hex)
        {
            return StringToByteArrayNoSpace(hex.Replace(" ", "").ToUpper());
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}

namespace MiscUtil.UI
{
    public static class ListBoxUtil
    {
        private const int MaxItemCount = 10000;
        private const int RemoveItemCount = 2000;
        public static void AddStringToListBoxAndScrollToBottom(ListBox lsb, string txt)
        {
            if(lsb.Items.Count > MaxItemCount)
            {
                for (int i = 0; i < RemoveItemCount; ++i)
                    lsb.Items.RemoveAt(i);
            }
            lsb.Items.Add(txt);
            lsb.TopIndex = Math.Max(lsb.Items.Count - lsb.ClientSize.Height / lsb.ItemHeight + 1, 0);
        }
    }

    public static class FormTools
    {
        public static int ConfigureFormWidth = 520;
        public static int ConfigureFormHeight = 400;
        public static void OnLoadResize(Form form)
        {
            if (form.Width > ConfigureFormWidth || form.Height > ConfigureFormHeight)
            {
                form.Width = ConfigureFormWidth;
                form.Height = ConfigureFormHeight;
                //form.center
            }
        }
    }
}

namespace MiscUtil.App
{
    public class AppInfo
    {
        public string version;
        public string path;
        public int size = 0;
        public string sha;
        public string mode;
        public string xml;
        public bool Validate()
        {
            return (version.Length > 0) && (path.Length > 0) && (size > 0) &&
                (sha.Length > 0) && (mode.Length > 0);
        }
    }

    public class FirmwareInfo
    {
        public struct FwFileInfo
        {
            public string kernelVer;
            public string softwareVer;
            public string revision;
            public int baud;
            public string path;
            public int size;
            public string crc;
            public string mode;
        }
        public FwFileInfo fwM = new FwFileInfo();
        public FwFileInfo fwS = new FwFileInfo();

        public string xml;
        public bool Validate()
        {
            return (fwM.size > 0) && (fwS.size > 0) && (fwM.crc.Length > 0) && (fwS.crc.Length > 0) &&
                (fwM.revision.Length > 0) && (fwS.revision.Length > 0) && (fwM.softwareVer.Length > 0) &&
                (fwS.softwareVer.Length > 0) && (fwM.kernelVer.Length > 0) && (fwS.kernelVer.Length > 0) &&
                (fwM.path.Length > 0) && (fwS.path.Length > 0) && (fwM.baud > 0) && (fwS.baud > 0);
        }

        public bool ValidatePlus()
        {
            return (fwM.size > 0) && (fwM.crc.Length > 0) &&
                (fwM.revision.Length > 0) && (fwM.softwareVer.Length > 0) &&
                (fwM.kernelVer.Length > 0) && (fwM.path.Length > 0) && (fwM.baud > 0);
        }
    }

    public static class AppTools
    {
        public enum SpecialFolder
        {
            AdministrativeTools = 48,
            //{user name}\Start Menu\Programs\Administrative Tools 
            ApplicationData = 26,
            //{user name}\Application Data 
            CommonAdministrativeTools = 47,
            //All Users\Start Menu\Programs\Administrative Tools 
            CommonApplicationData = 35,
            //All Users\Application Data 
            CommonDesktopDirectory = 25,
            //All Users\Desktop 
            CommonDocuments = 46,
            //All Users\Documents 
            CommonFavorites = 31,
            CommonNonLocalizedStartup = 30,
            //non localized common startup 
            CommonPrograms = 23,
            //All Users\Programs 
            CommonStartMenu = 22,
            //All Users\Start Menu 
            CommonStartup = 24,
            //All Users\Startup 
            CommonTemplates = 45,
            //All Users\Templates 
            ControlPanel = 3,
            //My Computer\Control Panel 
            Cookies = 33,
            DesktopDirectory = 16,
            //{user name}\Desktop 
            Favorites = 6,
            //{user name}\Favorites 
            Fonts = 20,
            //windows\fonts 
            History = 34,
            InternetCache = 32,
            LocalApplicationData = 28,
            //{user name}\Local Settings\Application Data (non roaming) 
            MyDocuments = 5,
            //My Documents 
            MyPictures = 39,
            //C:\Program Files\My Pictures 
            NetworkShortcuts = 19,
            //{user name}\nethood 
            NonLocalizedStartup = 29,
            //non localized startup 
            Printers = 4,
            //My Computer\Printers 
            PrintHood = 27,
            //{user name}\PrintHood 
            ProgramFiles = 38,
            //C:\Program Files 
            ProgramFilesCommon = 43,
            //C:\Program Files\Common 
            Programs = 2,
            //Start Menu\Programs 
            Recent = 8,
            //{user name}\Recent 
            RecycleBin = 10,
            //{desktop}\Recycle Bin 
            SendTo = 9,
            //{user name}\SendTo 
            StartMenu = 11,
            //{user name}\Start Menu 
            Startup = 7,
            //Start Menu\Programs\Startup 
            System = 37,
            //GetSystemDirectory() 
            Templates = 21,
            UserProfile = 40,
            //USERPROFILE 
            Windows = 36
            //GetWindowsDirectory() 
        }

        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);
        public static string GetSpecialFolderPath(SpecialFolder folder)
        {
            StringBuilder lpszPath = new StringBuilder(260);
            SHGetFolderPath(IntPtr.Zero, (int)folder, IntPtr.Zero, 0, lpszPath);
            return lpszPath.ToString();
        }

        const string AppFolderCompanyName = "\\Polaris";
        const string AppFolderAppName = "\\RTK_Viewer2";
        const string AppFolderDownloadName = "\\RTK_Viewer2_Tmp";
        public static string GetRtkViewerFolder(bool create)
        {
            StringBuilder sb = new StringBuilder(260);
            SHGetFolderPath(IntPtr.Zero, (int)SpecialFolder.LocalApplicationData, IntPtr.Zero, 0, sb);
            string path = sb.ToString() + AppFolderCompanyName;

            if (create && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += AppFolderAppName;
            if (create && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetRtkViewerDownloadFolder(bool create)
        {
            StringBuilder sb = new StringBuilder(260);
            SHGetFolderPath(IntPtr.Zero, (int)SpecialFolder.LocalApplicationData, IntPtr.Zero, 0, sb);
            string path = sb.ToString() + AppFolderCompanyName;

            if (create && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += AppFolderDownloadName;
            if (create && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetRtkViewerUpdaterPath()
        {
            return string.Format("{0}\\{1}", GetRtkViewerDownloadFolder(false), "RtkViewerUpdater.exe");
        }

        public static string GetRtkViewerLzmaPath()
        {
            return string.Format("{0}\\{1}", GetRtkViewerDownloadFolder(false), "Lzma.exe");
        }

        public static bool IsNewVersion(string current, string newone)
        {
            string[] curArray = current.Split('.');
            string[] newArray = newone.Split('.');

            for(int i = 0; i < 4; ++i)
            {
                if (Convert.ToInt32(curArray[i]) < Convert.ToInt32(newArray[i]))
                    return true;
            }
            return false;
        }

        public static string GetExeFileVersionInfo(string path)
        {
            return FileVersionInfo.GetVersionInfo(path).FileVersion;
        }

        public static string GetAppTitle() { return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false))?.Title; }
        public static string GetAppVersionString() { return Assembly.GetEntryAssembly().GetName().Version.ToString(); }
        public static string GetAppTitleWithVersion() { return GetAppTitle() + " V" + GetAppVersionString(); }

        public static AppInfo GetAppInfoFromXml(string xmlString)
        {
            AppInfo ai = new AppInfo();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);

            string appNode = xml.SelectNodes("STQXML/AppName").Item(0).InnerText.ToString();
            XmlNodeList nodeLists = xml.SelectNodes("STQXML/" + appNode + "/FileInfo");

            foreach (XmlNode oneNode in nodeLists)
            {
                String StrNodeName = oneNode.Name.ToString();
                foreach (XmlAttribute Attr in oneNode.Attributes)
                {
                    string attr = Attr.Name.ToString();
                    string valueStr = oneNode.Attributes[Attr.Name.ToString()].Value;

                    if (attr == "version")
                    {
                        ai.version = valueStr;
                    }
                    else if (attr == "path")
                    {
                        ai.path = valueStr;
                    }
                    else if (attr == "size")
                    {
                        ai.size = Convert.ToInt32(valueStr);
                    }
                    else if (attr == "sha")
                    {
                        ai.sha = valueStr;
                    }
                    else if (attr == "mode")
                    {
                        ai.mode = valueStr;
                    }
                }
            }

            XmlDocument doc = new XmlDocument();
            XmlElement stqxml = doc.CreateElement("STQXML");
            doc.AppendChild(stqxml);
            //Add AppName node
            XmlElement appname = doc.CreateElement("AppName");
            appname.InnerText = appNode;
            stqxml.AppendChild(appname);

            //Add RTKViewer Node
            XmlElement rtkViewer = doc.CreateElement(appNode);
            XmlElement fileInfo = doc.CreateElement("FileInfo");
            rtkViewer.AppendChild(fileInfo);
            stqxml.AppendChild(rtkViewer);
            fileInfo.SetAttribute("version", ai.version);
            fileInfo.SetAttribute("path", ai.path);
            fileInfo.SetAttribute("size", ai.size.ToString());
            fileInfo.SetAttribute("sha", ai.sha);
            fileInfo.SetAttribute("mode", ai.mode);

            ai.xml = doc.InnerXml;
            return ai;
        }

        private static FirmwareInfo.FwFileInfo ExactFwFileInfoXml(XmlNodeList nodeLists)
        {
            FirmwareInfo.FwFileInfo ffi = new FirmwareInfo.FwFileInfo();
            foreach (XmlNode oneNode in nodeLists)
            {
                String StrNodeName = oneNode.Name.ToString();
                foreach (XmlAttribute Attr in oneNode.Attributes)
                {
                    string attr = Attr.Name.ToString();
                    string v = oneNode.Attributes[Attr.Name.ToString()].Value;
                    if (attr == "kver")
                    {
                        ffi.kernelVer = v;
                    }
                    else if (attr == "sver")
                    {
                        ffi.softwareVer = v;
                    }
                    else if (attr == "rev")
                    {
                        ffi.revision = v;
                    }
                    else if (attr == "baud")
                    {
                        ffi.baud = Convert.ToInt32(v);
                    }
                    else if (attr == "path")
                    {
                        ffi.path = v;
                    }
                    else if (attr == "size")
                    {
                        ffi.size = Convert.ToInt32(v);
                    }
                    else if (attr == "crc")
                    {
                        ffi.crc = v;
                    }
                    else if (attr == "mode")
                    {
                        ffi.mode = v;
                    }
                }
            }
            return ffi;
        }

        public static FirmwareInfo GetFirmwareInfoFromXml(string xmlString)
        {
            FirmwareInfo fi = new FirmwareInfo();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);

            string appNode = xml.SelectNodes("STQXML/FirmwareName").Item(0).InnerText.ToString();
            XmlNodeList nodeLists = xml.SelectNodes("STQXML/" + appNode + "/FileInfoM");
            fi.fwM = ExactFwFileInfoXml(nodeLists);
            nodeLists = xml.SelectNodes("STQXML/" + appNode + "/FileInfoS");
            fi.fwS = ExactFwFileInfoXml(nodeLists);

            XmlDocument doc = new XmlDocument();
            XmlElement stqxml = doc.CreateElement("STQXML");
            doc.AppendChild(stqxml);
            //Add AppName node
            XmlElement appname = doc.CreateElement("FirmwareName");
            appname.InnerText = appNode;
            stqxml.AppendChild(appname);

            //Add RTKViewer Node
            XmlElement firmware = doc.CreateElement(appNode);
            XmlElement fileInfoM = doc.CreateElement("FileInfoM");
            firmware.AppendChild(fileInfoM);
            fileInfoM.SetAttribute("kver", fi.fwM.kernelVer);
            fileInfoM.SetAttribute("sver", fi.fwM.softwareVer);
            fileInfoM.SetAttribute("revision", fi.fwM.revision);
            fileInfoM.SetAttribute("baud", fi.fwM.baud.ToString());
            fileInfoM.SetAttribute("path", fi.fwM.path);
            fileInfoM.SetAttribute("size", fi.fwM.size.ToString());
            fileInfoM.SetAttribute("crc", fi.fwM.crc);
            fileInfoM.SetAttribute("mode", fi.fwM.mode);

            XmlElement fileInfoS = doc.CreateElement("FileInfoS");
            firmware.AppendChild(fileInfoS);
            fileInfoS.SetAttribute("kver", fi.fwS.kernelVer);
            fileInfoS.SetAttribute("sver", fi.fwS.softwareVer);
            fileInfoS.SetAttribute("revision", fi.fwS.revision);
            fileInfoS.SetAttribute("baud", fi.fwS.baud.ToString());
            fileInfoS.SetAttribute("path", fi.fwS.path);
            fileInfoS.SetAttribute("size", fi.fwS.size.ToString());
            fileInfoS.SetAttribute("crc", fi.fwS.crc);
            fileInfoS.SetAttribute("mode", fi.fwS.mode);
            stqxml.AppendChild(firmware);

            fi.xml = doc.InnerXml;
            return fi;
        }

        const string RegKeySource = "HKEY_CURRENT_USER\\Software\\Polaris RTKViewer";
        const string RegKeyPath = "Software\\Polaris RTKViewer";
        const string RegKeyFoldeer = "Polaris RTKViewer";
        const string RegKeyName = "source";
        public static void WriteSourceLocation(string path)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(RegKeyPath, true);
            if (reg == null)
            {
                RegistryKey regSw = Registry.CurrentUser.OpenSubKey("Software", true);
                regSw.CreateSubKey(RegKeyFoldeer);
                regSw.Close();
                reg = Registry.CurrentUser.OpenSubKey(RegKeyPath, true);
            }
            Registry.SetValue(RegKeySource, RegKeyName, path, RegistryValueKind.String);
            reg.Close();
            return;
        }

        public static string GetSourceLocation()
        {
            return (string)Registry.GetValue(RegKeySource, RegKeyName, "");
        }

        public static void LaunchExe(string path, string param)
        {
            var p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = param;
            p.Start();
        }

        public static bool LaunchExeWait(string path, string param, int timeout)
        {
            var p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = param;
            p.Start();
            return p.WaitForExit(timeout);
        }
        /// <summary>
        /// Extracts an embedded file to local file system.
        /// </summary>
        /// <param name="resName">Resource name of embedded file. NameSpace.FileName.Extension</param>
        /// <param name="fileName">Extracted file name</param>
        public static bool ExtractEmbeddedFile(string resName, string fileName)
        {
            try
            {
                FileTools.ExistDelete(fileName);
                Assembly assembly = Assembly.GetExecutingAssembly();

                using (var input = assembly.GetManifestResourceStream(resName))
                using (var output = File.Open(fileName, FileMode.CreateNew))
                {
                    if (input == null) throw new FileNotFoundException(resName + ": Embedded resoure file not found");

                    var buffer = new byte[32768];
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                    }
                    output.Flush();
                }
                return true;
            }
            catch(Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
                return false;
            }
        }
        
        private static Mutex mutex;
        public const string RtkViewerMutexName = "RtkViewer#99DBD532";
        public const string RtkViewerUpdaterMutexName = "RtkUpdater#D76D04ED";
        public const string RtkViewerFileName = "\\RtkViewer2.exe";

        public const string ViewerFtpFolderName = "Viewer2";
        public const string GlFirmwareFtpFolderName = "GlonassFw";
        public const string BdFirmwareFtpFolderName = "BeidouFw";
        //Added for Alpha+ on 20190210
        public const string PlusFirmwareFtpFolderName = "PhoenixBdFw";

        public static bool CreateMutex(string mutexName)
        {
            bool first = false;
            mutex = new Mutex(true, mutexName, out first);
            return !first;
        }
        public static void ReleaseMutex() { mutex.ReleaseMutex(); }

        public static string DebugTitle = "[RTKV]";
        //public static void ShowDebug(string s, 
        //    [CallerFilePath] string filePath = "",
        //    [CallerLineNumber] int lineNumber = 0)
        public static void ShowDebug(string s)
        {
            //Trace.WriteLine(string.Format("{0}({1})-{2}{3}", Path.GetFileName(filePath), lineNumber, DebugTitle, s));
            Trace.WriteLine(string.Format("{0}({1})-{2}{3}", "", "", DebugTitle, s));
        }
    }
}

namespace MiscUtil.Network
{
    public class UpdateServer
    {
        public delegate void FtpProgressFunction(int progressByte);
        /*
        public class WebClientAsync : WebClient
        {
            private int _timeoutMilliseconds;

            public WebClientAsync(int timeoutSeconds)
            {
                _timeoutMilliseconds = timeoutSeconds * 1000;

                System.Timers.Timer timer = new System.Timers.Timer(_timeoutMilliseconds);
                ElapsedEventHandler handler = null;

                handler = ((sender, args) =>
                {
                    timer.Elapsed -= handler;
                    this.CancelAsync();
                });

                timer.Elapsed += handler;
                timer.Enabled = true;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                request.Timeout = _timeoutMilliseconds;
                ((HttpWebRequest)request).ReadWriteTimeout = _timeoutMilliseconds;

                return request;
            }

            protected override void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
            {
                base.OnDownloadProgressChanged(e);
                timer.Reset(); //If this does not work try below
                timer.Start();
            }
        }
        */
        enum ServerType
        {
            None,
            Ftp,
            Http,
            Https
        }

        struct ServerItem
        {
            public string url;
            public int port;
            public ServerType type;
            public ServerItem(string u, int p, ServerType t)
            {
                url = u;
                port = p;
                type = t;
            }
        }

        static ServerItem[] serverList =
        {
#if DEBUG
            new ServerItem("192.168.0.83/alpha", 80, ServerType.Http),
#endif
            new ServerItem("203.66.45.201/alpha", 80, ServerType.Http),
            new ServerItem("203.66.45.201", 21, ServerType.Ftp),
            new ServerItem("www.polaris-gnss.com/alpha", 80, ServerType.Https),
            new ServerItem("agps.skytraq.com.tw", 21, ServerType.Ftp),
            new ServerItem("60.250.205.31", 21, ServerType.Ftp),
        };

        static int serverIndex = serverList.Length;
        public static bool IsServerAlive()
        {
            serverIndex = 0;
            foreach (ServerItem si in serverList)
            {
                if(MiscNetworkTool.IsServerAlive(si.url, si.port))
                {
                    break;
                }
                ++serverIndex;
            }
            return serverIndex < serverList.Length;
        }

        //..FtpExtensions.GetFtpFileString(AppTools.ViewerFtpFolderName, Program.swUpdateProfile);
        public static string GetFileString(string folder, string name)
        {
            if (serverIndex >= serverList.Length)
            {
                return "";
            }

            string ret = "";
            do
            {
                try
                {
                    if (serverList[serverIndex].type == ServerType.Ftp)
                    {
                        FtpExtensions.serverIp = serverList[serverIndex].url;
                        ret = FtpExtensions.GetFtpFileString(folder, name);
                    }

                    if (serverList[serverIndex].type == ServerType.Https)
                    {
                        ret = UrlToString(string.Format("https://{0}/{1}/{2}", 
                            serverList[serverIndex].url, folder, name), true);
                    }
                    if (serverList[serverIndex].type == ServerType.Http)
                    {
                        ret = UrlToString(string.Format("http://{0}/{1}/{2}", 
                            serverList[serverIndex].url, folder, name), false);
                    }

                    if (ret != "")
                    {
                        return ret;
                    }
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                }
                ++serverIndex;
            } while (serverIndex < serverList.Length);
            IsServerAlive();
            return "";
        }

        static int timeout = 2000;
        public static void SetTimeout(int t) { timeout = t; }
        public class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest WR = base.GetWebRequest(uri);
                WR.Timeout = timeout;
                return WR;
            }
        }

        static string UrlToString(String url, bool isSecurity)
        {
            AppTools.ShowDebug(string.Format("UrlToString({0},{1})", url, isSecurity));
            MyWebClient webclient = new MyWebClient();
            webclient.Headers.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)");
            webclient.Encoding = Encoding.UTF8;
            if (isSecurity)
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;  // SecurityProtocolType.Tls1.2;
            }
            AppTools.ShowDebug(ServicePointManager.SecurityProtocol.ToString());

            try
            {
                return webclient.DownloadString(url);
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug("UrlToString exception: " + ex.Message);
            }
            return "";
        }

        private static void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            if(e.BytesReceived == e.TotalBytesToReceive)
            {
                ma.Set();
            }
            if(myProgress != null)
            {
                myProgress((int)e.BytesReceived);
            }
        }

        static ManualResetEvent ma = new ManualResetEvent(false);
        static bool UrlToFile(string url, string localPath, bool isSecurity)
        {
            AppTools.ShowDebug(string.Format("UrlToFile({0},{1})", url, isSecurity));
            WebClient webclient = new WebClient();
            webclient.Headers.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)");
            webclient.Encoding = Encoding.UTF8;
            if (isSecurity)
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;  // SecurityProtocolType.Tls1.2;
            }
            AppTools.ShowDebug(ServicePointManager.SecurityProtocol.ToString());

            try
            {
                Uri uri = new Uri(url);
                webclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                ma.Reset();
                webclient.DownloadFileAsync(uri, localPath);
                ma.WaitOne();
                return true;
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
            }
            return false;
        }

        //public static bool GetFtpFile(string folder, string localPath, string remotePath, FtpProgressFunction p)
        static FtpProgressFunction myProgress = null;
        public static bool GetFile(string folder, string localPath, string remotePath, FtpProgressFunction p)
        {
            if (serverIndex >= serverList.Length)
            {
                return false;
            }

            bool ret = false;
            do
            {
                try
                {
                    if (serverList[serverIndex].type == ServerType.Ftp)
                    {
                        ret = FtpExtensions.GetFtpFile(folder, localPath, remotePath, p);
                    }

                    if (serverList[serverIndex].type == ServerType.Https)
                    {
                        myProgress = p;
                        ret = UrlToFile(string.Format("https://{0}/{1}/{2}", 
                            serverList[serverIndex].url, folder, remotePath), localPath, true);
                    }

                    if (serverList[serverIndex].type == ServerType.Http)
                    {
                        myProgress = p;
                        ret = UrlToFile(string.Format("http://{0}/{1}/{2}",
                            serverList[serverIndex].url, folder, remotePath), localPath, false);
                    }

                    if (ret)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug("GetFile exception: " + ex.Message);
                }
                ++serverIndex;
            } while (serverIndex < serverList.Length);
            IsServerAlive();
            return false;
        }


























        
        class FtpExtensions
        {
            public static string serverIp = "agps.skytraq.com.tw";
            private static string username = "alphauser";
            private static string password = "st28043557!!!";
            private static string dirName = "";
            private static string fromFileName = "";
            private static string toFileName = "";
            private static string fileOrDir = "";
            private static string fileName = "";

            public static string GetFtpFileString(string folder, string name)
            {
                bool downloadOk = false;
                StringBuilder sb = new StringBuilder();
                try
                {
                    dirName = string.Format(@"/{0}/", folder); // Path
                    if (FtpExtensions.FtpQuery() == null)
                    {
                        return "";
                    }

                    Array ftpFileList = FtpExtensions.FtpQuery();
                    foreach (string s in ftpFileList)
                    {
                        if (s == name)
                        {
                            fromFileName = s;    //File name to download
                            toFileName = s;  //remote path
                            downloadOk = FTPDownloadFile(sb);    //Do download
                            break;
                        }
                    }
                    return ((downloadOk) ? sb.ToString() : "");
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                }
                return "";
            }

            public static bool GetFtpFile(string folder, string localPath, string remotePath, FtpProgressFunction p)
            {
                bool downloadOk = false;
                string filename = Path.GetFileName(remotePath);
                string path = Path.GetDirectoryName(remotePath);
                StringBuilder sb = new StringBuilder();
                try
                {
                    dirName = string.Format("/{0}/{1}/", folder, path);               // Path
                    if (FtpQuery() == null)
                    {
                        return downloadOk;
                    }

                    Array ftpFileList = FtpExtensions.FtpQuery();
                    foreach (string s in ftpFileList)
                    {
                        if (s.ToLower() == filename.ToLower())
                        {
                            fromFileName = s;    //File name to download
                            toFileName = s;  //remote path
                            downloadOk = FtpExtensions.FTPDownloadFile(localPath, p);    //Do download
                            break;
                        }
                    }
                    return downloadOk;
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                }
                return false;
            }

            private static int _iFTPReTry;
            public static int iFTPReTry { get { return _iFTPReTry; } set { _iFTPReTry = value; } }


            public static Array FtpQuery()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string sURI = "FTP://" + serverIp + "/" + dirName;
                    FtpWebRequest myFTP = (FtpWebRequest)FtpWebRequest.Create(sURI); //建立FTP連線
                                                                                     //設定連線模式及相關參數
                    myFTP.Credentials = new System.Net.NetworkCredential(username, password); //帳密驗證
                    myFTP.Timeout = 2000; //等待時間
                    myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                    myFTP.Method = WebRequestMethods.Ftp.ListDirectory; //取得檔案清單

                    StreamReader myReadStream = new StreamReader(myFTP.GetResponse().GetResponseStream(), Encoding.Default); //取得FTP請求回應

                    //檔案清單
                    string sFTPFile; StringBuilder sbResult = new StringBuilder(); //,string[] sDownloadFiles;
                    while (!(myReadStream.EndOfStream))
                    {
                        sFTPFile = myReadStream.ReadLine();
                        sbResult.Append(sFTPFile + "\n");
                    }
                    myReadStream.Close();
                    myReadStream.Dispose();
                    sFTPFile = null;
                    sbResult.Remove(sbResult.ToString().LastIndexOf("\n"), 1); //檔案清單查詢結果
                    return sbResult.ToString().Split('\n'); //回傳至字串陣列
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());

                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FtpQuery();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public static List<string> FTPDetailQuery()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string sURI = "FTP://" + serverIp + "/" + dirName;
                    System.Net.FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(sURI); //建立FTP連線
                                                                                                                      //設定連線模式及相關參數
                    myFTP.Credentials = new System.Net.NetworkCredential(username, password); //帳密驗證
                    myFTP.Timeout = 2000; //等待時間
                    myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                    myFTP.Method = System.Net.WebRequestMethods.Ftp.ListDirectoryDetails; //取得詳細檔案清單

                    StreamReader myReadStream = new StreamReader(myFTP.GetResponse().GetResponseStream(), Encoding.Default); //取得FTP請求回應
                                                                                                                             //目錄清單
                    string sFileQuery;
                    string[] sFileList;
                    StringBuilder sbResult = new StringBuilder();
                    List<string> lFileResult = new List<string>();
                    while (!(myReadStream.EndOfStream))
                    {
                        sFileQuery = myReadStream.ReadLine();
                        sbResult.Append(sFileQuery + "\n");
                    }
                    myReadStream.Close();
                    myReadStream.Dispose();
                    sFileQuery = null;
                    sbResult.Remove(sbResult.ToString().LastIndexOf("\n"), 1); //檔案清單查詢結果
                    sFileList = sbResult.ToString().Split('\n'); //檔案清單轉換為字串陣列
                                                                 //判斷是否為資料夾
                    if (fileOrDir.ToLower() == "file")
                    {
                        fileOrDir = "-rw-r--r--";
                        //fileOrDir = "-r--r--r--";
                    }
                    else
                    {
                        fileOrDir = "drwxr-xr-x";
                    }
                    //解析資料夾
                    foreach (string myFileInfo in sFileList)
                    {
                        if (myFileInfo.IndexOf(fileOrDir) >= 0)
                        {
                            string[] sInfoStr = myFileInfo.Split(' ');
                            string sDirStr = null;
                            int iFileStr = 0;
                            //解析字元陣列
                            for (int myFileChar = 0; myFileChar < sInfoStr.Length; myFileChar++)
                            {
                                //字元陣列非空項則設為字串
                                if (sInfoStr[myFileChar] != null)
                                    iFileStr++;

                                //字串陣列第9個為FTP資料夾名稱
                                if (iFileStr > 8)
                                    sDirStr = sInfoStr[myFileChar] + " ";

                            }
                            sDirStr = sDirStr.Trim(); //去除字元空格
                            if (sDirStr != "." && sDirStr != "..")
                            {
                                lFileResult.Add(sDirStr);
                            }
                        }
                    }
                    return lFileResult;
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FTPDetailQuery();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public static DateTime FTPGetFileDate()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string sURI = "FTP://" + serverIp + "/" + dirName + "/" + fileName;

                    FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(sURI); //建立FTP連線
                                                                                                           //設定連線模式及相關參數
                    myFTP.Credentials = new System.Net.NetworkCredential(username, password); //帳密驗證
                    myFTP.Timeout = 2000; //等待時間
                    myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                    myFTP.Method = WebRequestMethods.Ftp.GetDateTimestamp; //取得資料修改日期

                    System.Net.FtpWebResponse myFTPFileDate = (System.Net.FtpWebResponse)myFTP.GetResponse(); //取得FTP請求回應
                    return myFTPFileDate.LastModified;
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(string.Format("FTP Dictionar Query Fail\n{0}", ex.Message));
                    //MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FTPGetFileDate();
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
            }

            public static int FTPGetFileSize()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string sURI = "FTP://" + serverIp + "/" + dirName + "/" + fileName;
                    FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(sURI); //建立FTP連線
                                                                                                           //設定連線模式及相關參數
                    myFTP.Credentials = new System.Net.NetworkCredential(username, password); //帳密驗證
                    myFTP.Timeout = 2000; //等待時間
                    myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                    myFTP.Method = System.Net.WebRequestMethods.Ftp.GetFileSize; //取得資料容量大小

                    System.Net.FtpWebResponse myFTPFileSize = (System.Net.FtpWebResponse)myFTP.GetResponse(); //取得FTP請求回應
                    return (int)myFTPFileSize.ContentLength;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("FTP File Size Query Fail" + "\n" + "{0}", ex.Message)
                    //MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                    AppTools.ShowDebug(ex.ToString());
                    AppTools.ShowDebug(ex.ToString());
                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FTPGetFileSize();
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            public static bool FTPUploadFile()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string sURI = "FTP://" + serverIp + "/" + dirName + "/" + toFileName;
                    System.Net.FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(sURI); //建立FTP連線
                                                                                                                      //設定連線模式及相關參數
                    myFTP.Credentials = new System.Net.NetworkCredential(username, password); //帳密驗證
                    myFTP.KeepAlive = false; //關閉/保持 連線
                    myFTP.Timeout = 2000; //等待時間
                    myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                    myFTP.UsePassive = false; //通訊埠接聽並等待連接
                    myFTP.Method = System.Net.WebRequestMethods.Ftp.UploadFile; //下傳檔案
                                                                                /* proxy setting (不使用proxy) */
                    myFTP.Proxy = null;
                    myFTP.Proxy = null;

                    //上傳檔案
                    System.IO.FileStream myReadStream = new System.IO.FileStream(fromFileName, FileMode.Open, FileAccess.Read); //檔案設為讀取模式
                    System.IO.Stream myWriteStream = myFTP.GetRequestStream(); //資料串流設為上傳至FTP
                    byte[] bBuffer = new byte[2047]; int iRead = 0; //傳輸位元初始化
                    do
                    {
                        iRead = myReadStream.Read(bBuffer, 0, bBuffer.Length); //讀取上傳檔案
                        myWriteStream.Write(bBuffer, 0, iRead); //傳送資料串流
                    } while (!(iRead == 0));

                    myReadStream.Flush();
                    myReadStream.Close();
                    myReadStream.Dispose();
                    myWriteStream.Flush();
                    myWriteStream.Close();
                    myWriteStream.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FTPUploadFile();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public static bool FTPDownloadFile(string path, FtpProgressFunction p)
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string uri = "FTP://" + serverIp + "/" + dirName + "/" + fromFileName;
                    FtpWebRequest myFTP = (FtpWebRequest)FtpWebRequest.Create(uri); //Create FTP connection
                                                                                    //設定連線模式及相關參數
                    myFTP.Credentials = new NetworkCredential(username, password); //credential
                    myFTP.Timeout = 2000;
                    myFTP.UseBinary = true;
                    myFTP.UsePassive = false;
                    myFTP.Method = WebRequestMethods.Ftp.DownloadFile;

                    FtpWebResponse ftpResponse = (FtpWebResponse)myFTP.GetResponse(); //取得FTP回應
                    FileStream myWriteStream = new FileStream(path, FileMode.Create, FileAccess.Write); //檔案設為寫入模式
                    Stream myReadStream = ftpResponse.GetResponseStream(); //資料串流設為接收FTP回應下載
                    byte[] buffer = new byte[2048];
                    int iRead = 0, total = 0;
                    do
                    {
                        iRead = myReadStream.Read(buffer, 0, buffer.Length); //接收資料串流
                        if (iRead > 0)
                        {
                            myWriteStream.Write(buffer, 0, iRead); //寫入下載檔案
                            total += iRead;
                            if (p != null)
                            {
                                p(total);
                            }
                        }
                    } while (iRead > 0);

                    myReadStream.Flush();
                    myReadStream.Close();
                    myReadStream.Dispose();
                    myWriteStream.Flush();
                    myWriteStream.Close();
                    myWriteStream.Dispose();
                    ftpResponse.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FTPDownloadFile(path, p);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public static bool FTPDownloadFile(StringBuilder sb)
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string uri = "FTP://" + serverIp + "/" + dirName + "/" + fromFileName;
                    FtpWebRequest myFTP = (FtpWebRequest)FtpWebRequest.Create(uri); //Create FTP connection
                                                                                    //設定連線模式及相關參數
                    myFTP.Credentials = new NetworkCredential(username, password); //credential
                    myFTP.Timeout = 2000;
                    myFTP.UseBinary = true;
                    myFTP.UsePassive = false;
                    myFTP.Method = WebRequestMethods.Ftp.DownloadFile;

                    FtpWebResponse ftpResponse = (FtpWebResponse)myFTP.GetResponse(); //取得FTP回應
                                                                                      //下載檔案
                                                                                      //FileStream myWriteStream = new FileStream(sToFileName, FileMode.Create, FileAccess.Write); //檔案設為寫入模式
                    Stream myReadStream = ftpResponse.GetResponseStream(); //資料串流設為接收FTP回應下載
                    byte[] buffer = new byte[2048];
                    int iRead = 0;
                    do
                    {
                        iRead = myReadStream.Read(buffer, 0, buffer.Length); //接收資料串流
                        if (iRead > 0)
                        {
                            sb.Append(Encoding.ASCII.GetString(buffer, 0, iRead));
                        }

                    } while (!(iRead == 0));

                    myReadStream.Flush();
                    myReadStream.Close();
                    myReadStream.Dispose();
                    ftpResponse.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FTPDownloadFile(sb);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public static Boolean FTPCreateDir()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string sURI = "FTP://" + serverIp + "/" + dirName;
                    System.Net.FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(sURI); //建立FTP連線
                                                                                                                      //設定連線模式及相關參數
                    myFTP.Credentials = new System.Net.NetworkCredential(username, password); //帳密驗證
                    myFTP.KeepAlive = false; //關閉/保持 連線
                    myFTP.Timeout = 2000; //等待時間
                    myFTP.UseBinary = true; //傳輸資料型別 二進位/文字
                    myFTP.Method = System.Net.WebRequestMethods.Ftp.MakeDirectory; //建立目錄模式

                    System.Net.FtpWebResponse myFtpResponse = (System.Net.FtpWebResponse)myFTP.GetResponse(); //創建目錄
                    myFtpResponse.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FTPCreateDir();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public static Boolean FTPDeleteFile()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string sURI = "FTP://" + serverIp + "/" + dirName + fileName;
                    System.Net.FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(sURI); //建立FTP連線
                                                                                                                      //設定連線模式及相關參數
                    myFTP.Credentials = new System.Net.NetworkCredential(username, password); //帳密驗證
                    myFTP.KeepAlive = false; //關閉/保持 連線
                    myFTP.Timeout = 2000; //等待時間
                    myFTP.Method = System.Net.WebRequestMethods.Ftp.DeleteFile; //刪除檔案

                    System.Net.FtpWebResponse myFtpResponse = (System.Net.FtpWebResponse)myFTP.GetResponse(); //刪除檔案/資料夾
                    myFtpResponse.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("FTP File Seach Fail" + "\n" + "{0}", ex.Message);
                    //MessageBox.Show(ex.Message , null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                    AppTools.ShowDebug(ex.ToString());
                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FTPDeleteFile();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public static Boolean FTPRemoveDirectory()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    string sURI = "FTP://" + serverIp + "/" + dirName;
                    System.Net.FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(sURI); //建立FTP連線
                                                                                                                      //設定連線模式及相關參數
                    myFTP.Credentials = new System.Net.NetworkCredential(username, password); //帳密驗證
                    myFTP.KeepAlive = false; //關閉/保持 連線
                    myFTP.Timeout = 2000; //等待時間
                    myFTP.Method = System.Net.WebRequestMethods.Ftp.RemoveDirectory; //移除資料夾

                    System.Net.FtpWebResponse myFtpResponse = (System.Net.FtpWebResponse)myFTP.GetResponse(); //刪除檔案/資料夾
                    myFtpResponse.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(string.Format("FTP File Seach Fail\n{0}", ex.Message));
                    MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                    iFTPReTry--;
                    if (iFTPReTry >= 0)
                    {
                        return FTPRemoveDirectory();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public static void cmdFTPDownloadFile()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    System.IO.FileStream myFTPCommand = new System.IO.FileStream("D:\\FTPCommand.txt", FileMode.Create, FileAccess.ReadWrite);
                    StreamWriter myCommand = new StreamWriter(myFTPCommand);
                    myCommand.BaseStream.Seek(0, SeekOrigin.Begin);
                    myCommand.WriteLine("open" + " " + serverIp + " ");
                    myCommand.WriteLine(username);
                    myCommand.WriteLine(password);
                    myCommand.WriteLine("get" + " " + dirName + "\"" + fromFileName + "\"" + " " + "\"" + toFileName + "\"");
                    myCommand.WriteLine("bye");
                    myCommand.Flush();
                    myCommand.Close();
                    myCommand.Dispose();
                    Process.Start(System.Environment.GetEnvironmentVariable("SystemRoot") + "\\system32\\ftp.exe", "-s:\"D:\\FTPCommand.txt\"").WaitForExit();
                    File.Delete("D:\\FTPCommand.txt");
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                }
            }

            public static List<string> cmdFTPQuery()
            {
                try
                {
                    dirName = dirName.Replace("\\", "/");
                    System.IO.FileStream myFTPCommand = new System.IO.FileStream("D:\\FTPCommand.txt", FileMode.Create, FileAccess.ReadWrite);
                    StreamWriter myCommand = new StreamWriter(myFTPCommand);
                    myCommand.BaseStream.Seek(0, SeekOrigin.Begin);
                    myCommand.WriteLine("open" + " " + serverIp + "\t");
                    myCommand.WriteLine(username);
                    myCommand.WriteLine(password);
                    myCommand.WriteLine("cd" + " " + "\"" + dirName + "\"");
                    myCommand.WriteLine("ls" + " " + "*" + fileName + "*");
                    myCommand.WriteLine("bye");
                    myCommand.Flush();
                    myCommand.Close();
                    myCommand.Dispose();
                    //建立cmd執行緒
                    Process myProcess = new Process();
                    myProcess.StartInfo.FileName = System.Environment.GetEnvironmentVariable("SystemRoot") + "\\system32\\cmd.exe";
                    //myProcess.StartInfo.Arguments = "/c " + Command(); //設定程式執行參數
                    myProcess.StartInfo.UseShellExecute = false; //關閉Shell的使用
                    myProcess.StartInfo.RedirectStandardInput = true; //重新導向標準輸入
                    myProcess.StartInfo.RedirectStandardOutput = true; //重新導向標準輸出
                    myProcess.StartInfo.RedirectStandardError = true; //重新導向錯誤輸出
                    myProcess.StartInfo.CreateNoWindow = true; //設定不顯示視窗
                    myProcess.Start();
                    myProcess.StandardInput.WriteLine("ftp -s:\"D:\\FTPCommand.txt\"");
                    myProcess.StandardInput.WriteLine("exit");
                    //解析檔案清單
                    string sFileQuery;
                    string[] sFileList;
                    List<string> lFileResult = new List<string>();
                    //sFileQuery = UrlEncode(myProcess.StandardOutput.ReadToEnd()); //從輸出流取得命令執行結果，解決中文亂碼的問題
                    //Application.DoEvents();
                    sFileQuery = myProcess.StandardOutput.ReadToEnd();
                    sFileQuery = sFileQuery.Replace("\n", null);
                    sFileList = sFileQuery.Split('\n');
                    foreach (string myFile in sFileList)
                    {
                        if (myFile.IndexOf(fileName) >= 0 && myFile.IndexOf(dirName) == 0)
                        {
                            string myStr = null;
                            if (myFile.IndexOf("版本") >= 0 && myFile.IndexOf(serverIp) > 0)
                            {
                                char[] myChar = myFile.ToCharArray();
                                Array.Reverse(myChar);
                                myStr = new string(myChar);
                                myStr = myFile.Substring(myFile.Length - myStr.IndexOf("\t"), myStr.IndexOf("\t"));
                            }
                            lFileResult.Add(myStr);
                        }
                    }
                    File.Delete("D:\\FTPCommand.txt");
                    return lFileResult;
                }
                catch (Exception ex)
                {
                    AppTools.ShowDebug(ex.ToString());
                    return null;
                }
            }

            public static String UrlEncode(String Str)
            {
                if (Str == null)
                    return null;

                return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Str));
            }
        }

    }

    public static class MiscNetworkTool
    {
        public static bool IsServerAlive(string url, int port)
        {
            try
            {
                TcpClient client = new TcpClient();
                var result = client.BeginConnect(url, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

                if (!success)
                {
                    return false;
                }
                client.Close();
                return true;
            }
            catch (Exception ex)
            {
                AppTools.ShowDebug(ex.ToString());
                return false;
            }
        }
    }
}

namespace MiscUtil.Skytraq
{
    static class SkytraqTools
    {
        public static byte CalcPromRawCheckSum(byte[] prom)
        {
            byte c = 0;
            foreach (byte b in prom)
            {
                c += b;
            }
            return c;
        }

        public static byte CalcCheckSum16(byte[] data, int start, int len)
        {
            UInt16 checkSum = 0;
            for (int i = 0; i < len; i += sizeof(UInt16))
            {
                UInt16 word = Convert.ToUInt16(data[start + i + 1] | data[start + i] << 8);
                checkSum += word;
            }
            return Convert.ToByte(((checkSum >> 8) + (checkSum & 0xFF)) & 0xFF);
        }
    }
}

namespace MiscUtil.IO
{
    public static class FileTools
    {
        public static void ExistDelete(string path)
        {
            if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
        }
    } 
}