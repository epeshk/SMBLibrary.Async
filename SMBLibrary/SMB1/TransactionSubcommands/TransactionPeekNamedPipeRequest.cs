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
    /// TRANS_PEEK_NMPIPE Request
    /// </summary>
    public class TransactionPeekNamedPipeRequest : TransactionSubcommand
    {
        // Setup:
        public ushort FID;

        public TransactionPeekNamedPipeRequest()
        {
        }

        public TransactionPeekNamedPipeRequest(Span<byte> setup)
        {
            FID = LittleEndianConverter.ToUInt16(setup, 2);
        }

        public override IMemoryOwner<byte> GetSetup()
        {
            var setup = Arrays.Rent(4);
            LittleEndianWriter.WriteUInt16(setup, 0, (ushort)SubcommandName);
            LittleEndianWriter.WriteUInt16(setup, 2, FID);
            return setup;
        }

        public override TransactionSubcommandName SubcommandName => TransactionSubcommandName.TRANS_PEEK_NMPIPE;
    }
}