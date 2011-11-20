using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet.Utils;
using PacketDotNet;

namespace Test.PacketType
{
    [TestFixture]
    class Ieee80211AuthenticationFrameTest
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
            Ieee80211AuthenticationFrame frame = (Ieee80211AuthenticationFrame)p.PayloadPacket;

            Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ManagementAuthentication, frame.FrameControl.Type);
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
            Assert.AreEqual(Ieee80211AuthenticationStatusCode.Success, frame.StatusCode);
            
            Assert.AreEqual(0x4AE3E90F, frame.FrameCheckSequence);
            Assert.AreEqual(30, frame.FrameSize);
        }

        /// <summary>
        /// Test that parsing a authentication frame with information elements yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor_WithInformationElements()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_authentication_frame_with_ie.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            Ieee80211AuthenticationFrame frame = (Ieee80211AuthenticationFrame)p.PayloadPacket;

            Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ManagementAuthentication, frame.FrameControl.Type);
            Assert.IsFalse(frame.FrameControl.ToDS);
            Assert.IsFalse(frame.FrameControl.FromDS);
            Assert.IsFalse(frame.FrameControl.MoreFragments);
            Assert.IsFalse(frame.FrameControl.Retry);
            Assert.IsFalse(frame.FrameControl.PowerManagement);
            Assert.IsFalse(frame.FrameControl.MoreData);
            Assert.IsFalse(frame.FrameControl.Wep);
            Assert.IsFalse(frame.FrameControl.Order);
            Assert.AreEqual(314, frame.Duration.Field); //this need expanding on in the future
            Assert.AreEqual("00173FB72C29", frame.DestinationAddress.ToString().ToUpper());
            Assert.AreEqual("0024B2F8D706", frame.SourceAddress.ToString().ToUpper());
            Assert.AreEqual("0024B2F8D706", frame.BssId.ToString().ToUpper());
            Assert.AreEqual(0, frame.SequenceControl.FragmentNumber);
            Assert.AreEqual(957, frame.SequenceControl.SequenceNumber);

            Assert.AreEqual(0, frame.AuthenticationAlgorithmNumber);
            Assert.AreEqual(2, frame.AuthenticationAlgorithmTransactionSequenceNumber);
            Assert.AreEqual(Ieee80211AuthenticationStatusCode.Success, frame.StatusCode);
            Assert.AreEqual(Ieee80211InformationElement.ElementId.VendorSpecific, frame.InformationElements.InformationElements[0].Id);
            Assert.AreEqual(9, frame.InformationElements.InformationElements[0].Length);

            Assert.AreEqual(0x5B12F62C, frame.FrameCheckSequence);
            Assert.AreEqual(41, frame.FrameSize);
        }
    }
}
