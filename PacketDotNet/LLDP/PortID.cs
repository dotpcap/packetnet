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
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A Port ID TLV
    /// </summary>
    [Serializable]
    public class PortID : TLV
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        private const Int32 SubTypeLength = 1;

        #region Constructors

        /// <summary>
        /// Creates a Port ID TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Port ID TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public PortID(Byte[] bytes, Int32 offset) :
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
        public PortID(PortSubTypes subType, Object subTypeValue)
        {
            Log.Debug("");

            this.EmptyTLVDataInit();

            this.Type = TLVTypes.PortID;
            this.SubType = subType;

            // method will resize the tlv
            this.SubTypeValue = subTypeValue;
        }

        /// <summary>
        /// Construct a PortID from a NetworkAddress
        /// </summary>
        /// <param name="networkAddress">
        /// A <see cref="LLDP.NetworkAddress"/>
        /// </param>
        public PortID(NetworkAddress networkAddress)
        {
            Log.DebugFormat("NetworkAddress {0}", networkAddress.ToString());

            var length = TLVTypeLength.TypeLengthLength + SubTypeLength;
            var bytes = new Byte[length];
            var offset = 0;
            this.TLVData = new ByteArraySegment(bytes, offset, length);

            this.Type = TLVTypes.PortID;
            this.SubType = PortSubTypes.NetworkAddress;
            this.SubTypeValue = networkAddress;
        }

        #endregion

        #region Properties

        /// <value>
        /// The type of the TLV subtype
        /// </value>
        public PortSubTypes SubType
        {
            get => (PortSubTypes) this.TLVData.Bytes[this.TLVData.Offset + TLVTypeLength.TypeLengthLength];
            set => this.TLVData.Bytes[this.TLVData.Offset + TLVTypeLength.TypeLengthLength] = (Byte)value;
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
        /// Offset to the value field
        /// </summary>
        private Int32 DataOffset => this.ValueOffset + SubTypeLength;

        /// <summary>
        /// Size of the value field
        /// </summary>
        private Int32 DataLength => this.Length - SubTypeLength;

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
            this.TLVData = new ByteArraySegment(bytes, offset, length);
        }

        private Object GetSubTypeValue()
        {
            Byte[] arrAddress;

            switch (this.SubType)
            {
                case PortSubTypes.InterfaceAlias:
                case PortSubTypes.InterfaceName:
                case PortSubTypes.LocallyAssigned:
                case PortSubTypes.PortComponent:
                case PortSubTypes.AgentCircuitID:
                    // get the address
                    arrAddress = new Byte[this.DataLength];
                    Array.Copy(this.TLVData.Bytes, this.DataOffset, arrAddress, 0, this.DataLength);
                    return arrAddress;
                case PortSubTypes.MACAddress:
                    // get the address
                    arrAddress = new Byte[this.DataLength];
                    Array.Copy(this.TLVData.Bytes, this.DataOffset, arrAddress, 0, this.DataLength);
                    PhysicalAddress address = new PhysicalAddress(arrAddress);
                    return address;
                case PortSubTypes.NetworkAddress:
                    // get the address
                    AddressFamily addressFamily = (AddressFamily) this.TLVData.Bytes[this.DataLength];
                    return this.GetNetworkAddress(addressFamily);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSubTypeValue(Object subTypeValue)
        {
            switch (this.SubType)
            {
                case PortSubTypes.InterfaceAlias:
                case PortSubTypes.InterfaceName:
                case PortSubTypes.LocallyAssigned:
                case PortSubTypes.PortComponent:
                case PortSubTypes.AgentCircuitID:
                    this.SetSubTypeValue((Byte[])subTypeValue);
                    break;
                case PortSubTypes.MACAddress:
                    this.SetSubTypeValue(((PhysicalAddress)subTypeValue).GetAddressBytes());
                    break;
                case PortSubTypes.NetworkAddress:
                    this.SetSubTypeValue(((NetworkAddress)subTypeValue).Bytes);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSubTypeValue(Byte[] val)
        {
            // does our current length match?
            Int32 dataLength = this.Length - SubTypeLength;
            if(dataLength != val.Length)
            {
                var headerLength = TLVTypeLength.TypeLengthLength + SubTypeLength;
                var newLength = headerLength + val.Length;
                var newBytes = new Byte[newLength];

                // copy the header data over
                Array.Copy(this.TLVData.Bytes, this.TLVData.Offset,
                           newBytes, 0,
                           headerLength);

                var offset = 0;
                this.TLVData = new ByteArraySegment(newBytes, offset, newLength);
            }

            Array.Copy(val, 0, this.TLVData.Bytes, this.ValueOffset + SubTypeLength,
                       val.Length);
        }

        private NetworkAddress GetNetworkAddress(AddressFamily addressFamily)
        {
            if(this.SubType != PortSubTypes.NetworkAddress)
            {
                throw new ArgumentOutOfRangeException("SubType != PortSubTypes.NetworkAddress");
            }

            var networkAddress = new NetworkAddress(this.TLVData.Bytes, this.DataOffset, this.DataLength);

            return networkAddress;
        }

        /// <summary>
        /// Convert this Port ID TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override String ToString ()
        {
            return $"[PortID: SubType={this.SubType}, SubTypeValue={this.SubTypeValue}]";
        }

        #endregion
    }
}