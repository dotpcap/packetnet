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
        public class QosNullDataFrameTest
        {
            /// <summary>
            /// Test that parsing a QOS null data frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_qos_null_data_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                QosNullDataFrame frame = (QosNullDataFrame)p.PayloadPacket;

                Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameTypes.QosNullData, frame.FrameControl.Type);
                Assert.IsFalse(frame.FrameControl.ToDS);
                Assert.IsTrue(frame.FrameControl.FromDS);
                Assert.IsFalse(frame.FrameControl.MoreFragments);
                Assert.IsFalse(frame.FrameControl.Retry);
                Assert.IsFalse(frame.FrameControl.PowerManagement);
                Assert.IsFalse(frame.FrameControl.MoreData);
                Assert.IsFalse(frame.FrameControl.Wep);
                Assert.IsFalse(frame.FrameControl.Order);
                Assert.AreEqual(314, frame.Duration.Field);
                Assert.AreEqual("78ACC00A5E0E", frame.DestinationAddress.ToString().ToUpper());
                Assert.AreEqual("00223FC1F378", frame.BssId.ToString().ToUpper());
                Assert.AreEqual("00223FC1F378", frame.SourceAddress.ToString().ToUpper());
                Assert.AreEqual(0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual(2292, frame.SequenceControl.SequenceNumber);
                Assert.AreEqual(0x00, frame.QosControl);
                Assert.AreEqual(0xDBF2B119, frame.FrameCheckSequence);
                Assert.AreEqual(26, frame.FrameSize);
            }
        } 
    }
}
