/*
This file is part of Packet.Net

Packet.Net is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Packet.Net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Packet.Net.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

﻿using System;
using System.IO;
using Packet.Net.Utils;

namespace Packet.Net
{
    /// <summary>
    /// Encapsulates and ensures that we have either a Packet OR
    /// a ByteArrayAndOffset, but not both
    /// </summary>
    internal class PacketOrByteArray
    {
        private ByteArrayAndOffset theByteArray;
        public ByteArrayAndOffset TheByteArray
        {
            get
            {
                return theByteArray;
            }

            set
            {
                thePacket = null;
                theByteArray = value;
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
                theByteArray = null;
                thePacket = value;
            }
        }

        public void AppendToMemoryStream(MemoryStream ms)
        {
            if(ThePacket != null)
            {
                ms.Write(ThePacket.Bytes);
            } else if(TheByteArray != null)
            {
                ms.Write(TheByteArray.RawBytes());
            }
        }
    }

    public abstract class Packet
    {
        private ByteArrayAndOffset header;

        private PacketOrByteArray payloadPacketOrData;

        private Packet parentPacket;

#if false
        private ByteArrayAndOffset payloadData;
        private Packet payloadPacket;
#endif

#if false
        private bool hasOwnMemory;

        /// <value>
        /// Returns true if the packet owns its own byte[] vs.
        /// if it is using a byte[] that was passed it 
        /// </value>
        public bool HasOwnMemory
        {
            get { return hasOwnMemory; }
            set { hasOwnMemory = value; }
        }
#else
        /// <value>
        /// Returns true if this packet, and the packets encapsulated inside of it,
        /// have allocated their own memory vs. are using the byte[] that was passed
        /// in from the outside.
        ///
        /// This works because when we reallocate memory we do so in separate chunks
        /// for header and payload and this causes ByteArrayAndOffset to indicate
        /// NeedsCopyForRawBytes to be false
        /// </value>
        internal bool HasOwnMemory
        {
            get
            {
                // if the header doesn't need a copy then it owns its own memory
                if(!header.NeedsCopyForActualBytes())
                    return true;

                // if we have no data or payload and we didn't already own the header bytes
                // then we must not own the data or payload bytes either
                if((payloadPacketOrData.TheByteArray == null) &&
                   (payloadPacketOrData.ThePacket == null))
                {
                    return false;
                }

                if(payloadPacketOrData.ThePacket == null)
                {
                    return !payloadPacketOrData.TheByteArray.NeedsCopyForActualBytes();
                } else
                {
                    return payloadPacketOrData.ThePacket.HasOwnMemory;
                }
            }
        }
#endif

        /// <summary>
        /// The packet that is carrying this one
        /// </summary>
        public Packet ParentPacket
        {
            get { return parentPacket; }
            set { parentPacket = value; }
        }

        /// <summary>
        /// Packet that this packet carries
        /// </summary>
        public Packet PayloadPacket
        {
            get { return payloadPacketOrData.ThePacket; }
            set
            {
                if (payloadPacketOrData.ThePacket == value)
                    throw new InvalidOperationException("A packet cannot have itself as its payload.");

                payloadPacketOrData.ThePacket = value;
                payloadPacketOrData.ThePacket.ParentPacket = this;
            }
        }

        public byte[] Header
        {
            get { return this.header.ActualBytes(); }
        }

        /// <summary>
        /// The encapsulated data, this may be data sent by a program, or another protocol
        /// </summary>
        public byte[] PayloadData
        {
            get
            {
                if(payloadPacketOrData.TheByteArray == null)
                {
                    return null;
                } else
                {
                    return payloadPacketOrData.TheByteArray.ActualBytes();
                }
            }

            //set { payloadData = value; }
        }

        /// <summary>
        /// byte[] containing this packet and its payload
        /// </summary>
        public virtual byte[] Bytes
        {
            get
            {
                if(HasOwnMemory)
                {
                    var ms = new MemoryStream();

                    // TODO: not sure if this is a performance gain or if
                    //       the compiler is smart enough to not call the get accessor for Header
                    //       twice, once when retrieving the header and again when retrieving the Length
                    var theHeader = Header;
                    ms.Write(theHeader, 0, theHeader.Length);

                    payloadPacketOrData.AppendToMemoryStream(ms);

                    return ms.ToArray();
                } else // fast path, this is the path typically taken
                       // for un-resized packets and payloads
                {
                    return header.Bytes;
                }  
            }
        }

        /// <summary>
        /// Turns an array of bytes into a packet
        /// </summary>
        /// <param name="data">The packets caught</param>
        /// <returns>An ethernet packet which has references to the higher protocols</returns>
        public static Packet Parse(byte[] data)
        {
            return EthernetPacket.Parse(data);
        }

        public virtual System.String ToColoredString(bool colored)
        {
            return String.Empty;
        }

        public virtual System.String ToColoredVerboseString(bool colored)
        {
            return String.Empty;
        }
    }
}
