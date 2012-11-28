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
using System.Text;
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
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Used internally when building new packet dissectors
        /// </summary>
        protected ByteArraySegment header;

        /// <summary>
        /// Used internally when building new packet dissectors
        /// </summary>
        protected PacketOrByteArraySegment payloadPacketOrData = new PacketOrByteArraySegment();

        /// <summary>
        /// The parent packet. Accessible via the 'ParentPacket' property
        /// </summary>
        private Packet parentPacket;

        /// <summary>
        /// Gets the total length of the packet.
        /// Recursively finds the length of this packet and all of the packets
        /// encapsulated by this packet
        /// </summary>
        /// <value>
        /// The total length of the packet.
        /// </value>
        protected int TotalPacketLength
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
        protected bool SharesMemoryWithSubPackets
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
                if (this == value)
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
                                                                   header.BytesLength - header.Offset);
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
        /// Constructor
        /// </summary>
        public Packet()
        {
        }

        /// <summary>
        /// Parse bytes into a packet
        /// </summary>
        /// <param name="LinkLayer">
        /// A <see cref="LinkLayers"/>
        /// </param>
        /// <param name="PacketData">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <returns>
        /// A <see cref="Packet"/>
        /// </returns>
        public static Packet ParsePacket(LinkLayers LinkLayer,
                                         byte[] PacketData)
        {
            Packet p;
            var bas = new ByteArraySegment(PacketData);

            log.DebugFormat("LinkLayer {0}", LinkLayer);

            switch(LinkLayer)
            {
            case LinkLayers.Ethernet:
                p = new EthernetPacket(bas);
                break;
            case LinkLayers.LinuxSLL:
                p = new LinuxSLLPacket(bas);
                break;
            case LinkLayers.Ppp:
                p = new PPPPacket(bas);
                break;
            case LinkLayers.Ieee80211:
                p = Ieee80211.MacFrame.ParsePacket(bas);
                break;
            case LinkLayers.Ieee80211_Radio:
                p = new Ieee80211.RadioPacket(bas);
                break;
            case LinkLayers.PerPacketInformation:
                p = new Ieee80211.PpiPacket(bas);
                break;
            default:
                throw new System.NotImplementedException("LinkLayer of " + LinkLayer + " is not implemented");
            }

            return p;
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

        /// <summary>Output this packet as a readable string</summary>
        public override System.String ToString()
        {
            return ToString(StringOutputType.Normal);
        }

        /// <summary cref="Packet.ToString()">
        ///
        /// Output the packet information in the specified format
        ///   Normal - outputs the packet info to a single line
        ///   Colored - outputs the packet info to a single line with coloring
        ///   Verbose - outputs detailed info about the packet
        ///   VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <param name="outputFormat">
        /// <see cref="StringOutputType" />
        /// </param>
        public virtual string ToString(StringOutputType outputFormat)
        {
            if(payloadPacketOrData.Type == PayloadType.Packet)
            {
                return payloadPacketOrData.ThePacket.ToString(outputFormat);
            } else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Prints the Packet PayloadData in Hex format
        ///  With the 16-byte segment number, raw bytes, and parsed ascii output
        /// Ex:
        ///  0010  00 18 82 6c 7c 7f 00 c0  9f 77 a3 b0 88 64 11 00   ...1|... .w...d..
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string PrintHex()
        {
            byte[] data = BytesHighPerformance.Bytes;
            var buffer = new StringBuilder();
            string segmentNumber = "";
            string bytes = "";
            string ascii = "";

            buffer.AppendLine("Data:  ******* Raw Hex Output - length=" + data.Length + " bytes");
            buffer.AppendLine("Data: Segment:                   Bytes:                              Ascii:");
            buffer.AppendLine("Data: --------------------------------------------------------------------------");

            // parse the raw data
            for(int i = 1; i <= data.Length; i++)
            {
                // add the current byte to the bytes hex string
                bytes += (data[i-1].ToString("x")).PadLeft(2, '0') + " ";

                // add the current byte to the asciiBytes array for later processing
                if(data[i-1] < 0x21 || data[i-1] > 0x7e)
                {
                    ascii += ".";
                }
                else
                {
                    ascii += Encoding.ASCII.GetString(new byte[1] { data[i-1] });
                }

                // add an additional space to split the bytes into
                //  two groups of 8 bytes
                if(i % 16 != 0 && i % 8 == 0)
                {
                    bytes += " ";
                    ascii += " ";
                }

                // append the output string
                if(i % 16 == 0)
                {
                    // add the 16 byte segment number
                    segmentNumber = ((((i - 16) / 16) * 10).ToString()).PadLeft(4, '0');

                    // build the line
                    buffer.AppendLine("Data: " + segmentNumber + "  " + bytes + "  " + ascii);

                    // reset for the next line
                    bytes = "";
                    ascii = "";

                    continue;
                }

                // handle the last pass
                if(i == data.Length)
                {
                    // add the 16 byte segment number
                    segmentNumber = (((((i - 16) / 16) + 1) * 10).ToString()).PadLeft(4, '0');

                    // build the line
                    buffer.AppendLine("Data: " + (segmentNumber.ToString()).PadLeft(4, '0') + "  " + bytes.PadRight(49, ' ') + "  " + ascii);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Extract a packet of a specific type or null if a packet of the given type isn't found
        /// NOTE: a 'dynamic' return type is possible here but costs ~7.8% in performance
        /// </summary>
        /// <param name='type'>
        /// Type.
        /// </param>
        public Packet Extract(System.Type type)
        {
            var p = this;

            // search for a packet type that matches the given one
            do
            {
                if(type.IsAssignableFrom(p.GetType ()))
                {
                    return p;
                }

                // move to the PayloadPacket
                p = p.PayloadPacket;
            } while(p != null);

            return null;
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
