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
    public abstract class TcpPacket : Packet
    {
        public const int HeaderMinimumLength = 20; // 20 bytes is the smallest tcp header

        /// <summary> Fetch the port number on the source host.</summary>
        virtual public int SourcePort
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + TcpFields.SourcePortPosition);
            }

            set
            {
                var theValue = (Int16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.SourcePortPosition);
            }
        }

        /// <summary> Fetches the port number on the destination host.</summary>
        virtual public int DestinationPort
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + TcpFields.DestinationPortPosition);
            }

            set
            {
                var theValue = (Int16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.DestinationPortPosition);
            }
        }

        /// <summary> Fetch the packet sequence number.</summary>
        public int SequenceNumber
        {
            get
            {
                return EndianBitConverter.Big.ToInt32(header.Bytes,
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
        public int AcknowledgmentNumber
        {
            get
            {
                return EndianBitConverter.Big.ToInt32(header.Bytes,
                                                      header.Offset + TcpFields.AckNumberPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.AckNumberPosition);
            }
        }

        /// <summary> Fetch the TCP data offset in 32bit words.</summary>
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

#if false
        /// <summary> Fetches the packet TCP header length.</summary>
        override public int HeaderLength
        {
            get
            {
                return TCPHeaderLength;
            }
        }

        /// <summary> Fetches the length of the payload data.</summary>
        virtual public int PayloadDataLength
        {
            get
            {
                return (IPPayloadLength - TCPHeaderLength);
            }
        }
#endif

        /// <summary> Fetch the window size.</summary>
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

#if false
        /// <summary> Fetch the header checksum.</summary>
        /// <summary> Set the checksum of the TCP header</summary>
        /// <param name="cs">the checksum value
        /// </param>
        virtual public int TCPChecksum
        {
            get
            {
                return GetTransportLayerChecksum(_ipPayloadOffset + TCPFields_Fields.TCP_CSUM_POS);
            }

            set
            {
                base.SetTransportLayerChecksum(value, TCPFields_Fields.TCP_CSUM_POS);
            }
        }
#endif

#if false
        /// <summary> Check if the TCP packet is valid, checksum-wise.</summary>
        public override bool ValidChecksum
        {
            get
            {
                return ValidIPChecksum && ValidTCPChecksum;
            }

        }

        virtual public bool ValidTCPChecksum
        {
            get
            {
                return base.IsValidTransportLayerChecksum(true);
            }
        }
#endif

#if false
        /// <returns> The TCP packet length in bytes.  This is the size of the
        /// IP packet minus the size of the IP header.
        /// </returns>
        virtual public int TCPPacketByteLength
        {
            get
            {
                return IPPayloadLength;
            }
        }
#endif

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
            get
            {
                return (AllFlags & TcpFields.TCP_URG_MASK) != 0;
            }

            set
            {
                setFlag(value, TcpFields.TCP_URG_MASK);
            }
        }

        /// <summary> Check the ACK flag, flag indicates if the ack number is valid.</summary>
        virtual public bool Ack
        {
            get
            {
                return (AllFlags & TcpFields.TCP_ACK_MASK) != 0;
            }

            set
            {
                setFlag(value, TcpFields.TCP_ACK_MASK);
            }
        }

        /// <summary> Check the PSH flag, flag indicates the receiver should pass the
        /// data to the application as soon as possible.
        /// </summary>
        virtual public bool Psh
        {
            get
            {
                return (AllFlags & TcpFields.TCP_PSH_MASK) != 0;
            }

            set
            {
                setFlag(value, TcpFields.TCP_PSH_MASK);
            }
        }

        /// <summary> Check the RST flag, flag indicates the session should be reset between
        /// the sender and the receiver.
        /// </summary>
        virtual public bool Rst
        {
            get
            {
                return (AllFlags & TcpFields.TCP_RST_MASK) != 0;
            }

            set
            {
                setFlag(value, TcpFields.TCP_RST_MASK);
            }
        }

        /// <summary> Check the SYN flag, flag indicates the sequence numbers should
        /// be synchronized between the sender and receiver to initiate
        /// a connection.
        /// </summary>
        virtual public bool Syn
        {
            get
            {
                return (AllFlags & TcpFields.TCP_SYN_MASK) != 0;
            }

            set
            {
                setFlag(value, TcpFields.TCP_SYN_MASK);
            }
        }

        /// <summary> Check the FIN flag, flag indicates the sender is finished sending.</summary>
        virtual public bool Fin
        {
            get
            {
                return (AllFlags & TcpFields.TCP_FIN_MASK) != 0;
            }

            set
            {
                setFlag(value, TcpFields.TCP_FIN_MASK);
            }
        }

        virtual public bool ECN
        {
            get
            {
                return (AllFlags & TcpFields.TCP_ECN_MASK) != 0;
            }

            set
            {
                setFlag(value, TcpFields.TCP_ECN_MASK);
            }
        }

        virtual public bool CWR
        {
            get
            {
                return (AllFlags & TcpFields.TCP_CWR_MASK) != 0;
            }

            set
            {
                setFlag(value, TcpFields.TCP_CWR_MASK);
            }
        }

        private void setFlag(bool on, int MASK)
        {
            if (on)
                AllFlags = AllFlags | MASK;
            else
                AllFlags = AllFlags & ~MASK;
        }

#if false
        /// <summary> Fetch the TCP header a byte array.</summary>
        virtual public byte[] TCPHeader
        {
            get
            {
                if (_tcpHeaderBytes == null)
                {
                    _tcpHeaderBytes = PacketEncoding.extractHeader(_ipPayloadOffset, TCPHeaderLength, Bytes);
                }
                return _tcpHeaderBytes;
            }
        }

        /// <summary> Fetch the TCP header as a byte array.</summary>
        override public byte[] Header
        {
            get
            {
                return TCPHeader;
            }
        }

        /// <summary> Fetch the TCP data as a byte array.</summary>
        virtual public byte[] TCPData
        {
            get
            {
                if (_tcpDataBytes == null)
                {
                    _tcpDataBytes = new byte[PayloadDataLength];
                    Array.Copy(Bytes, _ipPayloadOffset + TCPHeaderLength, _tcpDataBytes, 0, PayloadDataLength);
                }
                return _tcpDataBytes;
            }
            set
            {
                SetData(value);
            }
        }
#endif

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.YELLOW;
            }
        }

        /// <summary> </summary>
        private const long serialVersionUID = 1L;

        /// <summary>
        /// Create a new TCP packet from values
        /// </summary>
        public TcpPacket(int sourcePort,
                         int destinationPort)
        {
#if false
            int tcpHeaderLength = TCPFields_Fields.TCP_HEADER_LEN;
            int tcpPayloadLength = (tcpPayload != null) ? tcpPayload.Length : 0;

            int totalBytesRequired = 0;
            totalBytesRequired += ipPacket.EthernetHeaderLength;
            totalBytesRequired += ipPacket.IPHeaderLength;
            totalBytesRequired += tcpHeaderLength;
            totalBytesRequired += tcpPayloadLength;

            byte[] newBytes = new byte[totalBytesRequired];

            // copy the contents of the ip packet in, excluding the ip payload
            // since this TCPPacket IS the new payload
            Array.Copy(ipPacket.Bytes, newBytes,
                       ipPacket.EthernetHeaderLength + ipPacket.IPHeaderLength);

            // update the buffer that this packet is overlayed upon
            this.Bytes = newBytes;

            // set the port values
            this.SourcePort = sourcePort;
            this.DestinationPort = destinationPort;

            // set the TCPHeaderLength
            this.TCPHeaderLength = tcpHeaderLength;

            // set the ippacket protocol to tcp
            this.IPProtocol = SharpPcap.Packets.IPProtocol.IPProtocolType.TCP;

            // copy the data payload in, if we were given one
            if(tcpPayload != null)
            {
                TCPData = tcpPayload;
            }
#endif
        }

