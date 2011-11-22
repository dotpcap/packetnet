using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class InformationElementSection
        {
            private const int ElementIdLength = 1;
            private const int ElementLengthLength = 1;

            public InformationElementSection(ByteArraySegment infoElementBuffer)
            {
                InformationElements = new List<InformationElement>();
                //go through the buffer extracting out information element
                int idIndex = 0;
                while (idIndex < infoElementBuffer.Length)
                {

                    InformationElement.ElementId id = (InformationElement.ElementId)infoElementBuffer.Bytes[infoElementBuffer.Offset + idIndex];
                    Byte length = infoElementBuffer.Bytes[infoElementBuffer.Offset + idIndex + ElementIdLength];

                    Byte[] value = new Byte[length];
                    Array.Copy(infoElementBuffer.Bytes,
                        (infoElementBuffer.Offset + idIndex + ElementIdLength + ElementLengthLength),
                        value, 0, length);

                    InformationElement infoElement = new InformationElement(id, value);
                    InformationElements.Add(infoElement);

                    idIndex += (ElementIdLength + ElementLengthLength + infoElement.Length);
                }
            }

            public InformationElementSection(List<InformationElement> infoElements)
            {
                InformationElements = new List<InformationElement>(infoElements);
            }

            public List<InformationElement> InformationElements { get; set; }

            public int Length
            {
                get
                {
                    int length = 0;
                    foreach (InformationElement ie in InformationElements)
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
                    foreach (InformationElement ie in InformationElements)
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
}
