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
    /// Base class for all packet types.
    /// Defines helper methods and accessors for the architecture that underlies how
    /// packets interact and store their data.
    /// </summary>
    public abstract class Packet
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
#endif

        internal ByteArraySegment header;

        internal PacketOrByteArraySegment payloadPacketOrData = new PacketOrByteArraySegment();

        internal Packet parentPacket;

        internal PosixTimeval timeval;

        /// <value>
        /// PosixTimeval of this packet, can be the packet arrival time
        /// or the packet creation time
        /// </value>
        public virtual PosixTimeval Timeval
        {
            get { return timeval; }
        }

        // recursively finds the length of this packet and all of the packets
        // encapsulated by this packet
        internal int TotalPacketLength
        {
            get
            {
                int totalLength = 0;
                totalLength += header.Length;
    
                if(payloadPacketOrData.Type == PayloadType.Bytes)
                {
                    totalLength += payloadPacketOrData.TheByteArraySegment.Length;
                } else if(payloadPacketOrData.Type == PayloadType.Packet)
                {
                    totalLength += payloadPacketOrData.ThePacket.TotalPacketLength;
                }

                return totalLength;
            }
        }

        /// <value>
        /// Returns true if we already have a contiguous byte[] in either
        /// of these conditions:
        ///
        /// - This packet's header byte[] and payload byte[] are the same instance
        /// or
        /// - This packet's header byte[] and this packet's payload packet
        /// are the same instance and the offsets indicate that the bytes
        /// are contiguous
        /// </value>
        internal bool SharesMemoryWithSubPackets
        {
            get
            {
                log.Debug("");

                switch(payloadPacketOrData.Type)
                {
                case PayloadType.Bytes:
                    // is the byte array payload the same byte[] and does the offset indicate
                    // that the bytes are contiguous?
                    if((header.Bytes == payloadPacketOrData.TheByteArraySegment.Bytes) &&
                       ((header.Offset + header.Length) == payloadPacketOrData.TheByteArraySegment.Offset))
                    {
                        log.Debug("PayloadType.Bytes returning true");
                        return true;
                    } else
                    {
                        log.Debug("PayloadType.Bytes returning false");
                        return false;
                    }
                case PayloadType.Packet:
                    // is the byte array payload the same as the payload packet header and does
                    // the offset indicate that the bytes are contiguous?
                    if((header.Bytes == payloadPacketOrData.ThePacket.header.Bytes) &&
                       ((header.Offset + header.Length) == payloadPacketOrData.ThePacket.header.Offset))
                    {
                        // and does the sub packet share memory with its sub packets?
                        var retval = payloadPacketOrData.ThePacket.SharesMemoryWithSubPackets;
                        log.DebugFormat("PayloadType.Packet retval {0}", retval);
                        return retval;
                    } else
                    {
                        log.Debug("PayloadType.Packet returning false");
                        return false;
                    }
                case PayloadType.None:
                    // no payload data or packet thus we must share memory with
                    // our non-existent sub packets
                    log.Debug("PayloadType.None, returning true");
                    return true;
                default:
                    throw new System.NotImplementedException();
                }
            }
        }

        /// <summary>
        /// The packet that is carrying this one
        /// </summary>
        public virtual Packet ParentPacket
        {
            get { return parentPacket; }
            set { parentPacket = value; }
        }

        /// <value>
        /// Returns a 
        /// </value>
        public virtual byte[] Header
        {
            get { return this.header.ActualBytes(); }
        }

        /// <summary>
        /// Packet that this packet carries if one is present.
        /// Note that the packet MAY have a null PayloadPacket but
        /// a non-null PayloadData
        /// </summary>
        public virtual Packet PayloadPacket
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

        /// <summary>
        /// Payload byte[] if one is present.
        /// Note that the packet MAY have a null PayloadData but a
        /// non-null PayloadPacket
        /// </summary>
        public byte[] PayloadData
        {
            get
            {
                if(payloadPacketOrData.TheByteArraySegment == null)
                {
                    log.Debug("returning null");
                    return null;
                } else
                {
                    var retval = payloadPacketOrData.TheByteArraySegment.ActualBytes();
                    log.DebugFormat("retval.Length: {0}", retval.Length);
                    return retval;
                }
            }

            set
            {
                log.DebugFormat("value.Length {0}", value.Length);

                payloadPacketOrData.TheByteArraySegment = new ByteArraySegment(value, 0, value.Length);
            }
        }

        /// <summary>
        /// byte[] containing this packet and its payload
        /// NOTE: Use 'public virtual ByteArraySegment BytesHighPerformance' for highest performance
        /// </summary>
        public virtual byte[] Bytes
        {
            get
            {
                log.Debug("");

                // Retrieve the byte array container
                var ba = BytesHighPerformance;

                // ActualBytes() will copy bytes if necessary but will avoid a copy in the
                // case where our offset is zero and the byte[] length matches the
                // encapsulated Length
                return ba.ActualBytes();
            }
        }

        /// <value>
        /// The option to return a ByteArraySegment means that this method
        /// is higher performance as the data can start at an offset other than
        /// the first byte.
        /// </value>
        public virtual ByteArraySegment BytesHighPerformance
        {
            get
            {
                log.Debug("");

                // ensure calculated values are properly updated
                RecursivelyUpdateCalculatedValues();

                // if we share memory with all of our sub packets we can take a
                // higher performance path to retrieve the bytes
                if(SharesMemoryWithSubPackets)
                {
                    // The high performance path that is often taken because it is called on
                    // packets that have not had their header, or any of their sub packets, resized
                    var newByteArraySegment = new ByteArraySegment(header.Bytes,
                                                                   header.Offset,
                                                                   (header.Bytes.Length - header.Offset));
                    log.DebugFormat("SharesMemoryWithSubPackets, returning byte array {0}",
                                    newByteArraySegment.ToString());
                    return newByteArraySegment;
                } else // need to rebuild things from scratch
                {
                    log.Debug("rebuilding the byte array");

                    var ms = new MemoryStream();

                    // TODO: not sure if this is a performance gain or if
                    //       the compiler is smart enough to not call the get accessor for Header
                    //       twice, once when retrieving the header and again when retrieving the Length
                    var theHeader = Header;
                    ms.Write(theHeader, 0, theHeader.Length);

                    payloadPacketOrData.AppendToMemoryStream(ms);

                    var newBytes = ms.ToArray();

                    return new ByteArraySegment(newBytes, 0, newBytes.Length);
                }  
            }
        }

        /// <summary>
        /// Basic Packet constructor
        /// </summary>
        /// <param name="timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        public Packet(PosixTimeval timeval)
        {
            this.timeval = timeval;
        }

        /// <summary>
        /// Turns an array of bytes into an EthernetPacket
        /// </summary>
        /// <param name="data">The packets caught</param>
        /// <returns>An ethernet packet which has references to the higher protocols</returns>
        public static Packet Parse(byte[] data)
        {
            return new EthernetPacket(data, 0);
        }

        /// <summary>
        /// Parse a raw packet into its specific packets and payloads
        /// </summary>
        /// <param name="rawPacket">
        /// A <see cref="RawPacket"/>
        /// </param>
        /// <returns>
        /// A <see cref="Packet"/>
        /// </returns>
        public static Packet ParsePacket(RawPacket rawPacket)
        {
            return ParsePacket(rawPacket.LinkLayerType,
                               rawPacket.Timeval,
                               rawPacket.Data);
        }

        /// <summary>
        /// Parse bytes into a packet
        /// </summary>
        /// <param name="LinkLayer">
        /// A <see cref="LinkLayers"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="PacketData">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <returns>
        /// A <see cref="Packet"/>
        /// </returns>
        public static Packet ParsePacket(LinkLayers LinkLayer,
                                         PosixTimeval Timeval,
                                         byte[] PacketData)
        {
            switch(LinkLayer)
            {
            case LinkLayers.Ethernet:
                return new EthernetPacket(PacketData, 0, Timeval);
            case LinkLayers.LinuxSLL:
                return new LinuxSLLPacket(PacketData, 0, Timeval);
            default:
                throw new System.NotImplementedException("LinkLayer of " + LinkLayer + " is not implemented");
            }
        }

        /// <summary>
        /// Used to ensure that values like checksums and lengths are
        /// properly updated
        /// </summary>
        protected void RecursivelyUpdateCalculatedValues()
        {
            // call the possibly overridden method
            UpdateCalculatedValues();

            // if the packet contains another packet, call its
            if(payloadPacketOrData.Type == PayloadType.Packet)
            {
                payloadPacketOrData.ThePacket.RecursivelyUpdateCalculatedValues();
            }
        }

        /// <summary>
        /// Called to ensure that calculated values are updated before
        /// the packet bytes are retrieved
        /// 
        /// Classes should override this method to update things like
        /// checksums and lengths that take too much time or are too complex
        /// to update for each packet parameter change
        /// </summary>
        public virtual void UpdateCalculatedValues()
        { }

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
            if(payloadPacketOrData.Type == PayloadType.Packet)
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
            if(payloadPacketOrData.Type == PayloadType.Packet)
            {
                return payloadPacketOrData.ThePacket.ToColoredVerboseString(colored);
            } else
            {
                return String.Empty;
            }
        }

        /// <value>
        /// Color used when generating the text description of a packet
        /// </value>
        public virtual System.String Color
        {
            get
            {
                return AnsiEscapeSequences.Black;
            }
        }
    }
}
