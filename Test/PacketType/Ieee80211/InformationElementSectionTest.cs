using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Utils;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class InformationElementSectionTest
        {
            [Test]
            public void Test_Constructor_EmptyByteArray()
            {
                ByteArraySegment bas = new ByteArraySegment(new Byte[0]);
                InformationElementSection ieSection = new InformationElementSection(bas);

                Assert.AreEqual(0, ieSection.InformationElements.Count);
                Assert.AreEqual(0, ieSection.Length);
            }

            [Test]
            public void Test_Constructor_MultipleInfoElementByteArray()
            {
                //create a dummy info element section for the test. The example here uses 
                //two arbitrary field types (0xD3 & 0xDD). The important information for the test is the length
                //field which is the second byte on each row (0x5 & 0x4). The actual values are meaningless.
                Byte[] ieBytes = new Byte[] { 0xD3, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05,
                                              0xDD, 0x04, 0xFF, 0xFE, 0xFD, 0xFC };
                ByteArraySegment bas = new ByteArraySegment(ieBytes);

                InformationElementSection ieSection = new InformationElementSection(bas);
                Assert.AreEqual(2, ieSection.InformationElements.Count);
                Assert.AreEqual(13, ieSection.Length);

                Assert.AreEqual(InformationElement.ElementId.WifiProtectedAccess, ieSection.InformationElements[0].Id);
                Assert.AreEqual(5, ieSection.InformationElements[0].Length);
                Assert.IsTrue(ieSection.InformationElements[0].Value.SequenceEqual(new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 }));

                Assert.AreEqual(InformationElement.ElementId.VendorSpecific, ieSection.InformationElements[1].Id);
                Assert.AreEqual(4, ieSection.InformationElements[1].Length);
                Assert.IsTrue(ieSection.InformationElements[1].Value.SequenceEqual(new Byte[] { 0xFF, 0xFE, 0xFD, 0xFC }));
            }

            [Test]
            public void Test_Constructor_EmptyList()
            {
                List<InformationElement> ieList = new List<InformationElement>();
                InformationElementSection ieSection = new InformationElementSection(ieList);

                Assert.AreEqual(0, ieSection.InformationElements.Count);
                Assert.AreEqual(0, ieSection.Length);
            }

            [Test]
            public void Test_Constructor_MultipleInfoElementList()
            {
                InformationElement ie1 = new InformationElement(
                    InformationElement.ElementId.CfParameterSet, new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 });

                InformationElement ie2 = new InformationElement(
                    InformationElement.ElementId.ChallengeText, new Byte[] { 0xFF, 0xFE, 0xFD, 0xFC });

                List<InformationElement> ieList = new List<InformationElement>() { ie1, ie2 };

                InformationElementSection ieSection = new InformationElementSection(ieList);

                Assert.AreEqual(2, ieSection.InformationElements.Count);
                Assert.AreEqual(13, ieSection.Length);

                Assert.AreEqual(InformationElement.ElementId.CfParameterSet, ieSection.InformationElements[0].Id);
                Assert.AreEqual(ie1.Length, ieSection.InformationElements[0].Length);
                Assert.IsTrue(ieSection.InformationElements[0].Value.SequenceEqual(ie1.Value));

                Assert.AreEqual(InformationElement.ElementId.ChallengeText, ieSection.InformationElements[1].Id);
                Assert.AreEqual(ie2.Length, ieSection.InformationElements[1].Length);
                Assert.IsTrue(ieSection.InformationElements[1].Value.SequenceEqual(ie2.Value));
            }

            [Test]
            public void Test_Bytes_EmptySection()
            {
                ByteArraySegment bas = new ByteArraySegment(new Byte[0]);
                InformationElementSection ieSection = new InformationElementSection(bas);

                Assert.AreEqual(0, ieSection.Bytes.Length);
            }

            [Test]
            public void Test_Bytes_MultipleInfoElements()
            {
                InformationElement ie1 = new InformationElement(
                    InformationElement.ElementId.WifiProtectedAccess, new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 });

                InformationElement ie2 = new InformationElement(
                    InformationElement.ElementId.VendorSpecific, new Byte[] { 0xFF, 0xFE, 0xFD, 0xFC });

                List<InformationElement> ieList = new List<InformationElement>() { ie1, ie2 };
                InformationElementSection ieSection = new InformationElementSection(ieList);

                Byte[] expectedBytes = new Byte[] { 0xD3, 0x5, 0x1, 0x2, 0x3, 0x4, 0x5, 0xDD, 0x4, 0xFF, 0xFE, 0xFD, 0xFC };

                Byte[] b = ieSection.Bytes;

                Assert.AreEqual(0xD3, b[0]);
                Assert.AreEqual(0x5, b[1]);
                Assert.AreEqual(0x1, b[2]);
                Assert.AreEqual(0x2, b[3]);
                Assert.AreEqual(0x3, b[4]);
                Assert.AreEqual(0x4, b[5]);
                Assert.AreEqual(0x5, b[6]);
                Assert.AreEqual(0xDD, b[7]);
                Assert.AreEqual(0x4, b[8]);
                Assert.AreEqual(0xFF, b[9]);
                Assert.AreEqual(0xFE, b[10]);
                Assert.AreEqual(0xFD, b[11]);
                Assert.AreEqual(0xFC, b[12]);

                Assert.IsTrue(ieSection.Bytes.SequenceEqual(expectedBytes));
            }
        }
    }
}
