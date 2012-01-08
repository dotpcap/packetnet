using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class InformationElement
        {     

            public readonly static int ElementIdLength = 1;
            public readonly static int ElementLengthLength = 1;
            public readonly static int ElementIdPosition = 0;
            public readonly static int ElementLengthPosition;
            public readonly static int ElementValuePosition;

            static InformationElement ()
            {
                ElementLengthPosition = ElementIdPosition + ElementIdLength;
                ElementValuePosition = ElementLengthPosition + ElementLengthLength;
            }

            
            public enum ElementId
            {
                ServiceSetIdentity = 0x00,
                SupportedRates = 0x01,
                FhParamterSet = 0x02,
                DsParameterSet = 0x03,
                CfParameterSet = 0x04,
                TrafficIndicationMap = 0x05,
                IbssParameterSet = 0x06,
                Country = 0x07,
                HoppingParametersPattern = 0x08,
                HoppingPatternTable = 0x09,
                Request = 0x0A,
                ChallengeText = 0x10,
                PowerContstraint = 0x20,
                PowerCapability = 0x21,
                TransmitPowerControlRequest = 0x22,
                TransmitPowerControlReport = 0x23,
                SupportedChannels = 0x24,
                ChannelSwitchAnnouncement = 0x25,
                MeasurementRequest = 0x26,
                MeasurementReport = 0x27,
                Quiet = 0x28,
                IbssDfs = 0x29,
                ErpInformation = 0x2A,
                HighThroughputCapabilities = 0x2d,
                ErpInformation2 = 0x2F,
                RobustSecurityNetwork = 0x30,
                ExtendedSupportedRates = 0x32,
                HighThroughputInformation = 0x3d,
                WifiProtectedAccess = 0xD3,
                VendorSpecific = 0xDD
            }
            
            private ByteArraySegment bytes;
            
            public InformationElement (ByteArraySegment bas)
            {
                bytes = bas;
            }
            
            public InformationElement (ElementId id, Byte[] value)
            {
                var ie = new Byte[ElementIdLength + ElementLengthLength + value.Length];
                bytes = new ByteArraySegment (ie);
                Id = id;
                Value = value;
            }
   
            public ElementId Id
            { 
                get
                {
                    return (ElementId)bytes.Bytes [bytes.Offset + ElementIdPosition];
                }
                set
                {
                    bytes.Bytes [bytes.Offset + ElementIdPosition] = (byte)value;
                }
            }

            public byte ValueLength
            {
                get
                {
                    return bytes.Bytes [bytes.Offset + ElementLengthPosition];
                }
                //no set Length method as we dont want to allow a mismatch between
                //the length field and the actual length of the value
            }
            
            public byte ElementLength
            {
                get
                {
                    return (byte)(ElementIdLength + ElementLengthLength + ValueLength);
                }
                //no set Length method as we dont want to allow a mismatch between
                //the length field and the actual length of the value
            }

            public Byte[] Value
            {
                get
                {
                    var valueArray = new Byte[ValueLength];
                    Array.Copy (bytes.Bytes,
                        bytes.Offset + ElementValuePosition,
                        valueArray, 0, ValueLength);
                    return valueArray;
                }
                
                set
                {
                    if (value.Length > byte.MaxValue)
                    {
                        throw new ArgumentException ("The provided value is too long. Maximum allowed length is 255 bytes.");
                    }
                    //Decide if the current ByteArraySegement is big enough to hold the new info element
                    int newIeLength = ElementIdLength + ElementLengthLength + value.Length;
                    if (bytes.Length < newIeLength)
                    {
                        var newIe = new Byte[newIeLength];
                        newIe [ElementIdPosition] = bytes.Bytes [bytes.Offset + ElementIdPosition];
                        bytes = new ByteArraySegment (newIe);
                    }
                    
                    Array.Copy (value, 0, bytes.Bytes, bytes.Offset + ElementValuePosition, value.Length);
                    bytes.Length = newIeLength;
                    bytes.Bytes [bytes.Offset + ElementLengthPosition] = (byte)value.Length;
                    
                }
            }
            
            public Byte[] Bytes
            {
                get
                {
                    return bytes.ActualBytes();
                }
            }
            
            // override object.Equals
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                InformationElement ie = obj as InformationElement;
                return ((Id == ie.Id) && (Value.SequenceEqual(ie.Value)));
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                return Id.GetHashCode() ^ Value.GetHashCode();
            }
            
        } 
    }
}
