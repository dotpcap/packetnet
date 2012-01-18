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

