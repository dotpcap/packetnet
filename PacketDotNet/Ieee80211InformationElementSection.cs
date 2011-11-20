using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    public class Ieee80211InformationElementSection
    {
        private const int ElementIdLength = 1;
        private const int ElementLengthLength = 1;

        public Ieee80211InformationElementSection(ByteArraySegment infoElementBuffer)
        {
            InformationElements = new List<Ieee80211InformationElement>();
            //go through the buffer extracting out information element
            int idIndex = 0;
            while (idIndex < infoElementBuffer.Length)
            {
                
                Ieee80211InformationElement.ElementId id = (Ieee80211InformationElement.ElementId)infoElementBuffer.Bytes[infoElementBuffer.Offset + idIndex];
                Byte length = infoElementBuffer.Bytes[infoElementBuffer.Offset + idIndex + ElementIdLength];

                Byte[] value = new Byte[length];
                Array.Copy(infoElementBuffer.Bytes,
                    (infoElementBuffer.Offset + idIndex + ElementIdLength + ElementLengthLength),
                    value, 0, length);

                Ieee80211InformationElement infoElement = new Ieee80211InformationElement(id, value);
                InformationElements.Add(infoElement);

                idIndex += (ElementIdLength + ElementLengthLength + infoElement.Length);
            }
        }

        public Ieee80211InformationElementSection(List<Ieee80211InformationElement> infoElements)
        {
            InformationElements = new List<Ieee80211InformationElement>(infoElements);
        }

        public List<Ieee80211InformationElement> InformationElements { get; set; }

        public int Length 
        {
            get
            {
                int length = 0;
                foreach (Ieee80211InformationElement ie in InformationElements)
                {
                    length += (ie.Length + ElementIdLength + ElementLengthLength);
                }
                return length;
            }
        }

        public Byte[] Bytes
        {
            get
            {
                Byte[] bytes = new Byte[Length];
                int index = 0;
                foreach (Ieee80211InformationElement ie in InformationElements)
                {
                    Byte[] ieBytes = ie.Bytes;
                    Array.Copy(ieBytes, 0, bytes, index, ieBytes.Length);

                    index += ieBytes.Length;
                }

                return bytes;
            }
        }
    }
}
