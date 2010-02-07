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
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Internet Link layer packet
    /// See http://en.wikipedia.org/wiki/Link_Layer
    /// </summary>
    public class InternetLinkLayerPacket : Packet
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        public InternetLinkLayerPacket(PosixTimeval timeval) : base(timeval)
        {}

        /// <summary>
        /// Look for the innermost payload. This method is useful because
        /// while some packets are LinuxSSL->IpPacket or
        /// EthernetPacket->IpPacket, there are some packets that are
        /// EthernetPacket->PPPoEPacket->PPPPacket->IpPacket, and for these cases
        /// we really want to get to the IpPacket
        /// </summary>
        /// <returns>
        /// A <see cref="Packet"/>
        /// </returns>
        public static Packet GetInnerPayload(InternetLinkLayerPacket packet)
        {
            // is this an ethernet packet?
            if(packet is EthernetPacket)
            {
                log.Debug("packet is EthernetPacket");

                var thePayload = packet.PayloadPacket;

                // is this packets payload a PPPoEPacket? If so,
                // the PPPoEPacket payload should be a PPPPacket and we want
                // the payload of the PPPPpacket
                if(thePayload is PPPoEPacket)
                {
                    log.Debug("thePayload is PPPoEPacket");
                    return thePayload.PayloadPacket.PayloadPacket;
                }

                return thePayload;
            } else
            {
                log.Debug("else");
                return packet.PayloadPacket;
            }
        }
    }
}