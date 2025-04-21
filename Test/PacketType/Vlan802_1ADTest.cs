/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

    [TestFixture]
    public class Vlan802_1ADTest
    {
        /// <summary>
        /// Test that a vlan packet can be properly parsed
        /// </summary>
        [Test]
        public void ParsingDoubleVlanPacket()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "802.1ad_vlan_ipv4.pcap");
            dev.Open();

            Console.WriteLine("Reading packet data");
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();

            Console.WriteLine("Parsing");
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(p.ToString(StringOutputType.Verbose));

            var outerVlanTag = p.Extract<Ieee8021QPacket>();
            ClassicAssert.AreEqual(IeeeP8021PPriority.BestEffort, outerVlanTag.PriorityControlPoint);
            var outerTagId = 30;
            ClassicAssert.AreEqual(outerTagId, outerVlanTag.VlanIdentifier);
            ClassicAssert.AreEqual(false, outerVlanTag.CanonicalFormatIndicator);
            ClassicAssert.AreEqual(EthernetType.VLanTaggedFrame, outerVlanTag.Type);

            var innerVlanTag = (Ieee8021QPacket) outerVlanTag.PayloadPacket;
            ClassicAssert.AreEqual(IeeeP8021PPriority.BestEffort, innerVlanTag.PriorityControlPoint);
            var innerTagId = 100;
            ClassicAssert.AreEqual(innerTagId, innerVlanTag.VlanIdentifier);
            ClassicAssert.AreEqual(false, innerVlanTag.CanonicalFormatIndicator);
            ClassicAssert.AreEqual(EthernetType.IPv4, innerVlanTag.Type);
        }
    }