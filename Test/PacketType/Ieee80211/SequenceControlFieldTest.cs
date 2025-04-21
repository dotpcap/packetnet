using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet.Ieee80211;

namespace Test.PacketType.Ieee80211;

    [TestFixture]
    public class SequenceControlFieldTest
    {
        [Test]
        public void Test_Constructor()
        {
            var field = new SequenceControlField(0xA980);
            ClassicAssert.AreEqual(0, field.FragmentNumber);
            ClassicAssert.AreEqual(2712, field.SequenceNumber);
        }

        [Test]
        public void Test_ConstructorWithFragmentNumber()
        {
            var field = new SequenceControlField(0xA98A);
            ClassicAssert.AreEqual(10, field.FragmentNumber);
            ClassicAssert.AreEqual(2712, field.SequenceNumber);
        }

        [Test]
        public void Test_FragmentNumberProperty()
        {
            var field = new SequenceControlField(0xFF) { FragmentNumber = 10 };
            ClassicAssert.AreEqual(10, field.FragmentNumber);
        }

        [Test]
        public void Test_SequenceNumberProperty()
        {
            var field = new SequenceControlField(0xFFFF) { SequenceNumber = 2712 };
            ClassicAssert.AreEqual(2712, field.SequenceNumber);
        }
    }