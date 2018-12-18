using System;
using PacketDotNet;
using System.IO;

namespace Test
{
    public class PcapPacket
    {
        public static int PACKET_HEADER_LENGTH = 16;
        public Packet packet;
        public long time;
        public byte[] headerbuffer;
        public byte[] payloadbuffer;

        public static PcapPacket GetPacket(BinaryReader readBuffer, LinkLayers LinkLayer)
        {
            PcapPacket value = new PcapPacket();

            uint timeValSecs = 0;
            uint timeValUsecs = 0;
            int inclLen = 0; /* number of octets stored in packet saved in file */
            uint origLen = 0; /* actual length of packet */

            value.headerbuffer = new byte[PACKET_HEADER_LENGTH];

            if (readBuffer.Read(value.headerbuffer, 0, PACKET_HEADER_LENGTH) == 0)
                return null;

            timeValSecs = BitConverter.ToUInt32(value.headerbuffer, 0);
            timeValUsecs = BitConverter.ToUInt32(value.headerbuffer, 4);
            inclLen = (int)BitConverter.ToUInt32(value.headerbuffer, 8);
            origLen = BitConverter.ToUInt32(value.headerbuffer, 12);

            value.time = (long)(timeValSecs * TimeSpan.TicksPerSecond + timeValUsecs * TimeSpan.TicksPerMillisecond * 1.0e-3);

            value.payloadbuffer = new byte[inclLen];

            int requestLength = inclLen;
            int totalReceived = 0;
            int retryCount = 0;

            while (requestLength > 0)
            {

                int resultLength = readBuffer.Read(value.payloadbuffer, totalReceived, requestLength);
                if (requestLength != resultLength)
                {
                    //if it the result is length 0 and we are expecting more try a couple more times
                    if (resultLength == 0)
                    {
                        if (retryCount > 2)
                            return null;
                        retryCount++;
                    }
                }
                requestLength -= resultLength;
                totalReceived += resultLength;
            }
            try
            {
                value.packet = PacketDotNet.Packet.ParsePacket(LinkLayer, value.payloadbuffer);
            }
            catch (System.ArgumentException ex)
            {
                throw new Exception("Error parsing packet at " + value.time, ex);
            }
            return value;

        }
    }
}
