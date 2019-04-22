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
using NUnit.Framework;
using PacketDotNet.Ieee80211;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class FrameControlFieldTest
    {
        [Test]
        public void Test_Constructor_AckFrame()
        {
            var frameControl = new FrameControlField(0xD410);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(FrameControlField.FrameTypes.Control, frameControl.Type);
            Assert.AreEqual(FrameControlField.FrameSubTypes.ControlAck, frameControl.SubType);
            Assert.IsFalse(frameControl.FromDS);
            Assert.IsFalse(frameControl.Protected);
            Assert.IsFalse(frameControl.ToDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsTrue(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_BeaconFrame()
        {
            var frameControl = new FrameControlField(0x8000);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(FrameControlField.FrameTypes.Management, frameControl.Type);
            Assert.AreEqual(FrameControlField.FrameSubTypes.ManagementBeacon, frameControl.SubType);
            Assert.IsFalse(frameControl.FromDS);
            Assert.IsFalse(frameControl.Protected);
            Assert.IsFalse(frameControl.ToDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsFalse(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_ClearToSendFrame()
        {
            var frameControl = new FrameControlField(0xC400);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(FrameControlField.FrameTypes.Control, frameControl.Type);
            Assert.AreEqual(FrameControlField.FrameSubTypes.ControlCts, frameControl.SubType);
            Assert.IsFalse(frameControl.FromDS);
            Assert.IsFalse(frameControl.Protected);
            Assert.IsFalse(frameControl.ToDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsFalse(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_DataFrame()
        {
            var frameControl = new FrameControlField(0x0801);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(FrameControlField.FrameTypes.Data, frameControl.Type);
            Assert.AreEqual(FrameControlField.FrameSubTypes.Data, frameControl.SubType);

            Assert.IsTrue(frameControl.ToDS);
            Assert.IsFalse(frameControl.FromDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsFalse(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Protected);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_DisassociationFrame()
        {
            var frameControl = new FrameControlField(0xA01B);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(FrameControlField.FrameTypes.Management, frameControl.Type);
            Assert.AreEqual(FrameControlField.FrameSubTypes.ManagementDisassociation, frameControl.SubType);
            Assert.IsTrue(frameControl.FromDS);
            Assert.IsTrue(frameControl.ToDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Protected);
            Assert.IsTrue(frameControl.Retry);
            Assert.IsTrue(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_EncryptedDataFrame()
        {
            var frameControl = new FrameControlField(0x0842);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(FrameControlField.FrameTypes.Data, frameControl.Type);
            Assert.AreEqual(FrameControlField.FrameSubTypes.Data, frameControl.SubType);
            Assert.IsTrue(frameControl.FromDS);
            Assert.IsTrue(frameControl.Protected);

            Assert.IsFalse(frameControl.ToDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsFalse(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_SetFromDsProperty()
        {
            var frameControl = new FrameControlField { FromDS = true };

            Assert.IsTrue(frameControl.FromDS);
        }

        [Test]
        public void Test_SetMoreDataProperty()
        {
            var frameControl = new FrameControlField { MoreData = true };

            Assert.IsTrue(frameControl.MoreData);
        }

        [Test]
        public void Test_SetMoreFragmentsProperty()
        {
            var frameControl = new FrameControlField { MoreFragments = true };

            Assert.IsTrue(frameControl.MoreFragments);
        }

        [Test]
        public void Test_SetOrderProperty()
        {
            var frameControl = new FrameControlField { Order = true };

            Assert.IsTrue(frameControl.Order);
        }

        [Test]
        public void Test_SetPowerManagementProperty()
        {
            var frameControl = new FrameControlField { PowerManagement = true };

            Assert.IsTrue(frameControl.PowerManagement);
        }

        [Test]
        public void Test_SetProtocolVersionProperty()
        {
            var frameControl = new FrameControlField { ProtocolVersion = 3 };

            Assert.AreEqual(3, frameControl.ProtocolVersion);

            frameControl.ProtocolVersion = 2;
            Assert.AreEqual(2, frameControl.ProtocolVersion);

            frameControl.ProtocolVersion = 1;
            Assert.AreEqual(1, frameControl.ProtocolVersion);

            frameControl.ProtocolVersion = 0;
            Assert.AreEqual(0, frameControl.ProtocolVersion);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Invalid protocol version value. Value must be in the range 0-3.")]
        public void Test_SetProtocolVersionProperty_ValueTooLarge()
        {
            var _ = new FrameControlField { ProtocolVersion = 4 };
        }

        [Test]
        public void Test_SetRetryProperty()
        {
            var frameControl = new FrameControlField { Retry = true };

            Assert.IsTrue(frameControl.Retry);
        }

        [Test]
        public void Test_SetToDsProperty()
        {
            var frameControl = new FrameControlField { ToDS = true };

            Assert.IsTrue(frameControl.ToDS);
        }

        [Test]
        public void Test_SetTypeProperty()
        {
            var frameControl = new FrameControlField { SubType = FrameControlField.FrameSubTypes.Data };

            Assert.AreEqual(FrameControlField.FrameSubTypes.Data, frameControl.SubType);

            frameControl.SubType = FrameControlField.FrameSubTypes.ManagementAuthentication;
            Assert.AreEqual(FrameControlField.FrameSubTypes.ManagementAuthentication, frameControl.SubType);

            frameControl.SubType = FrameControlField.FrameSubTypes.ControlAck;
            Assert.AreEqual(FrameControlField.FrameSubTypes.ControlAck, frameControl.SubType);
        }

        [Test]
        public void Test_SetWepProperty()
        {
            var frameControl = new FrameControlField { Protected = true };

            Assert.IsTrue(frameControl.Protected);
        }
    }
}