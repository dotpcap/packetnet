﻿/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.IO;
using System.Text;
using PacketDotNet.Ieee80211;
using PacketDotNet.Utils;

#if DEBUG
using System.Reflection;
using log4net;
#endif

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
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Used internally when building new packet dissectors
        /// </summary>
        protected ByteArraySegment Header;

        /// <summary>
        /// Used internally when building new packet dissectors
        /// </summary>
        protected LazySlim<PacketOrByteArraySegment> PayloadPacketOrData = new(null);

        /// <summary>
        /// Gets the actual bytes containing this packet and its payload.
        /// </summary>
        /// <remarks>Use <see cref="BytesSegment" /> for optimal performance.</remarks>
        public virtual byte[] Bytes => BytesSegment.ActualBytes();

        /// <summary>
        /// Gets a <see cref="ByteArraySegment" /> with the data that can start at an offset other than the first byte.
        /// </summary>
        public virtual ByteArraySegment BytesSegment
        {
            get
            {
                // ensure calculated values are properly updated
                RecursivelyUpdateCalculatedValues();

                // if we share memory with all of our sub packets we can take a
                // higher performance path to retrieve the bytes
                if (SharesMemoryWithSubPackets)
                {
                    ByteArraySegment byteArraySegment;

                    // The high performance path that is often taken because it is called on
                    // packets that have not had their header, or any of their sub packets, resized
                    if (PayloadPacketOrData.IsValueCreated && PayloadPacketOrData.Value.Type == PayloadType.Packet && PayloadPacket != null)
                    {
                        byteArraySegment = new ByteArraySegment(Header.Bytes,
                                                                Header.Offset,
                                                                Header.Length + PayloadPacket.TotalPacketLength);
                    }
                    else if (PayloadPacketOrData.IsValueCreated && PayloadPacketOrData.Value.Type == PayloadType.Bytes && PayloadDataSegment != null)
                    {
                        byteArraySegment = new ByteArraySegment(Header.Bytes,
                                                                Header.Offset,
                                                                Header.Length + PayloadDataSegment.Length);
                    }
                    else
                    {
                        byteArraySegment = new ByteArraySegment(Header.Bytes,
                                                                Header.Offset,
                                                                Header.BytesLength - Header.Offset);
                    }

                    Log.DebugFormat("SharesMemoryWithSubPackets, returning byte array {0}", byteArraySegment);
                    return byteArraySegment;
                }

                Log.Debug("rebuilding the byte array");

                var memoryStream = new MemoryStream();

                var headerCopy = HeaderData;
                memoryStream.Write(headerCopy, 0, headerCopy.Length);

                PayloadPacketOrData.Value.AppendToMemoryStream(memoryStream);

                var bytes = memoryStream.ToArray();
                return new ByteArraySegment(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Color used when generating the text description of a packet
        /// </summary>
        public virtual string Color => AnsiEscapeSequences.Black;

        /// <summary>
        /// Gets a value indicating whether this packet has payload data.
        /// </summary>
        /// <value>
        /// <c>true</c> if this packet has payload data; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasPayloadData => PayloadPacketOrData.Value.Type == PayloadType.Bytes;

        /// <summary>
        /// Gets a value indicating whether this packet has a payload packet.
        /// </summary>
        /// <value>
        /// <c>true</c> if this packet has a payload packet; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasPayloadPacket => PayloadPacketOrData.Value.Type == PayloadType.Packet;

        /// <summary>
        /// Gets the bytes of the header's data.
        /// </summary>
        public virtual byte[] HeaderData => Header.ActualBytes();

        /// <summary>
        /// Gets the header's data as a <see cref="ByteArraySegment" />.
        /// </summary>
        public virtual ByteArraySegment HeaderDataSegment => Header.NextSegment();

        /// <summary>
        /// Gets the header as a <see cref="ByteArraySegment"/>.
        /// </summary>
        public virtual ByteArraySegment HeaderSegment => Header;

        /// <summary>
        /// Gets a value indicating whether the payload is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if the payload is initialized; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsPayloadInitialized => PayloadPacketOrData.IsValueCreated;

        /// <summary>
        /// The packet that is carrying this one
        /// </summary>
        public virtual Packet ParentPacket { get; set; }

        /// <summary>
        /// Gets or sets the bytes of the payload if present.
        /// </summary>
        /// <remarks>The packet MAY have a null <see cref="PayloadData" /> but a non-null <see cref="PayloadPacket" />.</remarks>
        public byte[] PayloadData
        {
            get => PayloadDataSegment?.ActualBytes();
            set
            {
                Log.DebugFormat("value.Length {0}", value.Length);

                PayloadDataSegment = new ByteArraySegment(value, 0, value.Length);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ByteArraySegment" /> of the payload if present.
        /// </summary>
        /// <remarks>The packet MAY have a null <see cref="PayloadData" /> but a non-null <see cref="PayloadPacket" />.</remarks>
        public ByteArraySegment PayloadDataSegment
        {
            get
            {
                if (PayloadPacketOrData.Value.ByteArraySegment == null)
                {
                    Log.Debug("returning null");
                    return null;
                }

                Log.DebugFormat("result.Length: {0}", PayloadPacketOrData.Value.ByteArraySegment.Length);
                return PayloadPacketOrData.Value.ByteArraySegment;
            }
            set => PayloadPacketOrData.Value.ByteArraySegment = value;
        }

        /// <summary>
        /// Packet that this packet carries if one is present.
        /// </summary>
        /// <remarks>The packet MAY have a null <see cref="PayloadPacket" /> but a non-null <see cref="PayloadData" />.</remarks>
        public virtual Packet PayloadPacket
        {
            get => PayloadPacketOrData.Value.Packet;
            set
            {
                if (this == value)
                    ThrowHelper.ThrowInvalidOperationException(ExceptionDescription.PacketAsPayloadPacket);

                PayloadPacketOrData.Value.Packet = value;
                PayloadPacketOrData.Value.Packet.ParentPacket = this;
            }
        }

        /// <summary>
        /// Gets the total length of the packet by recursively finding the length of this packet and all of the packets encapsulated by this packet.
        /// </summary>
        /// <value>
        /// The total length of the packet.
        /// </value>
        public int TotalPacketLength
        {
            get
            {
                var totalLength = 0;
                if (Header != null)
                    totalLength += Header.Length;

                if (PayloadPacketOrData.Value != null)
                {
                    switch (PayloadPacketOrData.Value.Type)
                    {
                        case PayloadType.Bytes:
                        {
                            totalLength += PayloadPacketOrData.Value.ByteArraySegment.Length;
                            break;
                        }
                        case PayloadType.Packet:
                        {
                            totalLength += PayloadPacketOrData.Value.Packet.TotalPacketLength;
                            break;
                        }
                    }
                }

                return totalLength;
            }
        }

        /// <summary>
        /// Returns true if we already have a contiguous byte[] in either
        /// of these conditions:
        /// - This packet's header byte[] and payload byte[] are the same instance
        /// or
        /// - This packet's header byte[] and this packet's payload packet
        /// are the same instance and the offsets indicate that the bytes
        /// are contiguous
        /// </summary>
        protected bool SharesMemoryWithSubPackets
        {
            get
            {
                Log.Debug("");

                var payloadType = PayloadPacketOrData.Value?.Type ?? PayloadType.None;

                switch (payloadType)
                {
                    case PayloadType.Bytes:
                    {
                        // is the byte array payload the same byte[] and does the offset indicate
                        // that the bytes are contiguous?
                        if (Header.Bytes == PayloadPacketOrData.Value?.ByteArraySegment.Bytes &&
                            Header.Offset + Header.Length == PayloadPacketOrData.Value?.ByteArraySegment.Offset)
                        {
                            Log.Debug("PayloadType.Bytes returning true");
                            return true;
                        }

                        Log.Debug("PayloadType.Bytes returning false");
                        return false;
                    }
                    case PayloadType.Packet:
                    {
                        // is the byte array payload the same as the payload packet header and does
                        // the offset indicate that the bytes are contiguous?
                        if (Header.Bytes == PayloadPacketOrData.Value?.Packet.Header.Bytes &&
                            Header.Offset + Header.Length == PayloadPacketOrData.Value?.Packet.Header.Offset)
                        {
                            // and does the sub packet share memory with its sub packets?
                            var result = PayloadPacketOrData.Value.Packet.SharesMemoryWithSubPackets;
                            Log.DebugFormat("PayloadType.Packet result {0}", result);
                            return result;
                        }

                        Log.Debug("PayloadType.Packet returning false");
                        return false;
                    }
                    case PayloadType.None:
                    {
                        // no payload data or packet thus we must share memory with
                        // our non-existent sub packets
                        Log.Debug("PayloadType.None, returning true");
                        return true;
                    }
                    default:
                    {
                        ThrowHelper.ThrowNotImplementedException();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Parse bytes into a packet
        /// </summary>
        /// <param name="linkLayers">
        /// A <see cref="LinkLayers" />
        /// </param>
        /// <param name="packetData">
        /// A <see cref="byte" />
        /// </param>
        /// <returns>
        /// A <see cref="Packet" />
        /// </returns>
        public static Packet ParsePacket(LinkLayers linkLayers, byte[] packetData)
        {
            Packet p;
            var byteArraySegment = new ByteArraySegment(packetData);

            Log.DebugFormat("LinkLayer {0}", linkLayers);

            switch (linkLayers)
            {
                case LinkLayers.Ethernet:
                {
                    p = new EthernetPacket(byteArraySegment);
                    break;
                }
                case LinkLayers.LinuxSll:
                {
                    p = new LinuxSllPacket(byteArraySegment);
                    break;
                }
                case LinkLayers.Null:
                {
                    p = new NullPacket(byteArraySegment);
                    break;
                }
                case LinkLayers.Ppp:
                {
                    p = new PppPacket(byteArraySegment);
                    break;
                }
                case LinkLayers.Ieee80211:
                {
                    p = MacFrame.ParsePacket(byteArraySegment);
                    break;
                }
                case LinkLayers.Ieee80211RadioTap:
                {
                    p = new RadioPacket(byteArraySegment);
                    break;
                }
                case LinkLayers.Ppi:
                {
                    p = new PpiPacket(byteArraySegment);
                    break;
                }
                case LinkLayers.Raw:
                case LinkLayers.RawLegacy:
                {
                    p = new RawIPPacket(byteArraySegment);
                    break;
                }
                default:
                {
                    ThrowHelper.ThrowNotImplementedException(ExceptionArgument.linkLayer);
                    p = null;
                    break;
                }
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
            if (PayloadPacketOrData.Value?.Type == PayloadType.Packet)
            {
                PayloadPacketOrData.Value.Packet.RecursivelyUpdateCalculatedValues();
            }
        }

        /// <summary>
        /// Called to ensure that calculated values are updated before
        /// the packet bytes are retrieved
        /// Classes should override this method to update things like
        /// checksums and lengths that take too much time or are too complex
        /// to update for each packet parameter change
        /// </summary>
        public virtual void UpdateCalculatedValues()
        { }

        /// <summary>Output this packet as a readable string</summary>
        public override string ToString()
        {
            return ToString(StringOutputType.Normal);
        }

        /// <summary cref="ToString()">
        /// Output the packet information in the specified format
        /// Normal - outputs the packet info to a single line
        /// Colored - outputs the packet info to a single line with coloring
        /// Verbose - outputs detailed info about the packet
        /// VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <param name="outputFormat">
        ///     <see cref="StringOutputType" />
        /// </param>
        public virtual string ToString(StringOutputType outputFormat)
        {
            return PayloadPacketOrData.Value.Type == PayloadType.Packet ? PayloadPacketOrData.Value.Packet.ToString(outputFormat) : String.Empty;
        }

        /// <summary>
        /// Prints the Packet PayloadData in Hex format
        /// With the 16-byte segment number, raw bytes, and parsed ascii output
        /// Ex:
        /// 0010  00 18 82 6c 7c 7f 00 c0  9f 77 a3 b0 88 64 11 00   ...1|... .w...d..
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public string PrintHex()
        {
            var data = BytesSegment.Bytes;
            var buffer = new StringBuilder();
            var bytes = "";
            var ascii = "";

            buffer.AppendLine("Data:  ******* Raw Hex Output - length=" + data.Length + " bytes");
            buffer.AppendLine("Data: Segment:                   Bytes:                              Ascii:");
            buffer.AppendLine("Data: --------------------------------------------------------------------------");

            // parse the raw data
            for (var i = 1; i <= data.Length; i++)
            {
                // add the current byte to the bytes hex string
                bytes += data[i - 1].ToString("x").PadLeft(2, '0') + " ";

                // add the current byte to the asciiBytes array for later processing
                if (data[i - 1] < 0x21 || data[i - 1] > 0x7e)
                {
                    ascii += ".";
                }
                else
                {
                    ascii += Encoding.ASCII.GetString(new[] { data[i - 1] });
                }

                // add an additional space to split the bytes into
                //  two groups of 8 bytes
                if (i % 16 != 0 && i % 8 == 0)
                {
                    bytes += " ";
                    ascii += " ";
                }

                // append the output string
                string segmentNumber;
                if (i % 16 == 0)
                {
                    // add the 16 byte segment number
                    segmentNumber = (((i - 16) / 16) * 10).ToString().PadLeft(4, '0');

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
                    segmentNumber = ((((i - 16) / 16) + 1) * 10).ToString().PadLeft(4, '0');

                    // build the line
                    buffer.AppendLine("Data: " + segmentNumber.PadLeft(4, '0') + "  " + bytes.PadRight(49, ' ') + "  " + ascii);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Extracts a packet of <see cref="T" />, or the current packet if it's <see cref="T" />, or <c>null</c> if it isn't found.
        /// </summary>
        /// <returns>The packet of type <see cref="T" />.</returns>
        public T Extract<T>() where T : Packet
        {
            var t = this;
            while (t != null)
            {
                if (t is T packet)
                    return packet;


                t = t.PayloadPacket;
            }

            return null;
        }
    }
}