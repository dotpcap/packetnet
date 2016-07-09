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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 *  Copyright 2016 Joseph D. Beshay <josephdbeshay@gmail.com>
 */
using System;

namespace PacketDotNet
{
    /// <summary>
    /// LLC+SNAP protocol field encoding information.
    /// </summary>
    public class SNAPFields
    {
        /// <summary> Width of the Destination Service Access Point in bytes.</summary>
        public readonly static int DSAPLength = 1;

        /// <summary> Width of the Source Service Access Point in bytes.</summary>
        public readonly static int SSAPLength = 1;

        /// <summary> Width of the Control field in bytes.</summary>
        public readonly static int ControlLength = 1;

        /// <summary> Width of the Organization Code in bytes.</summary>
        public readonly static int OrgCodeLength = 3;

        /// <summary> Width of the Ethernet Type in bytes.</summary>
        public readonly static int EtherTypeLength = 2;

        /// <summary> Position of the DSAP address within the SNAP header.</summary>
        public readonly static int DSAPPosition = 0;

        /// <summary> Position of the SSAP address within the SNAP header.</summary>
        public readonly static int SSAPPosition;

        /// <summary> Position of the Control field within the SNAP header.</summary>
        public readonly static int ControlPosition;

        /// <summary> Position of the Organization code within the SNAP header.</summary>
        public readonly static int OrgCodePosition;

        /// <summary> Position of the ethernet type field within the SNAP header.</summary>
        public readonly static int EtherTypePosition;

        /// <summary> Total length of a SNAP header in bytes.</summary>
        public readonly static int HeaderLength; // == 8

        static SNAPFields()
        {
            SSAPPosition = SNAPFields.DSAPPosition + SNAPFields.DSAPLength;
            ControlPosition = SNAPFields.SSAPPosition + SNAPFields.SSAPLength;
            OrgCodePosition = SNAPFields.ControlPosition + SNAPFields.ControlLength;
            EtherTypePosition = SNAPFields.OrgCodePosition + SNAPFields.OrgCodeLength;
            HeaderLength = SNAPFields.EtherTypePosition + SNAPFields.EtherTypeLength;
            
        }
    }
}
