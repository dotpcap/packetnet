using NUnit.Framework;
using PacketDotNet.Ieee80211;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class SequenceControlFieldTest
    {
        [Test]
        public void Test_Constructor()
        {
            var field = new SequenceControlField(0xA980);
            Assert.AreEqual(0, field.FragmentNumber);
            Assert.AreEqual(2712, field.SequenceNumber);
        }

        [Test]
        public void Test_ConstructorWithFragmentNumber()
        {
            var field = new SequenceControlField(0xA98A);
            Assert.AreEqual(10, field.FragmentNumber);
            Assert.AreEqual(2712, field.SequenceNumber);
        }

        [Test]
        public void Test_FragmentNumberProperty()
        {
            var field = new SequenceControlField(0xFF) { FragmentNumber = 10 };
            Assert.AreEqual(10, field.FragmentNumber);
        }

        [Test]
        public void Test_SequenceNumberProperty()
        {
            var field = new SequenceControlField(0xFFFF) { SequenceNumber = 2712 };
            Assert.AreEqual(2712, field.SequenceNumber);
        }
    }
}