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
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Wake-On-Lan
    /// See: http://en.wikipedia.org/wiki/Wake-on-LAN
    /// See: http://wiki.wireshark.org/WakeOnLAN
    /// </summary>
    public class WakeOnLanPacket : Packet
    {

        #region Preprocessor Directives

#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Create a Wake-On-LAN packet from the destination MAC address
        /// </summary>
        /// <param name="destinationMAC">
        /// A <see cref="System.Net.NetworkInformation.PhysicalAddress"/>
        /// </param>
        public WakeOnLanPacket(PhysicalAddress destinationMAC)
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int packetLength = syncSequence.Length + (EthernetFields.MacAddressLength * macRepetitions);
            var packetBytes = new byte[packetLength];
            var destinationMACBytes = destinationMAC.GetAddressBytes();

            // write the data to the payload
            // - synchronization sequence (6 bytes)
            // - destination MAC (16 copies of 6 bytes)
            for(int i = 0; i < packetLength; i+=EthernetFields.MacAddressLength)
            {
                // copy the syncSequence on the first pass
                if(i == 0)
                {
                    Array.Copy(syncSequence, 0, packetBytes, i, syncSequence.Length);
                }
                else
                {
                    Array.Copy(destinationMACBytes, 0, packetBytes, i, EthernetFields.MacAddressLength);
                }
            }

            header = new ByteArraySegment(packetBytes, offset, packetLength);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public WakeOnLanPacket(ByteArraySegment bas)
        {
            log.Debug("");

            if(WakeOnLanPacket.IsValid(bas))
            {
                // set the header field, header field values are retrieved from this byte array
                header = new ByteArraySegment(bas);
                header.Length = Bytes.Length;
            }
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
                byte[] destinationMAC = new byte[EthernetFields.MacAddressLength];
                Array.Copy(header.Bytes, header.Offset + syncSequence.Length,
                           destinationMAC, 0,
                           EthernetFields.MacAddressLength);
                return new PhysicalAddress(destinationMAC);
            }
            set
            {
                byte[] destinationMAC = value.GetAddressBytes();
                Array.Copy(destinationMAC, 0,
                           header.Bytes, header.Offset + syncSequence.Length,
                           EthernetFields.MacAddressLength);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the embedded Wake-On-LAN packet
        ///  or null if there is none
        /// </summary>
        /// <returns>
        /// A Wake-On-LAN packet
        /// </returns>
        [Obsolete("Use Packet.Extract() instead")]
        public static WakeOnLanPacket GetEncapsulated(Packet p)
        {
            // see if we have an ethernet packet that contains a wol packet
            var ethernetPacket = (EthernetPacket)p.Extract(typeof(EthernetPacket));
            if(ethernetPacket != null)
            {
                if(ethernetPacket.Type == EthernetPacketType.WakeOnLan)
                {
                    return (WakeOnLanPacket)ethernetPacket.PayloadPacket;
                }
            }

            // otherwise see if we have a udp packet (might have been sent to port 7 or 9) that
            // contains a wol packet
            var udpPacket = (UdpPacket)p.Extract(typeof(UdpPacket));
            if(udpPacket != null)
            {
                // if the destination port is 7 or 9 then this is already parsed as a
                // WakeOnLan packet so just return it
                if((udpPacket.DestinationPort == 7) || (udpPacket.DestinationPort == 9))
                {
                    return (WakeOnLanPacket)udpPacket.PayloadPacket;
                }
            }

            return null;
        }

        /// <summary>
        /// Generate a random WakeOnLanPacket
        /// </summary>
        /// <returns>
        /// A <see cref="WakeOnLanPacket"/>
        /// </returns>
        public static WakeOnLanPacket RandomPacket()
        {
            var rnd = new Random();

            byte[] destAddress = new byte[EthernetFields.MacAddressLength];

            rnd.NextBytes(destAddress);

            return new WakeOnLanPacket(new PhysicalAddress(destAddress));
        }

        /// <summary>
        /// Checks the validity of the Wake-On-LAN payload
        ///  - by checking the synchronization sequence
        ///  - by checking to see if there are 16 iterations of the Destination MAC address
        /// </summary>
        /// <returns>
        /// True if the Wake-On-LAN payload is valid
        /// </returns>
        public bool IsValid()
        {
            return IsValid(header);
        }

        /// <summary>
        /// See IsValid
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public static bool IsValid(ByteArraySegment bas)
        {
            // fetch the destination MAC from the payload
            byte[] destinationMAC = new byte[EthernetFields.MacAddressLength];
            Array.Copy(bas.Bytes, bas.Offset + syncSequence.Length, destinationMAC, 0, EthernetFields.MacAddressLength);

            // the buffer is used to store both the synchronization sequence
            //  and the MAC address, both of which are the same length (in bytes)
            byte[] buffer = new byte[EthernetFields.MacAddressLength];

            // validate the 16 repetitions of the wolDestinationMAC
            // - verify that the wolDestinationMAC address repeats 16 times in sequence
            for(int i = 0; i<(EthernetFields.MacAddressLength * macRepetitions); i+=EthernetFields.MacAddressLength)
            {
                // Extract the sample from the payload for comparison
                Array.Copy(bas.Bytes, bas.Offset + i, buffer, 0, buffer.Length);

                // check the synchronization sequence on the first pass
                if(i == 0)
                {
                    // validate the synchronization sequence
                    if(!buffer.SequenceEqual(syncSequence))
                        return false;
                }
                else
                {
                    // fail the validation on malformed WOL Magic Packets
                    if(!buffer.SequenceEqual(destinationMAC))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Compare two instances
        /// </summary>
        /// <param name="obj">
        /// A <see cref="System.Object"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            var wol = (WakeOnLanPacket)obj;

            return DestinationMAC.Equals(wol.DestinationMAC);
        }

        /// <summary>
        /// GetHashCode override
        /// </summary>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public override int GetHashCode()
        {
            return header.GetHashCode();
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
                buffer.AppendFormat("[{0}WakeOnLanPacket{1}: DestinationMAC={2}]",
                    color,
                    colorEscape,
                    DestinationMAC);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("destination", HexPrinter.PrintMACAddress(DestinationMAC));

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("WOL:  ******* WOL - \"Wake-On-Lan\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("WOL:");
                foreach(var property in properties)
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
        private static readonly byte[] syncSequence = new byte[6] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

        // the number of times the Destination MAC appears in the payload
        private static readonly int macRepetitions = 16;

        #endregion
    }
}