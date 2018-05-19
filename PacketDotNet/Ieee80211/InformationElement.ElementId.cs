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
 *  Copyright 2012 Chris Morgan <chmorgan@gmail.com>
 */


namespace PacketDotNet.Ieee80211
{
    public partial class InformationElement
    {
        /// <summary>
        /// Types of information elements
        /// </summary>
        public enum ElementId
        {
            /// <summary>
            /// Assign an identifier to the service set
            /// </summary>
            ServiceSetIdentity = 0x00,

            /// <summary>
            /// Specifies the data rates supported by the network
            /// </summary>
            SupportedRates = 0x01,

            /// <summary>
            /// Provides the parameters necessary to join a frequency-hopping 802.11 network
            /// </summary>
            FhParamterSet = 0x02,

            /// <summary>
            /// Direct-sequence 802.11 networks have one parameter, the channel number of the network
            /// </summary>
            DsParameterSet = 0x03,

            /// <summary>
            /// Contention-free parameter. Transmitted in Becons by access points that support
            /// contention-free operation.
            /// </summary>
            CfParameterSet = 0x04,

            /// <summary>
            /// Indicates which stations have buffered traffic waiting to be picked up
            /// </summary>
            TrafficIndicationMap = 0x05,

            /// <summary>
            /// Indicates the number of time units (TUs) between ATIM frames in an IBSS.
            /// </summary>
            IbssParameterSet = 0x06,

            /// <summary>
            /// Specifies regulatory constraints stations must adhere to based on the country the network is operating in.
            /// </summary>
            Country = 0x07,

            /// <summary>
            /// Specifies the hopping pattern of timeslots used in frequency hopping physical layers.
            /// </summary>
            HoppingParametersPattern = 0x08,

            /// <summary>
            /// Specifies the hopping pattern table used in frequency hopping physical layers.
            /// </summary>
            HoppingPatternTable = 0x09,

            /// <summary>
            /// Specifies the Ids of the information elements being requested in a <see cref="ProbeRequestFrame" />.
            /// </summary>
            Request = 0x0A,

            /// <summary>
            /// Specifies the encrypted challenge text that stations must decrypt as part of the authentication process.
            /// </summary>
            ChallengeText = 0x10,

            /// <summary>
            /// Specifies the difference between the regulatory maximum transmit power and any local constraint.
            /// </summary>
            PowerContstraint = 0x20,

            /// <summary>
            /// Specifies the minimum and maximum transmit power a station is capable of.
            /// </summary>
            PowerCapability = 0x21,

            /// <summary>
            /// Used to request radio link management information. This type of information element never has an associated value.
            /// </summary>
            TransmitPowerControlRequest = 0x22,

            /// <summary>
            /// Radio link managment report used by stations to tune their transmission power.
            /// </summary>
            TransmitPowerControlReport = 0x23,

            /// <summary>
            /// Specifies local constraints on the channels in use.
            /// </summary>
            SupportedChannels = 0x24,

            /// <summary>
            /// Announces an impending change of channel for the network.
            /// </summary>
            ChannelSwitchAnnouncement = 0x25,

            /// <summary>
            /// Requests a report on the state of the radio channel.
            /// </summary>
            MeasurementRequest = 0x26,

            /// <summary>
            /// A report of on the status of the radio channel.
            /// </summary>
            MeasurementReport = 0x27,

            /// <summary>
            /// Specifies the scheduling of temporary quiet periods on the channel.
            /// </summary>
            Quiet = 0x28,

            /// <summary>
            /// Specifies the details the Dynamic Frequency Selection (DFS) algorithm in use in the IBSS.
            /// </summary>
            IbssDfs = 0x29,

            /// <summary>
            /// Indicates whether or not the Extended Rate PHY is in use on the network at that time.
            /// </summary>
            ErpInformation = 0x2A,

            /// <summary>
            /// Specifies a stations high throughput capabilities.
            /// </summary>
            HighThroughputCapabilities = 0x2d,

            /// <summary>
            /// The erp information2.
            /// </summary>
            ErpInformation2 = 0x2F,

            /// <summary>
            /// Specifies details of the Robust Security Network encryption in use on the network.
            /// </summary>
            RobustSecurityNetwork = 0x30,

            /// <summary>
            /// Specifies more data rates supported by the network. This is identical to the Supported Rates element but it allows for a longer value.
            /// </summary>
            ExtendedSupportedRates = 0x32,

            /// <summary>
            /// Specified how high throughput capable stations will be operated in the network.
            /// </summary>
            HighThroughputInformation = 0x3d,

            /// <summary>
            /// Specifies details of the WiFi Protected Access encryption in use on the network.
            /// </summary>
            WifiProtectedAccess = 0xD3,

            /// <summary>
            /// Non standard information element implemented by the hardware vendor.
            /// </summary>
            VendorSpecific = 0xDD
        }
    }
}