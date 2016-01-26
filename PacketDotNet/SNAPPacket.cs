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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 *  Copyright 2016 Joseph D. Beshay <josephdbeshay@gmail.com>
 *  
 */
﻿using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// See https://en.wikipedia.org/wiki/Subnetwork_Access_Protocol
    /// </summary>
    [Serializable]
    public class SNAPPacket : InternetLinkLayerPacket
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

        /// <value>
        /// Destination Service Access Point
        /// </value>
        public virtual byte DSAP 
        { 
            get 
            {
                return (byte)EndianBitConverter.Big.ToByte(header.Bytes, header.Offset + SNAPFields.DSAPPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes((char) value, header.Bytes, header.Offset + SNAPFields.DSAPPosition);
            }
        }

        /// <value>
        /// Source Service Access Point
        /// </value>
        public virtual byte SSAP 
        { 
            get 
            {
                return (byte) EndianBitConverter.Big.ToByte(header.Bytes, header.Offset + SNAPFields.SSAPPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes((char) value, header.Bytes, header.Offset + SNAPFields.SSAPPosition);
            }
        }

        /// <value>
        /// Control Field for this SNAP packet.
        /// </value>
        public virtual byte ControlField
        { 
            get 
            {
                return (byte)EndianBitConverter.Big.ToByte(header.Bytes, header.Offset + SNAPFields.ControlPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes((char) value, header.Bytes, header.Offset + SNAPFields.ControlPosition);
            }
        }

        /// <value>
        /// Oragnization Code of packet that this SNAP packet encapsulates
        /// </value>
        public virtual int OrgCode
        { 
            get 
            {
                byte[] temp = new byte[4];
                Array.Copy(header.Bytes, header.Offset + SNAPFields.OrgCodePosition, temp, 1, SNAPFields.OrgCodeLength);
                return EndianBitConverter.Big.ToInt32(temp, 0);
            }
            set
            {
                byte[] temp = new byte[4];
                EndianBitConverter.Big.CopyBytes(value, temp, 0);
                Array.Copy(temp, 1, header.Bytes, header.Offset + SNAPFields.OrgCodePosition, SNAPFields.OrgCodeLength);
            }
        }

        /// <value>
        /// Type of packet that this SNAP packet encapsulates
        /// </value>
        public virtual EthernetPacketType EtherType
        {
            get
            {
                return (EthernetPacketType)EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                          header.Offset + SNAPFields.EtherTypePosition);
            }

            set
            {
                Int16 val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + SNAPFields.EtherTypePosition);
            }
        }


        /// <value>
        /// Payload packet, overridden to set the 'EtherType' field based on
        /// the type of packet being used here if the PayloadPacket is being set
        /// </value>
        public override Packet PayloadPacket
        {
            get
            {
                return base.PayloadPacket;
            }

            set
            {
                base.PayloadPacket = value;

                // set Type based on the type of the payload
                if(value is IPv4Packet)
                {
                    EtherType = EthernetPacketType.IpV4;
                } else if(value is IPv6Packet)
                {
                    EtherType = EthernetPacketType.IpV6;
                } else if(value is ARPPacket)
                {
                    EtherType = EthernetPacketType.Arp;
                }
                else if(value is LLDPPacket)
                {
                    EtherType = EthernetPacketType.LLDP;
                }
                else if(value is PPPoEPacket)
                {
                    EtherType = EthernetPacketType.PointToPointProtocolOverEthernetSessionStage;
                }
                else // NOTE: new types should be inserted here
                {
                    EtherType = EthernetPacketType.None;
                }
            }
        }

        

        /// <summary>
        /// Construct a new SNAP packet holding a logical ethernet packet.
        /// </summary>
        public SNAPPacket(byte DSAP,
                              byte SSAP,
                              byte Control, 
                              int OrgCode,
                              EthernetPacketType ethernetType)
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = SNAPFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            // set the instance values
            this.DSAP = DSAP;
            this.SSAP = SSAP;
            this.ControlField = Control;
            this.OrgCode = OrgCode;
            this.EtherType = ethernetType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public SNAPPacket(ByteArraySegment bas)
        {
            log.Debug("");

            // slice off the header portion
            header = new ByteArraySegment(bas);
            header.Length = SNAPFields.HeaderLength;

            // parse the encapsulated bytes
            payloadPacketOrData = ParseEncapsulatedBytes(header, EtherType);
        }

        /// <summary>
        /// Used by the SNAPPacket constructor. Located here because the LinuxSLL constructor
        /// also needs to perform the same operations as it contains an ethernet type
        /// </summary>
        /// <param name="Header">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        /// <param name="Type">
        /// A <see cref="EthernetPacketType"/>
        /// </param>
        /// <returns>
        /// A <see cref="PacketOrByteArraySegment"/>
        /// </returns>
        internal static PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment Header,
                                                                        EthernetPacketType Type)
        {
            // slice off the payload
            var payload = Header.EncapsulatedBytes();
            log.DebugFormat("payload {0}", payload.ToString());

            var payloadPacketOrData = new PacketOrByteArraySegment();

            // parse the encapsulated bytes
            switch(Type)
            {
            case EthernetPacketType.IpV4:
                payloadPacketOrData.ThePacket = new IPv4Packet(payload);
                break;
            case EthernetPacketType.IpV6:
                payloadPacketOrData.ThePacket = new IPv6Packet(payload);
                break;
            case EthernetPacketType.Arp:
                payloadPacketOrData.ThePacket = new ARPPacket(payload);
                break;
            case EthernetPacketType.LLDP:
                payloadPacketOrData.ThePacket = new LLDPPacket(payload);
                break;
            case EthernetPacketType.PointToPointProtocolOverEthernetSessionStage:
                payloadPacketOrData.ThePacket = new PPPoEPacket(payload);
                break;
            case EthernetPacketType.WakeOnLan:
                payloadPacketOrData.ThePacket = new WakeOnLanPacket(payload);
                break;
            case EthernetPacketType.VLanTaggedFrame:
                payloadPacketOrData.ThePacket = new Ieee8021QPacket(payload);
                break;
            default: // consider the sub-packet to be a byte array
                payloadPacketOrData.TheByteArraySegment = payload;
                break;
            }

            return payloadPacketOrData;
        }

        /// <summary>
        /// Returns the SNAPPacket inside of the Packet p or null if
        /// there is no encapsulated packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="SNAPPacket"/>
        /// </returns>
        [Obsolete("Use Packet.Extract() instead")]
        public static SNAPPacket GetEncapsulated(Packet p)
        {
            log.Debug("");

            if (p is SNAPPacket)
            {
                return (SNAPPacket)p;
            }

            return null;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override System.String Color
        {
            get
            {
                return AnsiEscapeSequences.DarkGray;
            }
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[LLC/SNAP Packet: DSAP={2}, SSAP={3}, Control={4}, OrgCode={5}, EtherType={6}]{1}",
                    color,
                    colorEscape,
                    DSAP,
                    SSAP,
                    ControlField,
                    OrgCode,
                    EtherType.ToString());
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("DSAP", DSAP.ToString());
                properties.Add("SSAP", SSAP.ToString());
                properties.Add("control", ControlField.ToString());
                properties.Add("OrganizationCode", OrgCode.ToString());
                properties.Add("EtherType", EtherType.ToString());

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("LLC/SNAP:  ******* LLC/SNAP - \"LLC/SNAP\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("SNAP:");
                foreach(var property in properties)
                {
                    buffer.AppendLine("SNAP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }
                buffer.AppendLine("SNAP:");
            }

            // append the base output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Generate a random SNAPPacket
        /// TODO: could improve this routine to set a random payload as well
        /// </summary>
        /// <returns>
        /// A <see cref="SNAPPacket"/>
        /// </returns>
        public static SNAPPacket RandomPacket()
        {
            var rnd = new Random();

            return new SNAPPacket((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256), rnd.Next(65535), EthernetPacketType.IpV4);
        }
    }
}
