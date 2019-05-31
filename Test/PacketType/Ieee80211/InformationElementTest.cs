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
using System.Linq;
using NUnit.Framework;
using PacketDotNet.Ieee80211;
using PacketDotNet.Utils;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class InformationElementTest
    {
        [Test]
        public void Test_Bytes()
        {
            byte[] value = { 0x1, 0x2, 0x3, 0x4, 0x5 };
            var infoElement = new InformationElement(
                                                     InformationElement.ElementId.VendorSpecific,
                                                     value);

            var ieArray = infoElement.Bytes;

            Assert.AreEqual(7, ieArray.Length);
            Assert.AreEqual((byte) InformationElement.ElementId.VendorSpecific, ieArray[0]);
            Assert.AreEqual(5, ieArray[1]);

            var actualValue = new byte[5];
            Array.Copy(ieArray, 2, actualValue, 0, 5);
            Assert.IsTrue(value.SequenceEqual(actualValue));
        }

        [Test]
        public void Test_Constructor_BufferCorrectSize()
        {
            byte[] value = { 0x0, 0x2, 0x1, 0x2 };
            var byteArraySegment = new ByteArraySegment(value);

            var infoElement = new InformationElement(byteArraySegment);

            Assert.AreEqual(2, infoElement.ValueLength);
            Assert.AreEqual(2, infoElement.Value.Length);
        }

        [Test]
        public void Test_Constructor_BufferLongerThanElement()
        {
            //This IE will have and length of 2 but there are three bytes of data available,
            //the last one should be ignored
            byte[] value = { 0x0, 0x2, 0x1, 0x2, 0x3 };
            var byteArraySegment = new ByteArraySegment(value);

            var infoElement = new InformationElement(byteArraySegment);

            Assert.AreEqual(2, infoElement.ValueLength);
            Assert.AreEqual(2, infoElement.Value.Length);
        }

        [Test]
        public void Test_Constructor_BufferTooShort()
        {
            //This IE will have and length of 5 but only three bytes of data
            byte[] value = { 0x0, 0x5, 0x1, 0x2, 0x3 };
            var byteArraySegment = new ByteArraySegment(value);

            var infoElement = new InformationElement(byteArraySegment);

            Assert.AreEqual(3, infoElement.ValueLength);
            Assert.AreEqual(3, infoElement.Value.Length);
        }

        [Test]
        public void Test_Constructor_ValidParameters()
        {
            byte[] value = { 0x1, 0x2, 0x3, 0x4, 0x5 };
            var infoElement = new InformationElement(
                                                     InformationElement.ElementId.ChallengeText,
                                                     value);

            Assert.AreEqual(InformationElement.ElementId.ChallengeText, infoElement.Id);
            Assert.AreEqual(value, infoElement.Value);
            Assert.AreEqual(5, infoElement.ValueLength);
        }

        [Test]
        public void Test_Constructor_ValueTooLong()
        {
            var ex = Assert.Throws<ArgumentException>(
                () =>
                {
                    var value = new byte[300];
                    new InformationElement(InformationElement.ElementId.CfParameterSet,
                                           value);
                });

            Assert.That(ex.Message, Is.EqualTo("The provided value is too long. Maximum allowed length is 255 bytes."));
        }

        [Test]
        public void Test_Equals_AreEqual()
        {
            byte[] value = { 0x1, 0x2, 0x3, 0x4, 0x5 };
            var infoElement = new InformationElement(InformationElement.ElementId.ChallengeText,
                                                     value);

            var otherInfoElement = new InformationElement(InformationElement.ElementId.ChallengeText,
                                                          value);

            Assert.IsTrue(infoElement.Equals(otherInfoElement));
        }

        [Test]
        public void Test_Equals_AreNotEqual()
        {
            byte[] value = { 0x1, 0x2, 0x3, 0x4, 0x5 };
            var infoElement = new InformationElement(InformationElement.ElementId.RobustSecurityNetwork,
                                                     value);

            byte[] otherValue = { 0xa, 0xb, 0xc, 0xd, 0xe };
            var otherInfoElement = new InformationElement(InformationElement.ElementId.ChallengeText,
                                                          otherValue);

            Assert.IsFalse(infoElement.Equals(otherInfoElement));
        }
    }
}