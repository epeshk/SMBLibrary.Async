/* Copyright (C) 2014-2017 Tal Aloni <tal.aloni.il@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */

using System;
using MemoryPools.Memory;
using Utilities;

namespace SMBLibrary.SMB1
{
    public class SMB1Header : IDisposable
    {
        public const int Length = 32;
        public static readonly byte[] ProtocolSignature = { 0xFF, 0x53, 0x4D, 0x42 };

        private byte[] Protocol; // byte[4], 0xFF followed by "SMB"
        public CommandName Command;
        public NTStatus Status;
        public HeaderFlags Flags;
        public HeaderFlags2 Flags2;
        //ushort PIDHigh
        public ulong SecurityFeatures;
        // public ushort Reserved;
        public ushort TID; // Tree ID
        //ushort PIDLow;
        public ushort UID; // User ID
        public ushort MID; // Multiplex ID

        public uint PID; // Process ID

        public SMB1Header Init()
        {
            Command = default;
            Status = default;
            Flags = default;
            Flags2 = default;
            SecurityFeatures = default;
            TID = default; 
            UID = default; 
            MID = default; 
            PID = default;
            Protocol = ProtocolSignature;
            return this;
        }

        public SMB1Header Init(Span<byte> buffer)
        {
            Protocol = ByteReader.ReadBytes_RentArray(buffer, 0, 4);
            Command = (CommandName)ByteReader.ReadByte(buffer, 4);
            Status = (NTStatus)LittleEndianConverter.ToUInt32(buffer, 5);
            Flags = (HeaderFlags)ByteReader.ReadByte(buffer, 9);
            Flags2 = (HeaderFlags2)LittleEndianConverter.ToUInt16(buffer, 10);
            var PIDHigh = LittleEndianConverter.ToUInt16(buffer, 12);
            SecurityFeatures = LittleEndianConverter.ToUInt64(buffer, 14);
            TID = LittleEndianConverter.ToUInt16(buffer, 24);
            var PIDLow = LittleEndianConverter.ToUInt16(buffer, 26);
            UID = LittleEndianConverter.ToUInt16(buffer, 28);
            MID = LittleEndianConverter.ToUInt16(buffer, 30);

            PID = (uint)((PIDHigh << 16) | PIDLow);
            return this;
        }

        public void WriteBytes(Span<byte> buffer, int offset)
        {
            var PIDHigh = (ushort)(PID >> 16);
            var PIDLow = (ushort)(PID & 0xFFFF);

            BufferWriter.WriteBytes(buffer, offset + 0, Protocol);
            BufferWriter.WriteByte(buffer, offset + 4, (byte)Command);
            LittleEndianWriter.WriteUInt32(buffer, offset + 5, (uint)Status);
            BufferWriter.WriteByte(buffer, offset + 9, (byte)Flags);
            LittleEndianWriter.WriteUInt16(buffer, offset + 10, (ushort)Flags2);
            LittleEndianWriter.WriteUInt16(buffer, offset + 12, PIDHigh);
            LittleEndianWriter.WriteUInt64(buffer, offset + 14, SecurityFeatures);
            LittleEndianWriter.WriteUInt16(buffer, offset + 24, TID);
            LittleEndianWriter.WriteUInt16(buffer, offset + 26, PIDLow);
            LittleEndianWriter.WriteUInt16(buffer, offset + 28, UID);
            LittleEndianWriter.WriteUInt16(buffer, offset + 30, MID);
        }

        public byte[] GetBytes()
        {
            var buffer = new byte[Length];
            WriteBytes(buffer, 0);
            return buffer;
        }

        public bool ReplyFlag => (Flags & HeaderFlags.Reply) > 0;

        /// <summary>
        /// SMB_FLAGS2_EXTENDED_SECURITY
        /// </summary>
        public bool ExtendedSecurityFlag
        {
            get => (Flags2 & HeaderFlags2.ExtendedSecurity) > 0;
            set
            {
                if (value)
                {
                    Flags2 |= HeaderFlags2.ExtendedSecurity;
                }
                else
                {
                    Flags2 &= ~HeaderFlags2.ExtendedSecurity;
                }
            }
        }

        public bool UnicodeFlag
        {
            get => (Flags2 & HeaderFlags2.Unicode) > 0;
            set
            {
                if (value)
                {
                    Flags2 |= HeaderFlags2.Unicode;
                }
                else
                {
                    Flags2 &= ~HeaderFlags2.Unicode;
                }
            }
        }

        public static bool IsValidSMB1Header(Span<byte> buffer)
        {
            if (buffer.Length >= 4)
            {
                var protocol = ByteReader.ReadBytes_RentArray(buffer, 0, 4);
                return ByteUtils.AreByteArraysEqual(protocol, ProtocolSignature);
            }
            return false;
        }

        public void Dispose()
        {
            ObjectsPool<SMB1Header>.Return(this);
        }
    }
}