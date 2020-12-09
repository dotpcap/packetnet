/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// Window Scale Factor Option
    /// Expands the definition of the TCP window to 32 bits
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc1323/
    /// </remarks>
    public class WindowScaleFactorOption : TcpOption
    {
        // the offset (in bytes) of the ScaleFactor Field
        private const int ScaleFactorFieldOffset = 2;

        /// <summary>
        /// Creates a Window Scale Factor Option
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        public WindowScaleFactorOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// The Window Scale Factor
        /// used as a multiplier to the window value
        /// The multiplier is equal to 1 left-shifted by the ScaleFactor
        /// So a scale factor of 7 would equal 1 &lt;&lt; 7 = 128
        /// </summary>
        public byte ScaleFactor
        {
            get => Bytes[ScaleFactorFieldOffset];
            set => Bytes[ScaleFactorFieldOffset] = value;
        }

        /// <summary>
        /// Returns the Option info as a string
        /// The multiplier is equal to a value of 1 left-shifted by the scale factor
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind + ": ScaleFactor=" + ScaleFactor + " (multiply by " + (1 << ScaleFactor) + ")]";
        }
    }
}