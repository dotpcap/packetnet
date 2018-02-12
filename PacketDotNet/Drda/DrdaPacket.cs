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
 *  Copyright 2017 Andrew <pandipd@outlook.com>
 */

using System;
using System.Collections.Generic;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Drda
{
    /// <summary>
    /// DrdaPacket
    /// See: https://en.wikipedia.org/wiki/Distributed_Data_Management_Architecture
    /// </summary>
    [Serializable]
    public class DrdaPacket:Packet
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

        private List<DrdaDDMPacket> _ddmList;

        /// <summary>
        /// Decde DDM Packet into List
        /// </summary>
        public List<DrdaDDMPacket> DrdaDDMPackets
        {
            get
            {
                if (this._ddmList == null)
                {
                    this._ddmList = new List<DrdaDDMPacket>();
                }
                if (this._ddmList.Count > 0) return this._ddmList;
                Int32 startOffset = this.HeaderByteArraySegment.Offset;
                while (startOffset < this.HeaderByteArraySegment.BytesLength)
                {
                    UInt16 length = EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes, startOffset);
                    if (startOffset + length <= this.HeaderByteArraySegment.BytesLength)
                    {
                        var ddmBas = new ByteArraySegment(this.HeaderByteArraySegment.Bytes, startOffset, length);
                        this._ddmList.Add(new DrdaDDMPacket(ddmBas, this));
                    }
                    startOffset += length;
                }
                Log.DebugFormat("DrdaDDMPacket.Count {0}", this._ddmList.Count);
                return this._ddmList;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas"></param>
        public DrdaPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            this.HeaderByteArraySegment = new ByteArraySegment(bas);

            // store the payload bytes
            this.PayloadPacketOrData = new PacketOrByteArraySegment
            {
                TheByteArraySegment = this.HeaderByteArraySegment.EncapsulatedBytes()
            };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas"></param>
        /// <param name="parentPacket"></param>
        public DrdaPacket(ByteArraySegment bas,Packet parentPacket) : this(bas)
        {
            Log.DebugFormat("ParentPacket.GetType() {0}", parentPacket.GetType());

            this.ParentPacket = parentPacket;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputFormat"></param>
        /// <returns></returns>
        public override String ToString(StringOutputType outputFormat)
        {
            return base.ToString(outputFormat);
        }
    }
}
