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

namespace PacketDotNet
{
    /// <summary>
    /// Information about network link layers.
    /// </summary>
    public class LinkLayer : Packet
    {
#if false
        /// <summary> Fetch the header length associated with various link-layer types.</summary>
        /// <param name="layerType">the link-layer code
        /// </param>
        /// <returns> the length of the header for the specified link-layer
        /// </returns>
        public static int LinkLayerLength(LinkLayers layerType)
        {
            switch (layerType)
            {                
            case LinkLayers.ArcNet: 
                return 6;

            case LinkLayers.Slip:
                return 16;

            case LinkLayers.SlipBSD:
                return 24;

            case LinkLayers.Null:
            case LinkLayers.Loop:
                return 4;

            case LinkLayers.Ppp:
            case LinkLayers.CiscoHDLC:
            case LinkLayers.PppSerial:
                return 4;

            case LinkLayers.PppBSD:
                return 24;

            case LinkLayers.Fddi:
                return 21;

            case LinkLayers.Ieee80211:
                return 22;

            case LinkLayers.AtmRfc1483:
                return 8;

            case LinkLayers.Raw:
                return 0;

            case LinkLayers.AtmClip:
                return 8;

            case LinkLayers.LinuxSLL:
                return 16;

            case LinkLayers.Ethernet:
            default: 
                return 14;
            }
        }

        /// <summary> Fetch the offset into the link-layer header where the protocol code
        /// can be found. Returns -1 if there is no embedded protocol code.
        /// </summary>
        /// <param name="layerType">the link-layer code
        /// </param>
        /// <returns> the offset in bytes
        /// </returns>
        public static int ProtocolOffset(LinkLayers layerType)
        {
            switch (layerType)
            {
            case LinkLayers.ArcNet: 
                return 2;

            case LinkLayers.Slip:
                return - 1;

            case LinkLayers.SlipBSD:
                return - 1;

            case LinkLayers.Null:
            case LinkLayers.Loop:
                return 0;

            case LinkLayers.Ppp:
            case LinkLayers.CiscoHDLC:
            case LinkLayers.PppSerial:
                return 2;

            case LinkLayers.PppBSD:
                return 5;

            case LinkLayers.Fddi:
                return 13;

            case LinkLayers.Ieee80211:
                return 14;

            case LinkLayers.AtmRfc1483:
                return 6;

            case LinkLayers.Raw:
                return - 1;

            case LinkLayers.AtmClip:
                return 6;

            case LinkLayers.LinuxSLL:
                return 14;

            case LinkLayers.Ethernet:
            default: 
                return 12;
            }
        }
#endif
    }
}