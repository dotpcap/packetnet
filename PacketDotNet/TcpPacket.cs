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

﻿using System;
using System.Collections.Generic;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// TcpPacket
    /// See: http://en.wikipedia.org/wiki/Transmission_Control_Protocol
    /// </summary>
    public class TcpPacket : TransportPacket
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

        /// <value>
        /// 20 bytes is the smallest tcp header
        /// </value>
        public const int HeaderMinimumLength = 20;

        /// <summary> Fetch the port number on the source host.</summary>
        virtual public ushort SourcePort
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + TcpFields.SourcePortPosition);
            }

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.SourcePortPosition);
            }
        }

        /// <summary> Fetches the port number on the destination host.</summary>
        virtual public ushort DestinationPort
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + TcpFields.DestinationPortPosition);
            }

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.DestinationPortPosition);
            }
        }

        /// <summary> Fetch the packet sequence number.</summary>
        public uint SequenceNumber
        {
            get
            {
                return EndianBitConverter.Big.ToUInt32(header.Bytes,
                                                       header.Offset + TcpFields.SequenceNumberPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.SequenceNumberPosition);
            }
        }

        /// <summary> Fetch the packet acknowledgment number.</summary>
        public uint AcknowledgmentNumber
        {
            get
            {
                return EndianBitConverter.Big.ToUInt32(header.Bytes,
                                                       header.Offset + TcpFields.AckNumberPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.AckNumberPosition);
            }
        }

        /// <summary> The size of the tcp header in 32bit words </summary>
        virtual public int DataOffset
        {
            get
            {
                var theByte = header.Bytes[header.Offset + TcpFields.DataOffsetPosition];
                return (theByte >> 4) & 0xF;
            }

            set
            {
                // read the original value
                var theByte = header.Bytes[header.Offset + TcpFields.DataOffsetPosition];

                // mask in the data offset value
                theByte = (byte)((theByte & 0x0F) | ((value << 4) & 0xF0));

                // write the value back
                header.Bytes[header.Offset + TcpFields.DataOffsetPosition] = theByte;
            }
        }

        /// <summary>
        /// The size of the receive window, which specifies the number of
        /// bytes (beyond the sequence number in the acknowledgment field) that
        /// the receiver is currently willing to receive.
        /// </summary>
        virtual public int WindowSize
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + TcpFields.WindowSizePosition);
            }

            set
            {
                var theValue = (Int16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.WindowSizePosition);
            }
        }

        /// <value>
        /// Tcp checksum field value of type UInt16
        /// </value>
        virtual public ushort Checksum
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                       header.Offset + TcpFields.ChecksumPosition);
            }

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.ChecksumPosition);
            }
        }

        /// <summary> Check if the TCP packet is valid, checksum-wise.</summary>
        public bool ValidChecksum
        {
            get
            {
                // IPv6 has no checksum so only the TCP checksum needs evaluation
                if (parentPacket.GetType() == typeof(IPv6Packet))
                    return ValidTCPChecksum;
                // For IPv4 both the IP layer and the TCP layer contain checksums 
                else
                    return ((IPv4Packet)ParentPacket).ValidIPChecksum && ValidTCPChecksum;
            }
        }

        /// <value>
        /// True if the tcp checksum is valid
        /// </value>
        virtual public bool ValidTCPChecksum
        {
            get
            {
                log.Debug("ValidTCPChecksum");
                var retval = ((IpPacket)ParentPacket).IsValidTransportLayerChecksum(IpPacket.TransportChecksumOption.AttachPseudoIPHeader);
                log.DebugFormat("ValidTCPChecksum {0}", retval);
                return retval;
            }
        }

        private int AllFlags
        {
            get
            {
                return header.Bytes[header.Offset + TcpFields.FlagsPosition];
            }

            set
            {
                header.Bytes[header.Offset + TcpFields.FlagsPosition] = (byte)value;
            }
        }

        /// <summary> Check the URG flag, flag indicates if the urgent pointer is valid.</summary>
        virtual public bool Urg
        {
            get { return (AllFlags & TcpFields.TCP_URG_MASK) != 0; }
            set { setFlag(value, TcpFields.TCP_URG_MASK); }
        }

        /// <summary> Check the ACK flag, flag indicates if the ack number is valid.</summary>
        virtual public bool Ack
        {
            get { return (AllFlags & TcpFields.TCP_ACK_MASK) != 0; }
            set { setFlag(value, TcpFields.TCP_ACK_MASK); }
        }

        /// <summary> Check the PSH flag, flag indicates the receiver should pass the
        /// data to the application as soon as possible.
        /// </summary>
        virtual public bool Psh
        {
            get { return (AllFlags & TcpFields.TCP_PSH_MASK) != 0; }
            set { setFlag(value, TcpFields.TCP_PSH_MASK); }
        }

        /// <summary> Check the RST flag, flag indicates the session should be reset between
        /// the sender and the receiver.
        /// </summary>
        virtual public bool Rst
        {
            get { return (AllFlags & TcpFields.TCP_RST_MASK) != 0; }
            set { setFlag(value, TcpFields.TCP_RST_MASK); }
        }

        /// <summary> Check the SYN flag, flag indicates the sequence numbers should
        /// be synchronized between the sender and receiver to initiate
        /// a connection.
        /// </summary>
        virtual public bool Syn
        {
            get { return (AllFlags & TcpFields.TCP_SYN_MASK) != 0; }
            set { setFlag(value, TcpFields.TCP_SYN_MASK); }
        }

        /// <summary> Check the FIN flag, flag indicates the sender is finished sending.</summary>
        virtual public bool Fin
        {
            get { return (AllFlags & TcpFields.TCP_FIN_MASK) != 0; }
            set { setFlag(value, TcpFields.TCP_FIN_MASK); }
        }

        /// <value>
        /// ECN flag
        /// </value>
        virtual public bool ECN
        {
            get { return (AllFlags & TcpFields.TCP_ECN_MASK) != 0; }
            set { setFlag(value, TcpFields.TCP_ECN_MASK); }
        }

        /// <value>
        /// CWR flag
        /// </value>
        virtual public bool CWR
        {
            get { return (AllFlags & TcpFields.TCP_CWR_MASK) != 0; }
            set { setFlag(value, TcpFields.TCP_CWR_MASK); }
        }

        private void setFlag(bool on, int MASK)
        {
            if (on)
                AllFlags = AllFlags | MASK;
            else
                AllFlags = AllFlags & ~MASK;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.Yellow;
            }
        }

        /// <summary>
        /// Create a new TCP packet from values
        /// </summary>
        public TcpPacket(ushort SourcePort,
                         ushort DestinationPort) : base(new PosixTimeval())
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = TcpFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArrayAndOffset(headerBytes, offset, length);

            // make this packet valid
            DataOffset = length / 4;

            // set instance values
            this.SourcePort = SourcePort;
            this.DestinationPort = DestinationPort;
        }

        /// <summary>
        /// byte[]/int offset constructor, timeval defaults to the current time
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public TcpPacket(byte[] Bytes, int Offset) :
            this(Bytes, Offset, new PosixTimeval())
        {
            log.Debug("");
        }

        /// <summary>
        /// byte[]/int offset/PosixTimeval constructor
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        public TcpPacket(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            header = new ByteArrayAndOffset(Bytes, Offset, Bytes.Length - Offset);

            // NOTE: we update the Length field AFTER the header field because
            // we need the header to be valid to retrieve the value of DataOffset
            header.Length = DataOffset * 4;

            // store the payload bytes
            payloadPacketOrData = new PacketOrByteArray();
            payloadPacketOrData.TheByteArray = header.EncapsulatedBytes();
        }

        /// <summary>
        /// Constructor when this packet is encapsulated in another packet
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="ParentPacket">
        /// A <see cref="Packet"/>
        /// </param>
        public TcpPacket(byte[] Bytes, int Offset, PosixTimeval Timeval,
                         Packet ParentPacket) :
            this(Bytes, Offset, Timeval)
        {
            log.DebugFormat("ParentPacket.GetType() {0}", ParentPacket.GetType());

            this.ParentPacket = ParentPacket;

            // if the parent packet is an IPv4Packet we need to adjust
            // the payload length because it is possible for us to have
            // X bytes of data but only (X - Y) bytes are actually valid
            if(this.ParentPacket is IPv4Packet)
            {
                // actual total length (tcp header + tcp payload)
                var ipv4Parent = (IPv4Packet)this.ParentPacket;
                var ipPayloadTotalLength = ipv4Parent.TotalLength - (ipv4Parent.HeaderLength * 4);

                log.DebugFormat("ipv4Parent.TotalLength {0}, ipv4Parent.HeaderLength {1}",
                                ipv4Parent.TotalLength,
                                ipv4Parent.HeaderLength * 4);

                var newTcpPayloadLength = ipPayloadTotalLength - this.Header.Length;

                log.DebugFormat("Header.Length {0}, Current payload length: {1}, new payload length {2}",
                                this.header.Length,
                                payloadPacketOrData.TheByteArray.Length,
                                newTcpPayloadLength);

                // the length of the payload is the total payload length
                // above, minus the length of the tcp header
                payloadPacketOrData.TheByteArray.Length = newTcpPayloadLength;
            }
        }

        /// <summary> Computes the TCP checksum, optionally updating the TCP checksum header.
        /// 
        /// </summary>
        /// <param name="update">Specifies whether or not to update the TCP checksum header
        /// after computing the checksum. A value of true indicates the
        /// header should be updated, a value of false indicates it should
        /// not be updated.
        /// </param>
        /// <returns> The computed TCP checksum.
        /// </returns>
        public int ComputeTCPChecksum(bool update)
        {
            if(update == true)
                throw new System.NotImplementedException();

            var newChecksum = ((IpPacket)ParentPacket).ComputeTransportLayerChecksum(TcpFields.ChecksumPosition, true);
            return newChecksum;
        }

        /// <summary> Same as <code>computeTCPChecksum(true);</code>
        /// 
        /// </summary>
        /// <returns> The computed TCP checksum value.
        /// </returns>
        public int ComputeTCPChecksum()
        {
            return ComputeTCPChecksum(true);
        }

        /// <summary> Fetch the urgent pointer.</summary>
        public int UrgentPointer
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + TcpFields.UrgentPointerPosition);
            }

            set
            {
                var theValue = (Int16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.UrgentPointerPosition);
            }
        }

