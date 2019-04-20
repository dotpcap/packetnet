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
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

#if DEBUG
using System.Reflection;
using log4net;
#endif

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// A Time to Live Tlv
    /// [Tlv Type Length : 2][Mgmt Addr length : 1][Mgmt Addr Subtype : 1][Mgmt Addr : 1-31]
    /// [Interface Subtype : 1][Interface number : 4][OID length : 1][OID : 0-128]
    /// </summary>
    [Serializable]
    public class ManagementAddress : Tlv
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
        /// Number of bytes in the interface number field
        /// </summary>
        private const int InterfaceNumberLength = 4;

        /// <summary>
        /// Number of bytes in the interface number subtype field
        /// </summary>
        private const int InterfaceNumberSubTypeLength = 1;

        /// <summary>
        /// Maximum number of bytes in the object identifier field
        /// </summary>
        private const int MaxObjectIdentifierLength = 128;

        /// <summary>
        /// Number of bytes in the AddressLength field
        /// </summary>
        private const int MgmtAddressLengthLength = 1;

        /// <summary>
        /// Number of bytes in the object identifier length field
        /// </summary>
        private const int ObjectIdentifierLengthLength = 1;

        /// <summary>
        /// Creates a Management Address Tlv
        /// </summary>
        /// <param name="bytes">
        /// The LLDP Data unit being modified
        /// </param>
        /// <param name="offset">
        /// The Management Address TLV's offset from the origin of the LLDP
        /// </param>
        public ManagementAddress(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Log.Debug("");
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
        public ManagementAddress
        (
            NetworkAddress managementAddress,
            InterfaceNumber interfaceSubType,
            uint ifNumber,
            string oid)
        {
            Log.Debug("");

            // NOTE: We presume that the mgmt address length and the
            //       object identifier length are zero
            var length = TlvTypeLength.TypeLengthLength +
                         MgmtAddressLengthLength +
                         InterfaceNumberSubTypeLength +
                         InterfaceNumberLength +
                         ObjectIdentifierLengthLength;

            var bytes = new byte[length];
            var offset = 0;
            TLVData = new ByteArraySegment(bytes, offset, length);

            // The lengths are both zero until the values are set
            AddressLength = 0;
            ObjIdLength = 0;

            Type = TlvType.ManagementAddress;

            Address = managementAddress;
            InterfaceSubType = interfaceSubType;
            InterfaceNumber = ifNumber;
            ObjectIdentifier = oid;
        }

        /// <value>
        /// The Management Address Length
        /// </value>
        public int AddressLength
        {
            get => TLVData.Bytes[ValueOffset];
            internal set => TLVData.Bytes[ValueOffset] = (byte) value;
        }

        /// <value>
        /// The Management Address Subtype
        /// Forward to the Address instance
        /// </value>
        public IanaAddressFamily AddressSubType => Address.AddressFamily;

        /// <value>
        /// Interface Number
        /// </value>
        public uint InterfaceNumber
        {
            get => EndianBitConverter.Big.ToUInt32(TLVData.Bytes,
                                                   InterfaceNumberOffset);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    TLVData.Bytes,
                                                    InterfaceNumberOffset);
        }

        /// <value>
        /// Interface Number Sub Type
        /// </value>
        public InterfaceNumber InterfaceSubType
        {
            get => (InterfaceNumber) TLVData.Bytes[ValueOffset + MgmtAddressLengthLength + Address.Length];
            set => TLVData.Bytes[ValueOffset + MgmtAddressLengthLength + Address.Length] = (byte) value;
        }

        /// <value>
        /// The Management Address
        /// </value>
        public NetworkAddress Address
        {
            get
            {
                var offset = ValueOffset + MgmtAddressLengthLength;

                return new NetworkAddress(TLVData.Bytes, offset, AddressLength);
            }

            set
            {
                var valueLength = value.Length;
                var valueBytes = value.Bytes;

                // is the new address the same size as the old address?
                if (AddressLength != valueLength)
                {
                    // need to resize the TLV and shift data fields down
                    var newLength = TlvTypeLength.TypeLengthLength +
                                    MgmtAddressLengthLength +
                                    valueLength +
                                    InterfaceNumberSubTypeLength +
                                    InterfaceNumberLength +
                                    ObjectIdentifierLengthLength +
                                    ObjIdLength;

                    var newBytes = new byte[newLength];

                    var headerLength = TlvTypeLength.TypeLengthLength + MgmtAddressLengthLength;
                    var oldStartOfAfterData = ValueOffset + MgmtAddressLengthLength + AddressLength;
                    var newStartOfAfterData = TlvTypeLength.TypeLengthLength + MgmtAddressLengthLength + value.Length;
                    var afterDataLength = InterfaceNumberSubTypeLength + InterfaceNumberLength + ObjectIdentifierLengthLength + ObjIdLength;

                    // copy the data before the mgmt address
                    Array.Copy(TLVData.Bytes,
                               TLVData.Offset,
                               newBytes,
                               0,
                               headerLength);

                    // copy the data over after the mgmt address over
                    Array.Copy(TLVData.Bytes,
                               oldStartOfAfterData,
                               newBytes,
                               newStartOfAfterData,
                               afterDataLength);

                    var offset = 0;
                    TLVData = new ByteArraySegment(newBytes, offset, newLength);

                    // update the address length field
                    AddressLength = valueLength;
                }

                // copy the new address into the appropriate position in the byte[]
                Array.Copy(valueBytes,
                           0,
                           TLVData.Bytes,
                           ValueOffset + MgmtAddressLengthLength,
                           valueLength);
            }
        }

        /// <value>
        /// Object ID
        /// </value>
        public string ObjectIdentifier
        {
            get => Encoding.UTF8.GetString(TLVData.Bytes,
                                           ObjectIdentifierOffset,
                                           ObjIdLength);
            set
            {
                var oid = Encoding.UTF8.GetBytes(value);

                // check for out-of-range sizes
                if (oid.Length > MaxObjectIdentifierLength)
                {
                    throw new ArgumentOutOfRangeException(nameof(oid), "length > maxObjectIdentifierLength of " + MaxObjectIdentifierLength);
                }

                // does the object identifier length match the existing one?
                if (ObjIdLength != oid.Length)
                {
                    var oldLength = TlvTypeLength.TypeLengthLength +
                                    MgmtAddressLengthLength +
                                    AddressLength +
                                    InterfaceNumberSubTypeLength +
                                    InterfaceNumberLength +
                                    ObjectIdentifierLengthLength;

                    var newLength = oldLength + oid.Length;

                    var newBytes = new byte[newLength];

                    // copy the original bytes over
                    Array.Copy(TLVData.Bytes,
                               TLVData.Offset,
                               newBytes,
                               0,
                               oldLength);

                    var offset = 0;
                    TLVData = new ByteArraySegment(newBytes, offset, newLength);

                    // update the length
                    ObjIdLength = (byte) value.Length;
                }

                Array.Copy(oid,
                           0,
                           TLVData.Bytes,
                           ObjectIdentifierOffset,
                           oid.Length);
            }
        }

        /// <value>
        /// Object ID Length
        /// </value>
        public byte ObjIdLength
        {
            get => TLVData.Bytes[ObjIdLengthOffset];

            internal set => TLVData.Bytes[ObjIdLengthOffset] = value;
        }

        private int InterfaceNumberOffset => ValueOffset + MgmtAddressLengthLength + AddressLength + InterfaceNumberSubTypeLength;

        private int ObjectIdentifierOffset => ObjIdLengthOffset + ObjectIdentifierLengthLength;

        private int ObjIdLengthOffset => InterfaceNumberOffset + InterfaceNumberLength;

        /// <summary>
        /// Convert this Management Address TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return
                $"[ManagementAddress: AddressLength={AddressLength}, AddressSubType={AddressSubType}, Address={Address}, InterfaceSubType={InterfaceSubType}, InterfaceNumber={InterfaceNumber}, ObjIdLength={ObjIdLength}, ObjectIdentifier={ObjectIdentifier}]";
        }
    }
}