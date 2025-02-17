/* Copyright (C) 2017-2019 Tal Aloni <tal.aloni.il@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using SMBLibrary.NetBios;
using Utilities;

namespace SMBLibrary.Tests
{
    [TestFixture]
    public class NetBiosTests
    {
        [Test]
        public void Test1()
        {
            byte[] buffer = new byte[] { 0x20, 0x46, 0x47, 0x45, 0x4e, 0x44, 0x4a, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x00 };
            int offset = 0;
            string name = NetBiosUtils.DecodeName(buffer, ref offset);
            byte[] encodedName = NetBiosUtils.EncodeName(name, String.Empty);
            Assert.IsTrue(ByteUtils.AreByteArraysEqual(buffer, encodedName));
        }

        [Test]
        public void Test2()
        {
            byte[] buffer = new byte[] { 0x20, 0x46, 0x47, 0x45, 0x4e, 0x44, 0x4a, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x43, 0x41, 0x41, 0x41, 0x00 };
            int offset = 0;
            string name = NetBiosUtils.DecodeName(buffer, ref offset);
            byte[] encodedName = NetBiosUtils.EncodeName(name, String.Empty);
            Assert.IsTrue(ByteUtils.AreByteArraysEqual(buffer, encodedName));
        }

        public void TestAll()
        {
            Test1();
            Test2();
        }
    }
}
