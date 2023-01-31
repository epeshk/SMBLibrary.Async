﻿using System;
using System.Text;
using MemoryPools.Memory;

namespace SMBLibrary
{
    /// <summary>
    ///   Implements the MD4 message digest algorithm in C#
    /// </summary>
    /// <remarks>
    ///   <p>
    ///     <b>References:</b>
    ///     <ol>
    ///       <li> Ronald L. Rivest,
    ///         "<a href = "http://www.roxen.com/rfc/rfc1320.html">
    ///            The MD4 Message-Digest Algorithm</a>",
    ///         IETF RFC-1320 (informational).
    ///       </li>
    ///     </ol>         
    ///   </p>
    /// </remarks>
    public class Md4
    {
        // MD4 specific object variables
        //-----------------------------------------------------------------------

        /// <summary>
        ///   The size in bytes of the input block to the transformation algorithm
        /// </summary>
        private const int BLOCK_LENGTH = 64; // = 512 / 8

        /// <summary>
        ///   512-bit work buffer = 16 x 32-bit words
        /// </summary>
        private readonly uint[] X = new uint[16];

        /// <summary>
        ///   4 32-bit words (interim result)
        /// </summary>
        private readonly uint[] context = new uint[4];

        /// <summary>
        ///   512-bit input buffer = 16 x 32-bit words holds until it reaches 512 bits
        /// </summary>
        private byte[] buffer = new byte[BLOCK_LENGTH];

        /// <summary>
        ///   Number of bytes procesed so far mod. 2 power of 64.
        /// </summary>
        private long count;


        // Constructors
        //------------------------------------------------------------------------
        public Md4()
        {
            EngineReset();
        }

        /// <summary>
        ///   This constructor is here to implement the clonability of this class
        /// </summary>
        /// <param name = "md"> </param>
        private Md4(Md4 md)
            : this()
        {
            //this();
            context = (uint[])md.context.Clone();
            buffer = (byte[])md.buffer.Clone();
            count = md.count;
        }

        // Clonable method implementation
        //-------------------------------------------------------------------------
        public object Clone()
        {
            return new Md4(this);
        }

        // JCE methods
        //-------------------------------------------------------------------------

        /// <summary>
        ///   Resets this object disregarding any temporary data present at the
        ///   time of the invocation of this call.
        /// </summary>
        private void EngineReset()
        {
            // initial values of MD4 i.e. A, B, C, D
            // as per rfc-1320; they are low-order byte first
            context[0] = 0x67452301;
            context[1] = 0xEFCDAB89;
            context[2] = 0x98BADCFE;
            context[3] = 0x10325476;
            count = 0L;
            for (var i = 0; i < BLOCK_LENGTH; i++)
                buffer[i] = 0;
        }


        /// <summary>
        ///   Continues an MD4 message digest using the input byte
        /// </summary>
        /// <param name = "b">byte to input</param>
        private void EngineUpdate(byte b)
        {
            // compute number of bytes still unhashed; ie. present in buffer
            var i = (int)(count % BLOCK_LENGTH);
            count++; // update number of bytes
            buffer[i] = b;
            if (i == BLOCK_LENGTH - 1)
                Transform(buffer, 0);
        }

        /// <summary>
        ///   MD4 block update operation
        /// </summary>
        /// <remarks>
        ///   Continues an MD4 message digest operation by filling the buffer, 
        ///   transform(ing) data in 512-bit message block(s), updating the variables
        ///   context and count, and leaving (buffering) the remaining bytes in buffer
        ///   for the next update or finish.
        /// </remarks>
        /// <param name = "input">input block</param>
        /// <param name = "offset">start of meaningful bytes in input</param>
        /// <param name = "len">count of bytes in input blcok to consider</param>
        private void EngineUpdate(Span<byte> input, int offset, int len)
        {
            // make sure we don't exceed input's allocated size/length
            if (offset < 0 || len < 0 || (long)offset + len > input.Length)
                throw new ArgumentOutOfRangeException();

            // compute number of bytes still unhashed; ie. present in buffer
            var bufferNdx = (int)(count % BLOCK_LENGTH);
            count += len; // update number of bytes
            var partLen = BLOCK_LENGTH - bufferNdx;
            var i = 0;
            if (len >= partLen)
            {
                input.Slice(offset + i, partLen).CopyTo(buffer.AsSpan(bufferNdx, partLen));

                Transform(buffer, 0);

                for (i = partLen; i + BLOCK_LENGTH - 1 < len; i += BLOCK_LENGTH)
                    Transform(input, offset + i);
                bufferNdx = 0;
            }
            // buffer remaining input
            if (i < len)
                input.Slice(offset + i, len-1).CopyTo(buffer.AsSpan(bufferNdx));
        }

