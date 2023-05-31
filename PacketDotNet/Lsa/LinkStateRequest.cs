using System;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Lsa;

    /// <summary>
    /// Link state request, send by the LSR packets
    /// </summary>
    public class LinkStateRequest
    {
        /// <summary>
        /// Size of LinkStateRequest in bytes
        /// </summary>
        public static readonly int Length = 12;

        private readonly ByteArraySegment _header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LinkStateRequest()
        {
            var b = new byte[Length];
            _header = new ByteArraySegment(b);
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
            _header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// The Router ID of the router that originated the LSR.
        /// </summary>
        public IPAddress AdvertisingRouter
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(_header.Bytes, _header.Offset + LinkStateRequestFields.AdvertisingRouterPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           _header.Bytes,
                           _header.Offset + LinkStateRequestFields.AdvertisingRouterPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public virtual byte[] Bytes => _header.ActualBytes();

        /// <summary>
        /// This field identifies the portion of the internet environment
        /// that is being described by the LSR.
        /// </summary>
        public IPAddress LinkStateID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(_header.Bytes, _header.Offset + LinkStateRequestFields.LinkStateIdPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           _header.Bytes,
                           _header.Offset + LinkStateRequestFields.LinkStateIdPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The type of the request
        /// </summary>
        public LinkStateAdvertisementType LSType
        {
            get => (LinkStateAdvertisementType) EndianBitConverter.Big.ToUInt32(_header.Bytes, _header.Offset + LinkStateRequestFields.LinkStateTypePosition);
            set => EndianBitConverter.Big.CopyBytes((uint) value, _header.Bytes, _header.Offset + LinkStateRequestFields.LinkStateTypePosition);
        }
    }