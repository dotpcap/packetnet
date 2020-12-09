/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using PacketDotNet.Utils;
#if DEBUG
using System.Reflection;
using log4net;

#endif

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// A Port ID TLV
    /// </summary>
    public class PortIdTlv : Tlv
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        private const int SubTypeLength = 1;

        /// <summary>
        /// Creates a Port ID TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Port ID TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public PortIdTlv(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Log.Debug("");
        }

        /// <summary>
        /// Creates a Port ID TLV and sets it value
        /// </summary>
        /// <param name="subType">
        /// The Port ID SubType
        /// </param>
        /// <param name="subTypeValue">
        /// The subtype's value
        /// </param>
        public PortIdTlv(PortSubType subType, object subTypeValue)
        {
            Log.Debug("");

            EmptyTLVDataInit();

            Type = TlvType.PortId;
            SubType = subType;

            // method will resize the tlv
            SubTypeValue = subTypeValue;
        }

        /// <summary>
        /// Construct a PortId from a NetworkAddress
        /// </summary>
        /// <param name="networkAddress">
        /// A <see cref="NetworkAddress" />
        /// </param>
        public PortIdTlv(NetworkAddress networkAddress)
        {
            Log.DebugFormat("NetworkAddress {0}", networkAddress);

            var length = TlvTypeLength.TypeLengthLength + SubTypeLength;
            var bytes = new byte[length];
            var offset = 0;
            Data = new ByteArraySegment(bytes, offset, length);

            Type = TlvType.PortId;
            SubType = PortSubType.NetworkAddress;
            SubTypeValue = networkAddress;
        }

        /// <value>
        /// The type of the TLV subtype
        /// </value>
        public PortSubType SubType
        {
            get => (PortSubType) Data.Bytes[Data.Offset + TlvTypeLength.TypeLengthLength];
            set => Data.Bytes[Data.Offset + TlvTypeLength.TypeLengthLength] = (byte) value;
        }

        /// <value>
        /// The TLV subtype value
        /// </value>
        public object SubTypeValue
        {
            get => GetSubTypeValue();
            set => SetSubTypeValue(value);
        }

        /// <summary>
        /// Size of the value field
        /// </summary>
        private int DataLength => Length - SubTypeLength;

        /// <summary>
        /// Offset to the value field
        /// </summary>
        private int DataOffset => ValueOffset + SubTypeLength;

        /// <summary>
        /// Helper method to reduce duplication in type specific constructors
        /// </summary>
        private void EmptyTLVDataInit()
        {
            var length = TlvTypeLength.TypeLengthLength + SubTypeLength;
            var bytes = new byte[length];
            var offset = 0;
            Data = new ByteArraySegment(bytes, offset, length);
        }

        private object GetSubTypeValue()
        {
            byte[] arrAddress;

            switch (SubType)
            {
                case PortSubType.InterfaceAlias:
                case PortSubType.InterfaceName:
                case PortSubType.LocallyAssigned:
                case PortSubType.PortComponent:
                case PortSubType.AgentCircuitId:
                {
                    // get the address
                    arrAddress = new byte[DataLength];
                    Array.Copy(Data.Bytes, DataOffset, arrAddress, 0, DataLength);
                    return arrAddress;
                }
                case PortSubType.MacAddress:
                {
                    // get the address
                    arrAddress = new byte[DataLength];
                    Array.Copy(Data.Bytes, DataOffset, arrAddress, 0, DataLength);
                    var address = new PhysicalAddress(arrAddress);
                    return address;
                }
                case PortSubType.NetworkAddress:
                {
                    // get the address
                    return GetNetworkAddress();
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void SetSubTypeValue(object subTypeValue)
        {
            switch (SubType)
            {
                case PortSubType.InterfaceAlias:
                case PortSubType.InterfaceName:
                case PortSubType.LocallyAssigned:
                case PortSubType.PortComponent:
                case PortSubType.AgentCircuitId:
                {
                    SetSubTypeValue((byte[]) subTypeValue);
                    break;
                }
                case PortSubType.MacAddress:
                {
                    SetSubTypeValue(((PhysicalAddress) subTypeValue).GetAddressBytes());
                    break;
                }
                case PortSubType.NetworkAddress:
                {
                    SetSubTypeValue(((NetworkAddress) subTypeValue).Bytes);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void SetSubTypeValue(byte[] val)
        {
            // does our current length match?
            var dataLength = Length - SubTypeLength;
            if (dataLength != val.Length)
            {
                var headerLength = TlvTypeLength.TypeLengthLength + SubTypeLength;
                var newLength = headerLength + val.Length;
                var newBytes = new byte[newLength];

                // copy the header data over
                Array.Copy(Data.Bytes,
                           Data.Offset,
                           newBytes,
                           0,
                           headerLength);

                var offset = 0;
                Data = new ByteArraySegment(newBytes, offset, newLength);
            }

            Array.Copy(val,
                       0,
                       Data.Bytes,
                       ValueOffset + SubTypeLength,
                       val.Length);
        }

        private NetworkAddress GetNetworkAddress()
        {
            if (SubType != PortSubType.NetworkAddress)
            {
                throw new ArgumentOutOfRangeException(nameof(SubType), "SubType != PortSubTypes.NetworkAddress");
            }

            var networkAddress = new NetworkAddress(Data.Bytes, DataOffset, DataLength);

            return networkAddress;
        }

        /// <summary>
        /// Convert this Port ID TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return $"[PortId: SubType={SubType}, SubTypeValue={SubTypeValue}]";
        }
    }
}