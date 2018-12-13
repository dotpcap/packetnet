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
 *  Copyright 2011 Georgi Baychev <georgi.baychev@gmail.com>
 */

using System;

namespace PacketDotNet
{
    internal class OSPFv2Fields
    {
        /// <summary> Length of the OSPF area ID (ip) field in bytes </summary>
        public static readonly Int32 AreaIDLength = 4;

        /// <summary> Position of the AreaID field </summary>
        public static readonly Int32 AreaIDPosition;

        /// <summary> One padding byte at the end of the header </summary>
        public static readonly Int32 AuthorizationLength = 8;

        /// <summary> Position of the Authorization bytes </summary>
        public static readonly Int32 AuthorizationPosition;

        /// <summary> Length of the OSPF instance AuType field in bytes </summary>
        public static readonly Int32 AuTypeLength = 2;

        /// <summary> Position of the AuType field </summary>
        public static readonly Int32 AuTypePosition;

        /// <summary> Length of the Backup Designated Router in bytes </summary>
        public static readonly Int32 BackupRouterIDLength = 4;

        /// <summary> Position of the Backup Designated Router.</summary>
        public static readonly Int32 BackupRouterIDPosition;

        /// <summary> Length of optional bits in bytes.</summary>
        public static readonly Int32 BitsLength = 1;

        /// <summary> Positon of the optional bits.</summary>
        public static readonly Int32 BitsPosition;

        /// <summary> Length of the OSPF checksum in bytes </summary>
        public static readonly Int32 ChecksumLength = 2;

        /// <summary> Position of the OSPF packet checksum </summary>
        public static readonly Int32 ChecksumPosition;

        /// <summary> Length of the options in bytes.</summary>
        public static readonly Int32 DBDescriptionOptionsLength = 1;

        /// <summary> Position of DB description options.</summary>
        public static readonly Int32 DBDescriptionOptionsPosition;

        /// <summary> Length of the DD sequence field in bytes </summary>
        public static readonly Int32 DDSequenceLength = 4;

        /// <summary> Positon of the DD sequence. </summary>
        public static readonly Int32 DDSequencePosition;

        /// <summary> Length of the Designated Router ID (ip) field in bytes </summary>
        public static readonly Int32 DesignatedRouterIDLength = 4;

        /// <summary> Position of the Designated Router ID (ip).</summary>
        public static readonly Int32 DesignatedRouterIDPosition;

        /// <summary> Length in bytes of an OSPF header.</summary>
        public static readonly Int32 HeaderLength;

        /// <summary> Length of the Hello Interaval in bytes.</summary>
        public static readonly Int32 HelloIntervalLength = 2;

        /// <summary> Position of the Hello Interaval.</summary>
        public static readonly Int32 HelloIntervalPosition;

        /// <summary> Length of the options in bytes.</summary>
        public static readonly Int32 HelloOptionsLength = 1;

        /// <summary> Position of the options.</summary>
        public static readonly Int32 HelloOptionsPosition;

        /// <summary> Length of the hello packet.</summary>
        public static readonly Int32 HelloPacketLength;

        // ------------------ ospf database description packet stuff

        /// <summary> Length of InterfaceMTU in bytes.</summary>
        public static readonly Int32 InterfaceMTULength = 2;

        /// <summary> Position of InterfaceMTU.</summary>
        public static readonly Int32 InterfaceMTUPosition;

        // ------------------ ospf link state ack
        /// <summary> Length of the LSA acknowledge</summary>
        public static readonly Int32 LSAAckLength = 20;

        /// <summary> Position of the LSA acknowledge</summary>
        public static readonly Int32 LSAAckPosition;

        /// <summary> Length of the LSA header in bytes </summary>
        public static readonly Int32 LSAHeaderLength = 20;

        /// <summary> Positon of the LSA header. </summary>
        public static readonly Int32 LSAHeaderPosition;

        // ------------------ ospf link state update

        /// <summary> Length of the LSA# field</summary>
        public static readonly Int32 LSANumberLength = 4;

        /// <summary> Positon of the LSA#.</summary>
        public static readonly Int32 LSANumberPosition;

        /// <summary> Positon of the LSA Updates.</summary>
        public static readonly Int32 LSAUpdatesPositon;

