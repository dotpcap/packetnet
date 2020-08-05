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
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class ParameterListOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterListOption" /> class.
        /// </summary>
        /// <param name="parameterList">The parameter list.</param>
        public ParameterListOption(HashSet<DhcpV4OptionType> parameterList) : base(DhcpV4OptionType.ParameterList)
        {
            ParameterList = parameterList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterListOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public ParameterListOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.ParameterList)
        {
            ParameterList = new HashSet<DhcpV4OptionType>();

            for (int i = 0; i < optionLength; i++)
            {
                if (offset + i < buffer.Length)
                    ParameterList.Add((DhcpV4OptionType) buffer[offset + i]);
            }
        }

        /// <inheritdoc />
        public override byte[] Data => ParameterList.Select(x => (byte) x).ToArray();

        /// <inheritdoc />
        public override int Length => ParameterList.Count;

        /// <summary>
        /// Gets or sets the parameter list.
        /// </summary>
        public HashSet<DhcpV4OptionType> ParameterList { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Parameter List: {ParameterList.Count} - {String.Join(", ", ParameterList)}";
        }
    }
}