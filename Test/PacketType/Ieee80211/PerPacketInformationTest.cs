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
        public class PerPacketInformationTest
        {
            /// <summary>
            /// Test that parsing an ip packet yields the proper field values
            /// </summary>
            [Test]
            public void ReadingPacketsFromFile ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_per_packet_information.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                PpiPacket p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;

                Assert.IsNotNull (p);
                Assert.AreEqual (0, p.Version);
                Assert.AreEqual (32, p.Length);
                Assert.AreEqual (1, p.PpiFields.Count);
                
                PpiCommon commonField = p.PpiFields [0] as PpiCommon;
                
                Assert.AreEqual (PpiFieldType.PpiCommon, commonField.FieldType);
                Assert.AreEqual (0, commonField.TSFTimer);
                Assert.IsTrue ((commonField.Flags & PpiCommon.CommonFlags.FcsIncludedInFrame) == PpiCommon.CommonFlags.FcsIncludedInFrame);
                Assert.AreEqual (2, commonField.Rate);
                Assert.AreEqual (2437, commonField.ChannelFrequency);
                Assert.AreEqual (0x00A0, commonField.ChannelFlags);
                Assert.AreEqual (0, commonField.FhssHopset);
                Assert.AreEqual (0, commonField.FhssPattern);
                Assert.AreEqual (-84, commonField.AntennaSignalPower);
                Assert.AreEqual (-100, commonField.AntennaSignalNoise);
                
                MacFrame macFrame = p.PayloadPacket as MacFrame;
                Assert.AreEqual(FrameControlField.FrameSubTypes.ControlCTS, macFrame.FrameControl.SubType);
                Assert.IsTrue(macFrame.AppendFcs);
            }
            
            [Test]
            public void ReadPacketWithInvalidFcs ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_ppi_fcs_present_and_invalid.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();
                
                PpiPacket p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;
				
				//The packet is corrupted in such a way that the type field has been changed
				//to a reserved/unused type. Therefore we don't expect there to be a packet
                Assert.IsNull (p.PayloadPacket);
                Assert.IsNotNull(p.PayloadData);
            }
            
            [Test]
            public void ReadPacketWithValidFcs ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_ppi_fcs_present_and_valid.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();
                
                PpiPacket p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;
                Assert.IsNotNull (p.PayloadPacket);
                MacFrame macFrame = p.PayloadPacket as MacFrame;
                Assert.IsTrue(macFrame.FCSValid);
                Assert.IsTrue(macFrame.AppendFcs);
            }
			
			[Test]
			public void ReadPacketWithNoFcs()
			{
				var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_ppi_without_fcs.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();
                
                PpiPacket p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;
                Assert.IsNotNull (p.PayloadPacket);
                MacFrame macFrame = p.PayloadPacket as MacFrame;
                Assert.IsFalse(macFrame.FCSValid);
                Assert.IsFalse(macFrame.AppendFcs);
			}
        } 
    }
}
