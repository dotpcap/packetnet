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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class PppPacketTest
    {
        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "PPPoEPPP.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket();
            var rawCapture = dev.GetNextPacket();
            dev.Close();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var ppp = p.Extract<PppPacket>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(ppp.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "PPPoEPPP.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket();
            var rawCapture = dev.GetNextPacket();
            dev.Close();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var ppp = p.Extract<PppPacket>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(ppp.ToString(StringOutputType.Verbose));
        }
    }
}