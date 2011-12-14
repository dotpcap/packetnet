using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PacketDotNet.Ieee80211;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class CapabilityInformationFieldTest
    {
        
        

        [Test]
        public void Test_SetIsEssProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.IsEss = true;

            Assert.IsTrue(capabilityField.IsEss);
        }

        [Test]
        public void Test_GetIsEssProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x0001);
            Assert.IsTrue(capabilityField.IsEss);
        }

        [Test]
        public void Test_SetIsIbssProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.IsIbss = true;

            Assert.IsTrue(capabilityField.IsIbss);
        }

        [Test]
        public void Test_GetIsIbssProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x0002);
            Assert.IsTrue(capabilityField.IsIbss);
        }

        [Test]
        public void Test_SetCfPollableProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.CfPollable = true;

            Assert.IsTrue(capabilityField.CfPollable);
        }

        [Test]
        public void Test_GetCfPollableProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x0004);
            Assert.IsTrue(capabilityField.CfPollable);
        }

        [Test]
        public void Test_SetCfPollRequestProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.CfPollRequest = true;

            Assert.IsTrue(capabilityField.CfPollRequest);
        }

        [Test]
        public void Test_GetCfPollRequestProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x0008);
            Assert.IsTrue(capabilityField.CfPollRequest);
        }


        [Test]
        public void Test_SetPrivacyProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.Privacy = true;

            Assert.IsTrue(capabilityField.Privacy);
        }

        [Test]
        public void Test_GetPrivacyProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x0010);
            Assert.IsTrue(capabilityField.Privacy);
        }

        [Test]
        public void Test_SetShortPreambleProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.ShortPreamble = true;

            Assert.IsTrue(capabilityField.ShortPreamble);
        }

        [Test]
        public void Test_GetShortPreambleProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x0020);
            Assert.IsTrue(capabilityField.ShortPreamble);
        }

        [Test]
        public void Test_SetPbccProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.Pbcc = true;

            Assert.IsTrue(capabilityField.Pbcc);
        }

        [Test]
        public void Test_GetPbccProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x0040);
            Assert.IsTrue(capabilityField.Pbcc);
        }

        [Test]
        public void Test_SetChannelAgilityProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.ChannelAgility = true;

            Assert.IsTrue(capabilityField.ChannelAgility);
        }

        [Test]
        public void Test_GetChannelAgilityProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x0080);
            Assert.IsTrue(capabilityField.ChannelAgility);
        }

        [Test]
        public void Test_SetShortTimeslotProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.ShortTimeSlot = true;

            Assert.IsTrue(capabilityField.ShortTimeSlot);
        }

        [Test]
        public void Test_GetShortTimeslotProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x0400);
            Assert.IsTrue(capabilityField.ShortTimeSlot);
        }

        [Test]
        public void Test_SetDssOfdmProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField();
            capabilityField.DssOfdm = true;

            Assert.IsTrue(capabilityField.DssOfdm);
        }

        [Test]
        public void Test_GetDssOfdmProperty()
        {
            CapabilityInformationField capabilityField = new CapabilityInformationField(0x2000);
            Assert.IsTrue(capabilityField.DssOfdm);
        }
    }
}
