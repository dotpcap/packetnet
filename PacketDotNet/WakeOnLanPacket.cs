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
 * Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using PacketDotNet.Utils;

#if DEBUG
using log4net;
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
        #region Preprocessor Directives

#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        #endregion


        #region Constructors

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
            var packetLength = SyncSequence.Length + (EthernetFields.MacAddressLength * MACRepetitions);
            var packetBytes = new byte[packetLength];
            var destinationMACBytes = destinationMAC.GetAddressBytes();

            // write the data to the payload
            // - synchronization sequence (6 bytes)
            // - destination MAC (16 copies of 6 bytes)
            for (var i = 0; i < packetLength; i += EthernetFields.MacAddressLength)
            {
                // copy the syncSequence on the first pass
                if (i == 0)
                {
                    Array.Copy(SyncSequence, 0, packetBytes, i, SyncSequence.Length);
                }
                else
                {
                    Array.Copy(destinationMACBytes, 0, packetBytes, i, EthernetFields.MacAddressLength);
                }
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

        #endregion


        #region Properties

        /// <summary>
        /// The Physical Address (MAC) of the host being woken up from sleep
        /// </summary>
        public PhysicalAddress DestinationMAC
        {
            get
            {
                var destinationMAC = new byte[EthernetFields.MacAddressLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + SyncSequence.Length,
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

        #endregion


        #region Methods

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
        /// Checks the validity of the Wake-On-LAN payload
        /// - by checking the synchronization sequence
        /// - by checking to see if there are 16 iterations of the Destination MAC address
        /// </summary>
        /// <returns>
        /// True if the Wake-On-LAN payload is valid
        /// </returns>
        public bool IsValid()
        {
            return IsValid(Header);
        }

        /// <summary>
        /// See <see cref="IsValid()"/>.
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <returns>
        /// A <see cref="bool" />
        /// </returns>
        public static bool IsValid(ByteArraySegment byteArraySegment)
        {
            // validate the 16 repetitions of the wolDestinationMAC
            // - verify that the wolDestinationMAC address repeats 16 times in sequence
            for (var i = 0; i < EthernetFields.MacAddressLength * MACRepetitions; i += EthernetFields.MacAddressLength)
            {
                var basOffset = byteArraySegment.Offset + i;

                // check the synchronization sequence on the first pass
                if (i == 0)
                {
                    // validate the synchronization sequence
                    for (int j = 0; j < EthernetFields.MacAddressLength; j++)
                    {
                        if (byteArraySegment.Bytes[basOffset + j] != SyncSequence[j])
                            return false;
                    }
                }
                else
                {
                    // fail the validation on malformed WOL Magic Packets
                    for (int j = 0; j < EthernetFields.MacAddressLength; j++)
                    {
                        if (byteArraySegment.Bytes[byteArraySegment.Offset + SyncSequence.Length + j] != byteArraySegment.Bytes[basOffset + j])
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
            if (obj == null || GetType() != obj.GetType())
                return false;


            var wol = (WakeOnLanPacket) obj;

            return DestinationMAC.Equals(wol.DestinationMAC);
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

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if (outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                buffer.AppendFormat("[{0}WakeOnLanPacket{1}: DestinationMAC={2}]",
                                    color,
                                    colorEscape,
                                    DestinationMAC);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    {"destination", HexPrinter.PrintMACAddress(DestinationMAC)}
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("WOL:  ******* WOL - \"Wake-On-Lan\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("WOL:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("WOL: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("WOL:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        #endregion


        #region Members

        // the WOL synchronization sequence
        private static readonly byte[] SyncSequence = {0xff, 0xff, 0xff, 0xff, 0xff, 0xff};

        // the number of times the Destination MAC appears in the payload
        private const int MACRepetitions = 16;

        #endregion
    }
}