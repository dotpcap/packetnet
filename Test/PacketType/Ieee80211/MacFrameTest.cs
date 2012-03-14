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
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet.Utils;
using System.Net.NetworkInformation;
using MiscUtil.Conversion;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class MacFrameTest
        {
            public MacFrameTest ()
            {
            }

            /// <summary>
            /// Test that the MacFrame.FCSValid property is working correctly
            /// </summary>
            [Test]
            public void FCSTest ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_association_request_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                // check that the fcs can be calculated correctly
                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                AssociationRequestFrame frame = (AssociationRequestFrame)p.PayloadPacket;
                Assert.AreEqual (0xde82c216, frame.FrameCheckSequence, "FCS mismatch");
                Assert.IsTrue (frame.FCSValid);

                // adjust the fcs of the packet and check that the FCSValid property returns false
                frame.FrameCheckSequence = 0x1;
                Assert.IsFalse (frame.FCSValid);                
            }
            
            [Test]
            public void Test_NoFCS ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_beacon_no_fcs.pcap");
                
                dev.Open ();
                

                RawCapture rawCapture;
                while ((rawCapture = dev.GetNextPacket ()) != null)
                {
                    Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                    Assert.IsNotNull (p.PayloadPacket);
                }

                // check that the fcs can be calculated correctly
                dev.Close ();
            }
            
            [Test]
            public void Test_AppendFcs_Raw80211WithFcs()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_raw_with_fcs.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();
                
                //For this test we are just going to ignore the radio packet that precedes the data frame
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                MacFrame macFrame = p as MacFrame;
                
                //When its raw 802.11 we cant tell if there is an FCS there so even though
                //there is we still expect AppendFcs to be false.
                Assert.IsFalse(macFrame.AppendFcs);
            }
            
            [Test]
            public void Test_AppendFcs_Raw80211WithoutFcs()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_raw_without_fcs.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();
                
                //For this test we are just going to ignore the radio packet that precedes the data frame
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                MacFrame macFrame = p as MacFrame;
                Assert.IsFalse(macFrame.AppendFcs);
            }
            
            [Test]
            public void Test_Bytes_ConstructedPacketWithFcsIncluded()
            {
                //We will use a DataDataFrame for this test but the 
                //type isn't really important.
                var dataFrame = new DataDataFrame();
                dataFrame.FrameControl.FromDS = true;
                dataFrame.BssId = PhysicalAddress.Parse("AABBCCDDEEFF");
                dataFrame.SourceAddress = PhysicalAddress.Parse("AABBCCDDEEFF");
                dataFrame.DestinationAddress = PhysicalAddress.Parse("112233445566");
                dataFrame.PayloadData = new byte[]{0x1, 0x2, 0x3, 0x4};
                
                //Force it to recalculate the FCS and include it when serialised
                dataFrame.UpdateFrameCheckSequence();
                dataFrame.AppendFcs = true;
                Assert.IsTrue(dataFrame.FCSValid);
                
                var expectedLength = dataFrame.FrameSize + dataFrame.PayloadData.Length + 4;
                
                byte[] frameBytes = dataFrame.Bytes;
                
                Assert.AreEqual(expectedLength, frameBytes.Length);
                Assert.AreEqual(dataFrame.FrameCheckSequence.ToString("X"),
                                EndianBitConverter.Big.ToUInt32(frameBytes, frameBytes.Length - 4).ToString("X"));
                Assert.AreEqual(dataFrame.FrameCheckSequence,
                                EndianBitConverter.Big.ToUInt32(frameBytes, frameBytes.Length - 4));
                
            }
            
            [Test]
            public void Test_Bytes_ConstructedPacketWithFcsNotIncluded()
            {
                //We will use a DataDataFrame for this test but the 
                //type isn't really important.
                var dataFrame = new DataDataFrame();
                dataFrame.FrameControl.FromDS = true;
                dataFrame.BssId = PhysicalAddress.Parse("AABBCCDDEEFF");
                dataFrame.SourceAddress = PhysicalAddress.Parse("AABBCCDDEEFF");
                dataFrame.DestinationAddress = PhysicalAddress.Parse("112233445566");
                dataFrame.PayloadData = new byte[]{0x01, 0x02, 0x03, 0x04};
                
                //Force it to recalculate the FCS but don't include it when serialised
                dataFrame.UpdateFrameCheckSequence();
                dataFrame.AppendFcs = false;
                
                var expectedLength = dataFrame.FrameSize + dataFrame.PayloadData.Length;
                var frameBytes = dataFrame.Bytes;
                
                Assert.AreEqual(expectedLength, frameBytes.Length);
                //Check that we get last four bytes of data at the end rather than the FCS
                Assert.AreEqual(0x01020304, EndianBitConverter.Big.ToUInt32(frameBytes, frameBytes.Length - 4));
            }
            
            [Test]
            public void Test_Bytes_ParsedPacketWithFcsIncluded()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_unencrypted_data_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();
                
                //For this test we are just going to ignore the radio packet that precedes the data frame
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                
                DataDataFrame frame = (DataDataFrame)p.PayloadPacket;
                frame.AppendFcs = true;
                
                var frameBytes = frame.Bytes;
                var expectedLength = frame.FrameSize + frame.PayloadData.Length + 4;
                Assert.AreEqual(expectedLength, frameBytes.Length);
                Assert.AreEqual(frame.FrameCheckSequence,
                                EndianBitConverter.Big.ToUInt32(frameBytes, frameBytes.Length - 4));
            }
            
            [Test]
            public void Test_Bytes_ParsedPacketWithFcsNotIncluded()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_unencrypted_data_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();
                
                //For this test we are just going to ignore the radio packet that precedes the data frame
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                
                DataDataFrame frame = (DataDataFrame)p.PayloadPacket;
                frame.AppendFcs = false;
                
                var frameBytes = frame.Bytes;
                var expectedLength = frame.FrameSize + frame.PayloadData.Length;
                Assert.AreEqual(expectedLength, frameBytes.Length);
                
                
                Assert.AreEqual(EndianBitConverter.Big.ToUInt32(frame.PayloadData, frame.PayloadData.Length - 4),
                                EndianBitConverter.Big.ToUInt32(frameBytes, frameBytes.Length - 4));
            }
        }
    }
}

