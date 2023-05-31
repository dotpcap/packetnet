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
using PacketDotNet.Utils;
#if DEBUG
using System.Reflection;
using log4net;

#endif

namespace PacketDotNet.Lldp;

    /// <summary>
    /// An Organization Specific Tlv
    /// [Tlv Type Length : 2][Organizationally Unique Identifier OUI : 3]
    /// [Organizationally Defined Subtype : 1][Organizationally Defined Information String : 0 - 507]
    /// </summary>
    public class OrganizationSpecificTlv : Tlv
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

        private const int OUILength = 3;
        private const int OUISubTypeLength = 1;

        /// <summary>
        /// Creates an Organization Specific Tlv
        /// </summary>
        /// <param name="bytes">
        /// The LLDP Data unit being modified
        /// </param>
        /// <param name="offset">
        /// The Organization Specific TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public OrganizationSpecificTlv(byte[] bytes, int offset) :
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
        public OrganizationSpecificTlv(byte[] oui, int subType, byte[] infoString)
        {
            Log.Debug("");

            var length = TlvTypeLength.TypeLengthLength + OUILength + OUISubTypeLength;
            var bytes = new byte[length];
            var offset = 0;
            Data = new ByteArraySegment(bytes, offset, length);

            Type = TlvType.OrganizationSpecific;

            OrganizationUniqueID = oui;
            OrganizationDefinedSubType = subType;
            OrganizationDefinedInfoString = infoString;
        }

        /// <summary>
        /// An Organizationally Defined Information String
        /// </summary>
        public byte[] OrganizationDefinedInfoString
        {
            get
            {
                var length = Length - (OUILength + OUISubTypeLength);

                var bytes = new byte[length];
                Array.Copy(Data.Bytes,
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
                    var headerLength = TlvTypeLength.TypeLengthLength + OUILength + OUISubTypeLength;

                    // resize the tlv
                    var newLength = headerLength + value.Length;
                    var bytes = new byte[newLength];

                    // copy the header bytes over
                    Array.Copy(Data.Bytes,
                               Data.Offset,
                               bytes,
                               0,
                               headerLength);

                    // assign a new ByteArrayAndOffset to tlvData
                    var offset = 0;
                    Data = new ByteArraySegment(bytes, offset, newLength);
                }

                // copy the byte array in
                Array.Copy(value,
                           0,
                           Data.Bytes,
                           ValueOffset + OUILength + OUISubTypeLength,
                           value.Length);
            }
        }

        /// <summary>
        /// An Organizationally Defined SubType
        /// </summary>
        public int OrganizationDefinedSubType
        {
            get => Data.Bytes[ValueOffset + OUILength];
            set => Data.Bytes[ValueOffset + OUILength] = (byte) value;
        }

        /// <summary>
        /// An Organizationally Unique Identifier
        /// </summary>
        public byte[] OrganizationUniqueID
        {
            get
            {
                var oui = new byte[OUILength];
                Array.Copy(Data.Bytes,
                           ValueOffset,
                           oui,
                           0,
                           OUILength);

                return oui;
            }
            set => Array.Copy(value,
                              0,
                              Data.Bytes,
                              ValueOffset,
                              OUILength);
        }

        /// <summary>
        /// Convert this Organization Specific TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return
                $"[OrganizationSpecific: OrganizationUniqueID={OrganizationUniqueID}, OrganizationDefinedSubType={OrganizationDefinedSubType}, OrganizationDefinedInfoString={OrganizationDefinedInfoString}]";
        }
    }