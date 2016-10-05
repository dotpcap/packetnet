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

using System;
using System.Collections.Generic;
using System.Text;

namespace PacketDotNet
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a LSA header.
    /// </summary>
    public class LSAFields
    {
        /// <summary> The length of the LSAge field in bytes</summary>
        public readonly static int LSAgeLength = 2;
        /// <summary> The length of the Options field in bytes</summary>
        public readonly static int OptionsLength = 1;
        /// <summary> The length of the LSType field in bytes</summary>
        public readonly static int LSTypeLength = 1;
        /// <summary> The length of the LinkStateID field in bytes</summary>
        public readonly static int LinkStateIDLength = 4;
        /// <summary> The length of the AdvertisingRouterID field in bytes</summary>
        public readonly static int AdvertisingRouterIDLength = 4;
        /// <summary> The length of the LSSeqeunceNumber field in bytes</summary>
        public readonly static int LSSequenceNumberLength = 4;
        /// <summary> The length of the Checksum field in bytes</summary>
        public readonly static int ChecksumLength = 2;
        /// <summary> The length of the Length field in bytes</summary>
        public readonly static int PacketLength = 2;

        /// <summary> The relative postion of the LSAge field</summary>
        public readonly static int LSAgePosition = 0;
        /// <summary> The relative postion of the Option field</summary>
        public readonly static int OptionsPosition;
        /// <summary> The relative postion of the LSType field</summary>
        public readonly static int LSTypePosition;
        /// <summary> The relative postion of the LinkStateID field</summary>
        public readonly static int LinkStateIDPosition;
        /// <summary> The relative postion of the AdvertisingRouterID field</summary>
        public readonly static int AdvertisingRouterIDPosition;
        /// <summary> The relative postion of the LSSequenceNumber field</summary>
        public readonly static int LSSequenceNumberPosition;
        /// <summary> The relative postion of the Checksum field</summary>
        public readonly static int ChecksumPosition;
        /// <summary> The relative postion of the Length field</summary>
        public readonly static int PacketLengthPosition;
        /// <summary> The relative postion of the header's end</summary>
        public readonly static int HeaderEnd;

        static LSAFields()
        {
            OptionsPosition = LSAgePosition + LSAgeLength;
            LSTypePosition = OptionsPosition + OptionsLength;
            LinkStateIDPosition = LSTypePosition + LSTypeLength;
            AdvertisingRouterIDPosition = LinkStateIDPosition + LinkStateIDLength;
            LSSequenceNumberPosition = AdvertisingRouterIDPosition + AdvertisingRouterIDLength;
            ChecksumPosition = LSSequenceNumberPosition + LSSequenceNumberLength;
            PacketLengthPosition = ChecksumPosition + ChecksumLength;
            HeaderEnd = PacketLength + PacketLengthPosition;
        }
    }

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Router-LSA
    /// </summary>
    public class RouterLSAFields : LSAFields
    {
        /// <summary> The length of the RouterOptions field in bytes</summary>
        public readonly static int RouterOptionsLength = 2;
        /// <summary> The length of the LinkNumber field in bytes</summary>
        public readonly static int LinkNumberLength = 2;

        /// <summary> The relative postion of the RouterOptions field</summary>
        public readonly static int RouterOptionsPosition;
        /// <summary> The relative postion of the LinkNumber field</summary>
        public readonly static int LinkNumberPosition;
        /// <summary> The relative postion of the start of the RouterLink(s)</summary>
        public readonly static int RouterLinksStart;

        static RouterLSAFields()
        {
            RouterOptionsPosition = HeaderEnd;
            LinkNumberPosition = RouterOptionsPosition + RouterOptionsLength;
            RouterLinksStart = LinkNumberPosition + LinkNumberLength;
        }
    }

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Network-LSA
    /// </summary>
    public class NetworkLSAFields : LSAFields
    {
        /// <summary> The length of the NetworkMask field in bytes</summary>
        public readonly static int NetworkMaskLength = 4;
        /// <summary> The length of the AttachedRouter field in bytes</summary>
        public readonly static int AttachedRouterLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public readonly static int NetworkMaskPosition;
        /// <summary> The relative postion of the AttachedRouter field</summary>
        public readonly static int AttachedRouterPosition;

        static NetworkLSAFields()
        {
            NetworkMaskPosition = HeaderEnd;
            AttachedRouterPosition = NetworkMaskPosition + NetworkMaskLength;
        }
    }

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Summary-LSA.
    /// </summary>
    public class SummaryLSAFields : LSAFields
    {
        /// <summary> The length of the NetworkMask field in bytes</summary>
        public readonly static int NetworkMaskLength = 4;
        /// <summary> The length of the Metric field in bytes</summary>
        public readonly static int MetricLength = 4;
        /// <summary> The length of the TOSMetric field in bytes</summary>
        public readonly static int TOSMetricLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public readonly static int NetworkMaskPosition;
        /// <summary> The relative postion of the Metric field</summary>
        public readonly static int MetricPosition;
        /// <summary> The relative postion of the TOSMetric field</summary>
        public readonly static int TOSMetricPosition;

        static SummaryLSAFields()
        {
            NetworkMaskPosition = HeaderEnd;
            MetricPosition  = NetworkMaskPosition + NetworkMaskLength;
            TOSMetricPosition = MetricPosition + MetricLength;
        }
    }

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a AS-External-LSA
    /// </summary>
    public class ASExternalLSAFields : LSAFields
    {
        /// <summary> The length of the NetworkMask field in bytes</summary>
        public readonly static int NetworkMaskLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public readonly static int NetworkMaskPosition;
        /// <summary> The relative postion of the Metric field</summary>
        public readonly static int MetricPosition;

        static ASExternalLSAFields()
        {
            NetworkMaskPosition = HeaderEnd;
            MetricPosition = NetworkMaskPosition + NetworkMaskLength;
        }
    }

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a RouterLink
    /// </summary>
    public class RouterLinkFields
    {
        /// <summary> The length of the LinkID field in bytes</summary>
        public readonly static int LinkIDLength = 4;
        /// <summary> The length of the LinkData field in bytes</summary>
        public readonly static int LinkDataLength = 4;
        /// <summary> The length of the Type field in bytes</summary>
        public readonly static int TypeLength = 1;
        /// <summary> The length of the TOSNumber field in bytes</summary>
        public readonly static int TOSNumberLength = 1;
        /// <summary> The length of the Metric field in bytes</summary>
        public readonly static int MetricLength = 2;

        /// <summary> The relative postion of the LinkID field</summary>
        public readonly static int LinkIDPosition;
        /// <summary> The relative postion of the LinkData field</summary>
        public readonly static int LinkDataPosition;
        /// <summary> The relative postion of the Type field</summary>
        public readonly static int TypePosition;
        /// <summary> The relative postion of the TOSNumber field</summary>
        public readonly static int TOSNumberPosition;
        /// <summary> The relative postion of the Metric field</summary>
        public readonly static int MetricPosition;
        /// <summary> The relative postion of the AdditionalMetrics field</summary>
        public readonly static int AdditionalMetricsPosition;

        static RouterLinkFields()
        {
            LinkIDPosition = 0;
            LinkDataPosition = LinkIDPosition + LinkIDLength;
            TypePosition = LinkDataPosition + LinkDataLength;
            TOSNumberPosition = TypePosition + TypeLength;
            MetricPosition = TOSNumberPosition + TOSNumberLength;
            AdditionalMetricsPosition = MetricPosition + MetricLength;
        }
    }

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a ASExternalLink
    /// </summary>
    public class ASExternalLinkFields
    {
        /// <summary> The length of the TOS field in bytes</summary>
        public readonly static int TOSLength = 4;
        /// <summary> The length of the ForwardingAddress field in bytes</summary>
        public readonly static int ForwardingAddressLength = 4;
        /// <summary> The length of the ExternalRouteTag field in bytes</summary>
        public readonly static int ExternalRouteTagLength = 4;

        /// <summary> The relative postion of the TOSPosition field</summary>
        public readonly static int TOSPosition;
        /// <summary> The relative postion of the ForwardingAddress field</summary>
        public readonly static int ForwardingAddressPosition;
        /// <summary> The relative postion of the ExternalRouteTag field</summary>
        public readonly static int ExternalRouteTagPosition;

        static ASExternalLinkFields()
        {
            TOSPosition = 0;
            ForwardingAddressPosition  = TOSPosition + TOSLength;
            ExternalRouteTagPosition = ForwardingAddressPosition + ForwardingAddressLength;
        }
    }

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a LinkStateRequest
    /// </summary>
    public class LinkStateRequestFields
    {
        /// <summary> The length of the LSType field in bytes</summary>
        public readonly static int LSTypeLength = 4;
        /// <summary> The length of the LinkStateID field in bytes</summary>
        public readonly static int LinkStateIdLength = 4;
        /// <summary> The length of the AdvertisingRouter field in bytes</summary>
        public readonly static int AdvertisingRouterLength = 4;

        /// <summary> The relative postion of the LSType field</summary>
        public readonly static int LSTypePosition;
        /// <summary> The relative postion of the LinkStateID field</summary>
        public readonly static int LinkStateIdPosition;
        /// <summary> The relative postion of the AdvertisingRouter field</summary>
        public readonly static int AdvertisingRouterPosition;

        static LinkStateRequestFields()
        {
            LSTypePosition = 0;
            LinkStateIdPosition  = LSTypePosition + LSTypeLength;
            AdvertisingRouterPosition = LinkStateIdPosition + LinkStateIdLength;
        }
    }
}
