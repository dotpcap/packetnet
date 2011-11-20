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
        public void test1()
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
        public void test2()
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
        public void test3()
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
        public void test4()
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
        public void test5()
        {
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField(0xA327);

            Assert.AreEqual(0x0, frameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ManagementDisassociation, frameControl.Type);
            Assert.IsTrue(frameControl.FromDS);
            Assert.IsTrue(frameControl.ToDS);
            Assert.IsTrue(frameControl.MoreFragments);
            Assert.IsTrue(frameControl.MoreData);

            Assert.IsFalse(frameControl.Wep);
            Assert.IsFalse(frameControl.Retry);
            Assert.IsFalse(frameControl.PowerManagement);
            Assert.IsFalse(frameControl.Order);
        }

        [Test]
        public void test6()
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
    }
}
