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

using System;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class UnsupportedOption : DhcpV4Option
    {
        private readonly byte[] _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionType">The offset.</param>
        /// <param name="optionLength">The option type.</param>
        public UnsupportedOption(byte[] buffer, int offset, DhcpV4OptionType optionType, int optionLength) : base(optionType)
        {
            var availableBufferLength = buffer.Length - offset;
            if (availableBufferLength < optionLength)
                optionLength = availableBufferLength >= 0 ? availableBufferLength : 0;

            _data = new byte[optionLength];
            
            Array.Copy(buffer, offset, _data, 0, optionLength);
        }

        /// <inheritdoc />
        public override byte[] Data => _data;

        /// <inheritdoc />
        public override int Length => _data.Length;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Unsupported: {OptionType}";
        }
    }
}