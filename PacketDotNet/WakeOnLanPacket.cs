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
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using PacketDotNet.Ethernet;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    ///     Wake-On-Lan
    ///     See: http://en.wikipedia.org/wiki/Wake-on-LAN
    ///     See: http://wiki.wireshark.org/WakeOnLAN
    /// </summary>
    public class WakeOnLanPacket : Packet
    {
        #region Preprocessor Directives

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

        #endregion

        #region Constructors

        /// <summary>
        ///     Create a Wake-On-LAN packet from the destination MAC address
        /// </summary>
        /// <param name="destinationMAC">
        ///     A <see cref="System.Net.NetworkInformation.PhysicalAddress" />
        /// </param>
        public WakeOnLanPacket(PhysicalAddress destinationMAC)
        {
            Log.Debug("");

            // allocate memory for this packet
            Int32 offset = 0;
            Int32 packetLength = SyncSequence.Length + (EthernetFields.MacAddressLength * MACRepetitions);
            var packetBytes = new Byte[packetLength];
            var destinationMACBytes = destinationMAC.GetAddressBytes();

            // write the data to the payload
            // - synchronization sequence (6 bytes)
            // - destination MAC (16 copies of 6 bytes)
            for (Int32 i = 0; i < packetLength; i += EthernetFields.MacAddressLength)
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

            this.HeaderByteArraySegment = new ByteArraySegment(packetBytes, offset, packetLength);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public WakeOnLanPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            if (IsValid(bas))
            {
                // set the header field, header field values are retrieved from this byte array
                this.HeaderByteArraySegment = new ByteArraySegment(bas)
                {
                    Length = this.Bytes.Length
                };
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     The Physical Address (MAC) of the host being woken up from sleep
        /// </summary>
        public PhysicalAddress DestinationMAC
        {
            get
            {
                Byte[] destinationMAC = new Byte[EthernetFields.MacAddressLength];
                Array.Copy(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + SyncSequence.Length,
                    destinationMAC, 0,
                    EthernetFields.MacAddressLength);
                return new PhysicalAddress(destinationMAC);
            }
            set
            {
                Byte[] destinationMAC = value.GetAddressBytes();
                Array.Copy(destinationMAC, 0,
                    this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + SyncSequence.Length,
                    EthernetFields.MacAddressLength);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Generate a random WakeOnLanPacket
        /// </summary>
        /// <returns>
        ///     A <see cref="WakeOnLanPacket" />
        /// </returns>
        public static WakeOnLanPacket RandomPacket()
        {
            var rnd = new Random();

            Byte[] destAddress = new Byte[EthernetFields.MacAddressLength];

            rnd.NextBytes(destAddress);

            return new WakeOnLanPacket(new PhysicalAddress(destAddress));
        }

        /// <summary>
        ///     Checks the validity of the Wake-On-LAN payload
        ///     - by checking the synchronization sequence
        ///     - by checking to see if there are 16 iterations of the Destination MAC address
        /// </summary>
        /// <returns>
        ///     True if the Wake-On-LAN payload is valid
        /// </returns>
        public Boolean IsValid()
        {
            return IsValid(this.HeaderByteArraySegment);
        }

        /// <summary>
        ///     See IsValid
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        /// <returns>
        ///     A <see cref="System.Boolean" />
        /// </returns>
        public static Boolean IsValid(ByteArraySegment bas)
        {
            // fetch the destination MAC from the payload
            Byte[] destinationMAC = new Byte[EthernetFields.MacAddressLength];
            Array.Copy(bas.Bytes, bas.Offset + SyncSequence.Length, destinationMAC, 0, EthernetFields.MacAddressLength);

            // the buffer is used to store both the synchronization sequence
            //  and the MAC address, both of which are the same length (in bytes)
            Byte[] buffer = new Byte[EthernetFields.MacAddressLength];

            // validate the 16 repetitions of the wolDestinationMAC
            // - verify that the wolDestinationMAC address repeats 16 times in sequence
            for (Int32 i = 0;
                i < (EthernetFields.MacAddressLength * MACRepetitions);
                i += EthernetFields.MacAddressLength)
            {
                // Extract the sample from the payload for comparison
                Array.Copy(bas.Bytes, bas.Offset + i, buffer, 0, buffer.Length);

                // check the synchronization sequence on the first pass
                if (i == 0)
                {
                    // validate the synchronization sequence
                    if (!buffer.SequenceEqual(SyncSequence))
                        return false;
                }
                else
                {
                    // fail the validation on malformed WOL Magic Packets
                    if (!buffer.SequenceEqual(destinationMAC))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Compare two instances
        /// </summary>
        /// <param name="obj">
        ///     A <see cref="System.Object" />
        /// </param>
        /// <returns>
        ///     A <see cref="System.Boolean" />
        /// </returns>
        public override Boolean Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            var wol = (WakeOnLanPacket) obj;

            return this.DestinationMAC.Equals(wol.DestinationMAC);
        }

        /// <summary>
        ///     GetHashCode override
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Int32" />
        /// </returns>
        public override Int32 GetHashCode()
        {
            return this.HeaderByteArraySegment.GetHashCode();
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            String color = "";
            String colorEscape = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = this.Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                    buffer.AppendFormat("[{0}WakeOnLanPacket{1}: DestinationMAC={2}]",
                        color,
                        colorEscape, this.DestinationMAC);
                    break;
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    Dictionary<String, String> properties = new Dictionary<String, String>
                    {
                        {"destination", HexPrinter.PrintMACAddress(this.DestinationMAC)}
                    };

                    // calculate the padding needed to right-justify the property names
                    Int32 padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("WOL:  ******* WOL - \"Wake-On-Lan\" - offset=? length=" +
                                      this.TotalPacketLength);
                    buffer.AppendLine("WOL:");
                    foreach (var property in properties)
                    {
                        buffer.AppendLine("WOL: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }

                    buffer.AppendLine("WOL:");
                    break;
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        #endregion

        #region Members

        // the WOL synchronization sequence
        private static readonly Byte[] SyncSequence = new Byte[6] {0xff, 0xff, 0xff, 0xff, 0xff, 0xff};

        // the number of times the Destination MAC appears in the payload
        private static readonly Int32 MACRepetitions = 16;

        #endregion
    }
}