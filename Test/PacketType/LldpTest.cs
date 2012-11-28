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
 */
using System;
using System.Net;
using System.Net.NetworkInformation;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.LLDP;
using PacketDotNet.Utils;

namespace Test.PacketType
{
    [TestFixture]
    public class LldpTest
    {
        [Test]
        public void ConstructFromValues()
        {
            var expectedChassisIDType = ChassisSubTypes.NetworkAddress;
            var expectedChassisIDNetworkAddress = new NetworkAddress(new IPAddress(new byte[4] { 0x0A, 0x00, 0x01, 0x01 }));
            var expectedPortIDBytes = new byte[15] { 0x30, 0x30, 0x31, 0x42, 0x35, 0x34, 0x39, 0x34, 0x35, 0x41, 0x38, 0x42, 0x3a, 0x50, 0x32 };
            ushort expectedTimeToLive = 120;
            string expectedPortDescription = "Port Description";
            string expectedSystemName = "SystemName";
            string expectedSystemDescription = "System Description";
            ushort expectedSystemCapabilitiesCapability = 18;
            ushort expectedSystemCapabilitiesEnabled = 16;
            var managementAddressNetworkAddress = new NetworkAddress(new IPAddress(new byte[4] { 0x0A, 0x00, 0x01, 0x01 }));
            var managementAddressObjectIdentifier = "Object Identifier";
            uint managementAddressInterfaceNumber = 0x44060124;

            var expectedOrganizationUniqueIdentifier = new byte[3] { 0x24, 0x10, 0x12 };
            var expectedOrganizationSubType = 2;
            var expectedOrganizationSpecificBytes = new byte[4] { 0xBA, 0xAD, 0xF0, 0x0D };

            var valuesLLDPPacket = new LLDPPacket();
            Console.WriteLine("valuesLLDPPacket.ToString() {0}", valuesLLDPPacket.ToString());
            valuesLLDPPacket.TlvCollection.Add(new ChassisID(expectedChassisIDType, expectedChassisIDNetworkAddress));
            Console.WriteLine("valuesLLDPPacket.ToString() {0}", valuesLLDPPacket.ToString());
            //valuesLLDPPacket.TlvCollection.Add(new PortID(lldpPacket, PortSubTypes.MACAddress, new PhysicalAddress(new byte[6] { 0x00, 0x1C, 0x23, 0xAF, 0x08, 0xF3 })));
            valuesLLDPPacket.TlvCollection.Add(new PortID(PortSubTypes.LocallyAssigned, expectedPortIDBytes));
            valuesLLDPPacket.TlvCollection.Add(new TimeToLive(expectedTimeToLive));
            valuesLLDPPacket.TlvCollection.Add(new PortDescription(expectedPortDescription));
            valuesLLDPPacket.TlvCollection.Add(new SystemName(expectedSystemName));
            valuesLLDPPacket.TlvCollection.Add(new SystemDescription(expectedSystemDescription));
            valuesLLDPPacket.TlvCollection.Add(new SystemCapabilities(expectedSystemCapabilitiesCapability, expectedSystemCapabilitiesEnabled));
            valuesLLDPPacket.TlvCollection.Add(new ManagementAddress(managementAddressNetworkAddress,
                                                                     InterfaceNumbering.SystemPortNumber,
                                                                     managementAddressInterfaceNumber,
                                                                     managementAddressObjectIdentifier));
            valuesLLDPPacket.TlvCollection.Add(new OrganizationSpecific(expectedOrganizationUniqueIdentifier,
                                                                        expectedOrganizationSubType,
                                                                        expectedOrganizationSpecificBytes));
            valuesLLDPPacket.TlvCollection.Add(new EndOfLLDPDU());

            var lldpBytes = valuesLLDPPacket.Bytes;

            Console.WriteLine("valuesLLDPPacket.ToString() {0}", valuesLLDPPacket.ToString());

            // reparse these bytes back into a lldp packet
            var lldpPacket = new LLDPPacket(new ByteArraySegment(lldpBytes));

            Console.WriteLine("lldpPacket.ToString() {0}", lldpPacket.ToString());

            int expectedTlvCount = 10;
            Assert.AreEqual(expectedTlvCount, lldpPacket.TlvCollection.Count);

            int count = 1;
            foreach (TLV tlv in lldpPacket.TlvCollection)
            {
                Console.WriteLine("Type: " + tlv.GetType().ToString());
                switch(count)
                {
                    case 1:
                        Assert.AreEqual(typeof(ChassisID), tlv.GetType());
                        var chassisID = (ChassisID)tlv;
                        Assert.AreEqual(ChassisSubTypes.NetworkAddress, chassisID.SubType);
                        Assert.AreEqual(typeof(NetworkAddress), chassisID.SubTypeValue.GetType());
                    Console.WriteLine(expectedChassisIDNetworkAddress.ToString());
                    Console.WriteLine(chassisID.SubTypeValue.ToString());
                        Assert.AreEqual(expectedChassisIDNetworkAddress, chassisID.SubTypeValue);
                        break;
                    case 2:
                        Assert.AreEqual(typeof(PortID), tlv.GetType());
                        var portID = (PortID)tlv;
                        Assert.AreEqual(PortSubTypes.LocallyAssigned, portID.SubType);
                        Assert.AreEqual(expectedPortIDBytes, portID.SubTypeValue);
                        //Assert.AreEqual(PortSubTypes.MACAddress, portID.SubType);
                        //var macAddress = new PhysicalAddress(new byte[6] { 0x00, 0x1C, 0x23, 0xAF, 0x08, 0xF3 });
                        //Assert.AreEqual(macAddress, portID.SubTypeValue);
                        break;
                    case 3:
                        Assert.AreEqual(typeof(TimeToLive), tlv.GetType());
                        Assert.AreEqual(expectedTimeToLive, ((TimeToLive)tlv).Seconds);
                        break;
                    case 4:
                        Assert.AreEqual(typeof(PortDescription), tlv.GetType());
                        Assert.AreEqual(expectedPortDescription, ((PortDescription)tlv).Description);
                        break;
                    case 5:
                        Assert.AreEqual(typeof(SystemName), tlv.GetType());
                        Assert.AreEqual(expectedSystemName, ((SystemName)tlv).Name);
                        break;
                    case 6:
                        Assert.AreEqual(typeof(SystemDescription), tlv.GetType());
                        Assert.AreEqual(expectedSystemDescription, ((SystemDescription)tlv).Description);
                        break;
                    case 7:
                        Assert.AreEqual(typeof(SystemCapabilities), tlv.GetType());
                        var capabilities = (SystemCapabilities)tlv;
                        Assert.IsTrue(capabilities.IsCapable(CapabilityOptions.Repeater));
                        Assert.IsTrue(capabilities.IsEnabled(CapabilityOptions.Router));
                        Assert.IsFalse(capabilities.IsCapable(CapabilityOptions.Bridge));
                        Assert.IsFalse(capabilities.IsCapable(CapabilityOptions.DocsisCableDevice));
                        Assert.IsFalse(capabilities.IsCapable(CapabilityOptions.Other));
                        Assert.IsFalse(capabilities.IsCapable(CapabilityOptions.StationOnly));
                        Assert.IsFalse(capabilities.IsCapable(CapabilityOptions.Telephone));
                        Assert.IsFalse(capabilities.IsCapable(CapabilityOptions.WLanAP));
                        break;
                    case 8:
                        Assert.AreEqual(typeof(ManagementAddress), tlv.GetType());
                        var mgmtAdd = (ManagementAddress)tlv;
                        Assert.AreEqual(AddressFamily.IPv4, mgmtAdd.AddressSubType);
                        Assert.AreEqual(managementAddressNetworkAddress, mgmtAdd.MgmtAddress);
                        Assert.AreEqual(InterfaceNumbering.SystemPortNumber, mgmtAdd.InterfaceSubType);
                        Assert.AreEqual(managementAddressInterfaceNumber, mgmtAdd.InterfaceNumber);
                        int expectedObjIdLength = managementAddressObjectIdentifier.Length;
                        Assert.AreEqual(expectedObjIdLength, mgmtAdd.ObjIdLength);
                        Assert.AreEqual(managementAddressObjectIdentifier, mgmtAdd.ObjectIdentifier);
                        break;
                    case 9:
                        Assert.AreEqual(typeof(OrganizationSpecific), tlv.GetType());
                        var orgSpecifig = (OrganizationSpecific)tlv;
                        Assert.AreEqual(expectedOrganizationUniqueIdentifier, orgSpecifig.OrganizationUniqueID);
                        Assert.AreEqual(expectedOrganizationSubType, orgSpecifig.OrganizationDefinedSubType);
                        Assert.AreEqual(expectedOrganizationSpecificBytes, orgSpecifig.OrganizationDefinedInfoString);
                        break;
                    case 10:
                        Assert.AreEqual(typeof(EndOfLLDPDU), tlv.GetType());
                        break;
                    default:
                        throw new ArgumentException();
                }

                // increment the counter
                count++;
            }
        }

