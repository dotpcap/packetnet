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

namespace PacketDotNet
{
    internal class OSPFv2Fields
    {
        /// <summary> Length of the OSPF packet version in bytes.</summary>
        public static readonly int VersionLength = 1;

        /// <summary> Length of the OSPF header type in bytes.</summary>
        public static readonly int TypeLength = 1;

        /// <summary> Length of the OSPF packet in bytes.</summary>
        public static readonly int PacketLength = 2;

        /// <summary> Length of the OSPF router ID (ip) field in bytes </summary>
        public static readonly int RouterIDLength = 4;

        /// <summary> Length of the OSPF area ID (ip) field in bytes </summary>
        public static readonly int AreaIDLength = 4;

        /// <summary> Length of the OSPF checksum in bytes </summary>
        public static readonly int ChecksumLength = 2;

        /// <summary> Length of the OSPF instance AuType field in bytes </summary>
        public static readonly int AuTypeLength = 2;

        /// <summary> One padding byte at the end of the header </summary>
        public static readonly int AuthorizationLength = 8;

        /// <summary> Position of the OSPF version.</summary>
        public static readonly int VersionPosition;

        /// <summary> Position of the the OSPF packet type.</summary>
        public static readonly int TypePosition;

        /// <summary> Position of the OSPF packet length </summary>
        public static readonly int PacketLengthPosition;

        /// <summary> Position of the RouterID field </summary>
        public static readonly int RouterIDPosition;

        /// <summary> Position of the AreaID field </summary>
        public static readonly int AreaIDPosition;

        /// <summary> Position of the OSPF packet checksum </summary>
        public static readonly int ChecksumPosition;

        /// <summary> Position of the AuType field </summary>
        public static readonly int AuTypePosition;

        /// <summary> Position of the Authorization bytes </summary>
        public static readonly int AuthorizationPosition;

        /// <summary> Length in bytes of an OSPF header.</summary>
        public static readonly int HeaderLength;

        // ------------------ ospf hello packet stuff

        /// <summary> Length of NetworkMask in bytes.</summary>
        public static readonly int NetworkMaskLength = 4;

        /// <summary> Length of the Hello Interaval in bytes.</summary>
        public static readonly int HelloIntervalLength = 2;

        /// <summary> Length of the options in bytes.</summary>
        public static readonly int HelloOptionsLength = 1;

        /// <summary> Length of RTR priority in bytes.</summary>
        public static readonly int RtrPriorityLength = 1;

        /// <summary> Length of the Router Dead Interval field in bytes </summary>
        public static readonly int RouterDeadIntervalLength = 4;

        /// <summary> Length of the Designated Router ID (ip) field in bytes </summary>
        public static readonly int DesignatedRouterIDLength = 4;

        /// <summary> Length of the Backup Designated Router in bytes </summary>
        public static readonly int BackupRouterIDLength = 4;

        /// <summary> Length of the Neighbor ID field in bytes </summary>
        public static readonly int NeighborIDLength = 4;

        /// <summary> Positon of the Networkmask.</summary>
        public static readonly int NetworkMaskPositon;

        /// <summary> Position of the Hello Interaval.</summary>
        public static readonly int HelloIntervalPosition;

        /// <summary> Position of the options.</summary>
        public static readonly int HelloOptionsPosition;

        /// <summary> Position of RTR priority.</summary>
        public static readonly int RtrPriorityPosition;

        /// <summary> Position of the Router Dead Interval.</summary>
        public static readonly int RouterDeadIntervalPosition;

        /// <summary> Position of the Designated Router ID (ip).</summary>
        public static readonly int DesignatedRouterIDPosition;

        /// <summary> Position of the Backup Designated Router.</summary>
        public static readonly int BackupRouterIDPosition;

        /// <summary> Start of the NeighborIDs (zero or more).</summary>
        public static readonly int NeighborIDStart;

        /// <summary> Length of the hello packet.</summary>
        public static readonly int HelloPacketLength;

        // ------------------ ospf database description packet stuff

        /// <summary> Length of InterfaceMTU in bytes.</summary>
        public static readonly int InterfaceMTULength = 2;

        /// <summary> Length of the options in bytes.</summary>
        public static readonly int DBDescriptionOptionsLength = 1;

        /// <summary> Length of optional bits in bytes.</summary>
        public static readonly int BitsLength = 1;

        /// <summary> Length of the DD sequence field in bytes </summary>
        public static readonly int DDSequenceLength = 4;

        /// <summary> Length of the LSA header in bytes </summary>
        public static readonly int LSAHeaderLength = 20;

        /// <summary> Position of InterfaceMTU.</summary>
        public static readonly int InterfaceMTUPosition;

        /// <summary> Position of DB description options.</summary>
        public static readonly int DBDescriptionOptionsPosition;

        /// <summary> Positon of the optional bits.</summary>
        public static readonly int BitsPosition;

        /// <summary> Positon of the DD sequence. </summary>
        public static readonly int DDSequencePosition;

        /// <summary> Positon of the LSA header. </summary>
        public static readonly int LSAHeaderPosition;

        // ------------------ ospf link state request packet stuff
        /// <summary> The start of the link state requests.</summary>
        public static readonly int LSRStart;

        // ------------------ ospf link state update

        /// <summary> Length of the LSA# field</summary>
        public static readonly int LSANumberLength = 4;

        /// <summary> Positon of the LSA#.</summary>
        public static readonly int LSANumberPosition;

        /// <summary> Positon of the LSA Updates.</summary>
        public static readonly int LSAUpdatesPositon;

        // ------------------ ospf link state ack
        /// <summary> Length of the LSA acknowledge</summary>
        public static readonly int LSAAckLength = 20;

        /// <summary> Position of the LSA acknowledge</summary>
        public static readonly int LSAAckPosition;

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