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
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A Time to Live TLV
    ///
    /// [TLV Type Length : 2][Mgmt Addr length : 1][Mgmt Addr Subtype : 1][Mgmt Addr : 1-31]
    /// [Interface Subtype : 1][Interface number : 4][OID length : 1][OID : 0-128]
    ///
    /// </summary>
    [Serializable]
    public class ManagementAddress : TLV
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
        /// Number of bytes in the AddressLength field
        /// </summary>
        private const int MgmtAddressLengthLength = 1;

        /// <summary>
        /// Number of bytes in the interface number subtype field
        /// </summary>
        private const int InterfaceNumberSubTypeLength = 1;

        /// <summary>
        /// Number of bytes in the interface number field
        /// </summary>
        private const int InterfaceNumberLength = 4;

        /// <summary>
        /// Number of bytes in the object identifier length field
        /// </summary>
        private const int ObjectIdentifierLengthLength = 1;

        /// <summary>
        /// Maximum number of bytes in the object identifier field
        /// </summary>
        private const int maxObjectIdentifierLength = 128;

        #region Constructors

        /// <summary>
        /// Creates a Management Address TLV
        /// </summary>
        /// <param name="bytes">
        /// The LLDP Data unit being modified
        /// </param>
        /// <param name="offset">
        /// The Management Address TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public ManagementAddress(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            log.Debug("");
        }

        /// <summary>
        /// Creates a Management Address TLV and sets it value
        /// </summary>
        /// <param name="managementAddress">
        /// The Management Address
        /// </param>
        /// <param name="interfaceSubType">
        /// The Interface Numbering Sub Type
        /// </param>
        /// <param name="ifNumber">
        /// The Interface Number
        /// </param>
        /// <param name="oid">
        /// The Object Identifier
        /// </param>
        public ManagementAddress(NetworkAddress managementAddress,
                                 InterfaceNumbering interfaceSubType, uint ifNumber,
                                 string oid)
        {
            log.Debug("");

            // NOTE: We presume that the mgmt address length and the
            //       object identifier length are zero
            var length = TLVTypeLength.TypeLengthLength + MgmtAddressLengthLength +
                         InterfaceNumberSubTypeLength + InterfaceNumberLength +
                         ObjectIdentifierLengthLength;
            var bytes = new byte[length];
            var offset = 0;
            this.tlvData = new ByteArraySegment(bytes, offset, length);

            // The lengths are both zero until the values are set
            this.AddressLength = 0;
            this.ObjIdLength = 0;

            this.Type = TLVTypes.ManagementAddress;

            this.MgmtAddress = managementAddress;
            this.InterfaceSubType = interfaceSubType;
            this.InterfaceNumber = ifNumber;
            this.ObjectIdentifier = oid;
        }

        #endregion

        #region Properties

        /// <value>
        /// The Management Address Length
        /// </value>
        public int AddressLength
        {
            get { return (int) this.tlvData.Bytes[this.ValueOffset]; }
            internal set { this.tlvData.Bytes[this.ValueOffset] = (byte)value; }
        }

        /// <value>
        /// The Management Address Subtype
        ///
        /// Forward to the MgmtAddress instance
        /// </value>
        public AddressFamily AddressSubType
        {
            get { return this.MgmtAddress.AddressFamily; }
        }

        /// <value>
        /// The Management Address
        /// </value>
        public NetworkAddress MgmtAddress
        {
            get
            {
                int offset = this.ValueOffset + MgmtAddressLengthLength;

                return new NetworkAddress(this.tlvData.Bytes, offset, this.AddressLength);
            }

            set
            {
                var valueLength = value.Length;
                var valueBytes = value.Bytes;

                // is the new address the same size as the old address?
                if(this.AddressLength != valueLength)
                {
                    // need to resize the tlv and shift data fields down
                    var newLength = TLVTypeLength.TypeLengthLength + MgmtAddressLengthLength +
                                    valueLength +
                                    InterfaceNumberSubTypeLength +
                                    InterfaceNumberLength +
                                    ObjectIdentifierLengthLength + this.ObjIdLength;

                    var newBytes = new byte[newLength];

                    int headerLength = TLVTypeLength.TypeLengthLength + MgmtAddressLengthLength;
                    int oldStartOfAfterData = this.ValueOffset + MgmtAddressLengthLength + this.AddressLength;
                    int newStartOfAfterData = TLVTypeLength.TypeLengthLength + MgmtAddressLengthLength + value.Length;
                    int afterDataLength = InterfaceNumberSubTypeLength + InterfaceNumberLength + ObjectIdentifierLengthLength + this.ObjIdLength;

                    // copy the data before the mgmt address
                    Array.Copy(this.tlvData.Bytes, this.tlvData.Offset,
                               newBytes, 0,
                               headerLength);

                    // copy the data over after the mgmt address over
                    Array.Copy(this.tlvData.Bytes, oldStartOfAfterData,
                               newBytes, newStartOfAfterData,
                               afterDataLength);

                    var offset = 0;
                    this.tlvData = new ByteArraySegment(newBytes, offset, newLength);

                    // update the address length field
                    this.AddressLength = valueLength;
                }

                // copy the new address into the appropriate position in the byte[]
                Array.Copy(valueBytes, 0, this.tlvData.Bytes, this.ValueOffset + MgmtAddressLengthLength,
                           valueLength);
            }
        }

        /// <value>
        /// Interface Number Sub Type
        /// </value>
        public InterfaceNumbering InterfaceSubType
        {
            get
            {
                return (InterfaceNumbering) this.tlvData.Bytes[this.ValueOffset + MgmtAddressLengthLength + this.MgmtAddress.Length];
            }

            set
            {
                this.tlvData.Bytes[this.ValueOffset + MgmtAddressLengthLength + this.MgmtAddress.Length] = (byte)value;
            }
        }

        private int InterfaceNumberOffset
        {
            get
            {
                return this.ValueOffset + MgmtAddressLengthLength + this.AddressLength + InterfaceNumberSubTypeLength;
            }
        }

        /// <value>
        /// Interface Number
        /// </value>
        public uint InterfaceNumber
        {
            get
            {
                return EndianBitConverter.Big.ToUInt32(this.tlvData.Bytes, this.InterfaceNumberOffset);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value, this.tlvData.Bytes, this.InterfaceNumberOffset);
            }
        }

        private int ObjIdLengthOffset
        {
            get
            {
                return this.InterfaceNumberOffset + InterfaceNumberLength;
            }
        }

        /// <value>
        /// Object ID Length
        /// </value>
        public byte ObjIdLength
        {
            get
            {
                return this.tlvData.Bytes[this.ObjIdLengthOffset];
            }

            internal set
            {
                this.tlvData.Bytes[this.ObjIdLengthOffset] = value;
            }
        }

        private int ObjectIdentifierOffset
        {
            get { return this.ObjIdLengthOffset + ObjectIdentifierLengthLength; }
        }

        /// <value>
        /// Object ID
        /// </value>
        public string ObjectIdentifier
        {
            get
            {
                return Encoding.UTF8.GetString(this.tlvData.Bytes, this.ObjectIdentifierOffset, this.ObjIdLength);
            }

            set
            {
                byte[] oid = Encoding.UTF8.GetBytes(value);

                // check for out-of-range sizes
                if(oid.Length > maxObjectIdentifierLength)
                {
                    throw new ArgumentOutOfRangeException("ObjectIdentifier", "length > maxObjectIdentifierLength of " + maxObjectIdentifierLength);
                }

                // does the object identifier length match the existing one?
                if(this.ObjIdLength != oid.Length)
                {
                    var oldLength = TLVTypeLength.TypeLengthLength + MgmtAddressLengthLength + this.AddressLength +
                                    InterfaceNumberSubTypeLength + InterfaceNumberLength +
                                    ObjectIdentifierLengthLength;
                    var newLength = oldLength + oid.Length;

                    var newBytes = new byte[newLength];

                    // copy the original bytes over
                    Array.Copy(this.tlvData.Bytes, this.tlvData.Offset,
                               newBytes, 0,
                               oldLength);

                    var offset = 0;
                    this.tlvData = new ByteArraySegment(newBytes, offset, newLength);

                    // update the length
                    this.ObjIdLength = (byte)value.Length;
                }

                Array.Copy(oid, 0, this.tlvData.Bytes, this.ObjectIdentifierOffset,
                           oid.Length);
            }
        }

        /// <summary>
        /// Convert this Management Address TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[ManagementAddress: AddressLength={0}, AddressSubType={1}, MgmtAddress={2}, InterfaceSubType={3}, InterfaceNumber={4}, ObjIdLength={5}, ObjectIdentifier={6}]", this.AddressLength, this.AddressSubType, this.MgmtAddress, this.InterfaceSubType, this.InterfaceNumber, this.ObjIdLength, this.ObjectIdentifier);
        }

        #endregion
    }
}