using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap.LibPcap;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        class AssociationResponseFrameTest
        {
            /// <summary>
            /// Test that parsing a association response frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_association_response_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                AssociationResponseFrame frame = (AssociationResponseFrame)p.PayloadPacket;

                Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameTypes.ManagementAssociationResponse, frame.FrameControl.Type);
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
                Assert.AreEqual(958, frame.SequenceControl.SequenceNumber);
                Assert.IsTrue(frame.CapabilityInformation.IsEss);
                Assert.IsFalse(frame.CapabilityInformation.IsIbss);
                Assert.IsFalse(frame.CapabilityInformation.CfPollable);
                Assert.IsFalse(frame.CapabilityInformation.CfPollRequest);
                Assert.IsTrue(frame.CapabilityInformation.Privacy);
                Assert.IsFalse(frame.CapabilityInformation.ShortPreamble);
                Assert.IsFalse(frame.CapabilityInformation.Pbcc);
                Assert.IsFalse(frame.CapabilityInformation.ChannelAgility);
                Assert.IsTrue(frame.CapabilityInformation.ShortTimeSlot);
                Assert.IsFalse(frame.CapabilityInformation.DssOfdm);
                Assert.AreEqual(AuthenticationStatusCode.Success, frame.StatusCode);
                Assert.AreEqual(2, frame.AssociationId);
                Assert.AreEqual(0xC61ACCD6, frame.FrameCheckSequence);
                Assert.AreEqual(3, frame.InformationElements.InformationElements.Count);
                Assert.AreEqual(57, frame.FrameSize);
            }
        } 
    }
}
