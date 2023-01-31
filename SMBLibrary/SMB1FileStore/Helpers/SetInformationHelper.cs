/* Copyright (C) 2014-2017 Tal Aloni <tal.aloni.il@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */

using System;

namespace SMBLibrary.SMB1
{
    public class SetInformationHelper
    {
        public static FileInformation ToFileInformation(SetInformation information)
        {
            if (information is SetFileBasicInfo)
            {
                var basicInfo = (SetFileBasicInfo)information;
                var fileBasicInfo = new FileBasicInformation();
                fileBasicInfo.CreationTime = basicInfo.CreationTime;
                fileBasicInfo.LastAccessTime = basicInfo.LastAccessTime;
                fileBasicInfo.LastWriteTime = basicInfo.LastWriteTime;
                fileBasicInfo.ChangeTime = basicInfo.LastChangeTime;
                fileBasicInfo.FileAttributes = (FileAttributes)basicInfo.ExtFileAttributes;
                fileBasicInfo.Reserved = basicInfo.Reserved;
                return fileBasicInfo;
            }

            if (information is SetFileDispositionInfo)
            {
                var fileDispositionInfo = new FileDispositionInformation();
                fileDispositionInfo.DeletePending = ((SetFileDispositionInfo)information).DeletePending;
                return fileDispositionInfo;
            }

            if (information is SetFileAllocationInfo)
            {
                // This information level is used to set the file length in bytes.
                // Note: the input will NOT be a multiple of the cluster size / bytes per sector.
                var fileAllocationInfo = new FileAllocationInformation();
                fileAllocationInfo.AllocationSize = ((SetFileAllocationInfo)information).AllocationSize;
                return fileAllocationInfo;
            }

            if (information is SetFileEndOfFileInfo)
            {
                var fileEndOfFileInfo = new FileEndOfFileInformation();
                fileEndOfFileInfo.EndOfFile = ((SetFileEndOfFileInfo)information).EndOfFile;
                return fileEndOfFileInfo;
            }

            throw new NotImplementedException();
        }
    }
}