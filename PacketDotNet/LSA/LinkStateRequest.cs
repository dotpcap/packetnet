using System;
using System.Net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;
namespace PacketDotNet.LSA
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

        internal ByteArraySegment Header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LinkStateRequest()
        {
            var b = new byte[Length];
            Header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset abd length
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
        public LinkStateRequest(byte[] packet, int offset, int length)
        {
            Header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// The Router ID of the router that originated the LSR.
        /// </summary>
        public IPAddress AdvertisingRouter
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + LinkStateRequestFields.AdvertisingRouterPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + LinkStateRequestFields.AdvertisingRouterPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public virtual byte[] Bytes => Header.ActualBytes();

        /// <summary>
        /// This field identifies the portion of the internet environment
        /// that is being described by the LSR.
        /// </summary>
        public IPAddress LinkStateID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + LinkStateRequestFields.LinkStateIdPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + LinkStateRequestFields.LinkStateIdPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The type of the request
        /// </summary>
        public LSAType LSType
        {
            get => (LSAType) EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LinkStateRequestFields.LSTypePosition);
            set => EndianBitConverter.Big.CopyBytes((uint) value, Header.Bytes, Header.Offset + LinkStateRequestFields.LSTypePosition);
        }
    }
}