using System;

namespace PacketDotNet
{
    /// <summary>
    /// Raw packet as loaded from a pcap device or file
    /// </summary>
    public class RawPacket
    {
        /// <value>
        /// Link layer from which this packet was captured
        /// </value>
        public LinkLayers LinkLayerType
        {
            get;
            set;
        }

        /// <value>
        /// The unix timeval when the packet was created
        /// </value>
        public PosixTimeval Timeval
        {
            get;
            set;
        }

        /// <summary> Fetch data portion of the packet.</summary>
        public virtual byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LinkLayerType">
        /// A <see cref="LinkLayers"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="Data">
        /// A <see cref="System.Byte"/>
        /// </param>
        public RawPacket(LinkLayers LinkLayerType,
                         PosixTimeval Timeval,
                         byte[] Data)
        {
            this.LinkLayerType = LinkLayerType;
            this.Timeval = Timeval;
            this.Data = Data;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[RawPacket: LinkLayerType={0}, Timeval={1}, Data={2}]", LinkLayerType, Timeval, Data);
        }
    }
}
