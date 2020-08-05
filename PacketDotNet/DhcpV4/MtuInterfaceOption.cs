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
 *  This file is licensed under the Apache License, Version 2.0.
 */

using PacketDotNet.Utils.Converters;

namespace PacketDotNet.DhcpV4
{
    public class MtuInterfaceOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MtuInterfaceOption" /> class.
        /// </summary>
        /// <param name="mtu">The MTU of the interface.</param>
        public MtuInterfaceOption(ushort mtu) : base(DhcpV4OptionType.MTUInterface)
        {
            Mtu = mtu;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MtuInterfaceOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        public MtuInterfaceOption(byte[] buffer, int offset) : base(DhcpV4OptionType.MTUInterface)
        {
            if (offset + 2 < buffer.Length)
                Mtu = EndianBitConverter.Big.ToUInt16(buffer, offset);
        }

        /// <inheritdoc />
        public override byte[] Data => EndianBitConverter.Big.GetBytes(Mtu);

        /// <inheritdoc />
        public override int Length => 2;

        /// <summary>
        /// Gets or sets the MTU of the interface.
        /// </summary>
        public ushort Mtu { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"MTU Interface: {Mtu}";
        }
    }
}