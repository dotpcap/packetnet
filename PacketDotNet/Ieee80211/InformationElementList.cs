using System;
using System.Linq;
using System.Collections.Generic;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class InformationElementList : List<InformationElement>
        {          
            public InformationElementList ()
            {
             
            }
            
            public InformationElementList (InformationElementList list)
               :base(list)
            {
             
            }
            
            public InformationElementList (ByteArraySegment bas)
            {
                int index = 0;
                while (index < bas.Length)
                {
                    Byte valueLength = bas.Bytes [bas.Offset + index + InformationElement.ElementLengthPosition];
                    var ieLength = InformationElement.ElementIdLength + InformationElement.ElementLengthLength + valueLength;
                    this.Add (new InformationElement (new ByteArraySegment (bas.Bytes, bas.Offset + index, ieLength)));

                    index += ieLength;
                }
            }
                       
            public int Length
            {
                get
                {
                    int length = 0;
                    foreach (InformationElement ie in this)
                    {
                        length += ie.ElementLength;
                    }
                    return length;
                }
            }
            
            public Byte[] Bytes
            {
                get
                {
                    var bytes = new Byte[Length];
                    int index = 0;
                    foreach (var ie in this)
                    {
                        var ieBytes = ie.Bytes;
                        Array.Copy (ieBytes, 0, bytes, index, ieBytes.Length);

                        index += ieBytes.Length;
                    }

                    return bytes;
                }
            }
            
            public InformationElement[] FindById (InformationElement.ElementId id)
            {
                return (from ie in this where ie.Id == id select ie).ToArray ();
            }
            
            public InformationElement FindFirstById (InformationElement.ElementId id)
            {
                return (from ie in this where ie.Id == id select ie).FirstOrDefault ();
            }
            
        }
    }
}

