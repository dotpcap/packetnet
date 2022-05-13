/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Text;
using PacketDotNet.Utils;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// Wake-On-Lan
    /// See: http://en.wikipedia.org/wiki/Wake-on-LAN
    /// See: http://wiki.wireshark.org/WakeOnLAN
    /// </summary>
    public sealed class WakeOnLanPacket : Packet
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
        
        // the WOL synchronization sequence
        private static readonly byte[] SyncSequence = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

        /// <summary>
        /// Create a Wake-On-LAN packet from the destination MAC address
        /// </summary>
        /// <param name="destinationMAC">
        /// A <see cref="PhysicalAddress" />
        /// </param>
        public WakeOnLanPacket(PhysicalAddress destinationMAC)
        {
            Log.Debug("");

            // allocate memory for this packet
            var packetLength = SyncSequence.Length + (EthernetFields.MacAddressLength * WakeOnLanFields.MacAddressRepetition);
            var packetBytes = new byte[packetLength];
            var destinationMACBytes = destinationMAC.GetAddressBytes();

            // write the data to the payload
            // - synchronization sequence (6 bytes)
            // - destination MAC (16 copies of 6 bytes)
            for (var i = 0; i < packetLength; i += EthernetFields.MacAddressLength)
            {
                // copy the syncSequence on the first pass
                if (i == 0)
                    Array.Copy(SyncSequence, 0, packetBytes, i, SyncSequence.Length);
                else
                    Array.Copy(destinationMACBytes, 0, packetBytes, i, EthernetFields.MacAddressLength);
            }

            Header = new ByteArraySegment(packetBytes, 0, packetLength);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
        public WakeOnLanPacket(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = Bytes.Length;
        }

        /// <summary>
        /// The Physical Address (MAC) of the host being woken up from sleep
        /// </summary>
        public PhysicalAddress DestinationAddress
        {
            get
            {
                var destinationMAC = new byte[EthernetFields.MacAddressLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + WakeOnLanFields.DestinationAddressPosition,
                           destinationMAC,
                           0,
                           EthernetFields.MacAddressLength);

                return new PhysicalAddress(destinationMAC);
            }
            set
            {
                var destinationMAC = value.GetAddressBytes();
                Array.Copy(destinationMAC,
                           0,
                           Header.Bytes,
                           Header.Offset + SyncSequence.Length,
                           EthernetFields.MacAddressLength);
            }
        }

        /// <summary>
        /// The Password field is optional, but if present, contains either 4 bytes or 6 bytes.
        /// If a 4-byte password is present, it will be dissected as an IPv4 address 
        /// and if a 6-byte password is present, it will be dissected as an Ethernet address.
        /// </summary>
        public byte[] Password
        {
            get
            {
                int passwordLength = HeaderData.Length - WakeOnLanFields.PasswordPosition;

                // If 6-byte length, it will be dissected as an Ethernet address.
                // If 4-byte length, it will be dissected as an IPv4 address.
                if (passwordLength is 6 or 4)
                {
                    byte[] hwAddress = new byte[passwordLength];
                    Array.Copy(Header.Bytes, Header.Offset + WakeOnLanFields.PasswordPosition,
                               hwAddress, 0, hwAddress.Length);
                    return hwAddress;
                }
                return Array.Empty<byte>();
            }
            set
            {
                var bytes = value;

                // checks if the new value matches with the specification.
                if (bytes.Length is 6 or 4)
                {
                    // checks if the byte can fit the current byteArraySegment.
                    if (bytes.Length == Password.Length)
                    {
                        // set the password
                        Array.Copy(bytes, 0,
                               Header.Bytes, Header.Offset + WakeOnLanFields.PasswordPosition,
                               bytes.Length);

                    }
                }
            }
        }

        /// <summary>
        /// Generate a random WakeOnLanPacket
        /// </summary>
        /// <returns>
        /// A <see cref="WakeOnLanPacket" />
        /// </returns>
        public static WakeOnLanPacket RandomPacket()
        {
            var rnd = new Random();

            var destAddress = new byte[EthernetFields.MacAddressLength];

            rnd.NextBytes(destAddress);

            return new WakeOnLanPacket(new PhysicalAddress(destAddress));
        }

        /// <summary>
        /// Checks the validity of the Wake-On-LAN payload by checking the synchronization sequence and if there are 16 iterations of the destination MAC address.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the Wake-On-LAN payload is valid.
        /// </returns>
        public bool IsValid()
        {
            return IsValid(Header);
        }

        /// <summary>
        /// Determines whether the payload can be decoded by <see cref="WakeOnLanPacket" />.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="transportPacket">The transport packet.</param>
        /// <returns>
        /// <c>true</c> if the payload can be decoded by <see cref="WakeOnLanPacket"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanDecode(ByteArraySegment payload, TransportPacket transportPacket)
        {
            // If this packet is going to port 0, 7 or 9, then it might be a WakeOnLan packet.
            return transportPacket.DestinationPort is WakeOnLanFields.Port0 or WakeOnLanFields.Port7 or WakeOnLanFields.Port9 && IsValid(payload);
        }

        /// <summary>
        /// See <see cref="IsValid()" />.
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <returns>A <see cref="bool" />.</returns>
        private static bool IsValid(ByteArraySegment byteArraySegment)
        {
            // validate the 16 repetitions of the wolDestinationMAC
            // verify that the wolDestinationMAC address repeats 16 times in sequence
            for (var i = 0; i < EthernetFields.MacAddressLength * WakeOnLanFields.MacAddressRepetition; i += EthernetFields.MacAddressLength)
            {
                var baseOffset = byteArraySegment.Offset + i;

                if (i == 0)
                {
                    // Check the synchronization sequence on the first pass.
                    for (var j = 0; j < EthernetFields.MacAddressLength; j++)
                    {
                        if (byteArraySegment.Bytes[baseOffset + j] != SyncSequence[j])
                            return false;
                    }
                }
                else
                {
                    for (var j = 0; j < EthernetFields.MacAddressLength; j++)
                    {
                        if (byteArraySegment.Bytes[byteArraySegment.Offset + SyncSequence.Length + j] != byteArraySegment.Bytes[baseOffset + j])
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Compare two instances
        /// </summary>
        /// <param name="obj">
        /// A <see cref="object" />
        /// </param>
        /// <returns>
        /// A <see cref="bool" />
        /// </returns>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if ((obj == null) || (GetType() != obj.GetType()))
                return false;


            var wol = (WakeOnLanPacket) obj;

            return DestinationAddress.Equals(wol.DestinationAddress);
        }

        /// <summary>
        /// GetHashCode override
        /// </summary>
        /// <returns>
        /// A <see cref="int" />
        /// </returns>
        public override int GetHashCode()
        {
            return Header.GetHashCode();
        }

        /// <inheritdoc cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat is StringOutputType.Colored or StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if (outputFormat is StringOutputType.Normal or StringOutputType.Colored)
            {
                buffer.AppendFormat("[{0}WakeOnLanPacket{1}: DestinationAddress={2}]",
                                    color,
                                    colorEscape,
                                    DestinationAddress);
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "destination", HexPrinter.PrintMACAddress(DestinationAddress) }
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("WOL:  ******* WOL - \"Wake-On-Lan\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("WOL:");
                foreach (var property in properties)
                    buffer.AppendLine("WOL: " + property.Key.PadLeft(padLength) + " = " + property.Value);

                buffer.AppendLine("WOL:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}