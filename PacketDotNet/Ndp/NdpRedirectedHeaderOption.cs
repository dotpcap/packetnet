/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using PacketDotNet.Utils;

namespace PacketDotNet.Ndp
{
    public class NdpRedirectedHeaderOption : NdpOption
    {
        /// <summary>The offset (in bytes) of the IP header + data field</summary>
        internal const int IPHeaderDataOffset = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="NdpMtuOption" /> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public NdpRedirectedHeaderOption(byte[] bytes, int offset, int length) : base(bytes, offset, length)
        { }

        /// <summary>
        /// Gets or sets the MTU.
        /// </summary>
        public ByteArraySegment IPHeaderAndData => OptionData;
    }
}