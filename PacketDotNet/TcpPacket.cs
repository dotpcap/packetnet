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
using System.Text;
using System.Collections.Generic;
using MiscUtil.Conversion;
using PacketDotNet.Tcp;
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
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
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
        virtual public UInt16 WindowSize
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + TcpFields.WindowSizePosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + TcpFields.WindowSizePosition);
            }
        }

        /// <value>
        /// Tcp checksum field value of type UInt16
        /// </value>
        override public ushort Checksum
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
                if (ParentPacket.GetType() == typeof(IPv6Packet))
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
                var retval = IsValidChecksum(TransportPacket.TransportChecksumOption.AttachPseudoIPHeader);
                log.DebugFormat("ValidTCPChecksum {0}", retval);
                return retval;
            }
        }

        /// <summary>
        /// Flags, 9 bits
        /// TODO: Handle the NS bit
        /// </summary>
        public byte AllFlags
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
                AllFlags = (byte)(AllFlags | MASK);
            else
                AllFlags = (byte)(AllFlags & ~MASK);
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
                         ushort DestinationPort)
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = TcpFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            // make this packet valid
            DataOffset = length / 4;

            // set instance values
            this.SourcePort = SourcePort;
            this.DestinationPort = DestinationPort;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public TcpPacket(ByteArraySegment bas)
        {
            log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            header = new ByteArraySegment(bas);

            // NOTE: we update the Length field AFTER the header field because
            // we need the header to be valid to retrieve the value of DataOffset
            header.Length = DataOffset * 4;

            // store the payload bytes
            payloadPacketOrData = new PacketOrByteArraySegment();
            payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        /// <param name="ParentPacket">
        /// A <see cref="Packet"/>
        /// </param>
        public TcpPacket(ByteArraySegment bas,
                         Packet ParentPacket) :
            this(bas)
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
                                payloadPacketOrData.TheByteArraySegment.Length,
                                newTcpPayloadLength);

                // the length of the payload is the total payload length
                // above, minus the length of the tcp header
                payloadPacketOrData.TheByteArraySegment.Length = newTcpPayloadLength;
            }
        }

        /// <summary>
        /// Computes the TCP checksum. Does not update the current checksum value
        /// </summary>
        /// <returns> The calculated TCP checksum.</returns>
        public int CalculateTCPChecksum()
        {
            var newChecksum = CalculateChecksum(TransportChecksumOption.AttachPseudoIPHeader);
            return newChecksum;
        }

        /// <summary>
        /// Update the checksum value.
        /// </summary>
        public void UpdateTCPChecksum()
        {
            log.Debug("");
            this.Checksum = (ushort)CalculateTCPChecksum();
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

        /// <summary>
        /// Parses options, pointed to by optionBytes into an array of Options
        /// </summary>
        /// <param name="optionBytes">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <returns>
        /// A <see cref="List<Option>"/>
        /// </returns>
        private List<Option> ParseOptions(byte[] optionBytes)
        {
            int offset = 0;
            OptionTypes type;
            byte length;

            if(optionBytes.Length == 0)
                return null;

            // reset the OptionsCollection list to prepare
            //  to be re-populated with new data
            var retval = new List<Option>();

            while(offset < optionBytes.Length)
            {
                type = (OptionTypes)optionBytes[offset + Option.KindFieldOffset];

                // some options have no length field, we cannot read
                // the length field if it isn't present or we risk
                // out-of-bounds issues
                if((type == OptionTypes.EndOfOptionList) ||
                   (type == OptionTypes.NoOperation))
                {
                    length = 1;
                } else
                {
                    length = optionBytes[offset + Option.LengthFieldOffset];
                }

                switch(type)
                {
                    case OptionTypes.EndOfOptionList:
                        retval.Add(new EndOfOptions(optionBytes, offset, length));
                        offset += EndOfOptions.OptionLength;
                        break;
                    case OptionTypes.NoOperation:
                        retval.Add(new NoOperation(optionBytes, offset, length));
                        offset += NoOperation.OptionLength;
                        break;
                    case OptionTypes.MaximumSegmentSize:
                        retval.Add(new MaximumSegmentSize(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.WindowScaleFactor:
                        retval.Add(new WindowScaleFactor(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.SACKPermitted:
                        retval.Add(new SACKPermitted(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.SACK:
                        retval.Add(new SACK(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.Echo:
                        retval.Add(new Echo(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.EchoReply:
                        retval.Add(new EchoReply(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.Timestamp:
                        retval.Add(new TimeStamp(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.AlternateChecksumRequest:
                        retval.Add(new AlternateChecksumRequest(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.AlternateChecksumData:
                        retval.Add(new AlternateChecksumData(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.MD5Signature:
                        retval.Add(new MD5Signature(optionBytes, offset, length));
                        offset += length;
                        break;
                    case OptionTypes.UserTimeout:
                        retval.Add(new UserTimeout(optionBytes, offset, length));
                        offset += length;
                        break;
                    // these fields aren't supported because they're still considered
                    //  experimental in their respecive RFC specifications
                    case OptionTypes.POConnectionPermitted:
                    case OptionTypes.POServiceProfile:
                    case OptionTypes.ConnectionCount:
                    case OptionTypes.ConnectionCountNew:
                    case OptionTypes.ConnectionCountEcho:
                    case OptionTypes.QuickStartResponse:
                        throw new NotSupportedException("Option: " + type.ToString() + " is not supported because its RFC specification is still experimental");
                    // add more options types here
                    default:
                        throw new NotImplementedException("Option: " + type.ToString() + " not supported in Packet.Net yet");
                }
            }

            return retval;
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build flagstring
                string flags = "{";
                if (Urg)
                    flags += "urg[0x" + System.Convert.ToString(UrgentPointer, 16) + "]|";
                if (Ack)
                    flags += "ack[" + AcknowledgmentNumber + " (0x" + System.Convert.ToString(AcknowledgmentNumber, 16) + ")]|";
                if (Psh)
                    flags += "psh|";
                if (Rst)
                    flags += "rst|";
                if (Syn)
                    flags += "syn[0x" + System.Convert.ToString(SequenceNumber, 16) + "," + SequenceNumber + "]|";
                flags = flags.TrimEnd('|');
                flags += "}";

                // build the output string
                buffer.AppendFormat("{0}[TCPPacket: SourcePort={2}, DestinationPort={3}, Flags={4}]{1}",
                    color,
                    colorEscape,
                    SourcePort,
                    DestinationPort,
                    flags);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("source port", SourcePort.ToString());
                properties.Add("destination port", DestinationPort.ToString());
                properties.Add("sequence number", SequenceNumber.ToString() + " (0x" + SequenceNumber.ToString("x") + ")");
                properties.Add("acknowledgement number", AcknowledgmentNumber.ToString() + " (0x" + AcknowledgmentNumber.ToString("x") + ")");
                // TODO: Implement a HeaderLength property for TCPPacket
                //properties.Add("header length", HeaderLength.ToString());
                properties.Add("flags", "(0x" + AllFlags.ToString("x") + ")");
                string flags = Convert.ToString(AllFlags, 2).PadLeft(8, '0');
                properties.Add("", flags[0] + "... .... = [" + flags[0] + "] congestion window reduced");
                properties.Add(" ", "." + flags[1] + ".. .... = [" + flags[1] + "] ECN - echo");
                properties.Add("  ", ".." + flags[2] + ". .... = [" + flags[2] + "] urgent");
                properties.Add("   ", "..." + flags[3] + " .... = [" + flags[3] + "] acknowledgement");
                properties.Add("    ", ".... " + flags[4] + "... = [" + flags[4] + "] push");
                properties.Add("     ", ".... ." + flags[5] + ".. = [" + flags[5] + "] reset");
                properties.Add("      ", ".... .."+ flags[6] + ". = [" + flags[6] + "] syn");
                properties.Add("       ", ".... ..." + flags[7] + " = [" + flags[7] + "] fin");
                properties.Add("window size", WindowSize.ToString());
                properties.Add("checksum", "0x" + Checksum.ToString() + " [" + (ValidChecksum ? "valid" : "invalid") + "]");
                properties.Add("options", "0x" + BitConverter.ToString(Options).Replace("-", "").PadLeft(12, '0'));
                var parsedOptions = OptionsCollection;
                if(parsedOptions != null)
                {
                    for(int i = 0; i < parsedOptions.Count; i++)
                    {
                        properties.Add("option" + (i + 1).ToString(), parsedOptions[i].ToString());
                    }
                }

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("TCP:  ******* TCP - \"Transmission Control Protocol\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("TCP:");
                foreach(var property in properties)
                {
                    if(property.Key.Trim() != "")
                    {
                        buffer.AppendLine("TCP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }
                    else
                    {
                        buffer.AppendLine("TCP: " + property.Key.PadLeft(padLength) + "   " + property.Value);
                    }
                }
                buffer.AppendLine("TCP:");
            }

            // append the base class output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Returns the TcpPacket embedded in Packet p or null if
        /// there is no embedded TcpPacket
        /// </summary>
        [Obsolete("Use Packet.Extract() instead")]
        public static TcpPacket GetEncapsulated(Packet p)
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

        /// <summary>
        /// Contains the Options list attached to the TCP header
        /// </summary>
        public List<Option> OptionsCollection
        {
            get
            {
                // evaluates the options field and generates a list of
                //  attached options
                return ParseOptions(this.Options);
            }
        }
    }
}
