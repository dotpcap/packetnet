using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PacketDotNet;

namespace Test.PacketType
{
    [TestFixture]
    class Ieee80211FrameControlFieldTest
    {
        [Test]
        public void Test_Constructor_EncryptedDataFrame()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField(0x0842);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.Data, frameControl.Type);
            Assert.IsTrue(frameControl.FromDS);
            Assert.IsTrue(frameControl.Wep);

            Assert.IsFalse(frameControl.ToDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsFalse(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_BeaconFrame()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField(0x8000);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ManagementBeacon, frameControl.Type);
            Assert.IsFalse(frameControl.FromDS);
            Assert.IsFalse(frameControl.Wep);
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
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField(0xC400);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ControlCTS, frameControl.Type);
            Assert.IsFalse(frameControl.FromDS);
            Assert.IsFalse(frameControl.Wep);
            Assert.IsFalse(frameControl.ToDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsFalse(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_AckFrame()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField(0xD410);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ControlACK, frameControl.Type);
            Assert.IsFalse(frameControl.FromDS);
            Assert.IsFalse(frameControl.Wep);
            Assert.IsFalse(frameControl.ToDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsTrue(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_DisassociationFrame()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField(0xA01B);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ManagementDisassociation, frameControl.Type);
            Assert.IsTrue(frameControl.FromDS);
            Assert.IsTrue(frameControl.ToDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Wep);
            Assert.IsTrue(frameControl.Retry);
            Assert.IsTrue(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_Constructor_DataFrame()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField(0x0801);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.Data, frameControl.Type);
            
            Assert.IsTrue(frameControl.ToDS);
            Assert.IsFalse(frameControl.FromDS);
            Assert.IsFalse(frameControl.MoreFragments);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsFalse(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.MoreData);
            Assert.IsFalse(frameControl.Wep);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void Test_SetTypeProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();

            frameControl.Type = Ieee80211FrameControlField.FrameTypes.Data;
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.Data, frameControl.Type);

            frameControl.Type = Ieee80211FrameControlField.FrameTypes.ManagementAuthentication;
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ManagementAuthentication, frameControl.Type);

            frameControl.Type = Ieee80211FrameControlField.FrameTypes.ControlACK;
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ControlACK, frameControl.Type);
        }

        [Test]
        public void Test_SetProtocolVersionProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();

            frameControl.ProtocolVersion = 3;
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
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();

            frameControl.ProtocolVersion = 4;
        }

        [Test]
        public void Test_SetToDsProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();
            frameControl.ToDS = true;

            Assert.IsTrue(frameControl.ToDS);
        }

        [Test]
        public void Test_SetFromDsProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();
            frameControl.FromDS = true;

            Assert.IsTrue(frameControl.FromDS);
        }

        [Test]
        public void Test_SetMoreFragmentsProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();
            frameControl.MoreFragments = true;

            Assert.IsTrue(frameControl.MoreFragments);
        }

        [Test]
        public void Test_SetRetryProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();
            frameControl.Retry = true;

            Assert.IsTrue(frameControl.Retry);
        }

        [Test]
        public void Test_SetPowerManagementProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();
            frameControl.PowerManagement = true;

            Assert.IsTrue(frameControl.PowerManagement);
        }

        [Test]
        public void Test_SetMoreDataProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();
            frameControl.MoreData = true;

            Assert.IsTrue(frameControl.MoreData);
        }

        [Test]
        public void Test_SetWepProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();
            frameControl.Wep = true;

            Assert.IsTrue(frameControl.Wep);
        }

        [Test]
        public void Test_SetOrderProperty()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();
            frameControl.Order = true;

            Assert.IsTrue(frameControl.Order);
        }

        
    }
}
