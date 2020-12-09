/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
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