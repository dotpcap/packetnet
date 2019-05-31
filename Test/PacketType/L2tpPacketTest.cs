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
 *  Copyright 2018 Steven Haufe<haufes@hotmail.com>
 */

using System;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class L2tpPacketTest
    {
        // L2TP
        [Test]
        public void L2tpParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "l2tp.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            var l2tp = p.Extract<L2tpPacket>();
            Assert.AreEqual(l2tp.TunnelID, 18994);
            Assert.AreEqual(l2tp.SessionID, 54110);
            Console.WriteLine(l2tp.GetType());
        }
    }
}