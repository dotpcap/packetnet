/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Tcp;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// TcpPacket
    /// See: http://en.wikipedia.org/wiki/Transmission_Control_Protocol
    /// </summary>
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
        public const int HeaderMinimumLength = 20;

        /// <summary>
        /// Create a new TCP packet from values
        /// </summary>
        public TcpPacket(ushort sourcePort, ushort destinationPort)
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = TcpFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);

            // make this packet valid
            DataOffset = length / 4;

            // set instance values
            SourcePort = sourcePort;
            DestinationPort = destinationPort;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public TcpPacket(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = DataOffset * 4;

            // NOTE: we update the Length field AFTER the header field because
            // we need the header to be valid to retrieve the value of DataOffset

            // store the payload bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment();
                var payload = Header.NextSegment();

                if (CustomPayloadDecoder != null && (result.Packet = CustomPayloadDecoder(payload, this)) != null)
                {
                    Log.Debug("Use CustomPayloadDecoder");
                    return result;
                }

                result.ByteArraySegment = payload;
                return result;
            });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public TcpPacket
        (
            ByteArraySegment byteArraySegment,
            Packet parentPacket)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);

            // NOTE: we update the Length field AFTER the header field because
            // we need the header to be valid to retrieve the value of DataOffset
            Header.Length = DataOffset * 4;

            // store the payload bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment();
                var payload = Header.NextSegment();

                if (CustomPayloadDecoder != null && (result.Packet = CustomPayloadDecoder(payload, this)) != null)
                {
                    Log.Debug("Use CustomPayloadDecoder");
                    return result;
                }

                result.ByteArraySegment = payload;

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

        /// <summary>Gets or sets the acknowledgment (ACK) flag, which indicates if the <see cref="AcknowledgmentNumber" /> is valid.</summary>
        public bool Acknowledgment
        {
            get => (Flags & TcpFields.TCPAckMask) != 0;
            set => SetFlag(value, TcpFields.TCPAckMask);
        }

        /// <summary>Fetch the packet acknowledgment number.</summary>
        public uint AcknowledgmentNumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes,
                                                   Header.Offset + TcpFields.AcknowledgmentNumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + TcpFields.AcknowledgmentNumberPosition);
        }

        /// <value>
        /// Tcp checksum field value of type UInt16
        /// </value>
        public override ushort Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.ChecksumPosition);
            set
            {
                var v = value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + TcpFields.ChecksumPosition);
            }
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.Yellow;

        /// <value>
        /// Congestion Window Reduced flag
        /// </value>
        public bool CongestionWindowReduced
        {
            get => (Flags & TcpFields.CongestionWindowReducedMask) != 0;
            set => SetFlag(value, TcpFields.CongestionWindowReducedMask);
        }

        /// <summary>The size of the tcp header in 32bit words </summary>
        public int DataOffset
        {
            get
            {
                var dataOffset = (byte) ((DataOffsetAndFlags >> 12) & 0xF);
                return dataOffset;
            }
            set
            {
                var dataOffset = DataOffsetAndFlags;

                dataOffset = (ushort) ((dataOffset & 0x0FFF) | ((value << 12) & 0xF000));

                // write the value back
                DataOffsetAndFlags = dataOffset;
            }
        }

        /// <summary>Fetches the port number on the destination host.</summary>
        public override ushort DestinationPort
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.DestinationPortPosition);
            set
            {
                var v = value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + TcpFields.DestinationPortPosition);
            }
        }

        /// <value>
        /// Gets or sets the explicit congestion notification echo (ECE) flag, which indicates that the TCP peer is ECN capable if the <see cref="Synchronize" /> flag is <c>true</c>,
        /// or otherwise that a packet with Congestion Experienced flag set in the IP header was received during normal transmission.
        /// </value>
        public bool ExplicitCongestionNotificationEcho
        {
            get => (Flags & TcpFields.ExplicitCongestionNotificationEchoMask) != 0;
            set => SetFlag(value, TcpFields.ExplicitCongestionNotificationEchoMask);
        }

        /// <summary>Gets or sets the finished (FIN) flag, which indicates whether the sender has finished sending.</summary>
        public bool Finished
        {
            get => (Flags & TcpFields.FinishedMask) != 0;
            set => SetFlag(value, TcpFields.FinishedMask);
        }

        /// <summary>
        /// The flags consisting of 9 bits.
        /// </summary>
        public ushort Flags
        {
            get
            {
                var flags = DataOffsetAndFlags & 0x1FF;
                return (ushort) flags;
            }
            set
            {
                var flags = DataOffsetAndFlags;

                flags = (ushort) ((flags & 0xFE00) | (value & 0x1FF));
                DataOffsetAndFlags = flags;
            }
        }

        /// <value>
        /// Gets or sets the nonce sum (NS) flag.
        /// </value>
        public bool NonceSum
        {
            get => (Flags & TcpFields.NonceSumMask) != 0;
            set => SetFlag(value, TcpFields.NonceSumMask);
        }

        /// <summary>
        /// Gets the bytes that represent the TCP options.
        /// </summary>
        public byte[] Options
        {
            get
            {
                if (Urgent)
                    ThrowHelper.ThrowNotImplementedException(ExceptionDescription.UrgentPointerSet);

                var optionsOffset = TcpFields.UrgentPointerPosition + TcpFields.UrgentPointerLength;
                var optionsLength = (DataOffset * 4) - optionsOffset;

                var optionBytes = new byte[optionsLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + optionsOffset,
                           optionBytes,
                           0,
                           optionsLength);

                return optionBytes;
            }
            set
            {
                var optionsOffset = Header.Offset + TcpFields.UrgentPointerPosition + TcpFields.UrgentPointerLength;
                Array.Copy(value, 0, Header.Bytes, optionsOffset, value.Length);
            }
        }

        /// <summary>
        /// Contains the Options list attached to the TCP header
        /// </summary>
        public List<TcpOption> OptionsCollection
        {
            get => ParseOptions(OptionsSegment);
            set
            {
                var optionsOffset = TcpFields.UrgentPointerPosition + TcpFields.UrgentPointerLength;

                foreach (var option in value)
                {
                    var optionBytes = option.Bytes;
                    Array.Copy(optionBytes, 0, Header.Bytes, Header.Offset + optionsOffset, optionBytes.Length);
                    optionsOffset += optionBytes.Length;
                }
            }
        }

        /// <summary>
        /// Gets the TCP options as a <see cref="ByteArraySegment" />.
        /// </summary>
        /// <exception cref="NotImplementedException">Urgent == true not implemented yet</exception>
        public ByteArraySegment OptionsSegment
        {
            get
            {
                if (Urgent)
                    ThrowHelper.ThrowNotImplementedException(ExceptionDescription.UrgentPointerSet);

                var optionsOffset = TcpFields.UrgentPointerPosition + TcpFields.UrgentPointerLength;
                var optionsLength = (DataOffset * 4) - optionsOffset;

                return new ByteArraySegment(Header.Bytes, Header.Offset + optionsOffset, optionsLength);
            }
        }

        /// <summary>
        /// Gets or sets the push (PSH) flag, which indicates the receiver should pass the data to the application as soon as possible.
        /// </summary>
        public bool Push
        {
            get => (Flags & TcpFields.PushMask) != 0;
            set => SetFlag(value, TcpFields.PushMask);
        }

        /// <summary>
        /// Gets or sets the reset (RST) flag, which indicates the session should be reset between
        /// the sender and the receiver.
        /// </summary>
        public bool Reset
        {
            get => (Flags & TcpFields.ResetMask) != 0;
            set => SetFlag(value, TcpFields.ResetMask);
        }

        /// <summary>Fetch the packet sequence number.</summary>
        public uint SequenceNumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes,
                                                   Header.Offset + TcpFields.SequenceNumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + TcpFields.SequenceNumberPosition);
        }

        /// <summary>Fetch the port number on the source host.</summary>
        public override ushort SourcePort
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.SourcePortPosition);
            set
            {
                var v = value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + TcpFields.SourcePortPosition);
            }
        }

        /// <summary>
        /// Gets or sets the synchronize (SYN flag), which indicates the sequence numbers should be synchronized between the sender and receiver to initiate a connection.
        /// </summary>
        public bool Synchronize
        {
            get => (Flags & TcpFields.SynchronizationMask) != 0;
            set => SetFlag(value, TcpFields.SynchronizationMask);
        }

        /// <summary>Gets or sets the urgent (URG) flag, which indicates if the <see cref="UrgentPointer" /> is valid.</summary>
        public bool Urgent
        {
            get => (Flags & TcpFields.UrgentMask) != 0;
            set => SetFlag(value, TcpFields.UrgentMask);
        }

        /// <summary>Fetch the urgent pointer.</summary>
        public int UrgentPointer
        {
            get => EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                  Header.Offset + TcpFields.UrgentPointerPosition);
            set
            {
                var v = (short) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + TcpFields.UrgentPointerPosition);
            }
        }

        /// <summary>Check if the TCP packet is valid, checksum-wise.</summary>
        public bool ValidChecksum
        {
            get
            {
                // IPv6 has no checksum so only the TCP checksum needs evaluation
                if (ParentPacket is IPv6Packet)
                    return ValidTcpChecksum;


                // For IPv4 both the IP layer and the TCP layer contain checksums
                return ((IPv4Packet) ParentPacket).ValidIPChecksum && ValidTcpChecksum;
            }
        }

        /// <value>
        /// True if the tcp checksum is valid
        /// </value>
        public bool ValidTcpChecksum
        {
            get
            {
                Log.Debug("ValidTcpChecksum");
                var result = IsValidChecksum(TransportChecksumOption.IncludePseudoIPHeader);
                Log.DebugFormat("ValidTcpChecksum {0}", result);
                return result;
            }
        }

        /// <summary>
        /// The size of the receive window, which specifies the number of
        /// bytes (beyond the sequence number in the acknowledgment field) that
        /// the receiver is currently willing to receive.
        /// </summary>
        public ushort WindowSize
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.WindowSizePosition);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + TcpFields.WindowSizePosition);
        }

        /// <summary>
        /// Gets or sets the data offset and flags.
        /// </summary>
        private ushort DataOffsetAndFlags
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + TcpFields.DataOffsetAndFlagsPosition);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + TcpFields.DataOffsetAndFlagsPosition);
        }

        private void SetFlag(bool on, int mask)
        {
            if (on)
                Flags = (ushort) (Flags | mask);
            else
                Flags = (ushort) (Flags & ~mask);
        }

        /// <summary>
        /// Decode payload to support Drda protocol
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public TcpPacket DecodePayload(PacketOrByteArraySegment result)
        {
            if ((result.ByteArraySegment == null) || (result.ByteArraySegment.Length < DrdaDdmFields.DDMHeadTotalLength))
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
        public ushort CalculateTcpChecksum()
        {
            return (ushort) CalculateChecksum(TransportChecksumOption.IncludePseudoIPHeader);
        }

        /// <summary>
        /// Update the checksum value.
        /// </summary>
        public void UpdateTcpChecksum()
        {
            Checksum = CalculateTcpChecksum();
        }

        /// <summary>
        /// Parses options, pointed to by optionBytes into an array of Options
        /// </summary>
        /// <param name="optionBytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <returns>
        /// A <see cref="List{Option}" />
        /// </returns>
        private static List<TcpOption> ParseOptions(ByteArraySegment optionBytes)
        {
            var offset = optionBytes.Offset;

            if (optionBytes.Length == 0)
                return null;


            // Reset the OptionsCollection list to prepare to be re-populated with new data.
            var options = new List<TcpOption>();

            // The TCP options should be bound by their options offset + length.
            var maxOffset = optionBytes.Offset + optionBytes.Length;

            // Include a basic check against the available length of the options buffer for invalid TCP packet data in the data offset field.
            while ((offset < maxOffset) && (offset < optionBytes.Bytes.Length - TcpOption.LengthFieldOffset))
            {
                var type = (OptionTypes) optionBytes.Bytes[offset + TcpOption.KindFieldOffset];

                // Some options have no length field, we cannot read the length field if it isn't present or we risk out-of-bounds issues.
                byte length;

                if ((type == OptionTypes.EndOfOptionList) || (type == OptionTypes.NoOperation))
                    length = 1;
                else
                    length = optionBytes.Bytes[offset + TcpOption.LengthFieldOffset];

                switch (type)
                {
                    case OptionTypes.EndOfOptionList:
                    {
                        options.Add(new EndOfOptionsOption(optionBytes.Bytes, offset, length));
                        offset += EndOfOptionsOption.OptionLength;
                        break;
                    }
                    case OptionTypes.NoOperation:
                    {
                        options.Add(new NoOperationOption(optionBytes.Bytes, offset, length));
                        offset += NoOperationOption.OptionLength;
                        break;
                    }
                    case OptionTypes.MaximumSegmentSize:
                    {
                        options.Add(new MaximumSegmentSizeOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.WindowScaleFactor:
                    {
                        options.Add(new WindowScaleFactorOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.SelectiveAcknowledgmentPermitted:
                    {
                        options.Add(new SelectiveAcknowledgmentPermittedOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.SelectiveAcknowledgment:
                    {
                        options.Add(new SelectiveAcknowledgmentOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.Echo:
                    {
                        options.Add(new EchoOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.EchoReply:
                    {
                        options.Add(new EchoReplyOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.Timestamp:
                    {
                        options.Add(new TimeStampOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.AlternateChecksumRequest:
                    {
                        options.Add(new AlternateChecksumRequestOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.AlternateChecksumData:
                    {
                        options.Add(new AlternateChecksumDataOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.MD5Signature:
                    {
                        options.Add(new MD5SignatureOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    case OptionTypes.UserTimeout:
                    {
                        options.Add(new UserTimeoutOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                    // These fields aren't supported because they're still considered experimental in their respective RFC specifications.
                    //case OptionTypes.PartialOrderConnectionPermitted:
                    //case OptionTypes.PartialOrderServiceProfile:
                    //case OptionTypes.ConnectionCount:
                    //case OptionTypes.ConnectionCountNew:
                    //case OptionTypes.ConnectionCountEcho:
                    //case OptionTypes.QuickStartResponse:
                    default:
                    {
                        options.Add(new UnsupportedOption(optionBytes.Bytes, offset, length));
                        offset += length;
                        break;
                    }
                }
            }

            return options;
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if ((outputFormat == StringOutputType.Colored) || (outputFormat == StringOutputType.VerboseColored))
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                {
                    // build flag string
                    var flags = "{";
                    if (Urgent)
                        flags += "urg[0x" + Convert.ToString(UrgentPointer, 16) + "]|";

                    if (Acknowledgment)
                        flags += "ack[" + AcknowledgmentNumber + " (0x" + Convert.ToString(AcknowledgmentNumber, 16) + ")]|";

                    if (Push)
                        flags += "psh|";

                    if (Reset)
                        flags += "rst|";

                    if (Synchronize)
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
                    var properties = new Dictionary<string, string>
                    {
                        { "source port", SourcePort.ToString() },
                        { "destination port", DestinationPort.ToString() },
                        { "sequence number", SequenceNumber + " (0x" + SequenceNumber.ToString("x") + ")" },
                        { "acknowledgement number", AcknowledgmentNumber + " (0x" + AcknowledgmentNumber.ToString("x") + ")" },
                        // TODO: Implement a HeaderLength property for TCPPacket
                        //properties.Add("header length", HeaderLength.ToString());
                        { "flags", "(0x" + Flags.ToString("x") + ")" }
                    };

                    var flags = Convert.ToString(Flags, 2).PadLeft(8, '0');
                    properties.Add("", flags[0] + "... .... = [" + flags[0] + "] congestion window reduced");
                    properties.Add(" ", "." + flags[1] + ".. .... = [" + flags[1] + "] Ecn - echo");
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
                        for (var i = 0; i < parsedOptions.Count; i++)
                            properties.Add("option" + (i + 1), parsedOptions[i].ToString());

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("TCP:  ******* TCP - \"Transmission Control Protocol\" - offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("TCP:");
                    foreach (var property in properties)
                        if (property.Key.Trim() != "")
                            buffer.AppendLine("TCP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                        else
                            buffer.AppendLine("TCP: " + property.Key.PadLeft(padLength) + "   " + property.Value);

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
            var srcPort = (ushort) rnd.Next(UInt16.MinValue, UInt16.MaxValue);
            var dstPort = (ushort) rnd.Next(UInt16.MinValue, UInt16.MaxValue);
            var tcpPacket = new TcpPacket(srcPort, dstPort);

            return tcpPacket;
        }
    }
}