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
 *  Copyright 2012 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Linq;
using PacketDotNet.Utils;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Information element, a variable-length component of management frames
    /// </summary>
    /// <exception cref='ArgumentException'>
    /// Is thrown when an argument passed to a method is invalid.
    /// </exception>
    public partial class InformationElement
    {
        /// <summary>
        /// The length in bytes of the Information Element id field.
        /// </summary>
        public static readonly Int32 ElementIdLength = 1;

        /// <summary>
        /// The index of the id field in an Information Element.
        /// </summary>
        public static readonly Int32 ElementIdPosition = 0;

        /// <summary>
        /// The length in bytes of the Information Element length field.
        /// </summary>
        public static readonly Int32 ElementLengthLength = 1;

        /// <summary>
        /// The index of the length field in an Information Element.
        /// </summary>
        public static readonly Int32 ElementLengthPosition;

        /// <summary>
        /// The index of the first byte of the value field in an Information Element.
        /// </summary>
        public static readonly Int32 ElementValuePosition;

        private ByteArraySegment _bytes;

        static InformationElement()
        {
            ElementLengthPosition = ElementIdPosition + ElementIdLength;
            ElementValuePosition = ElementLengthPosition + ElementLengthLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationElement" /> class.
        /// </summary>
        /// <param name='bas'>
        /// The bytes of the information element. The Offset property should point to the first byte of the element, the Id byte
        /// </param>
        public InformationElement(ByteArraySegment bas)
        {
            _bytes = bas;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationElement" /> class.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        /// <param name='value'>
        /// Value.
        /// </param>
        /// <exception cref='ArgumentException'>
        /// Is thrown when an argument passed to a method is invalid.
        /// </exception>
        public InformationElement(ElementId id, Byte[] value)
        {
            var ie = new Byte[ElementIdLength + ElementLengthLength + value.Length];
            _bytes = new ByteArraySegment(ie);
            Id = id;
            Value = value;
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public Byte[] Bytes => _bytes.ActualBytes();

        /// <summary>
        /// Gets the length of the element including the Id and Length field
        /// </summary>
        /// <value>
        /// The length of the element.
        /// </value>
        public Byte ElementLength => (Byte) (ElementIdLength + ElementLengthLength + ValueLength);

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public ElementId Id
        {
            get => (ElementId) _bytes.Bytes[_bytes.Offset + ElementIdPosition];
            set => _bytes.Bytes[_bytes.Offset + ElementIdPosition] = (Byte) value;
        }

        /// <summary>
        /// Gets or sets the value of the element
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <exception cref='ArgumentException'>
        /// Is thrown when the value is too large. Values are limited to a maximum size 255 bytes due the single
        /// byte length field.
        /// </exception>
        public Byte[] Value
        {
            get
            {
                var valueArray = new Byte[ValueLength];
                Array.Copy(_bytes.Bytes,
                           _bytes.Offset + ElementValuePosition,
                           valueArray,
                           0,
                           ValueLength);
                return valueArray;
            }

            set
            {
                if (value.Length > Byte.MaxValue)
                {
                    throw new ArgumentException("The provided value is too long. Maximum allowed length is 255 bytes.");
                }

                //Decide if the current ByteArraySegement is big enough to hold the new info element
                var newIeLength = ElementIdLength + ElementLengthLength + value.Length;
                if (_bytes.Length < newIeLength)
                {
                    var newIe = new Byte[newIeLength];
                    newIe[ElementIdPosition] = _bytes.Bytes[_bytes.Offset + ElementIdPosition];
                    _bytes = new ByteArraySegment(newIe);
                }

                Array.Copy(value, 0, _bytes.Bytes, _bytes.Offset + ElementValuePosition, value.Length);
                _bytes.Length = newIeLength;
                _bytes.Bytes[_bytes.Offset + ElementLengthPosition] = (Byte) value.Length;
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public Int32 ValueLength => Math.Min(_bytes.Length - ElementValuePosition,
                                             _bytes.Bytes[_bytes.Offset + ElementLengthPosition]);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to the current <see cref="InformationElement" />.
        /// </summary>
        /// <param name='obj'>
        /// The <see cref="System.Object" /> to compare with the current <see cref="InformationElement" />.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to the current
        /// <see cref="InformationElement" />; otherwise, <c>false</c>.
        /// </returns>
        public override Boolean Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return obj is InformationElement ie && (Id == ie.Id && Value.SequenceEqual(ie.Value));
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="InformationElement" /> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as
        /// a hash table.
        /// </returns>
        public override Int32 GetHashCode()
        {
            return Id.GetHashCode() ^ Value.GetHashCode();
        }
    }
}