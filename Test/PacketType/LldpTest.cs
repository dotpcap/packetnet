/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using PacketDotNet.Lldp;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

    [TestFixture]
    public class LldpTest
    {
        [Test]
        public void ConstructFromValues()
        {
            var expectedChassisIDType = ChassisSubType.NetworkAddress;
            var expectedChassisIDNetworkAddress = new NetworkAddress(new IPAddress(new byte[] { 0x0A, 0x00, 0x01, 0x01 }));
            var expectedPortIDBytes = new byte[] { 0x30, 0x30, 0x31, 0x42, 0x35, 0x34, 0x39, 0x34, 0x35, 0x41, 0x38, 0x42, 0x3a, 0x50, 0x32 };
            ushort expectedTimeToLive = 120;
            var expectedPortDescription = "Port Description";
            var expectedSystemName = "SystemName";
            var expectedSystemDescription = "System Description";
            ushort expectedSystemCapabilitiesCapability = 18;
            ushort expectedSystemCapabilitiesEnabled = 16;
            var managementAddressNetworkAddress = new NetworkAddress(new IPAddress(new byte[] { 0x0A, 0x00, 0x01, 0x01 }));
            var managementAddressObjectIdentifier = "Object Identifier";
            uint managementAddressInterfaceNumber = 0x44060124;

            var expectedOrganizationUniqueIdentifier = new byte[] { 0x24, 0x10, 0x12 };
            var expectedOrganizationSubType = 2;
            var expectedOrganizationSpecificBytes = new byte[] { 0xBA, 0xAD, 0xF0, 0x0D };

            var valuesLLDPPacket = new LldpPacket();
            Console.WriteLine("valuesLLDPPacket.ToString() {0}", valuesLLDPPacket);
            valuesLLDPPacket.TlvCollection.Add(new ChassisIdTlv(expectedChassisIDType, expectedChassisIDNetworkAddress));
            Console.WriteLine("valuesLLDPPacket.ToString() {0}", valuesLLDPPacket);
            //valuesLLDPPacket.TlvCollection.Add(new PortId(lldpPacket, PortSubTypes.MacAddress, new PhysicalAddress(new byte[6] { 0x00, 0x1C, 0x23, 0xAF, 0x08, 0xF3 })));
            valuesLLDPPacket.TlvCollection.Add(new PortIdTlv(PortSubType.LocallyAssigned, expectedPortIDBytes));
            valuesLLDPPacket.TlvCollection.Add(new TimeToLiveTlv(expectedTimeToLive));
            valuesLLDPPacket.TlvCollection.Add(new PortDescriptionTlv(expectedPortDescription));
            valuesLLDPPacket.TlvCollection.Add(new SystemNameTlv(expectedSystemName));
            valuesLLDPPacket.TlvCollection.Add(new SystemDescriptionTlv(expectedSystemDescription));
            valuesLLDPPacket.TlvCollection.Add(new SystemCapabilitiesTlv(expectedSystemCapabilitiesCapability, expectedSystemCapabilitiesEnabled));
            valuesLLDPPacket.TlvCollection.Add(new ManagementAddressTlv(managementAddressNetworkAddress,
                                                                     InterfaceNumber.SystemPortNumber,
                                                                     managementAddressInterfaceNumber,
                                                                     managementAddressObjectIdentifier));

            valuesLLDPPacket.TlvCollection.Add(new OrganizationSpecificTlv(expectedOrganizationUniqueIdentifier,
                                                                        expectedOrganizationSubType,
                                                                        expectedOrganizationSpecificBytes));

            valuesLLDPPacket.TlvCollection.Add(new EndOfLldpduTlv());

            var lldpBytes = valuesLLDPPacket.Bytes;

            Console.WriteLine("valuesLLDPPacket.ToString() {0}", valuesLLDPPacket);

            // reparse these bytes back into a lldp packet
            var lldpPacket = new LldpPacket(new ByteArraySegment(lldpBytes));

            Console.WriteLine("lldpPacket.ToString() {0}", lldpPacket);

            var expectedTlvCount = 10;
            ClassicAssert.AreEqual(expectedTlvCount, lldpPacket.TlvCollection.Count);

            var count = 1;
            foreach (var tlv in lldpPacket.TlvCollection)
            {
                Console.WriteLine("Type: " + tlv.GetType());
                switch (count)
                {
                    case 1:
                    {
                        ClassicAssert.AreEqual(typeof(ChassisIdTlv), tlv.GetType());
                        var chassisID = (ChassisIdTlv) tlv;
                        ClassicAssert.AreEqual(ChassisSubType.NetworkAddress, chassisID.SubType);
                        ClassicAssert.AreEqual(typeof(NetworkAddress), chassisID.SubTypeValue.GetType());
                        Console.WriteLine(expectedChassisIDNetworkAddress.ToString());
                        Console.WriteLine(chassisID.SubTypeValue.ToString());
                        ClassicAssert.AreEqual(expectedChassisIDNetworkAddress, chassisID.SubTypeValue);
                        break;
                    }
                    case 2:
                    {
                        ClassicAssert.AreEqual(typeof(PortIdTlv), tlv.GetType());
                        var portID = (PortIdTlv) tlv;
                        ClassicAssert.AreEqual(PortSubType.LocallyAssigned, portID.SubType);
                        ClassicAssert.AreEqual(expectedPortIDBytes, portID.SubTypeValue);
                        //ClassicAssert.AreEqual(PortSubTypes.MacAddress, portID.SubType);
                        //var macAddress = new PhysicalAddress(new byte[6] { 0x00, 0x1C, 0x23, 0xAF, 0x08, 0xF3 });
                        //ClassicAssert.AreEqual(macAddress, portID.SubTypeValue);
                        break;
                    }
                    case 3:
                    {
                        ClassicAssert.AreEqual(typeof(TimeToLiveTlv), tlv.GetType());
                        ClassicAssert.AreEqual(expectedTimeToLive, ((TimeToLiveTlv) tlv).Seconds);
                        break;
                    }
                    case 4:
                    {
                        ClassicAssert.AreEqual(typeof(PortDescriptionTlv), tlv.GetType());
                        ClassicAssert.AreEqual(expectedPortDescription, ((PortDescriptionTlv) tlv).Description);
                        break;
                    }
                    case 5:
                    {
                        ClassicAssert.AreEqual(typeof(SystemNameTlv), tlv.GetType());
                        ClassicAssert.AreEqual(expectedSystemName, ((SystemNameTlv) tlv).Name);
                        break;
                    }
                    case 6:
                    {
                        ClassicAssert.AreEqual(typeof(SystemDescriptionTlv), tlv.GetType());
                        ClassicAssert.AreEqual(expectedSystemDescription, ((SystemDescriptionTlv) tlv).Description);
                        break;
                    }
                    case 7:
                    {
                        ClassicAssert.AreEqual(typeof(SystemCapabilitiesTlv), tlv.GetType());
                        var capabilities = (SystemCapabilitiesTlv) tlv;
                        ClassicAssert.IsTrue(capabilities.IsCapable(CapabilityOptions.Repeater));
                        ClassicAssert.IsTrue(capabilities.IsEnabled(CapabilityOptions.Router));
                        ClassicAssert.IsFalse(capabilities.IsCapable(CapabilityOptions.Bridge));
                        ClassicAssert.IsFalse(capabilities.IsCapable(CapabilityOptions.DocsisCableDevice));
                        ClassicAssert.IsFalse(capabilities.IsCapable(CapabilityOptions.Other));
                        ClassicAssert.IsFalse(capabilities.IsCapable(CapabilityOptions.StationOnly));
                        ClassicAssert.IsFalse(capabilities.IsCapable(CapabilityOptions.Telephone));
                        ClassicAssert.IsFalse(capabilities.IsCapable(CapabilityOptions.WLanAP));
                        break;
                    }
                    case 8:
                    {
                        ClassicAssert.AreEqual(typeof(ManagementAddressTlv), tlv.GetType());
                        var mgmtAdd = (ManagementAddressTlv) tlv;
                        ClassicAssert.AreEqual(IanaAddressFamily.IPv4, mgmtAdd.AddressSubType);
                        ClassicAssert.AreEqual(managementAddressNetworkAddress, mgmtAdd.Address);
                        ClassicAssert.AreEqual(InterfaceNumber.SystemPortNumber, mgmtAdd.InterfaceSubType);
                        ClassicAssert.AreEqual(managementAddressInterfaceNumber, mgmtAdd.InterfaceNumber);
                        var expectedObjIdLength = managementAddressObjectIdentifier.Length;
                        ClassicAssert.AreEqual(expectedObjIdLength, mgmtAdd.ObjIdLength);
                        ClassicAssert.AreEqual(managementAddressObjectIdentifier, mgmtAdd.ObjectIdentifier);
                        break;
                    }
                    case 9:
                    {
                        ClassicAssert.AreEqual(typeof(OrganizationSpecificTlv), tlv.GetType());
                        var orgSpecifig = (OrganizationSpecificTlv) tlv;
                        ClassicAssert.AreEqual(expectedOrganizationUniqueIdentifier, orgSpecifig.OrganizationUniqueID);
                        ClassicAssert.AreEqual(expectedOrganizationSubType, orgSpecifig.OrganizationDefinedSubType);
                        ClassicAssert.AreEqual(expectedOrganizationSpecificBytes, orgSpecifig.OrganizationDefinedInfoString);
                        break;
                    }
                    case 10:
                    {
                        ClassicAssert.AreEqual(typeof(EndOfLldpduTlv), tlv.GetType());
                        break;
                    }
                    default:
                    {
                        throw new ArgumentException();
                    }
                }

                // increment the counter
                count++;
            }
        }

        [Test]
        public void LLDPParsing()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "lldp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var l = p.Extract<LldpPacket>();

            var count = 1;
            Console.WriteLine(l.TlvCollection.Count.ToString());
            foreach (var tlv in l.TlvCollection)
            {
                Console.WriteLine("Type: " + tlv.GetType());
                switch (count)
                {
                    case 1:
                    {
                        ClassicAssert.AreEqual(typeof(ChassisIdTlv), tlv.GetType());
                        var chassisID = (ChassisIdTlv) tlv;
                        ClassicAssert.AreEqual(chassisID.SubType, ChassisSubType.NetworkAddress);
                        ClassicAssert.AreEqual(typeof(NetworkAddress), chassisID.SubTypeValue.GetType());
                        var testAddress = new NetworkAddress(new IPAddress(new byte[] { 0xac, 0x10, 0x0a, 0x6d }));
                        ClassicAssert.AreEqual(testAddress, chassisID.SubTypeValue);
                        break;
                    }
                    case 2:
                    {
                        ClassicAssert.AreEqual(typeof(PortIdTlv), tlv.GetType());
                        var portID = (PortIdTlv) tlv;
                        ClassicAssert.AreEqual(PortSubType.LocallyAssigned, portID.SubType);
                        var subTypeValue = new byte[] { 0x30, 0x30, 0x31, 0x42, 0x35, 0x34, 0x39, 0x34, 0x35, 0x41, 0x38, 0x42, 0x3a, 0x50, 0x32 };
                        ClassicAssert.AreEqual(subTypeValue, portID.SubTypeValue);
                        break;
                    }
                    case 3:
                    {
                        ClassicAssert.AreEqual(typeof(TimeToLiveTlv), tlv.GetType());
                        ClassicAssert.AreEqual(180, ((TimeToLiveTlv) tlv).Seconds);
                        break;
                    }
                    case 4:
                    {
                        ClassicAssert.AreEqual(typeof(PortDescriptionTlv), tlv.GetType());
                        ClassicAssert.AreEqual("PC Port", ((PortDescriptionTlv) tlv).Description);
                        break;
                    }
                    case 5:
                    {
                        ClassicAssert.AreEqual(typeof(SystemNameTlv), tlv.GetType());
                        ClassicAssert.AreEqual("SEP001B54945A8B.elmec.ad", ((SystemNameTlv) tlv).Name);
                        break;
                    }
                    case 6:
                    {
                        ClassicAssert.AreEqual(typeof(SystemDescriptionTlv), tlv.GetType());
                        ClassicAssert.AreEqual("Cisco IP Phone CP-7911G,V2, SCCP11.8-4-3S", ((SystemDescriptionTlv) tlv).Description);
                        break;
                    }
                    case 7:
                    {
                        ClassicAssert.AreEqual(typeof(SystemCapabilitiesTlv), tlv.GetType());
                        var systemCapabilities = (SystemCapabilitiesTlv) tlv;
                        ClassicAssert.AreEqual(36, systemCapabilities.Capabilities);
                        ClassicAssert.AreEqual(36, systemCapabilities.Enabled);
                        ClassicAssert.IsTrue(systemCapabilities.IsCapable(CapabilityOptions.Bridge));
                        ClassicAssert.IsTrue(systemCapabilities.IsEnabled(CapabilityOptions.Telephone));
                        ClassicAssert.IsFalse(systemCapabilities.IsCapable(CapabilityOptions.Repeater));
                        ClassicAssert.IsFalse(systemCapabilities.IsEnabled(CapabilityOptions.WLanAP));
                        break;
                    }
                    case 8:
                    {
                        ClassicAssert.AreEqual(typeof(ManagementAddressTlv), tlv.GetType());
                        var managementAddress = (ManagementAddressTlv) tlv;
                        ClassicAssert.AreEqual(5, managementAddress.AddressLength);
                        ClassicAssert.AreEqual(1, (int) managementAddress.AddressSubType);
                        var mgmtAddress = new NetworkAddress(new IPAddress(new byte[] { 0xac, 0x10, 0x0a, 0x6d }));
                        ClassicAssert.AreEqual(mgmtAddress, managementAddress.Address);
                        ClassicAssert.AreEqual(1, (int) managementAddress.InterfaceSubType);
                        ClassicAssert.AreEqual(0, managementAddress.InterfaceNumber);
                        ClassicAssert.AreEqual(0, managementAddress.ObjIdLength);
                        break;
                    }
                    case 9:
                    {
                        ClassicAssert.AreEqual(typeof(OrganizationSpecificTlv), tlv.GetType());
                        var organizationSpecific = (OrganizationSpecificTlv) tlv;
                        ClassicAssert.AreEqual(new byte[] { 0x00, 0x12, 0x0f }, organizationSpecific.OrganizationUniqueID);
                        ClassicAssert.AreEqual(1, organizationSpecific.OrganizationDefinedSubType);
                        var infoString = new byte[] { 0x03, 0x00, 0x36, 0x00, 0x10 };
                        ClassicAssert.AreEqual(infoString, organizationSpecific.OrganizationDefinedInfoString);
                        break;
                    }
                    case 10:
                    {
                        ClassicAssert.AreEqual(typeof(EndOfLldpduTlv), tlv.GetType());
                        break;
                    }
                    default:
                    {
                        throw new ArgumentException();
                    }
                }

                // increment the counter
                count++;
            }
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "lldp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var l = p.Extract<LldpPacket>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(l.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "lldp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var l = p.Extract<LldpPacket>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(l.ToString(StringOutputType.Verbose));
        }

        [Test]
        public void RandomPacket()
        {
            LldpPacket.RandomPacket();
        }
    }