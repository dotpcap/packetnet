/*
This file is part of Packet.Net

Packet.Net is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Packet.Net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Packet.Net.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
﻿using System;
using System.Net.NetworkInformation;

namespace Packet.Net
{
    public class EthernetPacket : DataLinkPacket
    {
        /// <summary> MAC address of the host where the packet originated from.</summary>
        public virtual PhysicalAddress SourceHwAddress
        {
            get
            {
                byte[] hwAddress = new byte[EthernetFields.MacAddressLength];
                Array.Copy(_bytes, EthernetFields.SourceMacPosition,
                           hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }
            set
            {
                byte[] hwAddress = value.GetAddressBytes();
                if(hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                               + " not equal to the expected length of "
                                                               + macAddressLength);
                }

                Array.Copy(hwAddress, 0, _bytes, EthernetFields_Fields.ETH_SRC_POS,
                           hwAddress.Length);
            }
        }

        /// <summary> MAC address of the host where the packet originated from.</summary>
        public virtual PhysicalAddress DestinationHwAddress
        {
            get
            {
                byte[] hwAddress = new byte[macAddressLength];
                Array.Copy(_bytes, EthernetFields_Fields.ETH_DST_POS,
                           hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }
            set
            {
                byte[] hwAddress = value.GetAddressBytes();
                if(hwAddress.Length != macAddressLength)
                {
                    throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                               + " not equal to the expected length of "
                                                               + macAddressLength);
                }

                Array.Copy(hwAddress, 0, _bytes, EthernetFields_Fields.ETH_DST_POS,
                           hwAddress.Length);
            }
        }

        /// <value>
        /// Type of packet that this ethernet packet encapsulates
        /// </value>
        public virtual EthernetPacketType EthernetProtocol
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override byte[] Bytes
        {
            get 
            { 
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Construct a new ethernet packet from source and destination mac addresses
        /// </summary>
        public EthernetPacket(PhysicalAddress SourceHwAddress,
                              PhysicalAddress DestinationHwAddress,
                              EthernetPacketType ethernetPacketType,
                              byte[] EthernetPayload)
        {
#if false
            int ethernetPayloadLength = 0;
            if(EthernetPayload != null)
            {
                ethernetPayloadLength = EthernetPayload.Length;
            }

            _bytes = new byte[EthernetFields_Fields.ETH_HEADER_LEN + ethernetPayloadLength];
            _ethernetHeaderLength = EthernetFields_Fields.ETH_HEADER_LEN;
            _ethPayloadOffset = _ethernetHeaderLength;

            // if we have a payload, copy it into the byte array
            if(EthernetPayload != null)
            {
                Array.Copy(EthernetPayload, 0, _bytes, EthernetFields_Fields.ETH_HEADER_LEN, EthernetPayload.Length);
            }
#endif

            // set the instance values
            this.SourceHwAddress = SourceHwAddress;
            this.DestinationHwAddress = DestinationHwAddress;
            this.EthernetProtocol = ethernetPacketType;
        }

        /// <summary> Convert this ethernet packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this ethernet packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("EthernetPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences_Fields.RESET);
            buffer.Append(": ");
            buffer.Append(SourceHwAddress + " -> " + DestinationHwAddress);
            buffer.Append(" proto=" + EthernetProtocol.ToString() + " (0x" + System.Convert.ToString((ushort)EthernetProtocol, 16) + ")");
            buffer.Append(" l=" + EthernetHeaderLength); // + "," + data.length);
            buffer.Append(']');

            // append the base output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary> Convert this IP packet to a more verbose string.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            //TODO: just output the colored output for now
            return ToColoredString(colored);
        }

#if false
        new internal static Packet Parse(byte[] bytes)
        {
            byte[] header = new byte[14];
            byte[] payload = new byte[bytes.Length - 14];

            Array.Copy(bytes, header, 14);
            Array.Copy(bytes, 14, payload, 0, payload.Length);

            ushort ether_type = BitConverter.ToUInt16(new byte[] { header[13], header[12] }, 0);

            EthernetPacket eth_packet = new EthernetPacket();
            eth_packet.SetPacketHeader(header);

            eth_packet.PayloadData = payload;

            // This could be put in a try+catch block because there may be no Network Protocol encapsulated here
            eth_packet.PayloadPacket = (NetworkingPacket)NetworkingPacket.Parse((NetworkingProtocols)ether_type, payload);

            return eth_packet;
        }
#endif
    }
}
