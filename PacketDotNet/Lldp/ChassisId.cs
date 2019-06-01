/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using System.Text;
using PacketDotNet.Utils;
#if DEBUG
using System.Reflection;
using log4net;

#endif

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// A Chassis ID TLV
    /// </summary>
    public class ChassisId : Tlv
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

        /// <summary>
        /// Length of the sub type field in bytes
        /// </summary>
        private const int SubTypeLength = 1;

        /// <summary>
        /// Creates a Chassis ID TLV by parsing a byte[]
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Chassis ID TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public ChassisId(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Log.Debug("");
        }

        /// <summary>
        /// Creates a Chassis ID TLV and sets it value
        /// </summary>
        /// <param name="subType">
        /// The ChassisId subtype
        /// </param>
        /// <param name="subTypeValue">
        /// The subtype's value
        /// </param>
        public ChassisId(ChassisSubType subType, object subTypeValue)
        {
            Log.DebugFormat("subType {0}", subType);

            EmptyTLVDataInit();

            Type = TlvType.ChassisId;

            SubType = subType;

            // method will resize the tlv
            SubTypeValue = subTypeValue;
        }

        /// <summary>
        /// Create a ChassisId given a mac address
        /// </summary>
        /// <param name="macAddress">
        /// A <see cref="PhysicalAddress" />
        /// </param>
        public ChassisId(PhysicalAddress macAddress)
        {
            Log.DebugFormat("MACAddress {0}", macAddress);

            EmptyTLVDataInit();

            Type = TlvType.ChassisId;
            SubType = ChassisSubType.MacAddress;

            SubTypeValue = macAddress;
        }

        /// <summary>
        /// Create a ChassisId given an interface name
        /// http://tools.ietf.org/search/rfc2863 page 38
        /// </summary>
        /// <param name="interfaceName">
        /// A <see cref="string" />
        /// </param>
        public ChassisId(string interfaceName)
        {
            Log.DebugFormat("InterfaceName {0}", interfaceName);

            EmptyTLVDataInit();

            Type = TlvType.ChassisId;
            SubType = ChassisSubType.InterfaceName;

            SetSubTypeValue(interfaceName);
        }

        /// <summary>
        /// If SubType is ChassisComponent
        /// </summary>
        public byte[] ChassisComponent
        {
            get => (byte[]) GetSubTypeValue();
            set
            {
                SubType = ChassisSubType.ChassisComponent;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is InterfaceAlias
        /// </summary>
        public byte[] InterfaceAlias
        {
            get => (byte[]) GetSubTypeValue();
            set
            {
                SubType = ChassisSubType.InterfaceAlias;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is InterfaceName the interface name
        /// </summary>
        public string InterfaceName
        {
            get => (string) GetSubTypeValue();
            set
            {
                SubType = ChassisSubType.InterfaceName;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is MacAddress the mac address
        /// </summary>
        public PhysicalAddress MACAddress
        {
            get => (PhysicalAddress) GetSubTypeValue();
            set
            {
                SubType = ChassisSubType.MacAddress;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is NetworkAddress the network address
        /// </summary>
        public NetworkAddress NetworkAddress
        {
            get => (NetworkAddress) GetSubTypeValue();
            set
            {
                SubType = ChassisSubType.NetworkAddress;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is PortComponent
        /// </summary>
        public byte[] PortComponent
        {
            get => (byte[]) GetSubTypeValue();
            set
            {
                SubType = ChassisSubType.PortComponent;
                SetSubTypeValue(value);
            }
        }

        /// <value>
        /// The type of the TLV subtype
        /// </value>
        public ChassisSubType SubType
        {
            get => (ChassisSubType) Data.Bytes[ValueOffset];
            set => Data.Bytes[ValueOffset] = (byte) value;
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
            byte[] val;
            var dataOffset = ValueOffset + SubTypeLength;
            var dataLength = Length - SubTypeLength;

            switch (SubType)
            {
                case ChassisSubType.ChassisComponent:
                case ChassisSubType.InterfaceAlias:
                case ChassisSubType.LocallyAssigned:
                case ChassisSubType.PortComponent:
                {
                    val = new byte[dataLength];
                    Array.Copy(Data.Bytes,
                               dataOffset,
                               val,
                               0,
                               dataLength);

                    return val;
                }
                case ChassisSubType.NetworkAddress:
                {
                    return new NetworkAddress(Data.Bytes,
                                              dataOffset,
                                              dataLength);
                }
                case ChassisSubType.MacAddress:
                {
                    val = new byte[dataLength];
                    Array.Copy(Data.Bytes,
                               dataOffset,
                               val,
                               0,
                               dataLength);

                    return new PhysicalAddress(val);
                }
                case ChassisSubType.InterfaceName:
                {
                    return Encoding.ASCII.GetString(Data.Bytes, dataOffset, dataLength);
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void SetSubTypeValue(object val)
        {
            byte[] valBytes;

            // make sure we have the correct type
            switch (SubType)
            {
                case ChassisSubType.ChassisComponent:
                case ChassisSubType.InterfaceAlias:
                case ChassisSubType.LocallyAssigned:
                case ChassisSubType.PortComponent:
                {
                    valBytes = val as byte[] ?? throw new ArgumentOutOfRangeException(nameof(val), "expected byte[] for type");

                    SetSubTypeValue(valBytes);
                    break;
                }

                case ChassisSubType.NetworkAddress:
                {
                    if (!(val is NetworkAddress))
                    {
                        throw new ArgumentOutOfRangeException(nameof(val), "expected NetworkAddress instance for NetworkAddress");
                    }

                    valBytes = ((NetworkAddress) val).Bytes;

                    SetSubTypeValue(valBytes);
                    break;
                }
                case ChassisSubType.InterfaceName:
                {
                    if (!(val is string))
                    {
                        throw new ArgumentOutOfRangeException(nameof(val), "expected string for InterfaceName");
                    }

                    var interfaceName = (string) val;

                    valBytes = Encoding.ASCII.GetBytes(interfaceName);

                    SetSubTypeValue(valBytes);
                    break;
                }
                case ChassisSubType.MacAddress:
                {
                    if (!(val is PhysicalAddress))
                    {
                        throw new ArgumentOutOfRangeException(nameof(val), "expected PhysicalAddress for MacAddress");
                    }

                    var physicalAddress = (PhysicalAddress) val;

                    SetSubTypeValue(physicalAddress.GetAddressBytes());
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void SetSubTypeValue(byte[] subTypeValue)
        {
            // is the length different than the current length?
            if (subTypeValue.Length != Length)
            {
                var headerLength = TlvTypeLength.TypeLengthLength + SubTypeLength;
                var newTlvMemory = new byte[headerLength + subTypeValue.Length];

                // copy the header data over
                Array.Copy(Data.Bytes, Data.Offset, newTlvMemory, 0, headerLength);

                // update the TLV memory pointer, offset and length
                Data = new ByteArraySegment(newTlvMemory, 0, newTlvMemory.Length);
            }

            Array.Copy(subTypeValue,
                       0,
                       Data.Bytes,
                       ValueOffset + SubTypeLength,
                       subTypeValue.Length);
        }

        /// <summary>
        /// Convert this Chassis ID TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return $"[ChassisId: SubType={SubType}, SubTypeValue={SubTypeValue}]";
        }
    }
}