using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet.Utils;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class DisassociationFrameTest
        {
            /// <summary>
            /// Test that parsing a disassociation frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_disassociation_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                DisassociationFrame frame = (DisassociationFrame)p.PayloadPacket;

                Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameTypes.ManagementDisassociation, frame.FrameControl.Type);
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
                Assert.AreEqual(1311, frame.SequenceControl.SequenceNumber);
                Assert.AreEqual(Ieee80211ReasonCode.LEAVING_TO_ROAM, frame.Reason);

                Assert.AreEqual(0xB17572A4, frame.FrameCheckSequence);
                Assert.AreEqual(26, frame.FrameSize);
            }
        } 
    }
}
