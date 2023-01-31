using System;
using System.IO;

namespace Utilities
{
    public class BigEndianReader
    {
        public static short ReadInt16(Span<byte> buffer, ref int offset)
        {
            offset += 2;
            return BigEndianConverter.ToInt16(buffer, offset - 2);
        }

        public static ushort ReadUInt16(Span<byte> buffer, ref int offset)
        {
            offset += 2;
            return BigEndianConverter.ToUInt16(buffer, offset - 2);
        }

        public static uint ReadUInt24(Span<byte> buffer, int offset)
        {
            return (uint)((buffer[offset + 0] << 16) | (buffer[offset + 1] << 8) | (buffer[offset + 2] << 0));
        }

        public static uint ReadUInt24(Span<byte> buffer, ref int offset)
        {
            offset += 3;
            return ReadUInt24(buffer, offset - 3);
        }

        public static int ReadInt32(Span<byte> buffer, ref int offset)
        {
            offset += 4;
            return BigEndianConverter.ToInt32(buffer, offset - 4);
        }

        public static uint ReadUInt32(Span<byte> buffer, ref int offset)
        {
            offset += 4;
            return BigEndianConverter.ToUInt32(buffer, offset - 4);
        }

        public static long ReadInt64(Span<byte> buffer, ref int offset)
        {
            offset += 8;
            return BigEndianConverter.ToInt64(buffer, offset - 8);
        }

        public static ulong ReadUInt64(Span<byte> buffer, ref int offset)
        {
            offset += 8;
            return BigEndianConverter.ToUInt64(buffer, offset - 8);
        }

        public static Guid ReadGuidBytes(Span<byte> buffer, ref int offset)
        {
            offset += 16;
            return BigEndianConverter.ToGuid(buffer, offset - 16);
        }

        public static short ReadInt16(Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return BigEndianConverter.ToInt16(buffer, 0);
        }

        public static ushort ReadUInt16(Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return BigEndianConverter.ToUInt16(buffer, 0);
        }

        public static uint ReadUInt24(Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 1, 3);
            return BigEndianConverter.ToUInt32(buffer, 0);
        }

        public static int ReadInt32(Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BigEndianConverter.ToInt32(buffer, 0);
        }

        public static uint ReadUInt32(Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BigEndianConverter.ToUInt32(buffer, 0);
        }

        public static long ReadInt64(Stream stream)
        {
            var buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            return BigEndianConverter.ToInt64(buffer, 0);
        }

        public static ulong ReadUInt64(Stream stream)
        {
            var buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            return BigEndianConverter.ToUInt64(buffer, 0);
        }

        public static Guid ReadGuidBytes(Stream stream)
        {
            var buffer = new byte[16];
            stream.Read(buffer, 0, 16);
            return BigEndianConverter.ToGuid(buffer, 0);
        }
    }
}