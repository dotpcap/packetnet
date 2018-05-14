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
using System.Reflection;
using System.Text;
using log4net;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A Chassis ID TLV
    /// </summary>
    [Serializable]
    public class ChassisID : TLV
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
        private const Int32 SubTypeLength = 1;


        #region Constructors

        /// <summary>
        /// Creates a Chassis ID TLV by parsing a byte[]
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Chassis ID TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public ChassisID(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            Log.Debug("");
        }

        /// <summary>
        /// Creates a Chassis ID TLV and sets it value
        /// </summary>
        /// <param name="subType">
        /// The ChassisID subtype
        /// </param>
        /// <param name="subTypeValue">
        /// The subtype's value
        /// </param>
        public ChassisID(ChassisSubTypes subType, Object subTypeValue)
        {
            Log.DebugFormat("subType {0}", subType);

            EmptyTLVDataInit();

            Type = TLVTypes.ChassisID;

            SubType = subType;

            // method will resize the tlv
            SubTypeValue = subTypeValue;
        }

        /// <summary>
        /// Create a ChassisID given a mac address
        /// </summary>
        /// <param name="macAddress">
        /// A <see cref="PhysicalAddress" />
        /// </param>
        public ChassisID(PhysicalAddress macAddress)
        {
            Log.DebugFormat("MACAddress {0}", macAddress);

            EmptyTLVDataInit();

            Type = TLVTypes.ChassisID;
            SubType = ChassisSubTypes.MACAddress;

            SubTypeValue = macAddress;
        }

        /// <summary>
        /// Create a ChassisID given an interface name
        /// http://tools.ietf.org/search/rfc2863 page 38
        /// </summary>
        /// <param name="interfaceName">
        /// A <see cref="string" />
        /// </param>
        public ChassisID(String interfaceName)
        {
            Log.DebugFormat("InterfaceName {0}", interfaceName);

            EmptyTLVDataInit();

            Type = TLVTypes.ChassisID;
            SubType = ChassisSubTypes.InterfaceName;

            SetSubTypeValue(interfaceName);
        }

        #endregion


        #region Properties

        /// <value>
        /// The type of the TLV subtype
        /// </value>
        public ChassisSubTypes SubType
        {
            get => (ChassisSubTypes) TLVData.Bytes[ValueOffset];

            set => TLVData.Bytes[ValueOffset] = (Byte) value;
        }

        /// <value>
        /// The TLV subtype value
        /// </value>
        public Object SubTypeValue
        {
            get => GetSubTypeValue();
            set => SetSubTypeValue(value);
        }

        /// <summary>
        /// If SubType is ChassisComponent
        /// </summary>
        public Byte[] ChassisComponent
        {
            get => (Byte[]) GetSubTypeValue();
            set
            {
                SubType = ChassisSubTypes.ChassisComponent;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is InterfaceName the interface name
        /// </summary>
        public String InterfaceName
        {
            get => (String) GetSubTypeValue();
            set
            {
                SubType = ChassisSubTypes.InterfaceName;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is MACAddress the mac address
        /// </summary>
        public PhysicalAddress MACAddress
        {
            get => (PhysicalAddress) GetSubTypeValue();
            set
            {
                SubType = ChassisSubTypes.MACAddress;
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
                SubType = ChassisSubTypes.NetworkAddress;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is PortComponent
        /// </summary>
        public Byte[] PortComponent
        {
            get => (Byte[]) GetSubTypeValue();
            set
            {
                SubType = ChassisSubTypes.PortComponent;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is InterfaceAlias
        /// </summary>
        public Byte[] InterfaceAlias
        {
            get => (Byte[]) GetSubTypeValue();
            set
            {
                SubType = ChassisSubTypes.InterfaceAlias;
                SetSubTypeValue(value);
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Helper method to reduce duplication in type specific constructors
        /// </summary>
        private void EmptyTLVDataInit()
        {
            var length = TLVTypeLength.TypeLengthLength + SubTypeLength;
            var bytes = new Byte[length];
            var offset = 0;
            TLVData = new ByteArraySegment(bytes, offset, length);
        }

        private Object GetSubTypeValue()
        {
            Byte[] val;
            var dataOffset = ValueOffset + SubTypeLength;
            var dataLength = Length - SubTypeLength;

            switch (SubType)
            {
                case ChassisSubTypes.ChassisComponent:
                case ChassisSubTypes.InterfaceAlias:
                case ChassisSubTypes.LocallyAssigned:
                case ChassisSubTypes.PortComponent:
                    val = new Byte[dataLength];
                    Array.Copy(TLVData.Bytes,
                               dataOffset,
                               val,
                               0,
                               dataLength);
                    return val;
                case ChassisSubTypes.NetworkAddress:
                    return new NetworkAddress(TLVData.Bytes,
                                              dataOffset,
                                              dataLength);
                case ChassisSubTypes.MACAddress:
                    val = new Byte[dataLength];
                    Array.Copy(TLVData.Bytes,
                               dataOffset,
                               val,
                               0,
                               dataLength);
                    return new PhysicalAddress(val);
                case ChassisSubTypes.InterfaceName:
                    return Encoding.ASCII.GetString(TLVData.Bytes, dataOffset, dataLength);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSubTypeValue(Object val)
        {
            Byte[] valBytes;

            // make sure we have the correct type
            switch (SubType)
            {
                case ChassisSubTypes.ChassisComponent:
                case ChassisSubTypes.InterfaceAlias:
                case ChassisSubTypes.LocallyAssigned:
                case ChassisSubTypes.PortComponent:

                    valBytes = val as byte[] ?? throw new ArgumentOutOfRangeException(nameof(val), "expected byte[] for type");

                    SetSubTypeValue(valBytes);
                    break;
                case ChassisSubTypes.NetworkAddress:
                    if (!(val is NetworkAddress))
                    {
                        throw new ArgumentOutOfRangeException(nameof(val), "expected NetworkAddress instance for NetworkAddress");
                    }

                    valBytes = ((NetworkAddress) val).Bytes;

                    SetSubTypeValue(valBytes);
                    break;
                case ChassisSubTypes.InterfaceName:
                    if (!(val is String))
                    {
                        throw new ArgumentOutOfRangeException(nameof(val), "expected string for InterfaceName");
                    }

                    var interfaceName = (String) val;

                    valBytes = Encoding.ASCII.GetBytes(interfaceName);

                    SetSubTypeValue(valBytes);
                    break;
                case ChassisSubTypes.MACAddress:
                    if (!(val is PhysicalAddress))
                    {
                        throw new ArgumentOutOfRangeException(nameof(val), "expected PhysicalAddress for MACAddress");
                    }

                    var physicalAddress = (PhysicalAddress) val;

                    SetSubTypeValue(physicalAddress.GetAddressBytes());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSubTypeValue(Byte[] subTypeValue)
        {
            // is the length different than the current length?
            if (subTypeValue.Length != Length)
            {
                var headerLength = TLVTypeLength.TypeLengthLength + SubTypeLength;
                var newTlvMemory = new Byte[headerLength + subTypeValue.Length];

                // copy the header data over
                Array.Copy(TLVData.Bytes, TLVData.Offset, newTlvMemory, 0, headerLength);

                // update the tlv memory pointer, offset and length
                TLVData = new ByteArraySegment(newTlvMemory, 0, newTlvMemory.Length);
            }

            Array.Copy(subTypeValue,
                       0,
                       TLVData.Bytes,
                       ValueOffset + SubTypeLength,
                       subTypeValue.Length);
        }

        /// <summary>
        /// Convert this Chassis ID TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override String ToString()
        {
            return $"[ChassisID: SubType={SubType}, SubTypeValue={SubTypeValue}]";
        }

        #endregion
    }
}