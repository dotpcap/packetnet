/*
This file is part of Packet.Net

Packet.Net is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Packet.Net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Packet.Net.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

﻿using System;
using Packet.Net.Utils;

namespace Packet.Net
{
    public abstract class Packet
    {
        private ByteArrayAndOffset header;
        private ByteArrayAndOffset payloadData;

        private Packet parentPacket, payloadPacket;

        private bool hasOwnMemory;

        /// <value>
        /// Returns true if the packet owns its own byte[] vs.
        /// if it is using a byte[] that was passed it 
        /// </value>
        public bool HasOwnMemory
        {
            get { return hasOwnMemory; }
            set { hasOwnMemory = value; }
        }

        /// <summary>
        /// The packet that is carrying this one
        /// </summary>
        public Packet ParentPacket
        {
            get { return parentPacket; }
            set { parentPacket = value; }
        }

        /// <summary>
        /// Packet that this packet carries
        /// </summary>
        public Packet PayloadPacket
        {
            get { return payloadPacket; }
            set
            {
                if (payloadPacket == value)
                    throw new InvalidOperationException("A packet cannot have itself as its payload.");

                payloadPacket = value;
                payloadPacket.ParentPacket = this;
            }
        }

        public byte[] Header
        {
            get { return this.header; }
        }

        /// <summary>
        /// The encapsulated data, this may be data sent by a program, or another protocol
        /// </summary>
        public byte[] PayloadData
        {
            get { return payloadData; }
            set { payloadData = value; }
        }

        /// <summary>
        /// The packet and the packets its carrying as an array of bytes
        /// </summary>
        public abstract byte[] Bytes
        {
            get;
        }

        /// <summary>
        /// Turns an array of bytes into a packet
        /// </summary>
        /// <param name="data">The packets caught</param>
        /// <returns>An ethernet packet which has references to the higher protocols</returns>
        public static Packet Parse(byte[] data)
        {
            return EthernetPacket.Parse(data);
        }

        public virtual System.String ToColoredString(bool colored)
        {
            return String.Empty;
        }

        public virtual System.String ToColoredVerboseString(bool colored)
        {
            return String.Empty;
        }
    }
}
