using System;
using NUnit.Framework;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class SequenceControlFieldTest
        {
            [Test]
            public void Test_Construcutor()
            {
                SequenceControlField field = new SequenceControlField(0xA980);
                Assert.AreEqual(0, field.FragmentNumber);
                Assert.AreEqual(2712, field.SequenceNumber);
            }
            
            [Test]
            public void Test_ConstrucutorWithFragmentNumber()
            {
                SequenceControlField field = new SequenceControlField(0xA98A);
                Assert.AreEqual(10, field.FragmentNumber);
                Assert.AreEqual(2712, field.SequenceNumber);
            }
            
            [Test]
            public void Test_SequenceNumberProperty()
            {
                SequenceControlField field = new SequenceControlField(0xFFFF);
                field.SequenceNumber = 2712;
                Assert.AreEqual(2712, field.SequenceNumber);
            }
            
            [Test]
            public void Test_FragmentNumberProperty()
            {
                SequenceControlField field = new SequenceControlField(0xFF);
                field.FragmentNumber = 10;
                Assert.AreEqual(10, field.FragmentNumber);
            }
        }
    }
}

