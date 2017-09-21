/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PacketDotNet.Utils
{
    /// <summary>
    /// Implementation binary reader from naturally aligned memory stream.
    /// </summary>
    public class NaturalAlignmentBinaryReader : BinaryReader
    {
        /// <summary>
        /// Current count of bytes from begin of aligned memory
        /// </summary>
        protected int m_CurrentOffset;

        /// <summary>
        /// Aligned memory can begin before of memory stream. 
        /// In this case needed set count bytes from begin of aligned memory to start of input
        /// </summary>
        public NaturalAlignmentBinaryReader(Stream input, int offset = 0) : base(input)
        {
            m_CurrentOffset = offset;
        }

        /// <summary>
        /// Check for current offset is alignment by 16 bits and ignore paddingBytes before next read.
        /// </summary>
        public override short ReadInt16()
        {
            int paddingBytes = m_CurrentOffset % Marshal.SizeOf(typeof(Int16));
            if (paddingBytes > 0)
            {
                base.ReadBytes(paddingBytes);
                m_CurrentOffset += paddingBytes;
            }
            m_CurrentOffset += Marshal.SizeOf(typeof(Int16));
            return base.ReadInt16();
        }

        /// <summary>
        /// Check for current offset is alignment by 32 bits and ignore paddingBytes before next read.
        /// </summary>
        public override int ReadInt32()
        {
            int paddingBytes = m_CurrentOffset % Marshal.SizeOf(typeof(Int32));
            if (paddingBytes > 0)
            {
                base.ReadBytes(paddingBytes);
                m_CurrentOffset += paddingBytes;
            }
            m_CurrentOffset += Marshal.SizeOf(typeof(Int32));
            return base.ReadInt32();
        }

        /// <summary>
        /// Check for current offset is alignment by 64 bits and ignore paddingBytes before next read.
        /// </summary>
        public override long ReadInt64()
        {
            int paddingBytes = m_CurrentOffset % Marshal.SizeOf(typeof(Int64));
            if (paddingBytes > 0)
            {
                base.ReadBytes(paddingBytes);
                m_CurrentOffset += paddingBytes;
            }
            m_CurrentOffset += Marshal.SizeOf(typeof(Int64));
            return base.ReadInt64();
        }

        /// <summary>
        /// Check for current offset is alignment by 16 bits and ignore paddingBytes before next read.
        /// </summary>
        public override ushort ReadUInt16()
        {
            int paddingBytes = m_CurrentOffset % Marshal.SizeOf(typeof(UInt16));
            if (paddingBytes > 0)
            {
                base.ReadBytes(paddingBytes);
                m_CurrentOffset += paddingBytes;
            }
            m_CurrentOffset += Marshal.SizeOf(typeof(UInt16));
            return base.ReadUInt16();
        }

        /// <summary>
        /// Check for current offset is alignment by 32 bits and ignore paddingBytes before next read.
        /// </summary>
        public override uint ReadUInt32()
        {
            int paddingBytes = m_CurrentOffset % Marshal.SizeOf(typeof(UInt32));
            if (paddingBytes > 0)
            {
                base.ReadBytes(paddingBytes);
                m_CurrentOffset += paddingBytes;
            }
            m_CurrentOffset += Marshal.SizeOf(typeof(UInt32));
            return base.ReadUInt32();
        }

        /// <summary>
        /// Check for current offset is alignment by 64 bits and ignore paddingBytes before next read.
        /// </summary>
        public override ulong ReadUInt64()
        {
            int paddingBytes = m_CurrentOffset % Marshal.SizeOf(typeof(UInt64));
            if (paddingBytes > 0)
            {
                base.ReadBytes(paddingBytes);
                m_CurrentOffset += paddingBytes;
            }
            m_CurrentOffset += Marshal.SizeOf(typeof(UInt64));
            return base.ReadUInt64();
        }

        /// <summary>
        /// Save current position in m_CurrentOffset and read
        /// </summary>
        public override byte ReadByte()
        {
            m_CurrentOffset += Marshal.SizeOf(typeof(byte));
            return base.ReadByte();
        }

        /// <summary>
        /// Save current position in m_CurrentOffset and read
        /// </summary>
        public override sbyte ReadSByte()
        {
            m_CurrentOffset += Marshal.SizeOf(typeof(sbyte));
            return base.ReadSByte();
        }

        /// <summary>
        /// Save current position in m_CurrentOffset and read
        /// </summary>
        public override byte[] ReadBytes(int count)
        {
            byte[] retval = base.ReadBytes(count);
            m_CurrentOffset += retval.Length;
            return retval;
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override int PeekChar()
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override int Read()
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override int Read(byte[] buffer, int index, int count)
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override int Read(char[] buffer, int index, int count)
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override bool ReadBoolean()
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override char ReadChar()
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override char[] ReadChars(int count)
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override decimal ReadDecimal()
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override double ReadDouble()
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override float ReadSingle()
        {
            throw new NotImplementedException("Method not implemented");
        }

        /// <summary>
        /// Call base method can break m_CurrentOffset calculation
        /// </summary>
        public override string ReadString()
        {
            throw new NotImplementedException("Method not implemented");
        }
    }
}
