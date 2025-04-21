/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet.Ieee80211;
using PacketDotNet.Utils;

namespace Test.PacketType.Ieee80211;

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

            ClassicAssert.AreEqual(7, ieArray.Length);
            ClassicAssert.AreEqual((byte) InformationElement.ElementId.VendorSpecific, ieArray[0]);
            ClassicAssert.AreEqual(5, ieArray[1]);

            var actualValue = new byte[5];
            Array.Copy(ieArray, 2, actualValue, 0, 5);
            ClassicAssert.IsTrue(value.SequenceEqual(actualValue));
        }

        [Test]
        public void Test_Constructor_BufferCorrectSize()
        {
            byte[] value = { 0x0, 0x2, 0x1, 0x2 };
            var byteArraySegment = new ByteArraySegment(value);

            var infoElement = new InformationElement(byteArraySegment);

            ClassicAssert.AreEqual(2, infoElement.ValueLength);
            ClassicAssert.AreEqual(2, infoElement.Value.Length);
        }

        [Test]
        public void Test_Constructor_BufferLongerThanElement()
        {
            //This IE will have and length of 2 but there are three bytes of data available,
            //the last one should be ignored
            byte[] value = { 0x0, 0x2, 0x1, 0x2, 0x3 };
            var byteArraySegment = new ByteArraySegment(value);

            var infoElement = new InformationElement(byteArraySegment);

            ClassicAssert.AreEqual(2, infoElement.ValueLength);
            ClassicAssert.AreEqual(2, infoElement.Value.Length);
        }

        [Test]
        public void Test_Constructor_BufferTooShort()
        {
            //This IE will have and length of 5 but only three bytes of data
            byte[] value = { 0x0, 0x5, 0x1, 0x2, 0x3 };
            var byteArraySegment = new ByteArraySegment(value);

            var infoElement = new InformationElement(byteArraySegment);

            ClassicAssert.AreEqual(3, infoElement.ValueLength);
            ClassicAssert.AreEqual(3, infoElement.Value.Length);
        }

        [Test]
        public void Test_Constructor_ValidParameters()
        {
            byte[] value = { 0x1, 0x2, 0x3, 0x4, 0x5 };
            var infoElement = new InformationElement(
                                                     InformationElement.ElementId.ChallengeText,
                                                     value);

            ClassicAssert.AreEqual(InformationElement.ElementId.ChallengeText, infoElement.Id);
            ClassicAssert.AreEqual(value, infoElement.Value);
            ClassicAssert.AreEqual(5, infoElement.ValueLength);
        }

        [Test]
        public void Test_Constructor_ValueTooLong()
        {
            var ex = ClassicAssert.Throws<ArgumentException>(
                () =>
                {
                    var value = new byte[300];
                    new InformationElement(InformationElement.ElementId.CfParameterSet,
                                           value);
                });

            ClassicAssert.That(ex.Message, Is.EqualTo("The provided value is too long. Maximum allowed length is 255 bytes."));
        }

        [Test]
        public void Test_Equals_AreEqual()
        {
            byte[] value = { 0x1, 0x2, 0x3, 0x4, 0x5 };
            var infoElement = new InformationElement(InformationElement.ElementId.ChallengeText,
                                                     value);

            var otherInfoElement = new InformationElement(InformationElement.ElementId.ChallengeText,
                                                          value);

            ClassicAssert.IsTrue(infoElement.Equals(otherInfoElement));
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

            ClassicAssert.IsFalse(infoElement.Equals(otherInfoElement));
        }
    }