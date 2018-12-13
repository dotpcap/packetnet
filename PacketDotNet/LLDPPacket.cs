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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using PacketDotNet.LLDP;
using PacketDotNet.Utils;

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
    [Serializable]
    public class LLDPPacket : InternetLinkLayerPacket, IEnumerable
    {
        #region Preprocessor Directives

#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        #endregion


        #region Constructors

        /// <summary>
        /// Create an empty LLDPPacket
        /// </summary>
        public LLDPPacket()
        {
            Log.Debug("");

            // all lldp packets end with an EndOfLLDPDU tlv so add one
            // by default
            TlvCollection.Add(new EndOfLLDPDU());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public LLDPPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            Header = new ByteArraySegment(bas);

            // Initiate the TLV list from the existing data
            ParseByteArrayIntoTlvs(Header.Bytes, Header.Offset);
        }

        #endregion


        #region Properties

        /// <value>
        /// The current length of the LLDPDU
        /// </value>
        public Int32 Length { get; set; }

        /// <summary>
        /// LLDPPacket specific implementation of BytesHighPerformance
        /// Necessary because each TLV in the collection may have a
        /// byte[] that is not shared by other TLVs
        /// NOTE: There is potential for the same performance improvement that
        /// the Packet class uses where we check to see if each TLVs uses the
        /// same byte[] and that there are no gaps.
        /// </summary>
        public override ByteArraySegment BytesHighPerformance
        {
            get
            {
                var ms = new MemoryStream();
                foreach (var tlv in TlvCollection)
                {
                    var tlvBytes = tlv.Bytes;
                    ms.Write(tlvBytes, 0, tlvBytes.Length);
                }

                var offset = 0;
                var msArray = ms.ToArray();
                return new ByteArraySegment(msArray, offset, msArray.Length);
            }
        }

        /// <summary>
        /// Allows access of the TlvCollection by index
        /// </summary>
        /// <param name="index">The index of the item being set/retrieved in the collection</param>
        /// <returns>The requested TLV</returns>
        public TLV this[Int32 index]
        {
            get => TlvCollection[index];
            set => TlvCollection[index] = value;
        }

        /// <summary>
        /// Enables foreach functionality for this class
        /// </summary>
        /// <returns>The next item in the list</returns>
        public IEnumerator GetEnumerator()
        {
            return TlvCollection.GetEnumerator();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Parse byte[] into TLVs
        /// </summary>
        public void ParseByteArrayIntoTlvs(Byte[] bytes, Int32 offset)
        {
            Log.DebugFormat("bytes.Length {0}, offset {1}", bytes.Length, offset);

            var position = 0;

            TlvCollection.Clear();

            while (position < bytes.Length)
            {
                // The payload type
                var byteArraySegment = new ByteArraySegment(bytes, offset + position, TLVTypeLength.TypeLengthLength);
                var typeLength = new TLVTypeLength(byteArraySegment);

                // create a TLV based on the type and
                // add it to the collection
                var currentTlv = TLVFactory(bytes, offset + position, typeLength.Type);
                if (currentTlv == null)
                {
                    Log.Debug("currentTlv == null");
                    break;
                }

                Log.DebugFormat("Adding tlv {0}, Type {1}",
                                currentTlv.GetType(),
                                currentTlv.Type);
                TlvCollection.Add(currentTlv);

                // stop at the first end tlv we run into
                if (currentTlv is EndOfLLDPDU)
                {
                    break;
                }

                // Increment the position to seek the next TLV
                position += currentTlv.TotalLength;
            }

            Log.DebugFormat("Done, position {0}", position);
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="type">
        /// A <see cref="TLVTypes" />
        /// </param>
        /// <returns>
        /// A <see cref="TLV" />
        /// </returns>
        private static TLV TLVFactory(Byte[] bytes, Int32 offset, TLVTypes type)
        {
            switch (type)
            {
                case TLVTypes.ChassisID:
                    return new ChassisID(bytes, offset);
                case TLVTypes.PortID:
                    return new PortID(bytes, offset);
                case TLVTypes.TimeToLive:
                    return new TimeToLive(bytes, offset);
                case TLVTypes.PortDescription:
                    return new PortDescription(bytes, offset);
                case TLVTypes.SystemName:
                    return new SystemName(bytes, offset);
                case TLVTypes.SystemDescription:
                    return new SystemDescription(bytes, offset);
                case TLVTypes.SystemCapabilities:
                    return new SystemCapabilities(bytes, offset);
                case TLVTypes.ManagementAddress:
                    return new ManagementAddress(bytes, offset);
                case TLVTypes.OrganizationSpecific:
                    return new OrganizationSpecific(bytes, offset);
                case TLVTypes.EndOfLLDPU:
                    return new EndOfLLDPDU(bytes, offset);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Create a randomized LLDP packet with some basic TLVs
        /// </summary>
        /// <returns>
        /// A <see cref="Packet" />
        /// </returns>
        public static LLDPPacket RandomPacket()
        {
            var rnd = new Random();

            var lldpPacket = new LLDPPacket();

            var physicalAddressBytes = new Byte[EthernetFields.MacAddressLength];
            rnd.NextBytes(physicalAddressBytes);
            var physicalAddress = new PhysicalAddress(physicalAddressBytes);
            lldpPacket.TlvCollection.Add(new ChassisID(physicalAddress));

            var networkAddress = new Byte[IPv4Fields.AddressLength];
            rnd.NextBytes(networkAddress);
            lldpPacket.TlvCollection.Add(new PortID(new NetworkAddress(new IPAddress(networkAddress))));

            var seconds = (UInt16) rnd.Next(0, 120);
            lldpPacket.TlvCollection.Add(new TimeToLive(seconds));

            lldpPacket.TlvCollection.Add(new EndOfLLDPDU());

            return lldpPacket;
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
                // build the string of tlvs
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
                buffer.AppendFormat("{0}[LLDPPacket: TLVs={2}]{1}",
                                    color,
                                    colorEscape,
                                    tlvs);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // build the output string
                buffer.AppendLine("LLDP:  ******* LLDP - \"Link Layer Discovery Protocol\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("LLDP:");
                foreach (var tlv in TlvCollection)
                {
                    buffer.AppendLine("LLDP:" + tlv);
                }

                buffer.AppendLine("LLDP:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        #endregion


        #region Members

        /// <summary>
        /// Contains the TLV's in the LLDPDU
        /// </summary>
        public TLVCollection TlvCollection = new TLVCollection();

        #endregion
    }
}