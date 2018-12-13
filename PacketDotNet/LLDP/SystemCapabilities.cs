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
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A System Capabilities TLV
    /// [TLVTypeLength - 2 bytes][System Capabilities - 2 bytes][Enabled Capabilities - 2 bytes]
    /// </summary>
    [Serializable]
    public class SystemCapabilities : TLV
    {
        private const Int32 EnabledCapabilitiesLength = 2;
        private const Int32 SystemCapabilitiesLength = 2;


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
        public SystemCapabilities(Byte[] bytes, Int32 offset) :
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
        public SystemCapabilities(UInt16 capabilities, UInt16 enabled)
        {
            var length = TLVTypeLength.TypeLengthLength + SystemCapabilitiesLength + EnabledCapabilitiesLength;
            var bytes = new Byte[length];
            var offset = 0;
            TLVData = new ByteArraySegment(bytes, offset, length);

            Type = TLVTypes.SystemCapabilities;
            Capabilities = capabilities;
            Enabled = enabled;
        }

        #endregion


        #region Properties

        /// <value>
        /// A bitmap containing the available System Capabilities
        /// </value>
        public UInt16 Capabilities
        {
            get => EndianBitConverter.Big.ToUInt16(TLVData.Bytes,
                                                   TLVData.Offset + TLVTypeLength.TypeLengthLength);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    TLVData.Bytes,
                                                    TLVData.Offset + TLVTypeLength.TypeLengthLength);
        }

        /// <value>
        /// A bitmap containing the Enabled System Capabilities
        /// </value>
        public UInt16 Enabled
        {
            get => EndianBitConverter.Big.ToUInt16(TLVData.Bytes,
                                                   TLVData.Offset + TLVTypeLength.TypeLengthLength + SystemCapabilitiesLength);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    TLVData.Bytes,
                                                    ValueOffset + SystemCapabilitiesLength);
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
        public Boolean IsCapable(CapabilityOptions capability)
        {
            var mask = (UInt16) capability;
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
        public Boolean IsEnabled(CapabilityOptions capability)
        {
            var mask = (UInt16) capability;
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
        public override String ToString()
        {
            return $"[SystemCapabilities: Capabilities={Capabilities}, Enabled={Enabled}]";
        }

        #endregion
    }
}