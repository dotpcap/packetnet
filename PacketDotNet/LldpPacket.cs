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
using System.Collections;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using PacketDotNet.Lldp;
using PacketDotNet.Utils;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// A LLDP packet.
    /// As specified in IEEE Std 802.1AB
    /// </summary>
    /// <remarks>
    /// See http://en.wikipedia.org/wiki/Link_Layer_Discovery_Protocol for general info
    /// See IETF 802.1AB for the full specification
    /// </remarks>
    public class LldpPacket : InternetLinkLayerPacket, IEnumerable
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
        /// Contains the TLV's in the LLDPDU
        /// </summary>
        public TlvCollection TlvCollection = new TlvCollection();

        /// <summary>
        /// Create an empty LldpPacket
        /// </summary>
        public LldpPacket()
        {
            Log.Debug("");

            // all lldp packets end with an EndOfLldpdu TLV so add one
            // by default
            TlvCollection.Add(new EndOfLldpduTlv());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public LldpPacket(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            Header = new ByteArraySegment(byteArraySegment);

            // Initiate the TLV list from the existing data
            ParseByteArrayIntoTlvs(Header.Bytes, Header.Offset);
        }

        /// <summary>
        /// LldpPacket specific implementation of BytesSegment
        /// Necessary because each TLV in the collection may have a
        /// byte[] that is not shared by other TLVs
        /// NOTE: There is potential for the same performance improvement that
        /// the Packet class uses where we check to see if each TLVs uses the
        /// same byte[] and that there are no gaps.
        /// </summary>
        public override ByteArraySegment BytesSegment
        {
            get
            {
                var memoryStream = new MemoryStream();
                foreach (var tlv in TlvCollection)
                {
                    var tlvBytes = tlv.Bytes;
                    memoryStream.Write(tlvBytes, 0, tlvBytes.Length);
                }

                var msArray = memoryStream.ToArray();
                return new ByteArraySegment(msArray, 0, msArray.Length);
            }
        }

        /// <summary>
        /// Allows access of the TlvCollection by index
        /// </summary>
        /// <param name="index">The index of the item being set/retrieved in the collection</param>
        /// <returns>The requested Tlv</returns>
        public Tlv this[int index]
        {
            get => TlvCollection[index];
            set => TlvCollection[index] = value;
        }

        /// <value>
        /// The current length of the LLDPDU
        /// </value>
        public int Length { get; set; }

        /// <summary>
        /// Enables foreach functionality for this class
        /// </summary>
        /// <returns>The next item in the list</returns>
        public IEnumerator GetEnumerator()
        {
            return TlvCollection.GetEnumerator();
        }

        /// <summary>
        /// Parse byte[] into TLVs
        /// </summary>
        public void ParseByteArrayIntoTlvs(byte[] bytes, int offset)
        {
            Log.DebugFormat("bytes.Length {0}, offset {1}", bytes.Length, offset);

            var position = 0;

            TlvCollection.Clear();

            while (offset + position < bytes.Length)
            {
                // The payload type
                var byteArraySegment = new ByteArraySegment(bytes, offset + position, TlvTypeLength.TypeLengthLength);
                var typeLength = new TlvTypeLength(byteArraySegment);

                // create a TLV based on the type and add it to the collection
                var currentTlv = GetTlv(bytes, offset + position, typeLength.Type);
                if (currentTlv == null)
                {
                    Log.Debug("currentTlv == null");
                    break;
                }

                Log.DebugFormat("Adding TLV {0}, Type {1}",
                                currentTlv.GetType(),
                                currentTlv.Type);

                TlvCollection.Add(currentTlv);

                // stop at the first end TLV we run into
                if (currentTlv is EndOfLldpduTlv)
                    break;


                // Increment the position to seek the next Tlv
                position += currentTlv.TotalLength;
            }

            Log.DebugFormat("Done, position {0}", position);
        }

        /// <summary>
        /// Gets the TLV for the specified <see cref="type" />
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="type">
        /// A <see cref="TlvType" />
        /// </param>
        /// <returns>
        /// A <see cref="Tlv" />
        /// </returns>
        private static Tlv GetTlv(byte[] bytes, int offset, TlvType type)
        {
            switch (type)
            {
                case TlvType.ChassisId:
                {
                    return new ChassisIdTlv(bytes, offset);
                }
                case TlvType.PortId:
                {
                    return new PortIdTlv(bytes, offset);
                }
                case TlvType.TimeToLive:
                {
                    return new TimeToLiveTlv(bytes, offset);
                }
                case TlvType.PortDescription:
                {
                    return new PortDescriptionTlv(bytes, offset);
                }
                case TlvType.SystemName:
                {
                    return new SystemNameTlv(bytes, offset);
                }
                case TlvType.SystemDescription:
                {
                    return new SystemDescriptionTlv(bytes, offset);
                }
                case TlvType.SystemCapabilities:
                {
                    return new SystemCapabilitiesTlv(bytes, offset);
                }
                case TlvType.ManagementAddress:
                {
                    return new ManagementAddressTlv(bytes, offset);
                }
                case TlvType.OrganizationSpecific:
                {
                    return new OrganizationSpecificTlv(bytes, offset);
                }
                case TlvType.EndOfLldpu:
                {
                    return new EndOfLldpduTlv(bytes, offset);
                }
                default:
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Create a randomized LLDP packet with some basic TLVs
        /// </summary>
        /// <returns>
        /// A <see cref="Packet" />
        /// </returns>
        public static LldpPacket RandomPacket()
        {
            var rnd = new Random();

            var lldpPacket = new LldpPacket();

            var physicalAddressBytes = new byte[EthernetFields.MacAddressLength];
            rnd.NextBytes(physicalAddressBytes);
            var physicalAddress = new PhysicalAddress(physicalAddressBytes);
            lldpPacket.TlvCollection.Add(new ChassisIdTlv(physicalAddress));

            var networkAddress = new byte[IPv4Fields.AddressLength];
            rnd.NextBytes(networkAddress);
            lldpPacket.TlvCollection.Add(new PortIdTlv(new NetworkAddress(new IPAddress(networkAddress))));

            var seconds = (ushort) rnd.Next(0, 120);
            lldpPacket.TlvCollection.Add(new TimeToLiveTlv(seconds));

            lldpPacket.TlvCollection.Add(new EndOfLldpduTlv());

            return lldpPacket;
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

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                {
                    // build the string of TLVs
                    var tlvs = "{";
                    var r = new Regex(@"[^(\.)]([^\.]*)$");
                    foreach (var tlv in TlvCollection)
                    {
                        // regex trim the parent namespaces from the class type
                        //   (ex. "PacketDotNet.LLDP.TimeToLive" becomes "TimeToLive")
                        var m = r.Match(tlv.GetType().ToString());
                        tlvs += m.Groups[0].Value + "|";
                    }

                    tlvs = tlvs.TrimEnd('|');
                    tlvs += "}";

                    // build the output string
                    buffer.AppendFormat("{0}[LldpPacket: TLVs={2}]{1}",
                                        color,
                                        colorEscape,
                                        tlvs);

                    break;
                }
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                {
                    // build the output string
                    buffer.AppendLine("LLDP:  ******* LLDP - \"Link Layer Discovery Protocol\" - offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("LLDP:");
                    foreach (var tlv in TlvCollection)
                    {
                        buffer.AppendLine("LLDP:" + tlv);
                    }

                    buffer.AppendLine("LLDP:");
                    break;
                }
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}