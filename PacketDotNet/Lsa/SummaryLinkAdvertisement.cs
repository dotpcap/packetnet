using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Lsa;

    /// <summary>
    /// Summary-LSAs are the Type 3 and 4 LSAs.  These LSAs are originated
    /// by area border routers. Summary-LSAs describe inter-area
    /// destinations. Type 3 summary-LSAs are used when the destination is an IP network,
    /// Type 4 - an AS boundary router.
    /// </summary>
    public class SummaryLinkAdvertisement : LinkStateAdvertisement
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SummaryLinkAdvertisement()
        {
            var b = new byte[SummaryLinkAdvertisementFields.TosMetricPosition];
            Header = new ByteArraySegment(b);
            Type = LinkStateAdvertisementType.Summary;
            Length = (ushort) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Summary LSA with a list of TOS metrics
        /// </summary>
        public SummaryLinkAdvertisement(List<TypeOfServiceMetric> metrics)
        {
            var length = SummaryLinkAdvertisementFields.TosMetricPosition + metrics.Count * TypeOfServiceMetric.Length;
            var offset = SummaryLinkAdvertisementFields.TosMetricPosition;
            var b = new byte[length];

            foreach (var m in metrics)
            {
                Array.Copy(m.Bytes, 0, b, offset, TypeOfServiceMetric.Length);
                offset += TypeOfServiceMetric.Length;
            }

            Header = new ByteArraySegment(b);
            Type = LinkStateAdvertisementType.Summary;
            Length = (ushort) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        public SummaryLinkAdvertisement(byte[] packet, int offset, int length) :
            base(packet, offset, length)
        { }

        /// <summary>
        /// The cost of this route.  Expressed in the same units as the interface costs in the router-LSAs.
        /// </summary>
        public uint Metric
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + SummaryLinkAdvertisementFields.MetricPosition);
                return val & 0x00FFFFFF;
            }
            set
            {
                var v = value & 0x00FFFFFF;
                EndianBitConverter.Big.CopyBytes(v, Header.Bytes, Header.Offset + SummaryLinkAdvertisementFields.MetricPosition);
            }
        }

        /// <summary>
        /// For Type 3 summary-LSAs, this indicates the destination
        /// network's IP address mask. This field is not meaningful
        /// and must be zero for Type 4 summary-LSAs.
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + SummaryLinkAdvertisementFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + SummaryLinkAdvertisementFields.NetworkMaskPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Additional TOS-specific information  for backward compatibility
        /// with previous versions of the OSPF specification
        /// </summary>
        public List<TypeOfServiceMetric> TosMetrics
        {
            get
            {
                var ret = new List<TypeOfServiceMetric>();

                if ((Length - SummaryLinkAdvertisementFields.TosMetricPosition) % TypeOfServiceMetric.Length != 0)
                {
                    throw new Exception("Malformed summary LSA - bad TosMetrics size");
                }

                var tosCnt = (Length - SummaryLinkAdvertisementFields.TosMetricPosition) / TypeOfServiceMetric.Length;

                for (var i = 0; i < tosCnt; i++)
                {
                    var metric = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + SummaryLinkAdvertisementFields.TosMetricPosition + i * TypeOfServiceMetric.Length);
                    var m = new TypeOfServiceMetric
                    {
                        TypeOfService = (byte) ((metric & 0xFF000000) >> 24),
                        Metric = metric & 0x00FFFFFF
                    };

                    ret.Add(m);
                }

                return ret;
            }
        }
    }