/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4;

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