/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using PacketDotNet.Utils;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// Transport layer packet
    /// </summary>
    public abstract class TransportPacket : Packet
    {
        /// <summary>
        /// Callback function for UdpPacket Payload decoding.
        /// First parameter is the payload, second parameter if the UdpPacket itself
        /// returned value is the decoded payload as Packet or null if payload is invalid or not supported
        /// then internal decoding will continue
        /// </summary>
        public static Func<ByteArraySegment, TransportPacket, Packet> CustomPayloadDecoder;

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
        /// Options for use when creating a transport layer checksum
        /// </summary>
        public enum TransportChecksumOption
        {
            /// <summary>
            /// No extra options
            /// </summary>
            None,

            /// <summary>
            /// Includes a pseudo IP header to the transport data being checksummed.
            /// </summary>
            IncludePseudoIPHeader
        }

        /// <summary>
        /// The transport packet's checksum.
        /// </summary>
        public abstract ushort Checksum { get; set; }

        /// <summary>Fetch the port number on the target host.</summary>
        public abstract ushort DestinationPort { get; set; }

        /// <summary>Fetch the port number on the source host.</summary>
        public abstract ushort SourcePort { get; set; }

        /// <summary>
        /// Calculates the transport layer checksum, either for the
        /// tcp or udp packet
        /// </summary>
        /// <param name="option">
        ///     <see cref="TransportChecksumOption" />
        /// </param>
        /// <returns>
        /// A <see cref="int" />
        /// </returns>
        internal int CalculateChecksum(TransportChecksumOption option)
        {
            // save the checksum field value so it can be restored, altering the checksum is not
            // an intended side effect of this method
            var originalChecksum = Checksum;

            // reset the checksum field (checksum is calculated when this field is zeroed)
            Checksum = 0;

            // copy the tcp section with data
            var dataToChecksum = ((IPPacket) ParentPacket).PayloadPacket.BytesSegment;

            var bytes = option == TransportChecksumOption.IncludePseudoIPHeader
                ? ((IPPacket) ParentPacket).GetPseudoIPHeader(dataToChecksum.Length)
                : new byte[0];

            // calculate the one's complement sum of the tcp header
            var cs = ChecksumUtils.OnesComplementSum(dataToChecksum, bytes);

            // restore the checksum field value
            Checksum = originalChecksum;

            return cs;
        }

        /// <summary>
        /// Determine if the transport layer checksum is valid
        /// </summary>
        /// <param name="option">
        /// A <see cref="TransportChecksumOption" />
        /// </param>
        /// <returns>
        /// A <see cref="bool" />
        /// </returns>
        public virtual bool IsValidChecksum(TransportChecksumOption option)
        {
            var dataToChecksum = ((IPPacket) ParentPacket).PayloadPacket.BytesSegment;
            if (dataToChecksum.Offset + dataToChecksum.Length > dataToChecksum.BytesLength)
                return false;


            var bytes = option == TransportChecksumOption.IncludePseudoIPHeader
                ? ((IPPacket) ParentPacket).GetPseudoIPHeader(dataToChecksum.Length)
                : new byte[0];

            var onesSum = ChecksumUtils.OnesSum(dataToChecksum, bytes);

            Log.DebugFormat("option: {0}, byteArrayCombination.Length {1}",
                            option,
                            bytes.Length);

            const int expectedOnesSum = 0xFFFF;
            Log.DebugFormat("onesSum {0} expected {1}",
                            onesSum,
                            expectedOnesSum);

            return onesSum == expectedOnesSum;
        }
    }
}