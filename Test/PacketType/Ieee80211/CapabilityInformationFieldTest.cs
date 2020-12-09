/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using NUnit.Framework;
using PacketDotNet.Ieee80211;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class CapabilityInformationFieldTest
    {
        [Test]
        public void Test_GetCfPollableProperty()
        {
            var capabilityField = new CapabilityInformationField(0x0004);
            Assert.IsTrue(capabilityField.CfPollable);
        }

        [Test]
        public void Test_GetCfPollRequestProperty()
        {
            var capabilityField = new CapabilityInformationField(0x0008);
            Assert.IsTrue(capabilityField.CfPollRequest);
        }

        [Test]
        public void Test_GetChannelAgilityProperty()
        {
            var capabilityField = new CapabilityInformationField(0x0080);
            Assert.IsTrue(capabilityField.ChannelAgility);
        }

        [Test]
        public void Test_GetDssOfdmProperty()
        {
            var capabilityField = new CapabilityInformationField(0x2000);
            Assert.IsTrue(capabilityField.DssOfdm);
        }

        [Test]
        public void Test_GetIsEssProperty()
        {
            var capabilityField = new CapabilityInformationField(0x0001);
            Assert.IsTrue(capabilityField.IsEss);
        }

        [Test]
        public void Test_GetIsIbssProperty()
        {
            var capabilityField = new CapabilityInformationField(0x0002);
            Assert.IsTrue(capabilityField.IsIbss);
        }

        [Test]
        public void Test_GetPbccProperty()
        {
            var capabilityField = new CapabilityInformationField(0x0040);
            Assert.IsTrue(capabilityField.Pbcc);
        }

        [Test]
        public void Test_GetPrivacyProperty()
        {
            var capabilityField = new CapabilityInformationField(0x0010);
            Assert.IsTrue(capabilityField.Privacy);
        }

        [Test]
        public void Test_GetShortPreambleProperty()
        {
            var capabilityField = new CapabilityInformationField(0x0020);
            Assert.IsTrue(capabilityField.ShortPreamble);
        }

        [Test]
        public void Test_GetShortTimeslotProperty()
        {
            var capabilityField = new CapabilityInformationField(0x0400);
            Assert.IsTrue(capabilityField.ShortTimeSlot);
        }

        [Test]
        public void Test_SetCfPollableProperty()
        {
            var capabilityField = new CapabilityInformationField { CfPollable = true };

            Assert.IsTrue(capabilityField.CfPollable);
        }

        [Test]
        public void Test_SetCfPollRequestProperty()
        {
            var capabilityField = new CapabilityInformationField { CfPollRequest = true };

            Assert.IsTrue(capabilityField.CfPollRequest);
        }

        [Test]
        public void Test_SetChannelAgilityProperty()
        {
            var capabilityField = new CapabilityInformationField { ChannelAgility = true };

            Assert.IsTrue(capabilityField.ChannelAgility);
        }

        [Test]
        public void Test_SetDssOfdmProperty()
        {
            var capabilityField = new CapabilityInformationField { DssOfdm = true };

            Assert.IsTrue(capabilityField.DssOfdm);
        }

        [Test]
        public void Test_SetIsEssProperty()
        {
            var capabilityField = new CapabilityInformationField { IsEss = true };

            Assert.IsTrue(capabilityField.IsEss);
        }

        [Test]
        public void Test_SetIsIbssProperty()
        {
            var capabilityField = new CapabilityInformationField { IsIbss = true };

            Assert.IsTrue(capabilityField.IsIbss);
        }

        [Test]
        public void Test_SetPbccProperty()
        {
            var capabilityField = new CapabilityInformationField { Pbcc = true };

            Assert.IsTrue(capabilityField.Pbcc);
        }

        [Test]
        public void Test_SetPrivacyProperty()
        {
            var capabilityField = new CapabilityInformationField { Privacy = true };

            Assert.IsTrue(capabilityField.Privacy);
        }

        [Test]
        public void Test_SetShortPreambleProperty()
        {
            var capabilityField = new CapabilityInformationField { ShortPreamble = true };

            Assert.IsTrue(capabilityField.ShortPreamble);
        }

        [Test]
        public void Test_SetShortTimeslotProperty()
        {
            var capabilityField = new CapabilityInformationField { ShortTimeSlot = true };

            Assert.IsTrue(capabilityField.ShortTimeSlot);
        }
    }
}