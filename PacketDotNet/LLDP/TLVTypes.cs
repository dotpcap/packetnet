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
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
namespace PacketDotNet.LLDP
{
    /// <summary>
    /// The TLV Types
    /// </summary>
    /// <remarks>
    /// See IETF RFC 802.1AB for more info
    /// </remarks>
    public enum TLVTypes
    {
        /// <summary>Signifies the end of a LLDPU</summary>
        /// <description>
        /// The End Of LLDPDU TLV is a 2-octet, all-zero
        /// TLV that is used to mark the end of the TLV
        /// sequence in LLDPDUs
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        EndOfLLDPU = 0,
        /// <summary>A Chassis Identifier</summary>
        /// <description>
        /// A mandatory TLV that identifies the chassis
        /// containing the IEEE 802 LAN station
        /// associated with the transmitting LLDP agent
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        ChassisID = 1,
        /// <summary>A Port Identifier</summary>
        /// <description>
        /// A mandatory TLV that identifies the
        /// port component of the MSAP identifier associated
        /// with the transmitting LLDP agent.
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        PortID = 2,
        /// <summary>Specifies the Time to Live</summary>
        /// <description>
        /// Indicates the number of seconds that the
        /// recipient LLDP agent is to regard the information
        /// associated with this MSAP identifier to be valid
        ///
        /// A value of 0 signals that this source is no longer
        /// available and all information associated with it
        /// should be deleted.
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        TimeToLive = 3,
        /// <summary>A Description of the Port</summary>
        /// <description>
        /// The port description field shall contain an
        /// alpha-numeric string that indicates the port’s
        /// description.
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        PortDescription = 4,
        /// <summary>The System's Assigned Name</summary>
        /// <description>
        /// The System Name TLV allows network management
        /// to advertise the system’s assigned name.
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        SystemName = 5,
        /// <summary>A Description of the System</summary>
        /// <description>
        /// The System Description TLV allows network
        /// management to advertise the system’s description
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        SystemDescription = 6,
        /// <summary>A bitmap containing the System's capabilities</summary>
        /// <description>
        /// The System Capabilities TLV is an optional TLV
        /// that identifies the primary function(s) of the
        /// system and whether or not these primary functions
        /// are enabled.
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        SystemCapabilities = 7,
        /// <summary>The Management Address</summary>
        /// <description>
        /// The Management Address TLV identifies an address
        /// associated with the local LLDP agent that may be
        /// used to reach higher layer entities to assist
        /// discovery by network management.
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        ManagementAddress = 8,
        /// <summary>A vendor-specifid TLV</summary>
        /// <description>
        /// This TLV category is provided to allow different
        /// organizations, such as IEEE 802.1, IEEE 802.3, IETF,
        /// as well as individual software and equipment vendors,
        /// to define TLVs that advertise information to remote
        /// entities attached to the same media.
        /// </description>
        /// <remarks>Source: IETF RFC 802.1AB</remarks>
        OrganizationSpecific = 127
    };
}