#pragma warning disable 1591
        public enum OptionTypes
        {
            EndOfList = 0x0,
            Nop = 0x1,
            MaximumSegmentSize = 0x2,
            WindowScale = 0x3,
            SelectiveAckSupported = 0x4,
            Unknown5 = 0x5,
            Unknown6 = 0x6,
            Unknown7 = 0x7,
            Timestamp = 0x8 // http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_Timestamps
        }
#pragma warning restore 1591

        /// <summary>
        /// Bytes that represent the tcp options
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public byte[] Options
        {
            get
            {
                if(Urg)
                {
                    throw new System.NotImplementedException("Urg == true not implemented yet");
                }

                int optionsOffset = TcpFields.UrgentPointerPosition + TcpFields.UrgentPointerLength;
                int optionsLength = (DataOffset * 4) - optionsOffset;

                byte[] optionBytes = new byte[optionsLength];
                Array.Copy(header.Bytes, header.Offset + optionsOffset,
                           optionBytes, 0,
                           optionsLength);

                return optionBytes;
            }
        }

        /// <summary> Convert this TCP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this TCP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("TCPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences.Reset);
            buffer.Append(": ");
            buffer.Append(" SourcePort: ");
            if(Enum.IsDefined(typeof(IpPort), (ushort)SourcePort))
            {
                buffer.Append((IpPort)SourcePort);
                buffer.Append(" (" + SourcePort + ") ");
            } else
            {
                buffer.Append(SourcePort);
            }
            buffer.Append(" -> ");
            buffer.Append(" DestinationPort: ");
            if(Enum.IsDefined(typeof(IpPort), (ushort)DestinationPort))
            {
                buffer.Append((IpPort)DestinationPort);
                buffer.Append(" (" + DestinationPort + ") ");
            } else
            {
                buffer.Append(DestinationPort);
            }

            if (Urg)
                buffer.Append(" urg[0x" + System.Convert.ToString(UrgentPointer, 16) + "]");
            if (Ack)
                buffer.Append(" ack[" + AcknowledgmentNumber + " (0x" + System.Convert.ToString(AcknowledgmentNumber, 16) + ")]");
            if (Psh)
                buffer.Append(" psh");
            if (Rst)
                buffer.Append(" rst");
            if (Syn)
                buffer.Append(" syn[0x" + System.Convert.ToString(SequenceNumber, 16) + "," +
                              SequenceNumber + "]");
            if (Fin)
                buffer.Append(" fin");

            //FIXME: not sure what to put here
