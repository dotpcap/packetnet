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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class InformationElementTest
        {
            [Test]
            public void Test_Constructor_ValidParameters()
            {
                Byte[] value = new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 };
                InformationElement infoElement = new InformationElement(
                    InformationElement.ElementId.ChallengeText, value);

                Assert.AreEqual(InformationElement.ElementId.ChallengeText, infoElement.Id);
                Assert.AreEqual(value, infoElement.Value);
                Assert.AreEqual(5, infoElement.ValueLength);
            }

            [Test]
            [ExpectedException(typeof(ArgumentException), ExpectedMessage = "The provided value is too long. Maximum allowed length is 255 bytes.")]
            public void Test_Constructor_ValueTooLong ()
            {
                Byte[] value = new Byte[300];
                InformationElement infoElement = new InformationElement (
                    InformationElement.ElementId.CfParameterSet, value);
                
            }

            [Test]
            public void Test_Bytes()
            {
                Byte[] value = new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 };
                InformationElement infoElement = new InformationElement(
                    InformationElement.ElementId.VendorSpecific, value);

                Byte[] ieArray = infoElement.Bytes;

                Assert.AreEqual(7, ieArray.Length);
                Assert.AreEqual((Byte)InformationElement.ElementId.VendorSpecific, ieArray[0]);
                Assert.AreEqual(5, ieArray[1]);

                Byte[] actualValue = new Byte[5];
                Array.Copy(ieArray, 2, actualValue, 0, 5);
                Assert.IsTrue(value.SequenceEqual(actualValue));
            }

            [Test]
            public void Test_Equals_AreEqual()
            {
                Byte[] value = new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 };
                InformationElement infoElement = new InformationElement(
                    InformationElement.ElementId.ChallengeText, value);

                InformationElement otherInfoElement = new InformationElement(
                    InformationElement.ElementId.ChallengeText, value);

                Assert.IsTrue(infoElement.Equals(otherInfoElement));
            }

            [Test]
            public void Test_Equals_AreNotEqual()
            {
                Byte[] value = new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 };
                InformationElement infoElement = new InformationElement(
                    InformationElement.ElementId.RobustSecurityNetwork, value);

                Byte[] otherValue = new Byte[] { 0xa, 0xb, 0xc, 0xd, 0xe };
                InformationElement otherInfoElement = new InformationElement(
                    InformationElement.ElementId.ChallengeText, otherValue);

                Assert.IsFalse(infoElement.Equals(otherInfoElement));
            }
        } 
    }
}
