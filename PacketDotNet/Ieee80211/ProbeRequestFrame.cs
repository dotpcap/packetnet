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
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using PacketDotNet.Utils;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Probe request frames are used by stations to scan the area for existing networks.
    /// </summary>
    public class ProbeRequestFrame : ManagementFrame
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public ProbeRequestFrame(ByteArraySegment bas)
        {
            this.HeaderByteArraySegment = new ByteArraySegment(bas);

            this.FrameControl = new FrameControlField(this.FrameControlBytes);
            this.Duration = new DurationField(this.DurationBytes);
            this.DestinationAddress = this.GetAddress(0);
            this.SourceAddress = this.GetAddress(1);
            this.BssId = this.GetAddress(2);
            this.SequenceControl = new SequenceControlField(this.SequenceControlBytes);

            if (bas.Length > ProbeRequestFields.InformationElement1Position)
            {
                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                    (bas.Offset + ProbeRequestFields.InformationElement1Position),
                    (bas.Length - ProbeRequestFields.InformationElement1Position));

                this.InformationElements = new InformationElementList(infoElementsSegment);
            }
            else
            {
                this.InformationElements = new InformationElementList();
            }

            //cant set length until after we have handled the information elements
            //as they vary in length
            this.HeaderByteArraySegment.Length = this.FrameSize;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.ProbeRequestFrame" /> class.
        /// </summary>
        /// <param name='SourceAddress'>
        ///     Source address.
        /// </param>
        /// <param name='DestinationAddress'>
        ///     Destination address.
        /// </param>
        /// <param name='BssId'>
        ///     Bss identifier (Mac Address of the Access Point).
        /// </param>
        /// <param name='InformationElements'>
        ///     Information elements.
        /// </param>
        public ProbeRequestFrame(PhysicalAddress SourceAddress,
            PhysicalAddress DestinationAddress,
            PhysicalAddress BssId,
            InformationElementList InformationElements)
        {
            this.FrameControl = new FrameControlField();
            this.Duration = new DurationField();
            this.DestinationAddress = DestinationAddress;
            this.SourceAddress = SourceAddress;
            this.BssId = BssId;
            this.SequenceControl = new SequenceControlField();
            this.InformationElements = new InformationElementList(InformationElements);

            this.FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementProbeRequest;
        }

        /// <summary>
        ///     Length of the frame header.
        ///     This does not include the FCS, it represents only the header bytes that would
        ///     would preceed any payload.
        /// </summary>
        public override Int32 FrameSize => (MacFields.FrameControlLength +
                                            MacFields.DurationIDLength +
                                            (MacFields.AddressLength * 3) +
                                            MacFields.SequenceControlLength + this.InformationElements.Length);

        /// <summary>
        ///     Gets or sets the information elements included in the frame.
        /// </summary>
        /// <value>
        ///     The information elements.
        /// </value>
        /// <remarks>
        ///     Probe request frames normally contain information elements for
        ///     <see cref="F:InformationElement.ElementId.ServiceSetIdentity" />,
        ///     <see cref="F:InformationElement.ElementId.SupportedRates" /> and
        ///     <see cref="F:InformationElement.ElementId.ExtendedSupportedRates" /> in that order.
        /// </remarks>
        public InformationElementList InformationElements { get; set; }

        /// <summary>
        ///     Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if ((this.HeaderByteArraySegment == null) ||
                (this.HeaderByteArraySegment.Length >
                 (this.HeaderByteArraySegment.BytesLength - this.HeaderByteArraySegment.Offset)) ||
                (this.HeaderByteArraySegment.Length < this.FrameSize))
            {
                this.HeaderByteArraySegment = new ByteArraySegment(new Byte[this.FrameSize]);
            }

            this.FrameControlBytes = this.FrameControl.Field;
            this.DurationBytes = this.Duration.Field;
            this.SetAddress(0, this.DestinationAddress);
            this.SetAddress(1, this.SourceAddress);
            this.SetAddress(2, this.BssId);
            this.SequenceControlBytes = this.SequenceControl.Field;

            //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
            this.InformationElements.CopyTo(this.HeaderByteArraySegment,
                this.HeaderByteArraySegment.Offset + ProbeRequestFields.InformationElement1Position);

            this.HeaderByteArraySegment.Length = this.FrameSize;
        }

        private class ProbeRequestFields
        {
            public static readonly Int32 InformationElement1Position;

            static ProbeRequestFields()
            {
                InformationElement1Position = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            }
        }
    }
}