        /// <summary>
        ///   Completes the hash computation by performing final operations such
        ///   as padding.  At the return of this engineDigest, the MD engine is
        ///   reset.
        /// </summary>
        /// <returns>the array of bytes for the resulting hash value.</returns>
        private byte[] EngineDigest_Rental()
        {
            // pad output to 56 mod 64; as RFC1320 puts it: congruent to 448 mod 512
            var bufferNdx = (int)(count % BLOCK_LENGTH);
            var padLen = (bufferNdx < 56) ? (56 - bufferNdx) : (120 - bufferNdx);

            // padding is always binary 1 followed by binary 0's
            var tail = ExactArrayPool.Rent(padLen + 8);
            tail[0] = 0x80;
            Array.Clear(tail, 1, padLen - 1);

            // append length before final transform
            // save number of bits, casting the long to an array of 8 bytes
            // save low-order byte first.
            for (var i = 0; i < 8; i++)
                tail[padLen + i] = (byte)((count * 8) >> (8 * i));

            EngineUpdate(tail, 0, tail.Length);

            var result = ExactArrayPool.Rent(16);
            // cast this MD4's context (array of 4 uints) into an array of 16 bytes.
            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    result[i * 4 + j] = (byte)(context[i] >> (8 * j));
            
            ExactArrayPool.Return(tail);
            
            // reset the engine
            EngineReset();
            return result;
        }

        /// <summary>
        ///   Returns a byte hash from a string
        /// </summary>
        /// <param name = "s">string to hash</param>
        /// <returns>byte-array that contains the hash</returns>
        public static byte[] GetByteHashFromStringRental(string s)
        {
            var md4 = ObjectsPool<Md4>.Get().Init();
            try
            {
                Span<byte> bytes = stackalloc byte[s.Length << 1]; 
                Encoding.UTF8.GetBytes(s, bytes);
                md4.EngineUpdate(bytes, 0, bytes.Length);
                return md4.EngineDigest_Rental();
            }
            finally
            {
                ObjectsPool<Md4>.Return(md4);
            }
        }

        private Md4 Init()
        {
            EngineReset();
            return this;
        }

        /// <summary>
        ///   Returns a binary hash from an input byte array
        /// </summary>
        /// <param name = "b">byte-array to hash</param>
        /// <returns>binary hash of input</returns>
        public static byte[] GetByteHashFromBytes_Rental(Span<byte> bytes)
        {
            var md4 = ObjectsPool<Md4>.Get().Init();
            try
            {
                md4.EngineUpdate(bytes, 0, bytes.Length);
                return md4.EngineDigest_Rental();
            }
            finally
            {
                ObjectsPool<Md4>.Return(md4);
            }
        }

        /// <summary>
        ///   Returns a string that contains the hexadecimal hash
        /// </summary>
        /// <param name = "b">byte-array to input</param>
        /// <returns>String that contains the hex of the hash</returns>
        public string GetHexHashFromBytes(byte[] b)
        {
            var bytes = GetByteHashFromBytes_Rental(b);
            var res = BytesToHex(bytes, bytes.Length);
            ExactArrayPool.Return(bytes);
            return res;
        }

        /// <summary>
        ///   Returns a string that contains the hexadecimal hash
        /// </summary>
        /// <param name = "s">string to hash</param>
        /// <returns>String that contains the hex of the hash</returns>
        public string GetHexHashFromString(string s)
        {
            var b = GetByteHashFromStringRental(s);
            return BytesToHex(b, b.Length);
        }

        private static string BytesToHex(byte[] a, int len)
        {
            var temp = BitConverter.ToString(a);

            // We need to remove the dashes that come from the BitConverter
            var sb = new StringBuilder((len - 2) / 2); // This should be the final size

            for (var i = 0; i < temp.Length; i++)
                if (temp[i] != '-')
                    sb.Append(temp[i]);

            return sb.ToString();
        }

        // own methods
        //-----------------------------------------------------------------------------------

