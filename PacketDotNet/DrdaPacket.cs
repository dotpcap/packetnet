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
using System.Reflection;
using log4net;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// DrdaPacket
    /// See: https://en.wikipedia.org/wiki/Distributed_Data_Management_Architecture
    /// </summary>
    [Serializable]
    public class DrdaPacket : Packet
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

        private List<DrdaDDMPacket> ddmList;

        /// <summary>
        /// Decde DDM Packet into List
        /// </summary>
        public List<DrdaDDMPacket> DrdaDDMPackets
        {
            get
            {
                if (ddmList == null)
                {
                    ddmList = new List<DrdaDDMPacket>();
                }

                if (ddmList.Count > 0) return ddmList;


                var startOffset = Header.Offset;
                while (startOffset < Header.BytesLength)
                {
                    var length = EndianBitConverter.Big.ToUInt16(Header.Bytes, startOffset);
                    if (startOffset + length <= Header.BytesLength)
                    {
                        var ddmBas = new ByteArraySegment(Header.Bytes, startOffset, length);
                        ddmList.Add(new DrdaDDMPacket(ddmBas, this));
                    }

                    startOffset += length;
                }

                Log.DebugFormat("DrdaDDMPacket.Count {0}", ddmList.Count);
                return ddmList;
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
            Header = new ByteArraySegment(bas);

            // store the payload bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment {ByteArraySegment = Header.EncapsulatedBytes()};
                return result;
            });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas"></param>
        /// <param name="ParentPacket"></param>
        public DrdaPacket(ByteArraySegment bas, Packet ParentPacket) : this(bas)
        {
            Log.DebugFormat("ParentPacket.GetType() {0}", ParentPacket.GetType());

            this.ParentPacket = ParentPacket;
        }

        /// <summary>
        /// </summary>
        /// <param name="outputFormat"></param>
        /// <returns></returns>
        public override String ToString(StringOutputType outputFormat)
        {
            return base.ToString(outputFormat);
        }
    }
}