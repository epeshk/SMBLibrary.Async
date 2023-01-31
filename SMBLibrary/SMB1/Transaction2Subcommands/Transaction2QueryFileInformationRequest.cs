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
    /// TRANS2_QUERY_FILE_INFORMATION Request
    /// </summary>
    public class Transaction2QueryFileInformationRequest : Transaction2Subcommand
    {
        private const ushort SMB_INFO_PASSTHROUGH = 0x03E8;
        public const int ParametersLength = 4;
        // Parameters:
        public ushort FID;
        public ushort InformationLevel;
        // Data:
        public FullExtendedAttributeList GetExtendedAttributeList; // Used with QueryInformationLevel.SMB_INFO_QUERY_EAS_FROM_LIST

        public Transaction2QueryFileInformationRequest()
        {
            GetExtendedAttributeList = new FullExtendedAttributeList();
        }

        public Transaction2QueryFileInformationRequest(IMemoryOwner<byte> parameters, IMemoryOwner<byte> data, bool isUnicode)
        {
            FID = LittleEndianConverter.ToUInt16(parameters, 0);
            InformationLevel = LittleEndianConverter.ToUInt16(parameters, 2);

            if (!IsPassthroughInformationLevel && QueryInformationLevel == QueryInformationLevel.SMB_INFO_QUERY_EAS_FROM_LIST)
            {
                GetExtendedAttributeList = new FullExtendedAttributeList(data.Memory.Span, 0);
            }
        }

        public override void GetSetupInto(Span<byte> target)
        {
            LittleEndianConverter.GetBytes(target, (ushort)SubcommandName);
        }

        public override IMemoryOwner<byte> GetParameters(bool isUnicode)
        {
            var parameters = Arrays.Rent(ParametersLength);
            LittleEndianWriter.WriteUInt16(parameters.Memory.Span, 0, FID);
            LittleEndianWriter.WriteUInt16(parameters.Memory.Span, 2, InformationLevel);
            return parameters;
        }

        public override IMemoryOwner<byte> GetData(bool isUnicode)
        {
            if (!IsPassthroughInformationLevel && QueryInformationLevel == QueryInformationLevel.SMB_INFO_QUERY_EAS_FROM_LIST)
            {
                return GetExtendedAttributeList.GetBytes();
            }

            return MemoryOwner<byte>.Empty;
        }

        public bool IsPassthroughInformationLevel => (InformationLevel >= SMB_INFO_PASSTHROUGH);

        public QueryInformationLevel QueryInformationLevel
        {
            get => (QueryInformationLevel)InformationLevel;
            set => InformationLevel = (ushort)value;
        }

        public FileInformationClass FileInformationClass
        {
            get => (FileInformationClass)(InformationLevel - SMB_INFO_PASSTHROUGH);
            set => InformationLevel = (ushort)((ushort)value + SMB_INFO_PASSTHROUGH);
        }

        public override Transaction2SubcommandName SubcommandName => Transaction2SubcommandName.TRANS2_QUERY_FILE_INFORMATION;
    }
}