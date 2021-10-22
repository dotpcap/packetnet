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
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;
#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// EspPacket
    /// See: https://en.wikipedia.org/wiki/IPsec#Encapsulating_Security_Payload
    /// </summary>
    public sealed class EspPacket : Packet
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
        /// Create from values.
        /// </summary>
        public EspPacket()
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = EspFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public EspPacket(ByteArraySegment byteArraySegment, Packet parentPacket)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(byteArraySegment) {Length = EspFields.HeaderLength};

            ParentPacket = parentPacket;

            var next = Header.NextSegment();
            
            // try to decode, assuming a Null ciphering. Get first the last 96 bits (12 bytes) for the Authentication Data
            // + 1 byte for the Next Header + 1 byte for the pad length
            if (next.Length > 14)
            {
                AuthenticationData = new byte[12];

                // copy the last 12 bytes
                Array.Copy(next.Bytes, next.BytesLength - 12, AuthenticationData, 0, 12);
                var nextHeader = next.Bytes[next.BytesLength - 13];
                // Continue only if next header is Tcp or Udp
                if ((ProtocolType)nextHeader == ProtocolType.Tcp || (ProtocolType)nextHeader == ProtocolType.Udp)
                {
                    NextHeader = (ProtocolType) nextHeader;
                    PadLength = next.Bytes[next.BytesLength - 14];

                    if (next.Length > 14 + PadLength)
                    {
                        // so far ok, continue
                        Pad = new byte[PadLength];
                        Array.Copy(next.Bytes, next.BytesLength - 14 - PadLength, Pad, 0, PadLength);
                        
                        var startingOffset = Header.Offset + Header.Length;
                        var segmentLength = Math.Max(0, next.Length - 14 - PadLength);
                        var payloadData = new byte[segmentLength];
                        Array.Copy(next.Bytes, startingOffset, payloadData, 0, segmentLength);
                        var payload = new ByteArraySegment(payloadData, 0, segmentLength, segmentLength);
                        
                        if (NextHeader == ProtocolType.Tcp && segmentLength > 0)
                        {
                            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => new PacketOrByteArraySegment
                            {
                                Packet = new TcpPacket(payload, this)
                            });

                            return;
                        }
                    }
                }
            }

            // store the payload bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment {ByteArraySegment = Header.NextSegment()};
                return result;
            });
        }

        /// <summary>
        /// Gets or sets the SecurityParametersIndex (SPI).
        /// </summary>
        public uint SecurityParametersIndex
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset);
        }

        /// <summary>
        /// Gets or sets the SecurityParametersIndex (SPI).
        /// </summary>
        public uint SequenceNumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + EspFields.SecurityParametersIndexLength);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + EspFields.SecurityParametersIndexLength);
        }

        /// <summary>
        /// Identifies the next header field, which is the protocol of the encapsulated in packet unless there are extended headers.
        /// </summary>
        public ProtocolType NextHeader { get; set; }

        /// <summary>Pad length.</summary>
        public int PadLength { get; private set; }

        /// <summary>
        /// Gets or sets the Authentication Data.
        /// </summary>
        public byte[] AuthenticationData { get; set; }
        
        /// <summary>
        /// Gets or sets the Pad.
        /// </summary>
        public byte[] Pad { get; private set; }
    }
}
