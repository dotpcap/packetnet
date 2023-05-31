/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Lldp;

    /// <summary>
    /// A System Capabilities Tlv
    /// [TLVTypeLength - 2 bytes][System Capabilities - 2 bytes][Enabled Capabilities - 2 bytes]
    /// </summary>
    public class SystemCapabilitiesTlv : Tlv
    {
        private const int EnabledCapabilitiesLength = 2;
        private const int SystemCapabilitiesLength = 2;

        /// <summary>
        /// Creates a System Capabilities Tlv
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The System Capabilities TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public SystemCapabilitiesTlv(byte[] bytes, int offset) :
            base(bytes, offset)
        { }

        /// <summary>
        /// Creates a System Capabilities TLV and sets the value
        /// </summary>
        /// <param name="capabilities">
        /// A bitmap containing the available System Capabilities
        /// </param>
        /// <param name="enabled">
        /// A bitmap containing the enabled System Capabilities
        /// </param>
        public SystemCapabilitiesTlv(ushort capabilities, ushort enabled)
        {
            const int length = TlvTypeLength.TypeLengthLength + SystemCapabilitiesLength + EnabledCapabilitiesLength;
            var bytes = new byte[length];

            Data = new ByteArraySegment(bytes, 0, length);

            Type = TlvType.SystemCapabilities;
            Capabilities = capabilities;
            Enabled = enabled;
        }

        /// <value>
        /// A bitmap containing the available System Capabilities
        /// </value>
        public ushort Capabilities
        {
            get => EndianBitConverter.Big.ToUInt16(Data.Bytes,
                                                   Data.Offset + TlvTypeLength.TypeLengthLength);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Data.Bytes,
                                                    Data.Offset + TlvTypeLength.TypeLengthLength);
        }

        /// <value>
        /// A bitmap containing the Enabled System Capabilities
        /// </value>
        public ushort Enabled
        {
            get => EndianBitConverter.Big.ToUInt16(Data.Bytes,
                                                   Data.Offset + TlvTypeLength.TypeLengthLength + SystemCapabilitiesLength);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Data.Bytes,
                                                    ValueOffset + SystemCapabilitiesLength);
        }

        /// <summary>
        /// Checks whether the system is capable of a certain function
        /// </summary>
        /// <param name="capability">
        /// The capability being checked
        /// </param>
        /// <returns>
        /// Whether or not the system is capable of the function being tested
        /// </returns>
        public bool IsCapable(CapabilityOptions capability)
        {
            var mask = (ushort) capability;
            if ((Capabilities & mask) != 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the specified function has been enabled on the system
        /// </summary>
        /// <param name="capability">
        /// The capability being checked
        /// </param>
        /// <returns>
        /// Whether or not the specified function is enabled
        /// </returns>
        public bool IsEnabled(CapabilityOptions capability)
        {
            var mask = (ushort) capability;
            if ((Enabled & mask) != 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Convert this System Capabilities TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return $"[SystemCapabilities: Capabilities={Capabilities}, Enabled={Enabled}]";
        }
    }