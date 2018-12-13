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
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using PacketDotNet.MiscUtil.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Format of a CTS or an ACK frame
    /// </summary>
    public abstract class ManagementFrame : MacFrame
    {
        /// <summary>
        /// BssID
        /// </summary>
        public PhysicalAddress BssId { get; set; }

        /// <summary>
        /// DestinationAddress
        /// </summary>
        public PhysicalAddress DestinationAddress { get; set; }

        /// <summary>
        /// Sequence control field
        /// </summary>
        public SequenceControlField SequenceControl { get; set; }

        /// <summary>
        /// SourceAddress
        /// </summary>
        public PhysicalAddress SourceAddress { get; set; }


        /// <summary>
        /// Frame control bytes are the first two bytes of the frame
        /// </summary>
        protected UInt16 SequenceControlBytes
        {
            get
            {
                if (Header.Length >= MacFields.SequenceControlPosition + MacFields.SequenceControlLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + MacFields.Address1Position + (MacFields.AddressLength * 3));
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + MacFields.Address1Position + (MacFields.AddressLength * 3));
        }

        /// <summary>
        /// Returns a string with a description of the addresses used in the packet.
        /// This is used as a compoent of the string returned by ToString().
        /// </summary>
        /// <returns>
        /// The address string.
        /// </returns>
        protected override String GetAddressString()
        {
            return $"SA {SourceAddress} DA {DestinationAddress} BSSID {BssId}";
        }
    }
}