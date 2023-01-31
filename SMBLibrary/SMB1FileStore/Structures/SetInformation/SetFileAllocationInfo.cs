/* Copyright (C) 2014-2017 Tal Aloni <tal.aloni.il@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */

using System;
using System.Buffers;
using MemoryPools.Memory;
using Utilities;

namespace SMBLibrary.SMB1
{
    /// <summary>
    /// SMB_SET_FILE_ALLOCATION_INFO
    /// </summary>
    public class SetFileAllocationInfo : SetInformation
    {
        public const int Length = 8;

        public long AllocationSize;

        public SetFileAllocationInfo()
        {
        }

        public SetFileAllocationInfo(Span<byte> buffer) : this(buffer, 0)
        {
        }

        public SetFileAllocationInfo(Span<byte> buffer, int offset)
        {
            AllocationSize = LittleEndianConverter.ToInt64(buffer, offset);
        }

        public override IMemoryOwner<byte> GetBytes()
        {
            var buffer = Arrays.Rent(Length);
            LittleEndianWriter.WriteInt64(buffer.Memory.Span, 0, AllocationSize);
            return buffer;
        }

        public override SetInformationLevel InformationLevel => SetInformationLevel.SMB_SET_FILE_ALLOCATION_INFO;
    }
}