        /// <summary>
        ///   MD4 basic transformation
        /// </summary>
        /// <remarks>
        ///   Transforms context based on 512 bits from input block starting
        ///   from the offset'th byte.
        /// </remarks>
        /// <param name = "block">input sub-array</param>
        /// <param name = "offset">starting position of sub-array</param>
        private void Transform(Span<byte> block, int offset)
        {
            // decodes 64 bytes from input block into an array of 16 32-bit
            // entities. Use A as a temp var.
            for (var i = 0; i < 16; i++)
                X[i] = ((uint)block[offset++] & 0xFF) |
                       (((uint)block[offset++] & 0xFF) << 8) |
                       (((uint)block[offset++] & 0xFF) << 16) |
                       (((uint)block[offset++] & 0xFF) << 24);


            var A = context[0];
            var B = context[1];
            var C = context[2];
            var D = context[3];

            A = FF(A, B, C, D, X[0], 3);
            D = FF(D, A, B, C, X[1], 7);
            C = FF(C, D, A, B, X[2], 11);
            B = FF(B, C, D, A, X[3], 19);
            A = FF(A, B, C, D, X[4], 3);
            D = FF(D, A, B, C, X[5], 7);
            C = FF(C, D, A, B, X[6], 11);
            B = FF(B, C, D, A, X[7], 19);
            A = FF(A, B, C, D, X[8], 3);
            D = FF(D, A, B, C, X[9], 7);
            C = FF(C, D, A, B, X[10], 11);
            B = FF(B, C, D, A, X[11], 19);
            A = FF(A, B, C, D, X[12], 3);
            D = FF(D, A, B, C, X[13], 7);
            C = FF(C, D, A, B, X[14], 11);
            B = FF(B, C, D, A, X[15], 19);

            A = GG(A, B, C, D, X[0], 3);
            D = GG(D, A, B, C, X[4], 5);
            C = GG(C, D, A, B, X[8], 9);
            B = GG(B, C, D, A, X[12], 13);
            A = GG(A, B, C, D, X[1], 3);
            D = GG(D, A, B, C, X[5], 5);
            C = GG(C, D, A, B, X[9], 9);
            B = GG(B, C, D, A, X[13], 13);
            A = GG(A, B, C, D, X[2], 3);
            D = GG(D, A, B, C, X[6], 5);
            C = GG(C, D, A, B, X[10], 9);
            B = GG(B, C, D, A, X[14], 13);
            A = GG(A, B, C, D, X[3], 3);
            D = GG(D, A, B, C, X[7], 5);
            C = GG(C, D, A, B, X[11], 9);
            B = GG(B, C, D, A, X[15], 13);

            A = HH(A, B, C, D, X[0], 3);
            D = HH(D, A, B, C, X[8], 9);
            C = HH(C, D, A, B, X[4], 11);
            B = HH(B, C, D, A, X[12], 15);
            A = HH(A, B, C, D, X[2], 3);
            D = HH(D, A, B, C, X[10], 9);
            C = HH(C, D, A, B, X[6], 11);
            B = HH(B, C, D, A, X[14], 15);
            A = HH(A, B, C, D, X[1], 3);
            D = HH(D, A, B, C, X[9], 9);
            C = HH(C, D, A, B, X[5], 11);
            B = HH(B, C, D, A, X[13], 15);
            A = HH(A, B, C, D, X[3], 3);
            D = HH(D, A, B, C, X[11], 9);
            C = HH(C, D, A, B, X[7], 11);
            B = HH(B, C, D, A, X[15], 15);

            context[0] += A;
            context[1] += B;
            context[2] += C;
            context[3] += D;
        }

        // The basic MD4 atomic functions.

        private uint FF(uint a, uint b, uint c, uint d, uint x, int s)
        {
            var t = a + ((b & c) | (~b & d)) + x;
            return t << s | t >> (32 - s);
        }

        private uint GG(uint a, uint b, uint c, uint d, uint x, int s)
        {
            var t = a + ((b & (c | d)) | (c & d)) + x + 0x5A827999;
            return t << s | t >> (32 - s);
        }

        private uint HH(uint a, uint b, uint c, uint d, uint x, int s)
        {
            var t = a + (b ^ c ^ d) + x + 0x6ED9EBA1;
            return t << s | t >> (32 - s);
        }
    }

}