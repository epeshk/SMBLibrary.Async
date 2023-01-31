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
    /// SMB_QUERY_FS_DEVICE_INFO
    /// </summary>
    public class QueryFSDeviceInfo : QueryFSInformation
    {
        public const int FixedLength = 8;

        public DeviceType DeviceType;
        public DeviceCharacteristics DeviceCharacteristics;

        public QueryFSDeviceInfo()
        {
        }

        public QueryFSDeviceInfo(Span<byte> buffer, int offset)
        {
            DeviceType = (DeviceType)LittleEndianConverter.ToUInt32(buffer, offset + 0);
            DeviceCharacteristics = (DeviceCharacteristics)LittleEndianConverter.ToUInt32(buffer, offset + 4);
        }

        public override IMemoryOwner<byte> GetBytes(bool isUnicode)
        {
            var buffer = Arrays.Rent(Length);
            LittleEndianWriter.WriteUInt32(buffer.Memory.Span, 0, (uint)DeviceType);
            LittleEndianWriter.WriteUInt32(buffer.Memory.Span, 4, (uint)DeviceCharacteristics);
            return buffer;
        }

        public override int Length => FixedLength;

        public override QueryFSInformationLevel InformationLevel => QueryFSInformationLevel.SMB_QUERY_FS_DEVICE_INFO;
    }
}