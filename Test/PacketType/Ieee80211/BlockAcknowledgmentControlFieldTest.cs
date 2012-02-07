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

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class BlockAcknowledgmentControlFieldTest
        {
            [Test]
            public void Test_SetPolicyProperty ()
            {
                BlockAcknowledgmentControlField blockAckControl = new BlockAcknowledgmentControlField ();
                
                blockAckControl.Policy = BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed;
                Assert.AreEqual (BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed, blockAckControl.Policy);
                
                blockAckControl.Policy = BlockAcknowledgmentControlField.AcknowledgementPolicy.Immediate;
                Assert.AreEqual (BlockAcknowledgmentControlField.AcknowledgementPolicy.Immediate, blockAckControl.Policy);
                
            }
            
            [Test]
            public void Test_SetMultiTidProperty ()
            {
                BlockAcknowledgmentControlField blockAckControl = new BlockAcknowledgmentControlField ();
                
                blockAckControl.MultiTid = true;
                Assert.IsTrue(blockAckControl.MultiTid);
            }
            
            [Test]
            public void Test_SetCompressedBitmapProperty ()
            {
                BlockAcknowledgmentControlField blockAckControl = new BlockAcknowledgmentControlField ();
                
                blockAckControl.CompressedBitmap = true;
                Assert.IsTrue (blockAckControl.CompressedBitmap);
            }
            
            [Test]
            public void Test_SetTidProperty ()
            {
                BlockAcknowledgmentControlField blockAckControl = new BlockAcknowledgmentControlField ();
                
                blockAckControl.Tid = 0xF;
                Assert.AreEqual (0xF, blockAckControl.Tid);
            }
        }
    }
}

