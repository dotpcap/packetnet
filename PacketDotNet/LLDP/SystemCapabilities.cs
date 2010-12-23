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
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A System Capabilities TLV
    ///
    /// [TLVTypeLength - 2 bytes][System Capabilities - 2 bytes][Enabled Capabilities - 2 bytes]
    /// </summary>
    public class SystemCapabilities : TLV
    {
        private const int SystemCapabilitiesLength = 2;
        private const int EnabledCapabilitiesLength = 2;

        #region Constructors

        /// <summary>
        /// Creates a System Capabilities TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The System Capabilities TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public SystemCapabilities(byte[] bytes, int offset) :
            base(bytes, offset)
        {}

        /// <summary>
        /// Creates a System Capabilities TLV and sets the value
        /// </summary>
        /// <param name="capabilities">
        /// A bitmap containing the available System Capabilities
        /// </param>
        /// <param name="enabled">
        /// A bitmap containing the enabled System Capabilities
        /// </param>
        public SystemCapabilities(ushort capabilities, ushort enabled)
        {
            var length = TLVTypeLength.TypeLengthLength + SystemCapabilitiesLength + EnabledCapabilitiesLength;
            var bytes = new byte[length];
            var offset = 0;
            tlvData = new ByteArraySegment(bytes, offset, length);

            Type = TLVTypes.SystemCapabilities;
            Capabilities = capabilities;
            Enabled = enabled;
        }

        #endregion

        #region Properties

        /// <value>
        /// A bitmap containing the available System Capabilities
        /// </value>
        public ushort Capabilities
        {
            get
            {
                // get the capabilities
                return BigEndianBitConverter.Big.ToUInt16(tlvData.Bytes,
                                                          tlvData.Offset + TLVTypeLength.TypeLengthLength);
            }
            set
            {
                // set the capabilities
                EndianBitConverter.Big.CopyBytes(value,
                                                 tlvData.Bytes,
                                                 tlvData.Offset + TLVTypeLength.TypeLengthLength);
            }
        }

        /// <value>
        /// A bitmap containing the Enabled System Capabilities
        /// </value>
        public ushort Enabled
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(tlvData.Bytes,
                                                       tlvData.Offset + TLVTypeLength.TypeLengthLength + SystemCapabilitiesLength);
            }

            set
            {
                // Add the length of the previous field, the SystemCapabilities field, to get
                // to the location of the EnabledCapabilities
                EndianBitConverter.Big.CopyBytes(value, tlvData.Bytes,
                                                 ValueOffset + SystemCapabilitiesLength);
            }
        }

        #endregion

        #region Methods

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
            ushort mask = (ushort)capability;
            if ((Capabilities & mask) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            ushort mask = (ushort)capability;
            if ((Enabled & mask) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Convert this System Capabilities TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[SystemCapabilities: Capabilities={0}, Enabled={1}]", Capabilities, Enabled);
        }

        #endregion
    }
}