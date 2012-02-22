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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class RadioPacketTest
        {
            /// <summary>
            /// Test that parsing an ip packet yields the proper field values
            /// </summary>
            [Test]
            public void ReadingPacketsFromFile()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_plus_radiotap_header.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

                Assert.IsNotNull(p);

                Console.WriteLine(p.ToString());
            }
			
			[Test]
			public void ReadPacketWithNoFcs()
			{
				var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_radio_without_fcs.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();
                
                RadioPacket p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
                Assert.IsNotNull (p.PayloadPacket);
				MacFrame macFrame = p.PayloadPacket as MacFrame;
				Assert.IsFalse(macFrame.FCSValid);
			}

        } 
    }
}
