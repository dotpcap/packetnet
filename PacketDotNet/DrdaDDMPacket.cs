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
using System.Text;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// DrdaPacket
    /// See: https://en.wikipedia.org/wiki/DRDA
    /// </summary>
    public sealed class DrdaDDMPacket : Packet
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
        /// The Length field
        /// </summary>
        public UInt16 Length => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DrdaDDMFields.LengthPosition);

        /// <summary>
        /// The Magic field
        /// </summary>
        public Byte Magic => Header.Bytes[Header.Offset + DrdaDDMFields.MagicPosition];

        /// <summary>
        /// The Format field
        /// </summary>
        public Byte Format => Header.Bytes[Header.Offset + DrdaDDMFields.FormatPosition];

        /// <summary>
        /// The CorrelId field
        /// </summary>
        public UInt16 CorrelId => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DrdaDDMFields.CorrelIdPosition);

        /// <summary>
        /// The Length2 field
        /// </summary>
        public UInt16 Length2 => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DrdaDDMFields.Length2Position);

        /// <summary>
        /// The Code Point field
        /// </summary>
        public DrdaCodepointType CodePoint => (DrdaCodepointType) EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DrdaDDMFields.CodePointPosition);

        private List<DrdaDDMParameter> _parameters;

        /// <summary>
        /// Decode Parameters field
        /// </summary>
        public List<DrdaDDMParameter> Parameters
        {
            get
            {
                if (_parameters == null) _parameters = new List<DrdaDDMParameter>();
                if (_parameters.Count > 0) return _parameters;


                var offset = Header.Offset + DrdaDDMFields.DDMHeadTotalLength;
                var ddmTotalLength = Length;
                while (offset < Header.Offset + ddmTotalLength)
                {
                    Int32 length = EndianBitConverter.Big.ToUInt16(Header.Bytes, offset);
                    if (length == 0)
                    {
                        length = Header.Offset + ddmTotalLength - offset;
                    }

                    if (offset + length <= Header.Offset + ddmTotalLength)
                    {
                        var parameter = new DrdaDDMParameter
                        {
                            Length = length,
                            DrdaCodepoint = (DrdaCodepointType) EndianBitConverter.Big.ToUInt16(Header.Bytes, offset + DrdaDDMFields.ParameterLengthLength)
                        };

                        var startIndex = offset + DrdaDDMFields.ParameterLengthLength + DrdaDDMFields.ParameterCodePointLength;
                        var strLength = length - 4;
                        //For Type=Data or Type=QryDta,Decode bytes as utf-8 ascii string
                        if (parameter.DrdaCodepoint == DrdaCodepointType.DATA || parameter.DrdaCodepoint == DrdaCodepointType.QRYDTA)
                        {
                            startIndex++;
                            strLength -= 2;
                            parameter.Data = Encoding.UTF8.GetString(Header.Bytes, startIndex, strLength).Trim();
                        }
                        else
                        {
                            parameter.Data = StringConverter.EbcdicToAscii(Header.Bytes, startIndex, strLength).Trim();
                        }

                        _parameters.Add(parameter);
                    }

                    offset += length;
                }

                return _parameters;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">Payload Bytes</param>
        public DrdaDDMPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(bas);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">Payload Bytes</param>
        /// <param name="parentPacket">Parent Packet</param>
        public DrdaDDMPacket(ByteArraySegment bas, Packet parentPacket) : this(bas)
        {
            Log.DebugFormat("ParentPacket.GetType() {0}", parentPacket.GetType());

            ParentPacket = parentPacket;
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if (outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[DrdaDDMPacket: Length={2}, Magic=0x{3:x2}, Format=0x{4:x2}, CorrelId={5}, Length2={6}, CodePoint={7}]{1}",
                                    color,
                                    colorEscape,
                                    Length,
                                    Magic,
                                    Format,
                                    CorrelId,
                                    Length2,
                                    CodePoint);
                buffer.Append(" Paramters:{");
                foreach (var paramter in Parameters)
                {
                    buffer.AppendFormat("{0}[DrdaDDMParameter: Length={2}, CodePoint={3}, Data='{4}']{1}",
                                        color,
                                        colorEscape,
                                        paramter.Length,
                                        paramter.DrdaCodepoint,
                                        paramter.Data);
                }

                buffer.Append("}");
            }

            return buffer.ToString();
        }
    }
}