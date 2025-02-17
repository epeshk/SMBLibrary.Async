/* Copyright (C) 2017 Tal Aloni <tal.aloni.il@gmail.com>. All rights reserved.
 * Copyright (C) 2023 Eugene Peshkov and SMBLibrary.Async contributors. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Threading.Tasks;

namespace SMBLibrary.Client
{
    public interface ISMBFileStore : INTFileStore
    {
        Task<NTStatus> Disconnect();

        uint MaxReadSize
        {
            get;
        }

        uint MaxWriteSize
        {
            get;
        }
    }
}