        // ------------------ ospf link state request packet stuff
        /// <summary> The start of the link state requests.</summary>
        public static readonly Int32 LSRStart;

        /// <summary> Length of the Neighbor ID field in bytes </summary>
        public static readonly Int32 NeighborIDLength = 4;

        /// <summary> Start of the NeighborIDs (zero or more).</summary>
        public static readonly Int32 NeighborIDStart;

        // ------------------ ospf hello packet stuff

        /// <summary> Length of NetworkMask in bytes.</summary>
        public static readonly Int32 NetworkMaskLength = 4;

        /// <summary> Positon of the Networkmask.</summary>
        public static readonly Int32 NetworkMaskPositon;

        /// <summary> Length of the OSPF packet in bytes.</summary>
        public static readonly Int32 PacketLength = 2;

        /// <summary> Position of the OSPF packet length </summary>
        public static readonly Int32 PacketLengthPosition;

        /// <summary> Length of the Router Dead Interval field in bytes </summary>
        public static readonly Int32 RouterDeadIntervalLength = 4;

        /// <summary> Position of the Router Dead Interval.</summary>
        public static readonly Int32 RouterDeadIntervalPosition;

        /// <summary> Length of the OSPF router ID (ip) field in bytes </summary>
        public static readonly Int32 RouterIDLength = 4;

        /// <summary> Position of the RouterID field </summary>
        public static readonly Int32 RouterIDPosition;

        /// <summary> Length of RTR priority in bytes.</summary>
        public static readonly Int32 RtrPriorityLength = 1;

        /// <summary> Position of RTR priority.</summary>
        public static readonly Int32 RtrPriorityPosition;

        /// <summary> Length of the OSPF header type in bytes.</summary>
        public static readonly Int32 TypeLength = 1;

        /// <summary> Position of the the OSPF packet type.</summary>
        public static readonly Int32 TypePosition;

        /// <summary> Length of the OSPF packet version in bytes.</summary>
        public static readonly Int32 VersionLength = 1;

        /// <summary> Position of the OSPF version.</summary>
        public static readonly Int32 VersionPosition;

        static OSPFv2Fields()
        {
            VersionPosition = 0;
            TypePosition = VersionPosition + VersionLength;
            PacketLengthPosition = TypePosition + TypeLength;
            RouterIDPosition = PacketLengthPosition + PacketLength;
            AreaIDPosition = RouterIDPosition + RouterIDLength;
            ChecksumPosition = AreaIDPosition + AreaIDLength;
            AuTypePosition = ChecksumPosition + ChecksumLength;
            AuthorizationPosition = AuTypePosition + AuTypeLength;
            HeaderLength = AuthorizationPosition + AuthorizationLength;

            // ------------------ ospf hello packet stuff
            NetworkMaskPositon = HeaderLength;
            HelloIntervalPosition = NetworkMaskPositon + NetworkMaskLength;
            HelloOptionsPosition = HelloIntervalPosition + HelloIntervalLength;
            RtrPriorityPosition = HelloOptionsPosition + HelloOptionsLength;
            RouterDeadIntervalPosition = RtrPriorityPosition + RtrPriorityLength;
            DesignatedRouterIDPosition = RouterDeadIntervalPosition + RouterDeadIntervalLength;
            BackupRouterIDPosition = DesignatedRouterIDPosition + DesignatedRouterIDLength;
            NeighborIDStart = BackupRouterIDPosition + BackupRouterIDLength;
            HelloPacketLength = NeighborIDStart + NeighborIDLength;

            // ------------------ ospf database description packet stuff
            InterfaceMTUPosition = HeaderLength;
            DBDescriptionOptionsPosition = InterfaceMTUPosition + InterfaceMTULength;
            BitsPosition = DBDescriptionOptionsPosition + DBDescriptionOptionsLength;
            DDSequencePosition = BitsPosition + BitsLength;
            LSAHeaderPosition = DDSequencePosition + DDSequenceLength;

            // ------------------ ospf link state request packet stuff
            LSRStart = HeaderLength;

            // ------------------ ospf link state update
            LSANumberPosition = HeaderLength;
            LSAUpdatesPositon = LSANumberLength + LSANumberPosition;

            // ------------------ ospf link state ack
            LSAAckPosition = HeaderLength;
        }
    }
}