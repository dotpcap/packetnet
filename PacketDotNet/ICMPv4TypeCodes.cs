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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
namespace PacketDotNet
{
    /// <summary>
    /// Code constants for ICMP message types.
    /// From http://en.wikipedia.org/wiki/Internet_Control_Message_Protocol#List_of_permitted_control_messages_.28incomplete_list.29
    /// Note that these values represent the combined
    /// type and code fields, where the type field is the upper byte
    /// </summary>
    public enum ICMPv4TypeCodes : ushort
    {
#pragma warning disable 1591
        EchoReply = 0x0000,

        // Destination Unreachable replies
        DestinationNetworkUnreachable = 0x0300,
        DestinationHostUnreachable = 0x0301,
        DestinationProtocolUnreachable = 0x0302,
        DestinationPortUnreachable = 0x303,
        FragmentationRequiredAndDFFlagSet = 0x0304,
        SourceRouteFailed = 0x0305,
        DestinationNetworkUnknown = 0x0306,
        DestinationHostUnknown = 0x0307,
        SourceHostIsolated = 0x0308,
        NetworkAdministrativelyProhibited = 0x0309,
        NetworkUnreachableForTos = 0x030A,
        HostUnreachableForTos = 0x030B,
        CommunicationAdministrativelyProhibited = 0x030C,
        EchoRequest = 0x0800,
#pragma warning restore 1591

        //TODO: continue this list as user requested
    }
}
