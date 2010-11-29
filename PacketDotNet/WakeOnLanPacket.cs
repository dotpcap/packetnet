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
using System.Net.NetworkInformation;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Wake-On-Lan
    /// See: http://en.wikipedia.org/wiki/Wake-on-LAN
    /// </summary>
    public class WakeOnLanPacket : Packet
    {

        #region Preprocessor Directives

#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Create a Wake-On-LAN packet from the destination MAC address
        /// </summary>
        /// <param name="destinationMAC">
        /// A <see cref="System.Net.NetworkInformation.PhysicalAddress"/>
        /// </param>
        public WakeOnLanPacket(PhysicalAddress destinationMAC) : base(new PosixTimeval())
        {
            log.Debug("");

            int payloadLength = syncSequence.Length + (EthernetFields.MacAddressLength * 16);
            byte[] payload = new byte[payloadLength];
            byte[] destinationMACBytes = destinationMAC.GetAddressBytes();

            // write the data to the payload
            // - synchronization sequence (6 bytes)
            // - destination MAC (16 copies of 6 bytes)
            for(int i = 0; i < payloadLength; i++)
            {
                // copy the syncSequence on the first pass
                if(i == 0)
                {
                    Array.Copy(syncSequence, 0, payload, i, syncSequence.Length);
                }
                else
                {
                    Array.Copy(destinationMACBytes, 0, payload, i, EthernetFields.MacAddressLength);
                }
            }

            // Assigh the newly created payload array to the
            //  packet's PayloadData property
            PayloadData = payload;
        }

        /// <summary>
        /// Creates a Wake-On-LAN packet from a byte[]
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public WakeOnLanPacket(byte[] bytes, int offset) :
            this(bytes, offset, new PosixTimeval())
        {
            log.Debug("");
            this.PayloadData = bytes;
        }

        /// <summary>
        /// Creates a Wake-On-LAN packet from a byte[]
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        public WakeOnLanPacket(byte[] bytes, int offset, PosixTimeval timeval) :
            base(timeval)
        {
            log.Debug("");

            header = new ByteArraySegment(bytes, offset, bytes.Length - offset);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Physical Address (MAC) of the host being woken up from sleep
        /// </summary>
        public PhysicalAddress DestinationMAC
        {
            get
            {
                byte[] destinationMAC = new byte[EthernetFields.MacAddressLength];
                Array.Copy(this.PayloadData, syncSequence.Length, destinationMAC, 0, EthernetFields.MacAddressLength);
                return new PhysicalAddress(destinationMAC);
            }
            set
            {
                byte[] destinationMAC = value.GetAddressBytes();
                Array.Copy(destinationMAC, 0, this.PayloadData, syncSequence.Length, EthernetFields.MacAddressLength);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the embedded Wake-On-LAN packet
        ///  or null if there is none
        /// </summary>
        /// <returns>
        /// A Wake-On-LAN packet
        /// </returns>
        public static WakeOnLanPacket GetEncapsulated(Packet p)
        {
            if(p is EthernetPacket)
            {
                var payload = EthernetPacket.GetInnerPayload((InternetLinkLayerPacket)p);

                if(((EthernetPacket)p).Type == EthernetPacketType.WakeOnLan)
                {
                    if(WakeOnLanPacket.IsValid(p.PayloadData))
                        return new WakeOnLanPacket(p.PayloadData, 0);
                }

                if(payload != null && payload is IpPacket)
                {
                    var innerPayload = payload.PayloadPacket;

                    if(innerPayload != null && innerPayload is UdpPacket)
                    {
                        if(innerPayload.PayloadData != null && WakeOnLanPacket.IsValid(innerPayload.PayloadData))
                        {
                            return new WakeOnLanPacket(innerPayload.PayloadData, 0);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks the validity of the Wake-On-LAN payload
        ///  - by checking the synchronization sequence
        ///  - by checking to see if there are 16 iterations of the Destination MAC address
        /// </summary>
        /// <returns>
        /// True if the Wake-On-LAN payload is valid
        /// </returns>
        public bool IsValid()
        {
            return IsValid(this.PayloadData);
        }

        public static bool IsValid(byte[] payloadData)
        {
            // fetch the destination MAC from the payload
            byte[] destinationMAC = new byte[EthernetFields.MacAddressLength];
            Array.Copy(payloadData, syncSequence.Length, destinationMAC, 0, EthernetFields.MacAddressLength);

            // 6 is the length (in bytes) of both the syncSequence and EthernetFields.MacAddressLength
            byte[] buffer = new byte[6];

            // validate the 16 repetitions of the wolDestinationMAC
            // - verify that the wolDestinationMAC address repeats 16 times in sequence
            for(int i = 0; i<(EthernetFields.MacAddressLength * 16); i+=EthernetFields.MacAddressLength)
            {
                // Extract the sample from the payload for comparison
                Array.Copy(payloadData, i, buffer, 0, buffer.Length);

                // check the synchronization sequence on the first pass
                if(i == 0)
                {
                    // validate the synchronization sequence
                    if(!RandomUtils.ByteArrayEquals(buffer, syncSequence))
                        return false;
                }
                else
                {
                    // fail the validation on malformed WOL Magic Packets
                    if(!RandomUtils.ByteArrayEquals(buffer, destinationMAC))
                        return false;
                }
            }
            return true;
        }

        #endregion

        #region Members

        // The WOL synchronization sequence
        private static byte[] syncSequence = new byte[6] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

        #endregion
    }
}