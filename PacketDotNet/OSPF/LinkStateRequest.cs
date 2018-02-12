using System;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    ///     Link state request, send by the LSR packets
    /// </summary>
    public class LinkStateRequest
    {
        /// <summary>
        ///     Size of LinkStateRequest in bytes
        /// </summary>
        public static readonly Int32 Length = 12;

        internal ByteArraySegment Header;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public LinkStateRequest()
        {
            Byte[] b = new Byte[Length];
            this.Header = new ByteArraySegment(b);
        }

        /// <summary>
        ///     Constructs a packet from bytes and offset abd length
        /// </summary>
        /// <param name="packet">
        ///     A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        ///     A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        ///     A <see cref="System.Int32" />
        /// </param>
        public LinkStateRequest(Byte[] packet, Int32 offset, Int32 length)
        {
            this.Header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        ///     The Router ID of the router that originated the LSR.
        /// </summary>
        public IPAddress AdvertisingRouter
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.Header.Bytes,
                    this.Header.Offset + LinkStateRequestFields.AdvertisingRouterPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.Header.Bytes, this.Header.Offset + LinkStateRequestFields.AdvertisingRouterPosition,
                    address.Length);
            }
        }

        /// <summary>
        ///     Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public virtual Byte[] Bytes => this.Header.ActualBytes();

        /// <summary>
        ///     This field identifies the portion of the internet environment
        ///     that is being described by the LSR.
        /// </summary>
        public IPAddress LinkStateID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.Header.Bytes,
                    this.Header.Offset + LinkStateRequestFields.LinkStateIdPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.Header.Bytes, this.Header.Offset + LinkStateRequestFields.LinkStateIdPosition,
                    address.Length);
            }
        }

        /// <summary>
        ///     The type of the request
        /// </summary>
        public LSAType LSType
        {
            get => (LSAType) EndianBitConverter.Big.ToUInt32(this.Header.Bytes,
                this.Header.Offset + LinkStateRequestFields.LSTypePosition);
            set => EndianBitConverter.Big.CopyBytes((UInt32) value, this.Header.Bytes,
                this.Header.Offset + LinkStateRequestFields.LSTypePosition);
        }
    }
}