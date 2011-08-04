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
        Unassigned1 = 0x0100,
        Unassigned2 = 0x0200,
        Unreachable_Net = 0x0300,
        Unreachable_Host = 0x0301,
        Unreachable_Protocol = 0x0302,
        Unreachable_Port = 0x0303,
        Unreachable_FragmentationNeeded = 0x0304,
        Unreachable_SourceRouteFailed = 0x0305,
        Unreachable_DestinationNetworkUnknown = 0x0306,
        Unreachable_DestinationHostUnknown = 0x0307,
        Unreachable_SourceHostIsolated = 0x0308,
        Unreachable_NetworkProhibited = 0x0309,
        Unreachable_HostProhibited = 0x030A,
        Unreachable_NetworkUnreachableForServiceType = 0x030B,
        Unreachable_HostUnreachableForServiceType = 0x030C,
        Unreachable_CommunicationProhibited = 0x030D,
        Unreachable_HostPrecedenceViolation = 0x030E,
        Unreachable_PrecedenceCutoffInEffect = 0x030F,

        SourceQuench = 0x0400,

        AlternateHostAddress = 0x0500, // preserved for backwards compatibility
        Redirect_Network = 0x0500,
        Redirect_Host = 0x0501,
        Redirect_TypeOfServiceAndNetwork = 0x0502,
        Redirect_TypeOfServiceAndHost = 0x0503,

        Unassigned3 = 0x0700,
        EchoRequest = 0x0800,
        RouterAdvertisement = 0x0900,
        RouterSelection = 0x0A00,
        TimeExceeded = 0x0B00,

        Param_PointerIndicatesError = 0x0C00,
        Param_MissingRequiredOption = 0x0C01,
        Param_BadLength = 0x0C02,

        Timestamp = 0x0D00,
        TimestampReply = 0x0E00,
        InformationRequest = 0x0F00,
        InformationReply = 0x1000,
        AddressMaskRequest = 0x1100,
        AddressMaskReply = 0x1200,

        Reserved4Security = 0x1300,
        Reserved4robustnessExperiment1 = 0x1400,
        Reserved4robustnessExperiment2 = 0x1500,
        Reserved4robustnessExperiment3 = 0x1600,
        Reserved4robustnessExperiment4 = 0x1700,
        Reserved4robustnessExperiment5 = 0x1800,
        Reserved4robustnessExperiment6 = 0x1900,
        Reserved4robustnessExperiment7 = 0x1A00,
        Reserved4robustnessExperiment8 = 0x1B00,
        Reserved4robustnessExperiment9 = 0x1C00,
        Reserved4robustnessExperiment10 = 0x1D00,

        Traceroute = 0x1E00,
        DatagramConversionError = 0x1F00,
        MobileHostRedirect = 0x2000,
        IPv6WhereAreYou = 0x2100,
        IPv6IAmHere = 0x2200,
        MobileReqistrationRequest = 0x2300,
        MobileRegistrationReply = 0x2400,
        Skip = 0x2500,

        Photuri_BadSPI = 0x2600,
        Photuri_AuthenticationFailed = 0x2601,
        Photuri_DecompressionFailed = 0x2602,
        Photuri_DecryptionFailed = 0x2603,
        Photuri_NeedAuthentication = 0x2604,
        Photuri_NeedAuthorization = 0x2605,
#pragma warning restore 1591

        //TODO: continue this list as user requested
    }
}
