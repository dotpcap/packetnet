/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet.Ieee80211;

namespace Test.PacketType.Ieee80211;

    [TestFixture]
    public class FrameControlFieldTest
    {
        [Test]
        public void Test_Constructor_AckFrame()
        {
            var frameControl = new FrameControlField(0xD410);

            ClassicAssert.AreEqual(0x0, frameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameTypes.Control, frameControl.Type);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ControlAck, frameControl.SubType);
            ClassicAssert.IsFalse(frameControl.FromDS);
            ClassicAssert.IsFalse(frameControl.Protected);
            ClassicAssert.IsFalse(frameControl.ToDS);
            ClassicAssert.IsFalse(frameControl.MoreFragments);
            ClassicAssert.IsFalse(frameControl.Retry);
            ClassicAssert.IsTrue(frameControl.PowerManagement);
            ClassicAssert.IsFalse(frameControl.MoreData);
            ClassicAssert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_BeaconFrame()
        {
            var frameControl = new FrameControlField(0x8000);

            ClassicAssert.AreEqual(0x0, frameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameTypes.Management, frameControl.Type);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ManagementBeacon, frameControl.SubType);
            ClassicAssert.IsFalse(frameControl.FromDS);
            ClassicAssert.IsFalse(frameControl.Protected);
            ClassicAssert.IsFalse(frameControl.ToDS);
            ClassicAssert.IsFalse(frameControl.MoreFragments);
            ClassicAssert.IsFalse(frameControl.Retry);
            ClassicAssert.IsFalse(frameControl.PowerManagement);
            ClassicAssert.IsFalse(frameControl.MoreData);
            ClassicAssert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_ClearToSendFrame()
        {
            var frameControl = new FrameControlField(0xC400);

            ClassicAssert.AreEqual(0x0, frameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameTypes.Control, frameControl.Type);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ControlCts, frameControl.SubType);
            ClassicAssert.IsFalse(frameControl.FromDS);
            ClassicAssert.IsFalse(frameControl.Protected);
            ClassicAssert.IsFalse(frameControl.ToDS);
            ClassicAssert.IsFalse(frameControl.MoreFragments);
            ClassicAssert.IsFalse(frameControl.Retry);
            ClassicAssert.IsFalse(frameControl.PowerManagement);
            ClassicAssert.IsFalse(frameControl.MoreData);
            ClassicAssert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_DataFrame()
        {
            var frameControl = new FrameControlField(0x0801);

            ClassicAssert.AreEqual(0x0, frameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameTypes.Data, frameControl.Type);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.Data, frameControl.SubType);

            ClassicAssert.IsTrue(frameControl.ToDS);
            ClassicAssert.IsFalse(frameControl.FromDS);
            ClassicAssert.IsFalse(frameControl.MoreFragments);
            ClassicAssert.IsFalse(frameControl.Retry);
            ClassicAssert.IsFalse(frameControl.PowerManagement);
            ClassicAssert.IsFalse(frameControl.MoreData);
            ClassicAssert.IsFalse(frameControl.Protected);
            ClassicAssert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_DisassociationFrame()
        {
            var frameControl = new FrameControlField(0xA01B);

            ClassicAssert.AreEqual(0x0, frameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameTypes.Management, frameControl.Type);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ManagementDisassociation, frameControl.SubType);
            ClassicAssert.IsTrue(frameControl.FromDS);
            ClassicAssert.IsTrue(frameControl.ToDS);
            ClassicAssert.IsFalse(frameControl.MoreFragments);
            ClassicAssert.IsFalse(frameControl.MoreData);
            ClassicAssert.IsFalse(frameControl.Protected);
            ClassicAssert.IsTrue(frameControl.Retry);
            ClassicAssert.IsTrue(frameControl.PowerManagement);
            ClassicAssert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_EncryptedDataFrame()
        {
            var frameControl = new FrameControlField(0x0842);

            ClassicAssert.AreEqual(0x0, frameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameTypes.Data, frameControl.Type);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.Data, frameControl.SubType);
            ClassicAssert.IsTrue(frameControl.FromDS);
            ClassicAssert.IsTrue(frameControl.Protected);

            ClassicAssert.IsFalse(frameControl.ToDS);
            ClassicAssert.IsFalse(frameControl.MoreFragments);
            ClassicAssert.IsFalse(frameControl.Retry);
            ClassicAssert.IsFalse(frameControl.PowerManagement);
            ClassicAssert.IsFalse(frameControl.MoreData);
            ClassicAssert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_SetFromDsProperty()
        {
            var frameControl = new FrameControlField { FromDS = true };

            ClassicAssert.IsTrue(frameControl.FromDS);
        }

        [Test]
        public void Test_SetMoreDataProperty()
        {
            var frameControl = new FrameControlField { MoreData = true };

            ClassicAssert.IsTrue(frameControl.MoreData);
        }

        [Test]
        public void Test_SetMoreFragmentsProperty()
        {
            var frameControl = new FrameControlField { MoreFragments = true };

            ClassicAssert.IsTrue(frameControl.MoreFragments);
        }

        [Test]
        public void Test_SetOrderProperty()
        {
            var frameControl = new FrameControlField { Order = true };

            ClassicAssert.IsTrue(frameControl.Order);
        }

        [Test]
        public void Test_SetPowerManagementProperty()
        {
            var frameControl = new FrameControlField { PowerManagement = true };

            ClassicAssert.IsTrue(frameControl.PowerManagement);
        }

        [Test]
        public void Test_SetProtocolVersionProperty()
        {
            var frameControl = new FrameControlField { ProtocolVersion = 3 };

            ClassicAssert.AreEqual(3, frameControl.ProtocolVersion);

            frameControl.ProtocolVersion = 2;
            ClassicAssert.AreEqual(2, frameControl.ProtocolVersion);

            frameControl.ProtocolVersion = 1;
            ClassicAssert.AreEqual(1, frameControl.ProtocolVersion);

            frameControl.ProtocolVersion = 0;
            ClassicAssert.AreEqual(0, frameControl.ProtocolVersion);
        }

        [Test]
        public void Test_SetProtocolVersionProperty_ValueTooLarge()
        {
            var ex = ClassicAssert.Throws<ArgumentException>(
                () => { var _ = new FrameControlField { ProtocolVersion = 4 }; }
                );

            ClassicAssert.That(ex.Message, Is.EqualTo("Invalid protocol version value. Value must be in the range 0-3."));
        }

        [Test]
        public void Test_SetRetryProperty()
        {
            var frameControl = new FrameControlField { Retry = true };

            ClassicAssert.IsTrue(frameControl.Retry);
        }

        [Test]
        public void Test_SetToDsProperty()
        {
            var frameControl = new FrameControlField { ToDS = true };

            ClassicAssert.IsTrue(frameControl.ToDS);
        }

        [Test]
        public void Test_SetTypeProperty()
        {
            var frameControl = new FrameControlField { SubType = FrameControlField.FrameSubTypes.Data };

            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.Data, frameControl.SubType);

            frameControl.SubType = FrameControlField.FrameSubTypes.ManagementAuthentication;
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ManagementAuthentication, frameControl.SubType);

            frameControl.SubType = FrameControlField.FrameSubTypes.ControlAck;
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ControlAck, frameControl.SubType);
        }

        [Test]
        public void Test_SetWepProperty()
        {
            var frameControl = new FrameControlField { Protected = true };

            ClassicAssert.IsTrue(frameControl.Protected);
        }
    }