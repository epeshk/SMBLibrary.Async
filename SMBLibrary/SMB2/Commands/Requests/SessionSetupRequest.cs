/* Copyright (C) 2017 Tal Aloni <tal.aloni.il@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using Utilities;

namespace SMBLibrary.SMB2
{
    /// <summary>
    /// SMB2 SESSION_SETUP Request
    /// </summary>
    public class SessionSetupRequest : SMB2Request
    {
        public const int FixedSize = 24;
        public const int DeclaredSize = 25;

        private ushort StructureSize;
        public SessionSetupFlags Flags;
        public SecurityMode SecurityMode;
        public Capabilities Capabilities; // Values other than SMB2_GLOBAL_CAP_DFS should be treated as reserved.
        public uint Channel;
        private ushort SecurityBufferOffset;
        private ushort SecurityBufferLength;
        public ulong PreviousSessionId;
        public byte[] SecurityBuffer = new byte[0];

        public SessionSetupRequest() : base(SMB2CommandName.SessionSetup)
        {
            StructureSize = DeclaredSize;
        }

        public override void WriteCommandBytes(byte[] buffer, int offset)
        {
            SecurityBufferOffset = 0;
            SecurityBufferLength = (ushort)SecurityBuffer.Length;
            if (SecurityBuffer.Length > 0)
            {
                SecurityBufferOffset = SMB2Header.Length + FixedSize;
            }
            LittleEndianWriter.WriteUInt16(buffer, offset + 0, StructureSize);
            ByteWriter.WriteByte(buffer, offset + 2, (byte)Flags);
            ByteWriter.WriteByte(buffer, offset + 3, (byte)SecurityMode);
            LittleEndianWriter.WriteUInt32(buffer, offset + 4, (uint)Capabilities);
            LittleEndianWriter.WriteUInt32(buffer, offset + 8, Channel);
            LittleEndianWriter.WriteUInt16(buffer, offset + 12, SecurityBufferOffset);
            LittleEndianWriter.WriteUInt16(buffer, offset + 14, SecurityBufferLength);
            LittleEndianWriter.WriteUInt64(buffer, offset + 16, PreviousSessionId);
            ByteWriter.WriteBytes(buffer, offset + FixedSize, SecurityBuffer);
        }

        public override int CommandLength
        {
            get
            {
                return FixedSize + SecurityBuffer.Length;
            }
        }
    }
}
