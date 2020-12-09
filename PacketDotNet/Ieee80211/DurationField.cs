/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Duration field.
    /// </summary>
    public class DurationField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DurationField" /> class.
        /// </summary>
        public DurationField()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">
        /// A <see cref="ushort" />
        /// </param>
        public DurationField(ushort field)
        {
            Field = field;
        }

        /// <summary>
        /// This is the raw Duration field
        /// </summary>
        public ushort Field { get; set; }
    }
}