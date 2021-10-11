/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System.Collections.Generic;
using PacketDotNet.Utils;
#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// RTP Control Protocol
    /// See: https://en.wikipedia.org/wiki/RTP_Control_Protocol
    /// See: https://wiki.wireshark.org/RTCP
    /// </summary>
    public sealed class RtcpContainerPacket : Packet
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Create from values
        /// </summary>
        public RtcpContainerPacket()
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = RtcpFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public RtcpContainerPacket(ByteArraySegment byteArraySegment, Packet parentPacket)
        {
            Log.Debug("");

            LazyRtcpItems = new LazySlim<List<RtcpItemPacket>>(() =>
            {
                var list = new List<RtcpItemPacket>();
                ByteArraySegment segment = byteArraySegment;
                RtcpItemPacket item;
                do
                {
                    item = new RtcpItemPacket(segment, this);
                    list.Add(item);
                    segment = item.HeaderDataSegment;
                } while (segment.Length > 0);

                return list;
            });

            ParentPacket = parentPacket;
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.BlueBackground;

        internal LazySlim<List<RtcpItemPacket>> LazyRtcpItems { get; } = new (null);

        public List<RtcpItemPacket> RtcpItems => this.LazyRtcpItems.Value;
    }
}
