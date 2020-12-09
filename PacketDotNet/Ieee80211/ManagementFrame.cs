/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System.Net.NetworkInformation;
using PacketDotNet.Utils.Converters;

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
        protected ushort SequenceControlBytes
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
        /// This is used as a component of the string returned by ToString().
        /// </summary>
        /// <returns>
        /// The address string.
        /// </returns>
        protected override string GetAddressString()
        {
            return $"SA {SourceAddress} DA {DestinationAddress} BSSID {BssId}";
        }
    }
}