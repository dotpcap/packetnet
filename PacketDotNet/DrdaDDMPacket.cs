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

using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// DrdaPacket
    /// See: https://en.wikipedia.org/wiki/DRDA
    /// </summary>
    public sealed class DrdaDdmPacket : Packet
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

        private List<DrdaDdmParameter> _parameters;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">Payload Bytes</param>
        public DrdaDdmPacket(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(byteArraySegment);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">Payload Bytes</param>
        /// <param name="parentPacket">Parent Packet</param>
        public DrdaDdmPacket(ByteArraySegment byteArraySegment, Packet parentPacket) : this(byteArraySegment)
        {
            Log.DebugFormat("ParentPacket.GetType() {0}", parentPacket.GetType());

            ParentPacket = parentPacket;
        }

        /// <summary>
        /// The Code Point field
        /// </summary>
        public DrdaCodePointType CodePoint => (DrdaCodePointType) EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DrdaDdmFields.CodePointPosition);

        /// <summary>
        /// The CorrelId field
        /// </summary>
        public ushort CorrelId => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DrdaDdmFields.CorrelIdPosition);

        /// <summary>
        /// The Format field
        /// </summary>
        public byte Format => Header.Bytes[Header.Offset + DrdaDdmFields.FormatPosition];

        /// <summary>
        /// The Length field
        /// </summary>
        public ushort Length => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DrdaDdmFields.LengthPosition);

        /// <summary>
        /// The Length2 field
        /// </summary>
        public ushort Length2 => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DrdaDdmFields.Length2Position);

        /// <summary>
        /// The Magic field
        /// </summary>
        public byte Magic => Header.Bytes[Header.Offset + DrdaDdmFields.MagicPosition];

        /// <summary>
        /// The decoded parameters field.
        /// </summary>
        public List<DrdaDdmParameter> Parameters
        {
            get
            {
                if (_parameters == null) _parameters = new List<DrdaDdmParameter>();
                if (_parameters.Count > 0) return _parameters;


                var offset = Header.Offset + DrdaDdmFields.DDMHeadTotalLength;
                var ddmTotalLength = Length;
                while (offset < Header.Offset + ddmTotalLength)
                {
                    int length = EndianBitConverter.Big.ToUInt16(Header.Bytes, offset);
                    if (length == 0)
                    {
                        length = Header.Offset + ddmTotalLength - offset;
                    }

                    if (offset + length <= Header.Offset + ddmTotalLength)
                    {
                        var parameter = new DrdaDdmParameter
                        {
                            Length = length,
                            DrdaCodepoint = (DrdaCodePointType) EndianBitConverter.Big.ToUInt16(Header.Bytes, offset + DrdaDdmFields.ParameterLengthLength)
                        };

                        var startIndex = offset + DrdaDdmFields.ParameterLengthLength + DrdaDdmFields.ParameterCodePointLength;
                        var strLength = length - 4;
                        //For Type=Data or Type=QryDta,Decode bytes as utf-8 ascii string
                        if (parameter.DrdaCodepoint == DrdaCodePointType.Data || parameter.DrdaCodepoint == DrdaCodePointType.QueryAnswerSetData)
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

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
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
                buffer.AppendFormat("{0}[DrdaDdmPacket: Length={2}, Magic=0x{3:x2}, Format=0x{4:x2}, CorrelId={5}, Length2={6}, CodePoint={7}]{1}",
                                    color,
                                    colorEscape,
                                    Length,
                                    Magic,
                                    Format,
                                    CorrelId,
                                    Length2,
                                    CodePoint);

                buffer.Append(" Parameters:{");
                foreach (var parameter in Parameters)
                {
                    buffer.AppendFormat("{0}[DrdaDdmParameter: Length={2}, CodePoint={3}, Data='{4}']{1}",
                                        color,
                                        colorEscape,
                                        parameter.Length,
                                        parameter.DrdaCodepoint,
                                        parameter.Data);
                }

                buffer.Append("}");
            }

            return buffer.ToString();
        }
    }
}