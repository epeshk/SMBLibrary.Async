/* Copyright (C) 2017 Tal Aloni <tal.aloni.il@gmail.com>. All rights reserved.
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
    /// TRANS2_SET_FS_INFORMATION Request
    /// </summary>
    public class Transaction2SetFSInformationRequest : Transaction2Subcommand
    {
        private const ushort SMB_INFO_PASSTHROUGH = 0x03E8;
        public const int ParametersLength = 4;
        // Parameters:
        public ushort FID;
        public ushort InformationLevel; // This field MUST be a pass-through Information Level.
        // Data:
        public IMemoryOwner<byte> InformationBytes;

        public Transaction2SetFSInformationRequest()
        {
        }

        public Transaction2SetFSInformationRequest(IMemoryOwner<byte> parameters, IMemoryOwner<byte> data, bool isUnicode)
        {
            FID = LittleEndianConverter.ToUInt16(parameters, 0);
            InformationLevel = LittleEndianConverter.ToUInt16(parameters, 2);

            InformationBytes = data;
        }

        public override void GetSetupInto(Span<byte> target)
        {
            LittleEndianConverter.GetBytes(target, (ushort)SubcommandName);
        }

        public override IMemoryOwner<byte> GetParameters(bool isUnicode)
        {
            var parameters = Arrays.Rent(ParametersLength);
            LittleEndianWriter.WriteUInt16(parameters, 0, FID);
            LittleEndianWriter.WriteUInt16(parameters, 2, InformationLevel);
            return parameters;
        }

        public override IMemoryOwner<byte> GetData(bool isUnicode)
        {
            return InformationBytes;
        }

        public bool IsPassthroughInformationLevel => (InformationLevel >= SMB_INFO_PASSTHROUGH);

        public FileSystemInformationClass FileSystemInformationClass
        {
            get => (FileSystemInformationClass)(InformationLevel - SMB_INFO_PASSTHROUGH);
            set => InformationLevel = (ushort)((ushort)value + SMB_INFO_PASSTHROUGH);
        }

        /// <remarks>
        /// Support for pass-through Information Levels must be enabled.
        /// </remarks>
        public void SetFileSystemInformation(FileSystemInformation information)
        {
            FileSystemInformationClass = information.FileSystemInformationClass;
            InformationBytes = information.GetBytes();
        }

        public override Transaction2SubcommandName SubcommandName => Transaction2SubcommandName.TRANS2_SET_FS_INFORMATION;

        public override void Dispose()
        {
            base.Dispose();
            InformationBytes.Dispose();
        }
    }
}