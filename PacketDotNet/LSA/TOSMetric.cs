using System;
using PacketDotNet.MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// Additional TOS-specific information  for backward compatibility
    /// with previous versions of the OSPF specification
    /// </summary>
    public struct TOSMetric
    {
        ///<summary>The number of bytes a TOS metric occupy</summary>
        public static readonly Int32 TOSMetricLength = 4;

        /// <summary>
        /// IP Type of Service that this metric refers to.
        /// </summary>
        public Byte TOS;

        /// <summary>
        /// TOS-specific metric information.
        /// </summary>
        public UInt32 Metric;

        /// <summary>
        /// Gets the bytes that make up this packet.
        /// </summary>
        /// <value>Packet bytes</value>
        public Byte[] Bytes
        {
            get
            {
                var b = new Byte[TOSMetricLength];
                EndianBitConverter.Big.CopyBytes(Metric, b, 0);
                b[0] = TOS;
                return b;
            }
        }
    }
}