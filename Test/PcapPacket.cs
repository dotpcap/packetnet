using System;
using PacketDotNet;
using System.IO;

namespace Test
{
    public class PcapPacket
    {
        static int PACKET_HEADER_LENGTH = 16;
        public Packet Packet { get; private set; }

        public static PcapPacket GetPacket(BinaryReader readBuffer, LinkLayers LinkLayer)
        {
            PcapPacket value = new PcapPacket();
            byte[] headerbuffer;
            byte[] payloadbuffer;
            long time;

            headerbuffer = new byte[PACKET_HEADER_LENGTH];

            if (readBuffer.Read(headerbuffer, 0, PACKET_HEADER_LENGTH) == 0)
                return null;

            uint timeValSecs = BitConverter.ToUInt32(headerbuffer, 0);
            uint timeValUsecs = BitConverter.ToUInt32(headerbuffer, 4);
            // number of octets stored in packet saved in file 
            int inclLen = (int)BitConverter.ToUInt32(headerbuffer, 8);
            // actual length of packet 
            uint origLen = BitConverter.ToUInt32(headerbuffer, 12);

            time = (long)(timeValSecs * TimeSpan.TicksPerSecond + timeValUsecs * TimeSpan.TicksPerMillisecond * 1.0e-3);

            payloadbuffer = new byte[inclLen];

            int requestLength = inclLen;
            int totalReceived = 0;
            int retryCount = 0;

            while (requestLength > 0)
            {
                int resultLength = readBuffer.Read(payloadbuffer, totalReceived, requestLength);
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
                value.Packet = PacketDotNet.Packet.ParsePacket(LinkLayer, payloadbuffer);
            }
            catch (System.ArgumentException ex)
            {
                throw new Exception("Error parsing packet at " + time, ex);
            }
            return value;
        }
    }
}
