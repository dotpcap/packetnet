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
using System.Net;
using System.Net.NetworkInformation;
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
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
            log.Debug("");
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
            log.DebugFormat("subType {0}", subType);

            this.EmptyTLVDataInit();

            this.Type = TLVTypes.ChassisID;

            this.SubType = subType;

            // method will resize the tlv
            this.SubTypeValue = subTypeValue;
        }

        /// <summary>
        /// Create a ChassisID given a mac address
        /// </summary>
        /// <param name="MACAddress">
        /// A <see cref="PhysicalAddress"/>
        /// </param>
        public ChassisID(PhysicalAddress MACAddress)
        {
            log.DebugFormat("MACAddress {0}", MACAddress.ToString());

            this.EmptyTLVDataInit();

            this.Type = TLVTypes.ChassisID;
            this.SubType = ChassisSubTypes.MACAddress;

            this.SubTypeValue = MACAddress;
        }

        /// <summary>
        /// Create a ChassisID given an interface name
        /// http://tools.ietf.org/search/rfc2863 page 38
        /// </summary>
        /// <param name="InterfaceName">
        /// A <see cref="System.String"/>
        /// </param>
        public ChassisID(String InterfaceName)
        {
            log.DebugFormat("InterfaceName {0}", InterfaceName);

            this.EmptyTLVDataInit();

            this.Type = TLVTypes.ChassisID;
            this.SubType = ChassisSubTypes.InterfaceName;

            this.SetSubTypeValue(InterfaceName);
        }

        #endregion

        #region Properties

        /// <value>
        /// The type of the TLV subtype
        /// </value>
        public ChassisSubTypes SubType
        {
            get => (ChassisSubTypes) this.tlvData.Bytes[this.ValueOffset];

            set => this.tlvData.Bytes[this.ValueOffset] = (Byte)value;
        }

        /// <value>
        /// The TLV subtype value
        /// </value>
        public Object SubTypeValue
        {
            get => this.GetSubTypeValue();
            set => this.SetSubTypeValue(value);
        }

        /// <summary>
        /// If SubType is ChassisComponent
        /// </summary>
        public Byte[] ChassisComponent
        {
            get => (Byte[]) this.GetSubTypeValue();
            set
            {
                this.SubType = ChassisSubTypes.ChassisComponent;
                this.SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is InterfaceName the interface name
        /// </summary>
        public String InterfaceName
        {
            get => (String) this.GetSubTypeValue();
            set
            {
                this.SubType = ChassisSubTypes.InterfaceName;
                this.SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is MACAddress the mac address
        /// </summary>
        public PhysicalAddress MACAddress
        {
            get => (PhysicalAddress) this.GetSubTypeValue();
            set
            {
                this.SubType = ChassisSubTypes.MACAddress;
                this.SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is NetworkAddress the network address
        /// </summary>
        public NetworkAddress NetworkAddress
        {
            get => (NetworkAddress) this.GetSubTypeValue();
            set
            {
                this.SubType = ChassisSubTypes.NetworkAddress;
                this.SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is PortComponent
        /// </summary>
        public Byte[] PortComponent
        {
            get => (Byte[]) this.GetSubTypeValue();
            set
            {
                this.SubType = ChassisSubTypes.PortComponent;
                this.SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is InterfaceAlias
        /// </summary>
        public Byte[] InterfaceAlias
        {
            get => (Byte[]) this.GetSubTypeValue();
            set
            {
                this.SubType = ChassisSubTypes.InterfaceAlias;
                this.SetSubTypeValue(value);
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
            Int32 offset = 0;
            this.tlvData = new ByteArraySegment(bytes, offset, length);
        }

        private Object GetSubTypeValue()
        {
            Byte[] val;
            Int32 dataOffset = this.ValueOffset + SubTypeLength;
            Int32 dataLength = this.Length - SubTypeLength;

            switch (this.SubType)
            {
                case ChassisSubTypes.ChassisComponent:
                case ChassisSubTypes.InterfaceAlias:
                case ChassisSubTypes.LocallyAssigned:
                case ChassisSubTypes.PortComponent:
                    val = new Byte[dataLength];
                    Array.Copy(this.tlvData.Bytes, dataOffset,
                               val, 0,
                               dataLength);
                    return val;
                case ChassisSubTypes.NetworkAddress:
                    return new NetworkAddress(this.tlvData.Bytes,
                                              dataOffset,
                                              dataLength);
                case ChassisSubTypes.MACAddress:
                    val = new Byte[dataLength];
                    Array.Copy(this.tlvData.Bytes, dataOffset,
                               val, 0,
                               dataLength);
                    return new PhysicalAddress(val);
                case ChassisSubTypes.InterfaceName:
                    return System.Text.Encoding.ASCII.GetString(this.tlvData.Bytes, dataOffset, dataLength);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSubTypeValue(Object val)
        {
            Byte[] valBytes;

            // make sure we have the correct type
            switch (this.SubType)
            {
                case ChassisSubTypes.ChassisComponent:
                case ChassisSubTypes.InterfaceAlias:
                case ChassisSubTypes.LocallyAssigned:
                case ChassisSubTypes.PortComponent:
                    if(!(val is Byte[]))
                    {
                        throw new ArgumentOutOfRangeException("expected byte[] for type");
                    }

                    valBytes = (Byte[])val;

                    this.SetSubTypeValue(valBytes);
                    break;
                case ChassisSubTypes.NetworkAddress:
                    if(!(val is NetworkAddress))
                    {
                        throw new ArgumentOutOfRangeException("expected NetworkAddress instance for NetworkAddress");
                    }

                    valBytes = ((NetworkAddress)val).Bytes;

                    this.SetSubTypeValue(valBytes);
                    break;
                case ChassisSubTypes.InterfaceName:
                    if(!(val is String))
                    {
                        throw new ArgumentOutOfRangeException("expected string for InterfaceName");
                    }

                    var interfaceName = (String)val;

                    valBytes = System.Text.Encoding.ASCII.GetBytes(interfaceName);

                    this.SetSubTypeValue(valBytes);
                    break;
                case ChassisSubTypes.MACAddress:
                    if(!(val is PhysicalAddress))
                    {
                        throw new ArgumentOutOfRangeException("expected PhysicalAddress for MACAddress");
                    }

                    var physicalAddress = (PhysicalAddress)val;

                    this.SetSubTypeValue(physicalAddress.GetAddressBytes());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSubTypeValue(Byte[] subTypeValue)
        {
            // is the length different than the current length?
            if(subTypeValue.Length != this.Length)
            {
                var headerLength = TLVTypeLength.TypeLengthLength + SubTypeLength;
                var newTlvMemory = new Byte[headerLength + subTypeValue.Length];

                // copy the header data over
                Array.Copy(this.tlvData.Bytes, this.tlvData.Offset, newTlvMemory, 0, headerLength);

                // update the tlv memory pointer, offset and length
                this.tlvData = new ByteArraySegment(newTlvMemory, 0, newTlvMemory.Length);
            }

            Array.Copy(subTypeValue, 0, this.tlvData.Bytes, this.ValueOffset + SubTypeLength,
                       subTypeValue.Length);
        }

        /// <summary>
        /// Convert this Chassis ID TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override String ToString ()
        {
            return String.Format("[ChassisID: SubType={0}, SubTypeValue={1}]", this.SubType, this.SubTypeValue);
        }

        #endregion
    }
}