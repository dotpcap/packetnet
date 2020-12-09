/*
This file is part of PacketDotNet.

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
    public class BlockAcknowledgmentControlFieldTest
    {
        [Test]
        public void Test_SetCompressedBitmapProperty()
        {
            var blockAckControl = new BlockAcknowledgmentControlField { CompressedBitmap = true };

            Assert.IsTrue(blockAckControl.CompressedBitmap);
        }

        [Test]
        public void Test_SetMultiTidProperty()
        {
            var blockAckControl = new BlockAcknowledgmentControlField { MultiTid = true };

            Assert.IsTrue(blockAckControl.MultiTid);
        }

        [Test]
        public void Test_SetPolicyProperty()
        {
            var blockAckControl = new BlockAcknowledgmentControlField { Policy = BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed };

            Assert.AreEqual(BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed, blockAckControl.Policy);

            blockAckControl.Policy = BlockAcknowledgmentControlField.AcknowledgementPolicy.Immediate;
            Assert.AreEqual(BlockAcknowledgmentControlField.AcknowledgementPolicy.Immediate, blockAckControl.Policy);
        }

        [Test]
        public void Test_SetTidProperty()
        {
            var blockAckControl = new BlockAcknowledgmentControlField { Tid = 0xF };

            Assert.AreEqual(0xF, blockAckControl.Tid);
        }
    }
}