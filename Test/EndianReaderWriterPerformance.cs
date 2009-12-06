using System;
using System.Net;
using System.IO;
using NUnit.Framework;
using MiscUtil.IO;
using MiscUtil.Conversion;

namespace Test
{
    [TestFixture]
    public class EndianReaderWriterPerformance
    {
        [Test]
        public void Performance()
        {
            byte[] bytes;
            int testRuns;
            int startIndex;
            int expectedValue;
            ByteSetupMethods.Setup(out bytes, out testRuns, out startIndex,
                                   out expectedValue);

            var memStream = new MemoryStream(bytes);
            var endianReader = new EndianBinaryReader(EndianBitConverter.Big, memStream);

            var startTime = DateTime.Now;

            for(int i = 0; i < testRuns; i++)
            {
                endianReader.Seek(startIndex, SeekOrigin.Begin);
                var actualValue = endianReader.ReadInt32();

                // NOTE: Assert.AreEqual() significantly slows, by a factor of ~6x
                //       the execution of this loop, so we perform ourself and
                //       then call Assert.AreEqual() if the comparison fails.
                //       This doesn't reduce performance by a noticable amount
                if(actualValue != expectedValue)
                {
                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, testRuns, "Test runs");

            Console.WriteLine(rate.ToString());
        }
    }
}
