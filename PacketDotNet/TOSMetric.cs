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
        public static readonly int TOSMetricLength = 4;

        /// <summary>
        /// IP Type of Service that this metric refers to.
        /// </summary>
        public byte TOS;

        /// <summary>
        /// TOS-specific metric information.
        /// </summary>
        public uint Metric;

        /// <summary>
        /// Gets the bytes that make up this packet.
        /// </summary>
        /// <value>Packet bytes</value>
        public byte[] Bytes
        {
            get
            {
                byte[] b = new byte[TOSMetricLength];
                EndianBitConverter.Big.CopyBytes(this.Metric, b, 0);
                b[0] = this.TOS;
                return b;
            }
        }

    }
}