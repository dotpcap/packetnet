using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PacketDotNet;

namespace Test.PacketType
{
    [TestFixture]
    class Ieee80211InformationElementTest
    {
        [Test]
        public void Test_Constructor_ValidParameters()
        {
            Byte[] value = new Byte[]{0x1, 0x2, 0x3, 0x4, 0x5};
            Ieee80211InformationElement infoElement = new Ieee80211InformationElement(
                Ieee80211InformationElement.ElementId.ChallengeText, value);

            Assert.AreEqual(Ieee80211InformationElement.ElementId.ChallengeText, infoElement.Id);
            Assert.AreEqual(value, infoElement.Value);
            Assert.AreEqual(5, infoElement.Length);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Value is too long. Maximum allowed length is 256 bytes.")]
        public void Test_Constructor_ValueTooLong()
        {
            Byte[] value = new Byte[300];
            Ieee80211InformationElement infoElement = new Ieee80211InformationElement(
                Ieee80211InformationElement.ElementId.CfParameterSet, value);
        }

        [Test]
        public void Test_Bytes()
        {
            Byte[] value = new Byte[]{0x1, 0x2, 0x3, 0x4, 0x5};
            Ieee80211InformationElement infoElement = new Ieee80211InformationElement(
                Ieee80211InformationElement.ElementId.VendorSpecific, value);

            Byte[] ieArray = infoElement.Bytes;
            
            Assert.AreEqual(7, ieArray.Length);
            Assert.AreEqual((Byte)Ieee80211InformationElement.ElementId.VendorSpecific, ieArray[0]);
            Assert.AreEqual(5, ieArray[1]);

            Byte[] actualValue = new Byte[5];
            Array.Copy(ieArray, 2, actualValue, 0, 5);
            Assert.IsTrue(value.SequenceEqual(actualValue));
        }

        [Test]
        public void Test_Equals_AreEqual()
        {
            Byte[] value = new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 };
            Ieee80211InformationElement infoElement = new Ieee80211InformationElement(
                Ieee80211InformationElement.ElementId.ChallengeText, value);

            Ieee80211InformationElement otherInfoElement = new Ieee80211InformationElement(
                Ieee80211InformationElement.ElementId.ChallengeText, value);

            Assert.IsTrue(infoElement.Equals(otherInfoElement));
        }

        [Test]
        public void Test_Equals_AreNotEqual()
        {
            Byte[] value = new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 };
            Ieee80211InformationElement infoElement = new Ieee80211InformationElement(
                Ieee80211InformationElement.ElementId.RobustSecurityNetwork, value);

            Byte[] otherValue = new Byte[] { 0xa, 0xb, 0xc, 0xd, 0xe };
            Ieee80211InformationElement otherInfoElement = new Ieee80211InformationElement(
                Ieee80211InformationElement.ElementId.ChallengeText, otherValue);

            Assert.IsFalse(infoElement.Equals(otherInfoElement));
        }
    }
}
