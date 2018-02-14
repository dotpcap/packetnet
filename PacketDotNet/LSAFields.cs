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
        public static readonly Int32 LSAgeLength = 2;
        /// <summary> The length of the Options field in bytes</summary>
        public static readonly Int32 OptionsLength = 1;
        /// <summary> The length of the LSType field in bytes</summary>
        public static readonly Int32 LSTypeLength = 1;
        /// <summary> The length of the LinkStateID field in bytes</summary>
        public static readonly Int32 LinkStateIDLength = 4;
        /// <summary> The length of the AdvertisingRouterID field in bytes</summary>
        public static readonly Int32 AdvertisingRouterIDLength = 4;
        /// <summary> The length of the LSSeqeunceNumber field in bytes</summary>
        public static readonly Int32 LSSequenceNumberLength = 4;
        /// <summary> The length of the Checksum field in bytes</summary>
        public static readonly Int32 ChecksumLength = 2;
        /// <summary> The length of the Length field in bytes</summary>
        public static readonly Int32 PacketLength = 2;

        /// <summary> The relative postion of the LSAge field</summary>
        public static readonly Int32 LSAgePosition = 0;
        /// <summary> The relative postion of the Option field</summary>
        public static readonly Int32 OptionsPosition;
        /// <summary> The relative postion of the LSType field</summary>
        public static readonly Int32 LSTypePosition;
        /// <summary> The relative postion of the LinkStateID field</summary>
        public static readonly Int32 LinkStateIDPosition;
        /// <summary> The relative postion of the AdvertisingRouterID field</summary>
        public static readonly Int32 AdvertisingRouterIDPosition;
        /// <summary> The relative postion of the LSSequenceNumber field</summary>
        public static readonly Int32 LSSequenceNumberPosition;
        /// <summary> The relative postion of the Checksum field</summary>
        public static readonly Int32 ChecksumPosition;
        /// <summary> The relative postion of the Length field</summary>
        public static readonly Int32 PacketLengthPosition;
        /// <summary> The relative postion of the header's end</summary>
        public static readonly Int32 HeaderEnd;

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
        public static readonly Int32 RouterOptionsLength = 2;
        /// <summary> The length of the LinkNumber field in bytes</summary>
        public static readonly Int32 LinkNumberLength = 2;

        /// <summary> The relative postion of the RouterOptions field</summary>
        public static readonly Int32 RouterOptionsPosition;
        /// <summary> The relative postion of the LinkNumber field</summary>
        public static readonly Int32 LinkNumberPosition;
        /// <summary> The relative postion of the start of the RouterLink(s)</summary>
        public static readonly Int32 RouterLinksStart;

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
        public static readonly Int32 NetworkMaskLength = 4;
        /// <summary> The length of the AttachedRouter field in bytes</summary>
        public static readonly Int32 AttachedRouterLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public static readonly Int32 NetworkMaskPosition;
        /// <summary> The relative postion of the AttachedRouter field</summary>
        public static readonly Int32 AttachedRouterPosition;

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
        public static readonly Int32 NetworkMaskLength = 4;
        /// <summary> The length of the Metric field in bytes</summary>
        public static readonly Int32 MetricLength = 4;
        /// <summary> The length of the TOSMetric field in bytes</summary>
        public static readonly Int32 TOSMetricLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public static readonly Int32 NetworkMaskPosition;
        /// <summary> The relative postion of the Metric field</summary>
        public static readonly Int32 MetricPosition;
        /// <summary> The relative postion of the TOSMetric field</summary>
        public static readonly Int32 TOSMetricPosition;

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
        public static readonly Int32 NetworkMaskLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public static readonly Int32 NetworkMaskPosition;
        /// <summary> The relative postion of the Metric field</summary>
        public static readonly Int32 MetricPosition;

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
        public static readonly Int32 LinkIDLength = 4;
        /// <summary> The length of the LinkData field in bytes</summary>
        public static readonly Int32 LinkDataLength = 4;
        /// <summary> The length of the Type field in bytes</summary>
        public static readonly Int32 TypeLength = 1;
        /// <summary> The length of the TOSNumber field in bytes</summary>
        public static readonly Int32 TOSNumberLength = 1;
        /// <summary> The length of the Metric field in bytes</summary>
        public static readonly Int32 MetricLength = 2;

        /// <summary> The relative postion of the LinkID field</summary>
        public static readonly Int32 LinkIDPosition;
        /// <summary> The relative postion of the LinkData field</summary>
        public static readonly Int32 LinkDataPosition;
        /// <summary> The relative postion of the Type field</summary>
        public static readonly Int32 TypePosition;
        /// <summary> The relative postion of the TOSNumber field</summary>
        public static readonly Int32 TOSNumberPosition;
        /// <summary> The relative postion of the Metric field</summary>
        public static readonly Int32 MetricPosition;
        /// <summary> The relative postion of the AdditionalMetrics field</summary>
        public static readonly Int32 AdditionalMetricsPosition;

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
        public static readonly Int32 TOSLength = 4;
        /// <summary> The length of the ForwardingAddress field in bytes</summary>
        public static readonly Int32 ForwardingAddressLength = 4;
        /// <summary> The length of the ExternalRouteTag field in bytes</summary>
        public static readonly Int32 ExternalRouteTagLength = 4;

        /// <summary> The relative postion of the TOSPosition field</summary>
        public static readonly Int32 TOSPosition;
        /// <summary> The relative postion of the ForwardingAddress field</summary>
        public static readonly Int32 ForwardingAddressPosition;
        /// <summary> The relative postion of the ExternalRouteTag field</summary>
        public static readonly Int32 ExternalRouteTagPosition;

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
        public static readonly Int32 LSTypeLength = 4;
        /// <summary> The length of the LinkStateID field in bytes</summary>
        public static readonly Int32 LinkStateIdLength = 4;
        /// <summary> The length of the AdvertisingRouter field in bytes</summary>
        public static readonly Int32 AdvertisingRouterLength = 4;

        /// <summary> The relative postion of the LSType field</summary>
        public static readonly Int32 LSTypePosition;
        /// <summary> The relative postion of the LinkStateID field</summary>
        public static readonly Int32 LinkStateIdPosition;
        /// <summary> The relative postion of the AdvertisingRouter field</summary>
        public static readonly Int32 AdvertisingRouterPosition;

        static LinkStateRequestFields()
        {
            LSTypePosition = 0;
            LinkStateIdPosition  = LSTypePosition + LSTypeLength;
            AdvertisingRouterPosition = LinkStateIdPosition + LinkStateIdLength;
        }
    }
}
