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
 *  Copyright 2013 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class Vlan802_1QTest
    {
        /// <summary>
        /// Test that a vlan packet can be properly parsed
        /// </summary>
        [Test]
        public void ParsingVlanPacket()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/802.1q_vlan_ipv4_tcp.pcap");
            dev.Open();

            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();

            Console.WriteLine("Parsing");
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(p.ToString(StringOutputType.Verbose));

            var vlanTag = (Ieee8021QPacket)p.Extract(typeof(Ieee8021QPacket));
            Assert.AreEqual(IeeeP8021PPriorities.BestEffort_1, vlanTag.PriorityControlPoint);
            var tagId = 102;
            Assert.AreEqual(tagId, vlanTag.VLANIdentifier);
            Assert.AreEqual(false, vlanTag.CanonicalFormatIndicator);
        }
    }
}
