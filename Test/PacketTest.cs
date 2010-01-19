using System;
using NUnit.Framework;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class PacketTest
    {
        [Test]
        public void TestSettingPayloadData()
        {
            byte[] data = new byte[10];
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }

            // NOTE: we use TcpPacket because it has a simple constructor. We can't
            //       create a Packet() instance because Packet is an abstract class
            var p = new TcpPacket(10, 10);
            p.PayloadData = data;
        }
    }
}
