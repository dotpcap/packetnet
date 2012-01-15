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
