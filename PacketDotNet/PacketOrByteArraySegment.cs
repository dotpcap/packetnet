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
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.IO;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Encapsulates and ensures that we have either a Packet OR
    /// a ByteArraySegment but not both
    /// </summary>
    internal class PacketOrByteArraySegment
    {
        private ByteArraySegment theByteArraySegment;
        public ByteArraySegment TheByteArraySegment
        {
            get
            {
                return theByteArraySegment;
            }

            set
            {
                thePacket = null;
                theByteArraySegment = value;
            }
        }

        private Packet thePacket;
        public Packet ThePacket
        {
            get
            {
                return thePacket;
            }

            set
            {
                theByteArraySegment = null;
                thePacket = value;
            }
        }

        /// <summary>
        /// Appends to the MemoryStream either the byte[] represented by TheByteArray, or
        /// if ThePacket is non-null, the Packet.Bytes will be appended to the memory stream
        /// which will append ThePacket's header and any encapsulated packets it contains
        /// </summary>
        /// <param name="ms">
        /// A <see cref="MemoryStream"/>
        /// </param>
        public void AppendToMemoryStream(MemoryStream ms)
        {
            if(ThePacket != null)
            {
                var theBytes = ThePacket.Bytes;
                ms.Write(theBytes, 0, theBytes.Length);
            } else if(TheByteArraySegment != null)
            {
                var theBytes = TheByteArraySegment.ActualBytes();
                ms.Write(theBytes, 0, theBytes.Length);
            }
        }

        /// <value>
        /// Whether or not this container contains a packet, a byte[] or neither
        /// </value>
        public PayloadType Type
        {
            get
            {
                if(ThePacket != null)
                {
                    return PayloadType.Packet;
                } else if(TheByteArraySegment != null)
                {
                    return PayloadType.Bytes;
                } else
                {
                    return PayloadType.None;
                }
            }
        }
    }
}