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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Tcp;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// TcpPacket
    /// See: http://en.wikipedia.org/wiki/Transmission_Control_Protocol
    /// </summary>
    [Serializable]
    public sealed class TcpPacket : TransportPacket
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <value>
        /// 20 bytes is the smallest tcp header
        /// </value>
        public const Int32 HeaderMinimumLength = 20;

        /// <summary> Fetch the port number on the source host.</summary>
        public UInt16 SourcePort
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.SourcePortPosition);

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + TcpFields.SourcePortPosition);
            }
        }

        /// <summary> Fetches the port number on the destination host.</summary>
        public UInt16 DestinationPort
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.DestinationPortPosition);

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + TcpFields.DestinationPortPosition);
            }
        }

        /// <summary> Fetch the packet sequence number.</summary>
        public UInt32 SequenceNumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes,
                                                   Header.Offset + TcpFields.SequenceNumberPosition);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + TcpFields.SequenceNumberPosition);
        }

        /// <summary> Fetch the packet acknowledgment number.</summary>
        public UInt32 AcknowledgmentNumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes,
                                                   Header.Offset + TcpFields.AckNumberPosition);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + TcpFields.AckNumberPosition);
        }

        private UInt16 DataOffsetAndFlags
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.DataOffsetAndFlagsPosition);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + TcpFields.DataOffsetAndFlagsPosition);
        }

        /// <summary> The size of the tcp header in 32bit words </summary>
        public Int32 DataOffset
        {
            get
            {
                var dataOffset = (Byte) ((DataOffsetAndFlags >> 12) & 0xF);
                return dataOffset;
            }

            set
            {
                var dataOffset = DataOffsetAndFlags;

                dataOffset = (UInt16) ((dataOffset & 0x0FFF) | ((value << 12) & 0xF000));

                // write the value back
                DataOffsetAndFlags = dataOffset;
            }
        }

        /// <summary>
        /// The size of the receive window, which specifies the number of
        /// bytes (beyond the sequence number in the acknowledgment field) that
        /// the receiver is currently willing to receive.
        /// </summary>
        public UInt16 WindowSize
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.WindowSizePosition);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + TcpFields.WindowSizePosition);
        }

        /// <value>
        /// Tcp checksum field value of type UInt16
        /// </value>
        public override UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.ChecksumPosition);

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + TcpFields.ChecksumPosition);
            }
        }

        /// <summary> Check if the TCP packet is valid, checksum-wise.</summary>
        public Boolean ValidChecksum
        {
            get
            {
                // IPv6 has no checksum so only the TCP checksum needs evaluation
                if (ParentPacket is IPv6Packet)
                    return ValidTCPChecksum;


                // For IPv4 both the IP layer and the TCP layer contain checksums
                return ((IPv4Packet) ParentPacket).ValidIPChecksum && ValidTCPChecksum;
            }
        }

        /// <value>
        /// True if the tcp checksum is valid
        /// </value>
        public Boolean ValidTCPChecksum
        {
            get
            {
                Log.Debug("ValidTCPChecksum");
                var retval = IsValidChecksum(TransportChecksumOption.IncludePseudoIPHeader);
                Log.DebugFormat("ValidTCPChecksum {0}", retval);
                return retval;
            }
        }

        /// <summary>
        /// Flags, 9 bits
        /// </summary>
        public UInt16 AllFlags
        {
            get
            {
                var flags = DataOffsetAndFlags & 0x1FF;
                return (UInt16) flags;
            }

            set
            {
                var flags = DataOffsetAndFlags;

                flags = (UInt16) ((flags & 0xFE00) | (value & 0x1FF));
                DataOffsetAndFlags = flags;
            }
        }

        /// <summary> Check the URG flag, flag indicates if the urgent pointer is valid.</summary>
        public Boolean Urg
        {
            get => (AllFlags & TcpFields.TCPUrgMask) != 0;
            set => SetFlag(value, TcpFields.TCPUrgMask);
        }

        /// <summary> Check the ACK flag, flag indicates if the ack number is valid.</summary>
        public Boolean Ack
        {
            get => (AllFlags & TcpFields.TCPAckMask) != 0;
            set => SetFlag(value, TcpFields.TCPAckMask);
        }

        /// <summary>
        /// Check the PSH flag, flag indicates the receiver should pass the
        /// data to the application as soon as possible.
        /// </summary>
        public Boolean Psh
        {
            get => (AllFlags & TcpFields.TCPPshMask) != 0;
            set => SetFlag(value, TcpFields.TCPPshMask);
        }

        /// <summary>
        /// Check the RST flag, flag indicates the session should be reset between
        /// the sender and the receiver.
        /// </summary>
        public Boolean Rst
        {
            get => (AllFlags & TcpFields.TCPRstMask) != 0;
            set => SetFlag(value, TcpFields.TCPRstMask);
        }

        /// <summary>
        /// Check the SYN flag, flag indicates the sequence numbers should
        /// be synchronized between the sender and receiver to initiate
        /// a connection.
        /// </summary>
        public Boolean Syn
        {
            get => (AllFlags & TcpFields.TCPSynMask) != 0;
            set => SetFlag(value, TcpFields.TCPSynMask);
        }

        /// <summary> Check the FIN flag, flag indicates the sender is finished sending.</summary>
        public Boolean Fin
        {
            get => (AllFlags & TcpFields.TCPFinMask) != 0;
            set => SetFlag(value, TcpFields.TCPFinMask);
        }

        /// <value>
        /// ECN flag
        /// </value>
        public Boolean ECN
        {
            get => (AllFlags & TcpFields.TCPEcnMask) != 0;
            set => SetFlag(value, TcpFields.TCPEcnMask);
        }

        /// <value>
        /// CWR flag
        /// </value>
        public Boolean CWR
        {
            get => (AllFlags & TcpFields.TCPCwrMask) != 0;
            set => SetFlag(value, TcpFields.TCPCwrMask);
        }

        /// <value>
        /// NS flag
        /// </value>
        public Boolean NS
        {
            get => (AllFlags & TcpFields.TCPNsMask) != 0;
            set => SetFlag(value, TcpFields.TCPNsMask);
        }

        private void SetFlag(Boolean on, Int32 mask)
        {
            if (on)
                AllFlags = (UInt16) (AllFlags | mask);
            else
                AllFlags = (UInt16) (AllFlags & ~mask);
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.Yellow;

        /// <summary>
        /// Create a new TCP packet from values
        /// </summary>
        public TcpPacket
        (
            UInt16 sourcePort,
            UInt16 destinationPort)
        {
            Log.Debug("");

            // allocate memory for this packet
            const int offset = 0;

            var length = TcpFields.HeaderLength;
            var headerBytes = new Byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

            // make this packet valid
            DataOffset = length / 4;

            // set instance values
            SourcePort = sourcePort;
            DestinationPort = destinationPort;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public TcpPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = DataOffset * 4;

            // NOTE: we update the Length field AFTER the header field because
            // we need the header to be valid to retrieve the value of DataOffset

            // store the payload bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment {ByteArraySegment = Header.EncapsulatedBytes()};
                return result;
            });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public TcpPacket
        (
            ByteArraySegment bas,
            Packet parentPacket)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);

            // NOTE: we update the Length field AFTER the header field because
            // we need the header to be valid to retrieve the value of DataOffset
            Header.Length = DataOffset * 4;

            // store the payload bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment {ByteArraySegment = Header.EncapsulatedBytes()};

                // if the parent packet is an IPv4Packet we need to adjust
                // the payload length because it is possible for us to have
                // X bytes of data but only (X - Y) bytes are actually valid
                if (ParentPacket is IPv4Packet ipv4Parent)
                {
                    // actual total length (tcp header + tcp payload)
                    var ipPayloadTotalLength = ipv4Parent.TotalLength - (ipv4Parent.HeaderLength * 4);

                    Log.DebugFormat("ipv4Parent.TotalLength {0}, ipv4Parent.HeaderLength {1}",
                                    ipv4Parent.TotalLength,
                                    ipv4Parent.HeaderLength * 4);

                    var newTcpPayloadLength = ipPayloadTotalLength - Header.Length;

                    Log.DebugFormat("Header.Length {0}, Current payload length: {1}, new payload length {2}",
                                    Header.Length,
                                    result.ByteArraySegment.Length,
                                    newTcpPayloadLength);

                    // the length of the payload is the total payload length
                    // above, minus the length of the tcp header
                    result.ByteArraySegment.Length = newTcpPayloadLength;
                    DecodePayload(result);
                }

                return result;
            });

            Log.DebugFormat("ParentPacket.GetType() {0}", parentPacket.GetType());

            ParentPacket = parentPacket;
        }

        /// <summary>
        /// Decode Payload to Support Drda procotol
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public TcpPacket DecodePayload(PacketOrByteArraySegment result)
        {
            if (result.ByteArraySegment == null || result.ByteArraySegment.Length < DrdaDDMFields.DDMHeadTotalLength)
                return this;


            // Based on https://github.com/wireshark/wireshark/blob/fe219637a6748130266a0b0278166046e60a2d68/epan/dissectors/packet-drda.c#L757.



            // The first header is 6 bytes long, so the length in the second header should be 6 bytes less.
            if (result.ByteArraySegment.Bytes[result.ByteArraySegment.Offset + 2] == 0xD0)
            {
                var outerLength = EndianBitConverter.Big.ToUInt16(result.ByteArraySegment.Bytes,
                                                                  result.ByteArraySegment.Offset + 0);
                var innerLength = EndianBitConverter.Big.ToUInt16(result.ByteArraySegment.Bytes,
                                                                  result.ByteArraySegment.Offset + 6);
                if (outerLength - innerLength == 6)
                {
                    var drdaPacket = new DrdaPacket(result.ByteArraySegment, this);
                    result.Packet = drdaPacket;
                }
            }

            return this;
        }

        /// <summary>
        /// Computes the TCP checksum. Does not update the current checksum value
        /// </summary>
        /// <returns> The calculated TCP checksum.</returns>
        public UInt16 CalculateTCPChecksum()
        {
            return (ushort) CalculateChecksum(TransportChecksumOption.IncludePseudoIPHeader);
        }

        /// <summary>
        /// Update the checksum value.
        /// </summary>
        public void UpdateTCPChecksum()
        {
            Checksum = CalculateTCPChecksum();
        }

        /// <summary> Fetch the urgent pointer.</summary>
        public Int32 UrgentPointer
        {
            get => EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                  Header.Offset + TcpFields.UrgentPointerPosition);

            set
            {
                var theValue = (Int16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + TcpFields.UrgentPointerPosition);
            }
        }

        /// <summary>
        /// Bytes that represent the tcp options
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public Byte[] Options
        {
            get
            {
                if (Urg)
                {
                    throw new NotImplementedException("Urg == true not implemented yet");
                }

                var optionsOffset = TcpFields.UrgentPointerPosition + TcpFields.UrgentPointerLength;
                var optionsLength = (DataOffset * 4) - optionsOffset;

                var optionBytes = new Byte[optionsLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + optionsOffset,
                           optionBytes,
                           0,
                           optionsLength);

                return optionBytes;
            }
        }

        /// <summary>
        /// Parses options, pointed to by optionBytes into an array of Options
        /// </summary>
        /// <param name="optionBytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <returns>
        /// A <see cref="List&lt;Option&gt;" />
        /// </returns>
        private List<Option> ParseOptions(Byte[] optionBytes)
        {
            var offset = 0;

            if (optionBytes.Length == 0)
                return null;


            // reset the OptionsCollection list to prepare
            //  to be re-populated with new data
            var retval = new List<Option>();

            while (offset < optionBytes.Length)
            {
                var type = (OptionTypes) optionBytes[offset + Option.KindFieldOffset];

                // some options have no length field, we cannot read
                // the length field if it isn't present or we risk
                // out-of-bounds issues
                Byte length;
                if (type == OptionTypes.EndOfOptionList ||
                    type == OptionTypes.NoOperation)
                {
                    length = 1;
                }
                else
                {
                    length = optionBytes[offset + Option.LengthFieldOffset];
                }

                switch (type)
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
                        throw new NotSupportedException("Option: " + type + " is not supported because its RFC specification is still experimental");
                    // add more options types here
                    default:
                        throw new NotImplementedException("Option: " + type + " not supported in Packet.Net yet");
                }
            }

            return retval;
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                {
                    // build flagstring
                    var flags = "{";
                    if (Urg)
                        flags += "urg[0x" + Convert.ToString(UrgentPointer, 16) + "]|";
                    if (Ack)
                        flags += "ack[" + AcknowledgmentNumber + " (0x" + Convert.ToString(AcknowledgmentNumber, 16) + ")]|";
                    if (Psh)
                        flags += "psh|";
                    if (Rst)
                        flags += "rst|";
                    if (Syn)
                        flags += "syn[0x" + Convert.ToString(SequenceNumber, 16) + "," + SequenceNumber + "]|";
                    flags = flags.TrimEnd('|');
                    flags += "}";

                    // build the output string
                    buffer.AppendFormat("{0}[TCPPacket: SourcePort={2}, DestinationPort={3}, Flags={4}]{1}",
                                        color,
                                        colorEscape,
                                        SourcePort,
                                        DestinationPort,
                                        flags);
                    break;
                }
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                {
                    // collect the properties and their value
                    var properties = new Dictionary<String, String>
                    {
                        {"source port", SourcePort.ToString()},
                        {"destination port", DestinationPort.ToString()},
                        {"sequence number", SequenceNumber + " (0x" + SequenceNumber.ToString("x") + ")"},
                        {"acknowledgement number", AcknowledgmentNumber + " (0x" + AcknowledgmentNumber.ToString("x") + ")"},
                        // TODO: Implement a HeaderLength property for TCPPacket
                        //properties.Add("header length", HeaderLength.ToString());
                        {"flags", "(0x" + AllFlags.ToString("x") + ")"}
                    };
                    var flags = Convert.ToString(AllFlags, 2).PadLeft(8, '0');
                    properties.Add("", flags[0] + "... .... = [" + flags[0] + "] congestion window reduced");
                    properties.Add(" ", "." + flags[1] + ".. .... = [" + flags[1] + "] ECN - echo");
                    properties.Add("  ", ".." + flags[2] + ". .... = [" + flags[2] + "] urgent");
                    properties.Add("   ", "..." + flags[3] + " .... = [" + flags[3] + "] acknowledgement");
                    properties.Add("    ", ".... " + flags[4] + "... = [" + flags[4] + "] push");
                    properties.Add("     ", ".... ." + flags[5] + ".. = [" + flags[5] + "] reset");
                    properties.Add("      ", ".... .." + flags[6] + ". = [" + flags[6] + "] syn");
                    properties.Add("       ", ".... ..." + flags[7] + " = [" + flags[7] + "] fin");
                    properties.Add("window size", WindowSize.ToString());
                    properties.Add("checksum", "0x" + Checksum + " [" + (ValidChecksum ? "valid" : "invalid") + "]");
                    properties.Add("options", "0x" + BitConverter.ToString(Options).Replace("-", "").PadLeft(12, '0'));
                    var parsedOptions = OptionsCollection;
                    if (parsedOptions != null)
                    {
                        for (var i = 0; i < parsedOptions.Count; i++)
                        {
                            properties.Add("option" + (i + 1), parsedOptions[i].ToString());
                        }
                    }

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("TCP:  ******* TCP - \"Transmission Control Protocol\" - offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("TCP:");
                    foreach (var property in properties)
                    {
                        if (property.Key.Trim() != "")
                        {
                            buffer.AppendLine("TCP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                        }
                        else
                        {
                            buffer.AppendLine("TCP: " + property.Key.PadLeft(padLength) + "   " + property.Value);
                        }
                    }

                    buffer.AppendLine("TCP:");
                    break;
                }
            }

            // append the base class output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Create a randomized tcp packet with the given ip version
        /// </summary>
        /// <returns>
        /// A <see cref="Packet" />
        /// </returns>
        public static TcpPacket RandomPacket()
        {
            var rnd = new Random();

            // create a randomized TcpPacket
            var srcPort = (UInt16) rnd.Next(UInt16.MinValue, UInt16.MaxValue);
            var dstPort = (UInt16) rnd.Next(UInt16.MinValue, UInt16.MaxValue);
            var tcpPacket = new TcpPacket(srcPort, dstPort);

            return tcpPacket;
        }

        /// <summary>
        /// Contains the Options list attached to the TCP header
        /// </summary>
        public List<Option> OptionsCollection => ParseOptions(Options);
    }
}