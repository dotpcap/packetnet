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
            //store the length so we can get it latter without have to calculate it
            Length = infoElementBuffer.Length;

            InformationElements = new List<Ieee80211InformationElement>();
            //go through the buffer extracting out information element
            int idIndex = 0;
            while (idIndex < infoElementBuffer.Length)
            {
                Ieee80211InformationElement infoElement = new Ieee80211InformationElement();
                infoElement.Id = (Ieee80211InformationElement.ElementId)infoElementBuffer.Bytes[infoElementBuffer.Offset + idIndex];
                infoElement.Length = infoElementBuffer.Bytes[infoElementBuffer.Offset + idIndex + ElementIdLength];

                infoElement.Value = new byte[infoElement.Length];
                Array.Copy(infoElementBuffer.Bytes,
                    (infoElementBuffer.Offset + idIndex + ElementIdLength + ElementLengthLength),
                    infoElement.Value, 0, infoElement.Length);

                InformationElements.Add(infoElement);

                idIndex += (ElementIdLength + ElementLengthLength + infoElement.Length);
            }
        }

        public List<Ieee80211InformationElement> InformationElements { get; set; }

        public int Length { get; set; }
    }
}
