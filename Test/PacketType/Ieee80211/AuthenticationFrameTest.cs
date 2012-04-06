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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet.Utils;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using System.Net.NetworkInformation;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class AuthenticationFrameTest
        {
            /// <summary>
            /// Test that parsing a authentication frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_authentication_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                AuthenticationFrame frame = (AuthenticationFrame)p.PayloadPacket;

                Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameSubTypes.ManagementAuthentication, frame.FrameControl.SubType);
                Assert.IsFalse(frame.FrameControl.ToDS);
                Assert.IsFalse(frame.FrameControl.FromDS);
                Assert.IsFalse(frame.FrameControl.MoreFragments);
                Assert.IsFalse(frame.FrameControl.Retry);
                Assert.IsFalse(frame.FrameControl.PowerManagement);
                Assert.IsFalse(frame.FrameControl.MoreData);
                Assert.IsFalse(frame.FrameControl.Wep);
                Assert.IsFalse(frame.FrameControl.Order);
                Assert.AreEqual(248, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual("0024B2F8D706", frame.DestinationAddress.ToString().ToUpper());
                Assert.AreEqual("00173FB72C29", frame.SourceAddress.ToString().ToUpper());
                Assert.AreEqual("0024B2F8D706", frame.BssId.ToString().ToUpper());
                Assert.AreEqual(0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual(1327, frame.SequenceControl.SequenceNumber);

                Assert.AreEqual(0, frame.AuthenticationAlgorithmNumber);
                Assert.AreEqual(1, frame.AuthenticationAlgorithmTransactionSequenceNumber);
                Assert.AreEqual(AuthenticationStatusCode.Success, frame.StatusCode);

                Assert.AreEqual(0x4AE3E90F, frame.FrameCheckSequence);
                Assert.AreEqual(30, frame.FrameSize);
            }

            /// <summary>
            /// Test that parsing a authentication frame with information elements yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor_WithInformationElements ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_authentication_frame_with_ie.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                AuthenticationFrame frame = (AuthenticationFrame)p.PayloadPacket;

                Assert.AreEqual (0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementAuthentication, frame.FrameControl.SubType);
                Assert.IsFalse (frame.FrameControl.ToDS);
                Assert.IsFalse (frame.FrameControl.FromDS);
                Assert.IsFalse (frame.FrameControl.MoreFragments);
                Assert.IsFalse (frame.FrameControl.Retry);
                Assert.IsFalse (frame.FrameControl.PowerManagement);
                Assert.IsFalse (frame.FrameControl.MoreData);
                Assert.IsFalse (frame.FrameControl.Wep);
                Assert.IsFalse (frame.FrameControl.Order);
                Assert.AreEqual (314, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual ("00173FB72C29", frame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0024B2F8D706", frame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0024B2F8D706", frame.BssId.ToString ().ToUpper ());
                Assert.AreEqual (0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual (957, frame.SequenceControl.SequenceNumber);

                Assert.AreEqual (0, frame.AuthenticationAlgorithmNumber);
                Assert.AreEqual (2, frame.AuthenticationAlgorithmTransactionSequenceNumber);
                Assert.AreEqual (AuthenticationStatusCode.Success, frame.StatusCode);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, frame.InformationElements [0].Id);
                Assert.AreEqual (9, frame.InformationElements [0].ValueLength);

                Assert.AreEqual (0x5B12F62C, frame.FrameCheckSequence);
                Assert.AreEqual (41, frame.FrameSize);
            }
            
            [Test]
            public void Test_Constructor_ConstructWithValues ()
            {
                InformationElement ssidInfoElement = new InformationElement (InformationElement.ElementId.ServiceSetIdentity, 
                                                                           new Byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f });
                InformationElement vendorElement = new InformationElement (InformationElement.ElementId.VendorSpecific,
                                                                           new Byte[] {0x01, 0x02, 0x03, 0x04, 0x05});
                
                
                AuthenticationFrame frame = new AuthenticationFrame (PhysicalAddress.Parse ("111111111111"),
                                                                     PhysicalAddress.Parse ("222222222222"),
                                                                     PhysicalAddress.Parse ("333333333333"),
                                                                     new InformationElementList (){ssidInfoElement, vendorElement});
                
                frame.FrameControl.ToDS = false;
                frame.FrameControl.FromDS = true;
                frame.FrameControl.MoreFragments = true;
                
                frame.Duration.Field = 0x1234;
                
                frame.SequenceControl.SequenceNumber = 0x77;
                frame.SequenceControl.FragmentNumber = 0x1;
                
                frame.AuthenticationAlgorithmNumber = 0x4444;
                frame.AuthenticationAlgorithmTransactionSequenceNumber = 0x5555;
                frame.StatusCode = AuthenticationStatusCode.Success;
                
                frame.UpdateFrameCheckSequence ();
                UInt32 fcs = frame.FrameCheckSequence;
                
                //serialize the frame into a byte buffer
                var bytes = frame.Bytes;
                var bas = new ByteArraySegment (bytes);

                //create a new frame that should be identical to the original
                AuthenticationFrame recreatedFrame = MacFrame.ParsePacket (bas) as AuthenticationFrame;
                recreatedFrame.UpdateFrameCheckSequence();
                
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementAuthentication, recreatedFrame.FrameControl.SubType);
                Assert.IsFalse (recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue (recreatedFrame.FrameControl.FromDS);
                Assert.IsTrue (recreatedFrame.FrameControl.MoreFragments);
                
                Assert.AreEqual (0x77, recreatedFrame.SequenceControl.SequenceNumber);
                Assert.AreEqual (0x1, recreatedFrame.SequenceControl.FragmentNumber);
                
                Assert.AreEqual (0x4444, recreatedFrame.AuthenticationAlgorithmNumber);
                Assert.AreEqual (0x5555, recreatedFrame.AuthenticationAlgorithmTransactionSequenceNumber);
                Assert.AreEqual (AuthenticationStatusCode.Success, recreatedFrame.StatusCode);
                
                Assert.AreEqual ("111111111111", recreatedFrame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("222222222222", recreatedFrame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("333333333333", recreatedFrame.BssId.ToString ().ToUpper ());
                
                Assert.AreEqual (fcs, recreatedFrame.FrameCheckSequence);
            }
			
			[Test]
			public void Test_ConstructorWithCorruptBuffer ()
			{
				//buffer is way too short for frame. We are just checking it doesn't throw
				byte[] corruptBuffer = new byte[]{0x01};
				AuthenticationFrame frame = new AuthenticationFrame(new ByteArraySegment(corruptBuffer));
				Assert.IsFalse(frame.FCSValid);
			}
        } 
    }
}
