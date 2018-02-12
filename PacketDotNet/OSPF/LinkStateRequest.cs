using System;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Link state request, send by the LSR packets
    /// </summary>
    public class LinkStateRequest
    {
        /// <summary>
        /// Size of LinkStateRequest in bytes
        /// </summary>
        public static readonly int Length = 12;

        internal ByteArraySegment header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LinkStateRequest()
        {
            byte[] b = new byte[Length];
            this.header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset abd length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32"/>
        /// </param>
        public LinkStateRequest(byte[] packet, int offset, int length)
        {
            this.header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// The type of the request
        /// </summary>
        public LSAType LSType
        {
            get => (LSAType)EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + LinkStateRequestFields.LSTypePosition);
            set => EndianBitConverter.Big.CopyBytes((UInt32)value, this.header.Bytes, this.header.Offset + LinkStateRequestFields.LSTypePosition);
        }

        /// <summary>
        /// The Router ID of the router that originated the LSR.
        /// </summary>
        public IPAddress AdvertisingRouter
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + LinkStateRequestFields.AdvertisingRouterPosition);
                return new IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + LinkStateRequestFields.AdvertisingRouterPosition),
                    address.Length);
            }
        }

        /// <summary>
        /// This field identifies the portion of the internet environment
        /// that is being described by the LSR.
        /// </summary>
        public IPAddress LinkStateID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + LinkStateRequestFields.LinkStateIdPosition);
                return new IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + LinkStateRequestFields.LinkStateIdPosition),
                    address.Length);
            }
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public virtual byte[] Bytes => this.header.ActualBytes();
    }
}