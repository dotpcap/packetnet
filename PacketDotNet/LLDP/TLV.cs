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
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A Type-Length-Value object
    /// </summary>
    public class TLV
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

        #region Constructors

        /// <summary>
        /// Create a tlv
        /// </summary>
        public TLV()
        {
        }

        /// <summary>
        /// Creates a TLV
        /// </summary>
        /// <param name="bytes">
        /// Bytes that comprise the TLV
        /// </param>
        /// <param name="offset">
        /// The TLVs offset from the start of byte[] bytes
        /// </param>
        public TLV(byte[] bytes, int offset)
        {
            // setup a local ByteArrayAndOffset in order to retrieve the value length
            // NOTE: we cannot set tlvData to retrieve the value length as
            //       setting tlvData results in the TypeLength.Length being updated with
            //       the length of the ByteArrayAndOffset which would overwrite the value
            //       we are trying to retrieve
            var byteArraySegment = new ByteArraySegment(bytes, offset, TLVTypeLength.TypeLengthLength);
            TypeLength = new TLVTypeLength(byteArraySegment);

            // set the tlvData assuming we have at least the bytes required for the
            // type/length fields
            tlvData =  new ByteArraySegment(bytes, offset, TypeLength.Length + TLVTypeLength.TypeLengthLength);

            // retrieve the actual length
            tlvData.Length = TypeLength.Length + TLVTypeLength.TypeLengthLength;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Length of value portion of the TLV
        /// NOTE: Does not include the length of the Type and Length fields
        /// </summary>
        public int Length
        {
            get { return TypeLength.Length; }

            // Length set property is internal because the tlv length is
            // automatically set based on the length of the tlv value
            internal set { TypeLength.Length = value; }
        }

        /// <summary>
        /// Total length of the TLV, including the length of the Type and Length fields
        /// </summary>
        public int TotalLength
        {
            get { return tlvData.Length; }
        }

        /// <summary>
        /// Tlv type
        /// </summary>
        public TLVTypes Type
        {
            get { return TypeLength.Type; }

            set
            {
                log.DebugFormat("value {0}", value);
                TypeLength.Type = value;
            }
        }

        /// <summary>
        /// Offset to the value bytes of the TLV
        /// </summary>
        internal int ValueOffset
        {
            get { return tlvData.Offset + TLVTypeLength.TypeLengthLength; }
        }

        /// <summary>
        /// Return a byte[] that contains the tlv
        /// </summary>
        public virtual byte[] Bytes
        {
            get
            {
                return tlvData.ActualBytes();
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Points to the TLV data
        /// </summary>
        private ByteArraySegment _tlvData;

        /// <summary>
        /// Points to the TLV data
        /// </summary>
        internal ByteArraySegment tlvData
        {
            get { return _tlvData; }

            set
            {
                _tlvData = value;

                // create a new TypeLength that points at the new ByteArrayAndOffset
                TypeLength = new TLVTypeLength(value);

                // update the length based upon the length of the ByteArrayAndOffset
                TypeLength.Length = value.Length - TLVTypeLength.TypeLengthLength;
            }
        }

        /// <summary>
        /// Interface to this TLVs type and length
        /// </summary>
        protected TLVTypeLength TypeLength;

        #endregion
    }
}