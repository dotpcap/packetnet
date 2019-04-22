using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Lsa
{
    /// <summary>
    /// Additional TOS-specific information  for backward compatibility
    /// with previous versions of the OSPF specification
    /// </summary>
    public struct TypeOfServiceMetric
    {
        ///<summary>The number of bytes a TOS metric occupy.</summary>
        public static readonly int Length = 4;

        /// <summary>
        /// IP Type of Service that this metric refers to.
        /// </summary>
        public byte TypeOfService;

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
                var b = new byte[Length];
                EndianBitConverter.Big.CopyBytes(Metric, b, 0);
                b[0] = TypeOfService;
                return b;
            }
        }
    }
}