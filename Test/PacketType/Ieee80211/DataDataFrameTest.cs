using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class DataDataFrameTest
        {
            /// <summary>
            /// Test that parsing an unecrypted data frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor_UnencryptedDataFrame()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_unencrypted_data_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                DataDataFrame frame = (DataDataFrame)p.PayloadPacket;

                Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameTypes.Data, frame.FrameControl.Type);
                Assert.IsTrue(frame.FrameControl.ToDS);
                Assert.IsFalse(frame.FrameControl.FromDS);
                Assert.IsFalse(frame.FrameControl.MoreFragments);
                Assert.IsFalse(frame.FrameControl.Retry);
                Assert.IsFalse(frame.FrameControl.PowerManagement);
                Assert.IsFalse(frame.FrameControl.MoreData);
                Assert.IsFalse(frame.FrameControl.Wep);
                Assert.IsFalse(frame.FrameControl.Order);
                Assert.AreEqual(44, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual("0024B2F8D706", frame.DestinationAddress.ToString().ToUpper());
                Assert.AreEqual("00173FB72C29", frame.SourceAddress.ToString().ToUpper());
                Assert.AreEqual("0024B2F8D706", frame.BssId.ToString().ToUpper());
                Assert.AreEqual(0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual(2712, frame.SequenceControl.SequenceNumber);

                Assert.AreEqual(0x8F1A5F60, frame.FrameCheckSequence);
                Assert.AreEqual(24, frame.FrameSize);
                Assert.AreEqual(681, frame.PayloadData.Length);
            }

            /// <summary>
            /// Test that parsing an ecrypted data frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor_EncryptedDataFrame()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_encrypted_data_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                DataDataFrame frame = (DataDataFrame)p.PayloadPacket;

                Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameTypes.Data, frame.FrameControl.Type);
                Assert.IsFalse(frame.FrameControl.ToDS);
                Assert.IsTrue(frame.FrameControl.FromDS);
                Assert.IsFalse(frame.FrameControl.MoreFragments);
                Assert.IsFalse(frame.FrameControl.Retry);
                Assert.IsFalse(frame.FrameControl.PowerManagement);
                Assert.IsFalse(frame.FrameControl.MoreData);
                Assert.IsTrue(frame.FrameControl.Wep);
                Assert.IsFalse(frame.FrameControl.Order);
                Assert.AreEqual(0, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual("33330000000C", frame.DestinationAddress.ToString().ToUpper());
                Assert.AreEqual("00173FB72C29", frame.SourceAddress.ToString().ToUpper());
                Assert.AreEqual("0024B2F8D706", frame.BssId.ToString().ToUpper());
                Assert.AreEqual(0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual(1561, frame.SequenceControl.SequenceNumber);

                Assert.AreEqual(0xC77B6323, frame.FrameCheckSequence);
                Assert.AreEqual(24, frame.FrameSize);
                Assert.AreEqual(218, frame.PayloadData.Length);

            }
        } 
    }
}
