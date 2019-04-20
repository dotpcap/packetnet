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

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// Code constants for ICMP message types.
    /// From http://en.wikipedia.org/wiki/Internet_Control_Message_Protocol#List_of_permitted_control_messages_.28incomplete_list.29
    /// Note that these values represent the combined
    /// type and code fields, where the type field is the upper byte
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ICMPv4TypeCodes : ushort
    {
#pragma warning disable 1591
        EchoReply = 0x0000,
        Unassigned1 = 0x0100,
        Unassigned2 = 0x0200,
        UnreachableNet = 0x0300,
        UnreachableHost = 0x0301,
        UnreachableProtocol = 0x0302,
        UnreachablePort = 0x0303,
        UnreachableFragmentationNeeded = 0x0304,
        UnreachableSourceRouteFailed = 0x0305,
        UnreachableDestinationNetworkUnknown = 0x0306,
        UnreachableDestinationHostUnknown = 0x0307,
        UnreachableSourceHostIsolated = 0x0308,
        UnreachableNetworkProhibited = 0x0309,
        UnreachableHostProhibited = 0x030A,
        UnreachableNetworkUnreachableForServiceType = 0x030B,
        UnreachableHostUnreachableForServiceType = 0x030C,
        UnreachableCommunicationProhibited = 0x030D,
        UnreachableHostPrecedenceViolation = 0x030E,
        UnreachablePrecedenceCutoffInEffect = 0x030F,

        SourceQuench = 0x0400,

        AlternateHostAddress = 0x0500, // preserved for backwards compatibility
        RedirectNetwork = 0x0500,
        RedirectHost = 0x0501,
        RedirectTypeOfServiceAndNetwork = 0x0502,
        RedirectTypeOfServiceAndHost = 0x0503,

        Unassigned3 = 0x0700,
        EchoRequest = 0x0800,
        RouterAdvertisement = 0x0900,
        RouterSelection = 0x0A00,
        TimeExceeded = 0x0B00,

        ParamPointerIndicatesError = 0x0C00,
        ParamMissingRequiredOption = 0x0C01,
        ParamBadLength = 0x0C02,

        Timestamp = 0x0D00,
        TimestampReply = 0x0E00,
        InformationRequest = 0x0F00,
        InformationReply = 0x1000,
        AddressMaskRequest = 0x1100,
        AddressMaskReply = 0x1200,

        Reserved4Security = 0x1300,
        Reserved4RobustnessExperiment1 = 0x1400,
        Reserved4RobustnessExperiment2 = 0x1500,
        Reserved4RobustnessExperiment3 = 0x1600,
        Reserved4RobustnessExperiment4 = 0x1700,
        Reserved4RobustnessExperiment5 = 0x1800,
        Reserved4RobustnessExperiment6 = 0x1900,
        Reserved4RobustnessExperiment7 = 0x1A00,
        Reserved4RobustnessExperiment8 = 0x1B00,
        Reserved4RobustnessExperiment9 = 0x1C00,
        Reserved4RobustnessExperiment10 = 0x1D00,

        Traceroute = 0x1E00,
        DatagramConversionError = 0x1F00,
        MobileHostRedirect = 0x2000,
        Pv6WhereAreYou = 0x2100,
        Pv6IAmHere = 0x2200,
        MobileReqistrationRequest = 0x2300,
        MobileRegistrationReply = 0x2400,
        Skip = 0x2500,

        PhoturiBadSPI = 0x2600,
        PhoturiAuthenticationFailed = 0x2601,
        PhoturiDecompressionFailed = 0x2602,
        PhoturiDecryptionFailed = 0x2603,
        PhoturiNeedAuthentication = 0x2604,
        PhoturiNeedAuthorization = 0x2605,
#pragma warning restore 1591

        //TODO: continue this list as user requested
    }
}