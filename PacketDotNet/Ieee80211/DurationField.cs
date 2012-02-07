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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Duration field.
        /// </summary>
        public class DurationField
        {
            /// <summary>
            /// This is the raw Duration field
            /// 
            /// </summary>
            public UInt16 Field { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DurationField"/> class.
            /// </summary>
            public DurationField()
            {
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="field">
            /// A <see cref="UInt16"/>
            /// </param>
            public DurationField(UInt16 field)
            {
                this.Field = field;
            }
        } 
    }
}
