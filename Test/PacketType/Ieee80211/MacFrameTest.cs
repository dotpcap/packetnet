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

using System;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using NUnit.Framework;

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
            public void FCSTest()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_association_request_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                // check that the fcs can be calculated correctly
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                AssociationRequestFrame frame = (AssociationRequestFrame)p.PayloadPacket;
                Assert.AreEqual(0xde82c216, frame.FrameCheckSequence, "FCS mismatch");
                Assert.IsTrue(frame.FCSValid);

                // adjust the fcs of the packet and check that the FCSValid property returns false
                frame.FrameCheckSequence = 0x1;
                Assert.IsFalse(frame.FCSValid);                
            }
        }
    }
}