//            buffer.Append(" l=" + TCPHeaderLength + "," + PayloadDataLength);
            buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary> Convert this TCP packet to a verbose.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("TCPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences.Reset);
            buffer.Append(": ");
            buffer.Append("sport=" + SourcePort + ", ");
            buffer.Append("dport=" + DestinationPort + ", ");
            buffer.Append("seqn=0x" + System.Convert.ToString(SequenceNumber, 16) + ", ");
            buffer.Append("ackn=0x" + System.Convert.ToString(AcknowledgmentNumber, 16) + ", ");
            //FIXME: what is header length now?
//            buffer.Append("hlen=" + HeaderLength + ", ");
            buffer.Append("urg=" + Urg + ", ");
            buffer.Append("ack=" + Ack + ", ");
            buffer.Append("psh=" + Psh + ", ");
            buffer.Append("rst=" + Rst + ", ");
            buffer.Append("syn=" + Syn + ", ");
            buffer.Append("fin=" + Fin + ", ");
            buffer.Append("wsize=" + WindowSize + ", ");
            //FIXME: probably want to fix this one
//            buffer.Append("sum=0x" + System.Convert.ToString(Checksum, 16));
#if false
            if (this.ValidTCPChecksum)
                buffer.Append(" (correct), ");
            else
                buffer.Append(" (incorrect, should be " + ComputeTCPChecksum(false) + "), ");
#endif
            buffer.Append("uptr=0x" + System.Convert.ToString(UrgentPointer, 16));
            buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredVerboseString(colored));

            return buffer.ToString();
        }

        /// <summary>
        /// Returns the TcpPacket embedded in Packet p or null if
        /// there is no embedded TcpPacket
        /// </summary>
        public static TcpPacket GetType(Packet p)
        {
            if(p is InternetLinkLayerPacket)
            {
                var payload = InternetLinkLayerPacket.GetInnerPayload((InternetLinkLayerPacket)p);
                if(payload is IpPacket)
                {
                    var innerPayload = payload.PayloadPacket;
                    if(innerPayload is TcpPacket)
                    {
                        return (TcpPacket)innerPayload;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Create a randomized tcp packet with the given ip version
        /// </summary>
        /// <returns>
        /// A <see cref="Packet"/>
        /// </returns>
        public static TcpPacket RandomPacket()
        {
            var rnd = new Random();

            // create a randomized TcpPacket
            var srcPort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);
            var dstPort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);
            var tcpPacket = new TcpPacket(srcPort, dstPort);

            return tcpPacket;
        }
    }
}
