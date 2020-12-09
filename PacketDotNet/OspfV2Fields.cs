/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    public struct OspfV2Fields
    {
        /// <summary>Length of the OSPF area ID (ip) field in bytes </summary>
        public static readonly int AreaIDLength = 4;

        /// <summary>Position of the AreaId field </summary>
        public static readonly int AreaIDPosition;

        /// <summary>One padding byte at the end of the header </summary>
        public static readonly int AuthorizationLength = 8;

        /// <summary>Position of the Authorization bytes </summary>
        public static readonly int AuthorizationPosition;

        /// <summary>Length of the OSPF instance AuType field in bytes </summary>
        public static readonly int AuTypeLength = 2;

        /// <summary>Position of the AuType field </summary>
        public static readonly int AuTypePosition;

        /// <summary>Length of the Backup Designated Router in bytes </summary>
        public static readonly int BackupRouterIDLength = 4;

        /// <summary>Position of the Backup Designated Router.</summary>
        public static readonly int BackupRouterIDPosition;

        /// <summary>Length of optional bits in bytes.</summary>
        public static readonly int BitsLength = 1;

        /// <summary>Position of the optional bits.</summary>
        public static readonly int BitsPosition;

        /// <summary>Length of the OSPF checksum in bytes </summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Position of the OSPF packet checksum </summary>
        public static readonly int ChecksumPosition;

        /// <summary>Length of the options in bytes.</summary>
        public static readonly int DBDescriptionOptionsLength = 1;

        /// <summary>Position of DB description options.</summary>
        public static readonly int DescriptionOptionsPosition;

        /// <summary>Length of the DD sequence field in bytes </summary>
        public static readonly int DDSequenceLength = 4;

        /// <summary>Position of the DD sequence.</summary>
        public static readonly int DDSequencePosition;

        /// <summary>Length of the Designated Router ID (ip) field in bytes </summary>
        public static readonly int DesignatedRouterIDLength = 4;

        /// <summary>Position of the Designated Router ID (ip).</summary>
        public static readonly int DesignatedRouterIDPosition;

        /// <summary>Length in bytes of an OSPF header.</summary>
        public static readonly int HeaderLength;

        /// <summary>Length of the Hello Interval in bytes.</summary>
        public static readonly int HelloIntervalLength = 2;

        /// <summary>Position of the Hello Interval.</summary>
        public static readonly int HelloIntervalPosition;

        /// <summary>Length of the options in bytes.</summary>
        public static readonly int HelloOptionsLength = 1;

        /// <summary>Position of the options.</summary>
        public static readonly int HelloOptionsPosition;

        /// <summary>Length of the hello packet.</summary>
        public static readonly int HelloPacketLength;

        // ------------------ ospf database description packet stuff

        /// <summary>Length of InterfaceMtu in bytes.</summary>
        public static readonly int InterfaceMTULength = 2;

        /// <summary>Position of InterfaceMtu.</summary>
        public static readonly int InterfaceMTUPosition;

        // ------------------ ospf link state ack
        /// <summary>Length of the LSA acknowledge</summary>
        public static readonly int LSAAckLength = 20;

        /// <summary>Position of the LSA acknowledge</summary>
        public static readonly int LSAAckPosition;

        /// <summary>Length of the LSA header in bytes </summary>
        public static readonly int LSAHeaderLength = 20;

        /// <summary>Position of the LSA header.</summary>
        public static readonly int LSAHeaderPosition;

        // ------------------ ospf link state update

        /// <summary>Length of the LSA# field</summary>
        public static readonly int LSANumberLength = 4;

        /// <summary>Position of the LSA#.</summary>
        public static readonly int LSANumberPosition;

        /// <summary>Position of the LSA Updates.</summary>
        public static readonly int LSAUpdatesPosition;

        // ------------------ ospf link state request packet stuff
        /// <summary>The start of the link state requests.</summary>
        public static readonly int LSRStart;

        /// <summary>Length of the Neighbor ID field in bytes </summary>
        public static readonly int NeighborIDLength = 4;

        /// <summary>Start of the NeighborIDs (zero or more).</summary>
        public static readonly int NeighborIDStart;

        // ------------------ ospf hello packet stuff

        /// <summary>Length of NetworkMask in bytes.</summary>
        public static readonly int NetworkMaskLength = 4;

        /// <summary>Position of the Networkmask.</summary>
        public static readonly int NetworkMaskPosition;

        /// <summary>Length of the OSPF packet in bytes.</summary>
        public static readonly int PacketLength = 2;

        /// <summary>Position of the OSPF packet length </summary>
        public static readonly int PacketLengthPosition;

        /// <summary>Length of the Router Dead Interval field in bytes </summary>
        public static readonly int RouterDeadIntervalLength = 4;

        /// <summary>Position of the Router Dead Interval.</summary>
        public static readonly int RouterDeadIntervalPosition;

        /// <summary>Length of the OSPF router ID (ip) field in bytes </summary>
        public static readonly int RouterIDLength = 4;

        /// <summary>Position of the RouterId field </summary>
        public static readonly int RouterIDPosition;

        /// <summary>Length of RTR priority in bytes.</summary>
        public static readonly int RtrPriorityLength = 1;

        /// <summary>Position of RTR priority.</summary>
        public static readonly int RtrPriorityPosition;

        /// <summary>Length of the OSPF header type in bytes.</summary>
        public static readonly int TypeLength = 1;

        /// <summary>Position of the the OSPF packet type.</summary>
        public static readonly int TypePosition;

        /// <summary>Length of the OSPF packet version in bytes.</summary>
        public static readonly int VersionLength = 1;

        /// <summary>Position of the OSPF version.</summary>
        public static readonly int VersionPosition;

        static OspfV2Fields()
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
            NetworkMaskPosition = HeaderLength;
            HelloIntervalPosition = NetworkMaskPosition + NetworkMaskLength;
            HelloOptionsPosition = HelloIntervalPosition + HelloIntervalLength;
            RtrPriorityPosition = HelloOptionsPosition + HelloOptionsLength;
            RouterDeadIntervalPosition = RtrPriorityPosition + RtrPriorityLength;
            DesignatedRouterIDPosition = RouterDeadIntervalPosition + RouterDeadIntervalLength;
            BackupRouterIDPosition = DesignatedRouterIDPosition + DesignatedRouterIDLength;
            NeighborIDStart = BackupRouterIDPosition + BackupRouterIDLength;
            HelloPacketLength = NeighborIDStart + NeighborIDLength;

            // ------------------ ospf database description packet stuff
            InterfaceMTUPosition = HeaderLength;
            DescriptionOptionsPosition = InterfaceMTUPosition + InterfaceMTULength;
            BitsPosition = DescriptionOptionsPosition + DBDescriptionOptionsLength;
            DDSequencePosition = BitsPosition + BitsLength;
            LSAHeaderPosition = DDSequencePosition + DDSequenceLength;

            // ------------------ ospf link state request packet stuff
            LSRStart = HeaderLength;

            // ------------------ ospf link state update
            LSANumberPosition = HeaderLength;
            LSAUpdatesPosition = LSANumberLength + LSANumberPosition;

            // ------------------ ospf link state ack
            LSAAckPosition = HeaderLength;
        }
    }
}