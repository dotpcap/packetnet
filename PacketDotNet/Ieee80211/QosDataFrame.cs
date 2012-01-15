/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class QosDataFrame : DataFrame
        {
            private class QosDataField
            {
                public readonly static int QosControlLength = 2;

                public readonly static int QosControlPosition;

                static QosDataField()
                {
                    QosControlPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                }
            }

            public UInt16 QosControl { get; set; }

            public UInt16 QosControlBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                          header.Offset + QosDataField.QosControlPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + QosDataField.QosControlPosition);
                }
            }

            public override int FrameSize
            {
                get
                {
                    //if we are in WDS mode then there are 4 addresses (normally it is just 3)
                    int numOfAddressFields = (FrameControl.ToDS && FrameControl.FromDS) ? 4 : 3;

                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * numOfAddressFields) +
                        MacFields.SequenceControlLength +
                        QosDataField.QosControlLength);
                }
            }


            public QosDataFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                SequenceControl = new SequenceControlField (SequenceControlBytes);
                QosControl = QosControlBytes;
                ReadAddresses ();
                
                header.Length = FrameSize;
                int payloadLength = header.BytesLength - (header.Offset + header.Length) - MacFields.FrameCheckSequenceLength;
                payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes (payloadLength);
                
                //Must do this after setting header.Length and handling payload as they are used in calculating the posistion of the FCS
                FrameCheckSequence = FrameCheckSequenceBytes;
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [QosDataFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString(),
                                     SourceAddress.ToString(),
                                     BssId.ToString());
            }
        } 
    }
}
