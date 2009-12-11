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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

﻿using System;
using System.IO;
using PacketDotNet.Utils;

namespace PacketDotNet
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
            } else if(TheByteArray != null)
            {
                var theBytes = TheByteArray.ActualBytes();
                ms.Write(theBytes, 0, theBytes.Length);
            }
        }

        public PayloadType Type
        {
            get
            {
                if(ThePacket != null)
                {
                    return PayloadType.Packet;
                } else if(TheByteArray != null)
                {
                    return PayloadType.Bytes;
                } else
                {
                    return PayloadType.None;
                }
            }
        }
    }

    internal enum PayloadType
    {
        Packet,
        Bytes,
        None
    }

    /// <summary>
    /// Base class for all packet types.
    /// Defines helper methods and accessors for the architecture that underlies how
    /// packets interact and store their data.
    /// </summary>
    public abstract class Packet
    {
        internal ByteArrayAndOffset header;

        internal PacketOrByteArray payloadPacketOrData;

        internal Packet parentPacket;

        internal Timeval timeval;

        /// <value>
        /// Returns true if the same byte[] represents this packet's header byte[]
        /// and payload byte[], or this packet's header byte[] and that of the payload packet
        /// and that the offsets are contiguous
        /// </value>
        internal bool SharesMemoryWithSubPackets
        {
            get
            {
                if(payloadPacketOrData == null)
                {
                    // no payload packet or data so we must share memory with the
                    // non-existent payload
                    return true;
                }

                switch(payloadPacketOrData.Type)
                {
                case PayloadType.Bytes:
                    // is the byte array payload the same byte[] and does the offset indicate
                    // that the bytes are contiguous?
                    if((header.Bytes == payloadPacketOrData.TheByteArray.Bytes) &&
                       ((header.Offset + header.Length) == payloadPacketOrData.TheByteArray.Offset))
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                case PayloadType.Packet:
                    // is the byte array payload the same as the payload packet header and does
                    // the offset indicate that the bytes are contiguous?
                    if((header.Bytes == payloadPacketOrData.ThePacket.header.Bytes) &&
                       ((header.Offset + header.Length) == payloadPacketOrData.ThePacket.header.Offset))
                    {
                        // and does the sub packet share memory with its sub packets?
                        return payloadPacketOrData.ThePacket.SharesMemoryWithSubPackets;
                    } else
                    {
                        return false;
                    }
                case PayloadType.None:
                    // no payload data or packet thus we must share memory with
                    // our non-existent sub packets
                    return true;
                default:
                    throw new System.NotImplementedException();
                }
            }
        }

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

        /// <value>
        /// Returns a 
        /// </value>
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
        /// NOTE: Use 'public virtual ByteArrayAndOffset BytesHighPerformance' for highest performance
        /// </summary>
        public virtual byte[] Bytes
        {
            get
            {
                // Retrieve the byte array container
                var ba = BytesHighPerformance;

                // ActualBytes() will copy bytes if necessary but will avoid a copy in the
                // case where our offset is zero and the byte[] length matches the
                // encapsulated Length
                return ba.ActualBytes();
            }
        }

        public virtual ByteArrayAndOffset BytesHighPerformance
        {
            get
            {
                // if we share memory with all of our sub packets we can take a
                // higher performance path to retrieve the bytes
                if(SharesMemoryWithSubPackets)
                {
                    // The high performance path that is often taken because it is called on
                    // packets that have not had their header, or any of their sub packets, resized
                    var newByteArrayAndOffset = new ByteArrayAndOffset(header.Bytes, header.Offset, header.Bytes.Length);
                    return newByteArrayAndOffset;
                } else // need to rebuild things from scratch
                {
                    var ms = new MemoryStream();

                    // TODO: not sure if this is a performance gain or if
                    //       the compiler is smart enough to not call the get accessor for Header
                    //       twice, once when retrieving the header and again when retrieving the Length
                    var theHeader = Header;
                    ms.Write(theHeader, 0, theHeader.Length);

                    payloadPacketOrData.AppendToMemoryStream(ms);

                    var newBytes = ms.ToArray();

                    return new ByteArrayAndOffset(newBytes, 0, newBytes.Length);
                }  
            }
        }

        public Packet(Timeval timeval)
        {
            this.timeval = timeval;
        }

        public Packet()
        {
            this.timeval = new Timeval();
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

        /// <summary>
        /// Returns a ansi colored string. This routine calls
        /// the ToColoredString() of the payload packet if one
        /// is present.
        /// </summary>
        /// <param name="colored">
        /// A <see cref="System.Boolean"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public virtual System.String ToColoredString(bool colored)
        {
            if((payloadPacketOrData != null) &&
               (payloadPacketOrData.Type == PayloadType.Packet))
            {
                return payloadPacketOrData.ThePacket.ToColoredString(colored);
            } else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns a verbose ansi colored string. This routine calls
        /// the ToColoredVerboseString() of the payload packet if one
        /// is present.
        /// </summary>
        /// <param name="colored">
        /// A <see cref="System.Boolean"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public virtual System.String ToColoredVerboseString(bool colored)
        {
            if((payloadPacketOrData != null) &&
               (payloadPacketOrData.Type == PayloadType.Packet))
            {
                return payloadPacketOrData.ThePacket.ToColoredVerboseString(colored);
            } else
            {
                return String.Empty;
            }
        }

        public virtual System.String Color
        {
            get
            {
                return AnsiEscapeSequences.Black;
            }
        }
    }
}
