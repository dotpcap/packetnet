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

