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
using System;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// Transport layer packet
    /// </summary>
    public abstract class TransportPacket : Packet
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif
        /// <summary>
        /// Constructor
        /// </summary>
        public TransportPacket()
        {
        }

        /// <value>
        /// The Checksum version
        /// </value>
        public abstract ushort Checksum
        {
            get;
            set;
        }

        /// <summary>
        /// Calculates the transport layer checksum, either for the
        /// tcp or udp packet
        /// </summary>
        /// <param name="option"><see cref="TransportPacket.TransportChecksumOption"/></param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        internal int CalculateChecksum(TransportChecksumOption option)
        {
            // save the checksum field value so it can be restored, altering the checksum is not
            // an intended side effect of this method
            var originalChecksum = Checksum;

            // reset the checksum field (checksum is calculated when this field is
            // zeroed)
            Checksum = 0;

            // copy the tcp section with data
            byte[] dataToChecksum = ((IpPacket)ParentPacket).PayloadPacket.Bytes;

             if (option == TransportChecksumOption.AttachPseudoIPHeader)
                dataToChecksum = ((IpPacket)ParentPacket).AttachPseudoIPHeader(dataToChecksum);

            // calculate the one's complement sum of the tcp header
            int cs = ChecksumUtils.OnesComplementSum(dataToChecksum);

            // restore the checksum field value
            Checksum = originalChecksum;

            return cs;
        }

        /// <summary>
        /// Determine if the transport layer checksum is valid
        /// </summary>
        /// <param name="option">
        /// A <see cref="TransportChecksumOption"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public virtual bool IsValidChecksum(TransportChecksumOption option)
        {
            var upperLayer = ((IpPacket)ParentPacket).PayloadPacket.Bytes;

            log.DebugFormat("option: {0}, upperLayer.Length {1}",
                            option, upperLayer.Length);

            if (option == TransportChecksumOption.AttachPseudoIPHeader)
                upperLayer = ((IpPacket)ParentPacket).AttachPseudoIPHeader(upperLayer);

            var onesSum = ChecksumUtils.OnesSum(upperLayer);
            const int expectedOnesSum = 0xffff;
            log.DebugFormat("onesSum {0} expected {1}",
                            onesSum,
                            expectedOnesSum);

            return (onesSum == expectedOnesSum);
        }

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
            /// Attach a pseudo IP header to the transport data being checksummed
            /// </summary>
            AttachPseudoIPHeader,
        }
    }
}
