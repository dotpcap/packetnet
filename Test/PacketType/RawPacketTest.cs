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
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class RawPacketTest
    {
        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new OfflinePcapDevice("../../CaptureFiles/arp_request_response.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextRawPacket();
            var rawPacket = dev.GetNextRawPacket();
            dev.Close();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(rawPacket.ToString());
        }
    }
}