#if false
        /// <summary> Create a new TCP packet.</summary>
        public TCPPacket(int byteOffsetToEthernetPayload, byte[] bytes)
            : this(byteOffsetToEthernetPayload, bytes, false)
        {
        }

        /// <summary> Create a new TCP packet.</summary>
        public TCPPacket(int byteOffsetToEthernetPayload, byte[] bytes, bool isEmpty)
            : base(byteOffsetToEthernetPayload, bytes)
        {
        }

        /// <summary> Create a new TCP packet.</summary>
        public TCPPacket(int byteOffsetToEthernetPayload, byte[] bytes, Timeval tv)
            : this(byteOffsetToEthernetPayload, bytes)
        {
            this._timeval = tv;
        }

        /// <summary> Fetch the header checksum.</summary>
        public int Checksum
        {
            get
            {
                return TCPChecksum;
            }
            set
            {
                TCPChecksum=value;
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
            return base.ComputeTransportLayerChecksum(TCPFields_Fields.TCP_CSUM_POS, update, true);
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
#endif

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

#if false
        /// <summary> Sets the data section of this tcp packet</summary>
        /// <param name="data">the data bytes
        /// </param>
        private void SetData(byte[] data)
        {
            //reset cached tcp data
            _tcpDataBytes = null;

            // the new packet is the length of the headers + the size of the TCPPacket data payload
            int headerLength = TCPHeaderLength + IPHeaderLength + EthernetHeaderLength;
            int newPacketLength = headerLength + data.Length;

            byte[] newPacketBytes = new byte[newPacketLength];

            // copy the headers into the new packet
            Array.Copy(Bytes, newPacketBytes, headerLength);

            // copy the data into the new packet, immediately after the headers
            Array.Copy(data, 0, newPacketBytes, headerLength, data.Length);

            // make the old headers and new data bytes the new packet bytes
            this.Bytes = newPacketBytes;

            // NOTE: TCPHeaderLength remains the same, we only updated the data portion
            // of the tcp packet

            //update ip total length
            IPPayloadLength = TCPHeaderLength + data.Length;

            //update also offset and pcap header
            OnOffsetChanged();
        }
#endif

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
                buffer.Append(AnsiEscapeSequences.RESET);
            buffer.Append(": ");
#if false
            buffer.Append(SourceAddress);
            buffer.Append('.');
#endif
            if(Enum.IsDefined(typeof(IpPort), SourcePort))
            {
                buffer.Append((IpPort)SourcePort);
            } else
            {
                buffer.Append(SourcePort);
            }
            buffer.Append(" -> ");
#if false
            buffer.Append(DestinationAddress);
            buffer.Append('.');
#endif
            if(Enum.IsDefined(typeof(IpPort), DestinationPort))
            {
                buffer.Append((IpPort)DestinationPort);
            } else
            {
                buffer.Append(DestinationPort);
            }
            if (Urg)
                buffer.Append(" urg[0x" + System.Convert.ToString(UrgentPointer, 16) + "]");
            if (Ack)
                buffer.Append(" ack[0x" + System.Convert.ToString(AcknowledgmentNumber, 16) + "]");
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
                buffer.Append(AnsiEscapeSequences.RESET);
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
    }
}
