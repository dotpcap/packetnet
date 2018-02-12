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

using System;
using System.IO;
using System.Text;
using PacketDotNet.Ethernet;
using PacketDotNet.Ieee80211;
using PacketDotNet.IP;
using PacketDotNet.LinuxSLL;
using PacketDotNet.PPP;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    ///     Base class for all packet types.
    ///     Defines helper methods and accessors for the architecture that underlies how
    ///     packets interact and store their data.
    /// </summary>
    [Serializable]
    public abstract class Packet
    {
#if DEBUG
        private static readonly log4net.ILog Log =
 log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        ///     Used internally when building new packet dissectors
        /// </summary>
        protected ByteArraySegment HeaderByteArraySegment;

        /// <summary>
        ///     Used internally when building new packet dissectors
        /// </summary>
        protected PacketOrByteArraySegment PayloadPacketOrData = new PacketOrByteArraySegment();

        /// <summary>
        ///     The parent packet. Accessible via the 'ParentPacket' property
        /// </summary>
        private Packet _parentPacket;

        /// <summary>
        ///     Gets the total length of the packet.
        ///     Recursively finds the length of this packet and all of the packets
        ///     encapsulated by this packet
        /// </summary>
        /// <value>
        ///     The total length of the packet.
        /// </value>
        protected Int32 TotalPacketLength
        {
            get
            {
                Int32 totalLength = 0;
                totalLength += this.HeaderByteArraySegment.Length;

                if (this.PayloadPacketOrData.Type == PayloadType.Bytes)
                {
                    totalLength += this.PayloadPacketOrData.TheByteArraySegment.Length;
                }
                else if (this.PayloadPacketOrData.Type == PayloadType.Packet)
                {
                    totalLength += this.PayloadPacketOrData.ThePacket.TotalPacketLength;
                }

                return totalLength;
            }
        }

        /// <value>
        ///     Returns true if we already have a contiguous byte[] in either
        ///     of these conditions:
        ///     - This packet's header byte[] and payload byte[] are the same instance
        ///     or
        ///     - This packet's header byte[] and this packet's payload packet
        ///     are the same instance and the offsets indicate that the bytes
        ///     are contiguous
        /// </value>
        protected Boolean SharesMemoryWithSubPackets
        {
            get
            {
                Log.Debug("");

                switch (this.PayloadPacketOrData.Type)
                {
                    case PayloadType.Bytes:
                        // is the byte array payload the same byte[] and does the offset indicate
                        // that the bytes are contiguous?
                        if ((this.HeaderByteArraySegment.Bytes == this.PayloadPacketOrData.TheByteArraySegment.Bytes) &&
                            ((this.HeaderByteArraySegment.Offset + this.HeaderByteArraySegment.Length) ==
                             this.PayloadPacketOrData.TheByteArraySegment.Offset))
                        {
                            Log.Debug("PayloadType.Bytes returning true");
                            return true;
                        }
                        else
                        {
                            Log.Debug("PayloadType.Bytes returning false");
                            return false;
                        }
                    case PayloadType.Packet:
                        // is the byte array payload the same as the payload packet header and does
                        // the offset indicate that the bytes are contiguous?
                        if ((this.HeaderByteArraySegment.Bytes ==
                             this.PayloadPacketOrData.ThePacket.HeaderByteArraySegment.Bytes) &&
                            ((this.HeaderByteArraySegment.Offset + this.HeaderByteArraySegment.Length) ==
                             this.PayloadPacketOrData.ThePacket.HeaderByteArraySegment.Offset))
                        {
                            // and does the sub packet share memory with its sub packets?
                            var retval = this.PayloadPacketOrData.ThePacket.SharesMemoryWithSubPackets;
                            Log.DebugFormat("PayloadType.Packet retval {0}", retval);
                            return retval;
                        }
                        else
                        {
                            Log.Debug("PayloadType.Packet returning false");
                            return false;
                        }
                    case PayloadType.None:
                        // no payload data or packet thus we must share memory with
                        // our non-existent sub packets
                        Log.Debug("PayloadType.None, returning true");
                        return true;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        ///     The packet that is carrying this one
        /// </summary>
        public virtual Packet ParentPacket
        {
            get => this._parentPacket;
            set => this._parentPacket = value;
        }

        /// <value>
        ///     Returns a
        /// </value>
        public virtual Byte[] Header => this.HeaderByteArraySegment.ActualBytes();

        /// <summary>
        ///     Packet that this packet carries if one is present.
        ///     Note that the packet MAY have a null PayloadPacket but
        ///     a non-null PayloadData
        /// </summary>
        public virtual Packet PayloadPacket
        {
            get => this.PayloadPacketOrData.ThePacket;
            set
            {
                if (this == value)
                    throw new InvalidOperationException("A packet cannot have itself as its payload.");

                this.PayloadPacketOrData.ThePacket = value;
                this.PayloadPacketOrData.ThePacket.ParentPacket = this;
            }
        }

        /// <summary>
        ///     Payload byte[] if one is present.
        ///     Note that the packet MAY have a null PayloadData but a
        ///     non-null PayloadPacket
        /// </summary>
        public Byte[] PayloadData
        {
            get
            {
                if (this.PayloadPacketOrData.TheByteArraySegment == null)
                {
                    Log.Debug("returning null");
                    return null;
                }

                var retval = this.PayloadPacketOrData.TheByteArraySegment.ActualBytes();
                Log.DebugFormat("retval.Length: {0}", retval.Length);
                return retval;
            }

            set
            {
                Log.DebugFormat("value.Length {0}", value.Length);

                this.PayloadPacketOrData.TheByteArraySegment = new ByteArraySegment(value, 0, value.Length);
            }
        }

        /// <summary>
        ///     byte[] containing this packet and its payload
        ///     NOTE: Use 'public virtual ByteArraySegment BytesHighPerformance' for highest performance
        /// </summary>
        public virtual Byte[] Bytes
        {
            get
            {
                Log.Debug("");

                // Retrieve the byte array container
                var ba = this.BytesHighPerformance;

                // ActualBytes() will copy bytes if necessary but will avoid a copy in the
                // case where our offset is zero and the byte[] length matches the
                // encapsulated Length
                return ba.ActualBytes();
            }
        }

        /// <value>
        ///     The option to return a ByteArraySegment means that this method
        ///     is higher performance as the data can start at an offset other than
        ///     the first byte.
        /// </value>
        public virtual ByteArraySegment BytesHighPerformance
        {
            get
            {
                Log.Debug("");

                // ensure calculated values are properly updated
                this.RecursivelyUpdateCalculatedValues();

                // if we share memory with all of our sub packets we can take a
                // higher performance path to retrieve the bytes
                if (this.SharesMemoryWithSubPackets)
                {
                    // The high performance path that is often taken because it is called on
                    // packets that have not had their header, or any of their sub packets, resized
                    var newByteArraySegment = new ByteArraySegment(this.HeaderByteArraySegment.Bytes,
                        this.HeaderByteArraySegment.Offset,
                        this.HeaderByteArraySegment.BytesLength - this.HeaderByteArraySegment.Offset);
                    Log.DebugFormat("SharesMemoryWithSubPackets, returning byte array {0}",
                        newByteArraySegment.ToString());
                    return newByteArraySegment;
                }

                Log.Debug("rebuilding the byte array");

                var ms = new MemoryStream();

                // TODO: not sure if this is a performance gain or if
                //       the compiler is smart enough to not call the get accessor for Header
                //       twice, once when retrieving the header and again when retrieving the Length
                var theHeader = this.Header;
                ms.Write(theHeader, 0, theHeader.Length);

                this.PayloadPacketOrData.AppendToMemoryStream(ms);

                var newBytes = ms.ToArray();

                return new ByteArraySegment(newBytes, 0, newBytes.Length);
            }
        }

        /// <summary>
        ///     Parse bytes into a packet
        /// </summary>
        /// <param name="linkLayer">
        ///     A <see cref="LinkLayers" />
        /// </param>
        /// <param name="packetData">
        ///     A <see cref="System.Byte" />
        /// </param>
        /// <returns>
        ///     A <see cref="Packet" />
        /// </returns>
        public static Packet ParsePacket(LinkLayers linkLayer,
            Byte[] packetData)
        {
            Packet p;
            var bas = new ByteArraySegment(packetData);

            Log.DebugFormat("LinkLayer {0}", linkLayer);

            switch (linkLayer)
            {
                case LinkLayers.Ethernet:
                    p = new EthernetPacket(bas);
                    break;
                case LinkLayers.LinuxSLL:
                    p = new LinuxSLLPacket(bas);
                    break;
                case LinkLayers.Null:
                    p = new NullPacket(bas);
                    break;
                case LinkLayers.Ppp:
                    p = new PPPPacket(bas);
                    break;
                case LinkLayers.Ieee80211:
                    p = MacFrame.ParsePacket(bas);
                    break;
                case LinkLayers.Ieee80211_Radio:
                    p = new RadioPacket(bas);
                    break;
                case LinkLayers.PerPacketInformation:
                    p = new PpiPacket(bas);
                    break;
                case LinkLayers.Raw:
                    p = new RawIPPacket(bas);
                    break;
                default:
                    throw new NotImplementedException("LinkLayer of " + linkLayer + " is not implemented");
            }

            return p;
        }

        /// <summary>
        ///     Used to ensure that values like checksums and lengths are
        ///     properly updated
        /// </summary>
        protected void RecursivelyUpdateCalculatedValues()
        {
            // call the possibly overridden method
            this.UpdateCalculatedValues();

            // if the packet contains another packet, call its
            if (this.PayloadPacketOrData.Type == PayloadType.Packet)
            {
                this.PayloadPacketOrData.ThePacket.RecursivelyUpdateCalculatedValues();
            }
        }

        /// <summary>
        ///     Called to ensure that calculated values are updated before
        ///     the packet bytes are retrieved
        ///     Classes should override this method to update things like
        ///     checksums and lengths that take too much time or are too complex
        ///     to update for each packet parameter change
        /// </summary>
        public virtual void UpdateCalculatedValues()
        {
        }

        /// <summary>Output this packet as a readable string</summary>
        public override String ToString()
        {
            return this.ToString(StringOutputType.Normal);
        }

        /// <summary cref="Packet.ToString()">
        ///     Output the packet information in the specified format
        ///     Normal - outputs the packet info to a single line
        ///     Colored - outputs the packet info to a single line with coloring
        ///     Verbose - outputs detailed info about the packet
        ///     VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <param name="outputFormat">
        ///     <see cref="StringOutputType" />
        /// </param>
        public virtual String ToString(StringOutputType outputFormat)
        {
            if (this.PayloadPacketOrData.Type == PayloadType.Packet)
            {
                return this.PayloadPacketOrData.ThePacket.ToString(outputFormat);
            }

            return String.Empty;
        }

        /// <summary>
        ///     Prints the Packet PayloadData in Hex format
        ///     With the 16-byte segment number, raw bytes, and parsed ascii output
        ///     Ex:
        ///     0010  00 18 82 6c 7c 7f 00 c0  9f 77 a3 b0 88 64 11 00   ...1|... .w...d..
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" />
        /// </returns>
        public String PrintHex()
        {
            Byte[] data = this.BytesHighPerformance.Bytes;
            var buffer = new StringBuilder();
            String segmentNumber = "";
            String bytes = "";
            String ascii = "";

            buffer.AppendLine("Data:  ******* Raw Hex Output - length=" + data.Length + " bytes");
            buffer.AppendLine("Data: Segment:                   Bytes:                              Ascii:");
            buffer.AppendLine("Data: --------------------------------------------------------------------------");

            // parse the raw data
            for (Int32 i = 1; i <= data.Length; i++)
            {
                // add the current byte to the bytes hex string
                bytes += (data[i - 1].ToString("x")).PadLeft(2, '0') + " ";

                // add the current byte to the asciiBytes array for later processing
                if (data[i - 1] < 0x21 || data[i - 1] > 0x7e)
                {
                    ascii += ".";
                }
                else
                {
                    ascii += Encoding.ASCII.GetString(new Byte[1] {data[i - 1]});
                }

                // add an additional space to split the bytes into
                //  two groups of 8 bytes
                if (i % 16 != 0 && i % 8 == 0)
                {
                    bytes += " ";
                    ascii += " ";
                }

                // append the output string
                if (i % 16 == 0)
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
                if (i == data.Length)
                {
                    // add the 16 byte segment number
                    segmentNumber = (((((i - 16) / 16) + 1) * 10).ToString()).PadLeft(4, '0');

                    // build the line
                    buffer.AppendLine("Data: " + (segmentNumber).PadLeft(4, '0') + "  " + bytes.PadRight(49, ' ') +
                                      "  " + ascii);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        ///     Extract a packet of a specific type or null if a packet of the given type isn't found
        ///     NOTE: a 'dynamic' return type is possible here but costs ~7.8% in performance
        /// </summary>
        /// <param name='type'>
        ///     Type.
        /// </param>
        public Packet Extract(Type type)
        {
            var p = this;

            // search for a packet type that matches the given one
            do
            {
                if (type.IsInstanceOfType(p))
                {
                    return p;
                }

                // move to the PayloadPacket
                p = p.PayloadPacket;
            } while (p != null);

            return null;
        }

        /// <value>
        ///     Color used when generating the text description of a packet
        /// </value>
        public virtual String Color => AnsiEscapeSequences.Black;
    }
}