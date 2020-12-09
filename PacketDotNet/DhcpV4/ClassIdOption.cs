/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  This file is licensed under the Apache License, Version 2.0.
 */

using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class ClassIdOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassIdOption" /> class.
        /// </summary>
        /// <param name="classId">The class identifier.</param>
        public ClassIdOption(string classId) : base(DhcpV4OptionType.ClassId)
        {
            ClassId = classId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassIdOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public ClassIdOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.ClassId)
        {
            ClassId = Encoding.ASCII.GetString(buffer, offset, optionLength);
        }

        /// <summary>
        /// Gets or sets the class identifier.
        /// </summary>
        public string ClassId { get; set; }

        /// <inheritdoc />
        public override byte[] Data => Encoding.ASCII.GetBytes(ClassId);

        /// <inheritdoc />
        public override int Length => ClassId.Length;

        /// <inheritdoc />
        public override string ToString()
        {
            return "Class Id: " + ClassId;
        }
    }
}