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
using System.Text;
using System.Collections.Generic;
using MiscUtil.Conversion;
using PacketDotNet.Tcp;
using PacketDotNet.Utils;

namespace PacketDotNet
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
        private static readonly ILogInactive log;
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
                if (ddmList.Count > 0) return this.ddmList;
                int startOffset = header.Offset;
                while (startOffset < header.BytesLength)
                {
                    ushort length = BigEndianBitConverter.Big.ToUInt16(header.Bytes, startOffset);
                    if (startOffset + length <= header.BytesLength)
                    {
                        var ddmBas = new ByteArraySegment(header.Bytes, startOffset, length);
                        ddmList.Add(new DrdaDDMPacket(ddmBas, this));
                    }
                    startOffset += length;
                }
                log.DebugFormat("DrdaDDMPacket.Count {0}",ddmList.Count);
                return ddmList;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas"></param>
        public DrdaPacket(ByteArraySegment bas)
        {
            log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            header = new ByteArraySegment(bas);

            // store the payload bytes
            payloadPacketOrData = new PacketOrByteArraySegment();
            payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas"></param>
        /// <param name="ParentPacket"></param>
        public DrdaPacket(ByteArraySegment bas,Packet ParentPacket) : this(bas)
        {
            log.DebugFormat("ParentPacket.GetType() {0}", ParentPacket.GetType());

            this.ParentPacket = ParentPacket;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputFormat"></param>
        /// <returns></returns>
        public override string ToString(StringOutputType outputFormat)
        {
            return base.ToString(outputFormat);
        }
    }
}
