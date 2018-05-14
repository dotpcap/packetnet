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
using System.Reflection;
using log4net;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// An Organization Specific TLV
    /// [TLV Type Length : 2][Organizationally Unique Identifier OUI : 3]
    /// [Organizationally Defined Subtype : 1][Organizationally Defined Information String : 0 - 507]
    /// </summary>
    [Serializable]
    public class OrganizationSpecific : TLV
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

        private const Int32 OUILength = 3;
        private const Int32 OUISubTypeLength = 1;


        #region Constructors

        /// <summary>
        /// Creates an Organization Specific TLV
        /// </summary>
        /// <param name="bytes">
        /// The LLDP Data unit being modified
        /// </param>
        /// <param name="offset">
        /// The Organization Specific TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public OrganizationSpecific(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            Log.Debug("");
        }

        /// <summary>
        /// Creates an Organization Specific TLV and sets it value
        /// </summary>
        /// <param name="oui">
        /// An Organizationally Unique Identifier
        /// </param>
        /// <param name="subType">
        /// An Organizationally Defined SubType
        /// </param>
        /// <param name="infoString">
        /// An Organizationally Defined Information String
        /// </param>
        public OrganizationSpecific(Byte[] oui, Int32 subType, Byte[] infoString)
        {
            Log.Debug("");

            var length = TLVTypeLength.TypeLengthLength + OUILength + OUISubTypeLength;
            var bytes = new Byte[length];
            var offset = 0;
            TLVData = new ByteArraySegment(bytes, offset, length);

            Type = TLVTypes.OrganizationSpecific;

            OrganizationUniqueID = oui;
            OrganizationDefinedSubType = subType;
            OrganizationDefinedInfoString = infoString;
        }

        #endregion


        #region Properties

        /// <summary>
        /// An Organizationally Unique Identifier
        /// </summary>
        public Byte[] OrganizationUniqueID
        {
            get
            {
                var oui = new Byte[OUILength];
                Array.Copy(TLVData.Bytes,
                           ValueOffset,
                           oui,
                           0,
                           OUILength);
                return oui;
            }

            set => Array.Copy(value,
                              0,
                              TLVData.Bytes,
                              ValueOffset,
                              OUILength);
        }

        /// <summary>
        /// An Organizationally Defined SubType
        /// </summary>
        public Int32 OrganizationDefinedSubType
        {
            get => TLVData.Bytes[ValueOffset + OUILength];
            set => TLVData.Bytes[ValueOffset + OUILength] = (Byte) value;
        }

        /// <summary>
        /// An Organizationally Defined Information String
        /// </summary>
        public Byte[] OrganizationDefinedInfoString
        {
            get
            {
                var length = Length - (OUILength + OUISubTypeLength);

                var bytes = new Byte[length];
                Array.Copy(TLVData.Bytes,
                           ValueOffset + OUILength + OUISubTypeLength,
                           bytes,
                           0,
                           length);

                return bytes;
            }

            set
            {
                var length = Length - (OUILength + OUISubTypeLength);

                // do we have the right sized tlv?
                if (value.Length != length)
                {
                    var headerLength = TLVTypeLength.TypeLengthLength + OUILength + OUISubTypeLength;

                    // resize the tlv
                    var newLength = headerLength + value.Length;
                    var bytes = new Byte[newLength];

                    // copy the header bytes over
                    Array.Copy(TLVData.Bytes,
                               TLVData.Offset,
                               bytes,
                               0,
                               headerLength);

                    // assign a new ByteArrayAndOffset to tlvData
                    var offset = 0;
                    TLVData = new ByteArraySegment(bytes, offset, newLength);
                }

                // copy the byte array in
                Array.Copy(value,
                           0,
                           TLVData.Bytes,
                           ValueOffset + OUILength + OUISubTypeLength,
                           value.Length);
            }
        }

        /// <summary>
        /// Convert this Organization Specific TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override String ToString()
        {
            return
                $"[OrganizationSpecific: OrganizationUniqueID={OrganizationUniqueID}, OrganizationDefinedSubType={OrganizationDefinedSubType}, OrganizationDefinedInfoString={OrganizationDefinedInfoString}]";
        }

        #endregion
    }
}