        [Test]
        public void LLDPParsing()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/lldp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var l = (LLDPPacket)p.Extract(typeof(LLDPPacket));

            int count = 1;
            Console.WriteLine(l.TlvCollection.Count.ToString());
            foreach (TLV tlv in l.TlvCollection)
            {
                Console.WriteLine("Type: " + tlv.GetType().ToString());
                switch(count)
                {
                    case 1:
                        Assert.AreEqual(typeof(ChassisID), tlv.GetType());
                        var chassisID = (ChassisID)tlv;
                        Assert.AreEqual(chassisID.SubType, ChassisSubTypes.NetworkAddress);
                        Assert.AreEqual(typeof(NetworkAddress), chassisID.SubTypeValue.GetType());
                        var testAddress = new NetworkAddress(new IPAddress(new byte[4] { 0xac, 0x10, 0x0a, 0x6d }));
                        Assert.AreEqual(testAddress, chassisID.SubTypeValue);
                        break;
                    case 2:
                        Assert.AreEqual(typeof(PortID), tlv.GetType());
                        var portID = (PortID)tlv;
                        Assert.AreEqual(PortSubTypes.LocallyAssigned, portID.SubType);
                        byte[] subTypeValue = new byte[15] { 0x30, 0x30, 0x31, 0x42, 0x35, 0x34, 0x39, 0x34, 0x35, 0x41, 0x38, 0x42, 0x3a, 0x50, 0x32 };
                        Assert.AreEqual(subTypeValue, portID.SubTypeValue);
                        break;
                    case 3:
                        Assert.AreEqual(typeof(TimeToLive), tlv.GetType());
                        Assert.AreEqual(180, ((TimeToLive)tlv).Seconds);
                        break;
                    case 4:
                        Assert.AreEqual(typeof(PortDescription), tlv.GetType());
                        Assert.AreEqual("PC Port", ((PortDescription)tlv).Description);
                        break;
                    case 5:
                        Assert.AreEqual(typeof(SystemName), tlv.GetType());
                        Assert.AreEqual("SEP001B54945A8B.elmec.ad", ((SystemName)tlv).Name);
                        break;
                    case 6:
                        Assert.AreEqual(typeof(SystemDescription), tlv.GetType());
                        Assert.AreEqual("Cisco IP Phone CP-7911G,V2, SCCP11.8-4-3S", ((SystemDescription)tlv).Description);
                        break;
                    case 7:
                        Assert.AreEqual(typeof(SystemCapabilities), tlv.GetType());
                        var systemCapabilities = (SystemCapabilities)tlv;
                        Assert.AreEqual(36, systemCapabilities.Capabilities);
                        Assert.AreEqual(36, systemCapabilities.Enabled);
                        Assert.IsTrue(systemCapabilities.IsCapable(CapabilityOptions.Bridge));
                        Assert.IsTrue(systemCapabilities.IsEnabled(CapabilityOptions.Telephone));
                        Assert.IsFalse(systemCapabilities.IsCapable(CapabilityOptions.Repeater));
                        Assert.IsFalse(systemCapabilities.IsEnabled(CapabilityOptions.WLanAP));
                        break;
                    case 8:
                        Assert.AreEqual(typeof(ManagementAddress), tlv.GetType());
                        var managementAddress = (ManagementAddress)tlv;
                        Assert.AreEqual(5, managementAddress.AddressLength);
                        Assert.AreEqual(1, (int)managementAddress.AddressSubType);
                        var mgmtAddress = new NetworkAddress(new IPAddress(new byte[4] { 0xac, 0x10, 0x0a, 0x6d }));
                        Assert.AreEqual(mgmtAddress, managementAddress.MgmtAddress);
                        Assert.AreEqual(1, (int)managementAddress.InterfaceSubType);
                        Assert.AreEqual(0, managementAddress.InterfaceNumber);
                        Assert.AreEqual(0, managementAddress.ObjIdLength);
                        break;
                    case 9:
                        Assert.AreEqual(typeof(OrganizationSpecific), tlv.GetType());
                        var organizationSpecific = (OrganizationSpecific)tlv;
                        Assert.AreEqual(new byte[3] { 0x00, 0x12, 0x0f }, organizationSpecific.OrganizationUniqueID);
                        Assert.AreEqual(1, organizationSpecific.OrganizationDefinedSubType);
                        byte[] infoString = new byte[5] { 0x03, 0x00, 0x36, 0x00, 0x10 };
                        Assert.AreEqual(infoString, organizationSpecific.OrganizationDefinedInfoString);
                        break;
                    case 10:
                        Assert.AreEqual(typeof(EndOfLLDPDU), tlv.GetType());
                        break;
                    default:
                        throw new ArgumentException();
                }
                // increment the counter
                count++;
            }
        }

        [Test]
        public void RandomPacket()
        {
            LLDPPacket.RandomPacket();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/lldp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var l = (LLDPPacket)p.Extract(typeof(LLDPPacket));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(l.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/lldp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var l = (LLDPPacket)p.Extract(typeof(LLDPPacket));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(l.ToString(StringOutputType.Verbose));
        }
    }
}