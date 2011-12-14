using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PacketDotNet.Utils;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class ContentionFreeEndFrameTest
        {
            /// <summary>
            /// Test that parsing a contention free end frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_contention_free_end_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                ContentionFreeEndFrame frame = (ContentionFreeEndFrame)p.PayloadPacket;

                Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameTypes.ControlCFEnd, frame.FrameControl.Type);
                Assert.IsFalse(frame.FrameControl.ToDS);
                Assert.IsFalse(frame.FrameControl.FromDS);
                Assert.IsFalse(frame.FrameControl.MoreFragments);
                Assert.IsFalse(frame.FrameControl.Retry);
                Assert.IsFalse(frame.FrameControl.PowerManagement);
                Assert.IsFalse(frame.FrameControl.MoreData);
                Assert.IsFalse(frame.FrameControl.Wep);
                Assert.IsTrue(frame.FrameControl.Order);
                Assert.AreEqual(0, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual("FFFFFFFFFFFF", frame.ReceiverAddress.ToString().ToUpper());
                Assert.AreEqual("001B2FDCFC12", frame.BssId.ToString().ToUpper());

                Assert.AreEqual(0x0AE8A403, frame.FrameCheckSequence);
                Assert.AreEqual(16, frame.FrameSize);
            }
        } 
    }
}
