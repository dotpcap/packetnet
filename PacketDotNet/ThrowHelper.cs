/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Net.Sockets;

// ReSharper disable InconsistentNaming
namespace PacketDotNet
{
    internal static class ThrowHelper
    {
        /// <summary>
        /// Throws an invalid address family exception.
        /// </summary>
        /// <param name="addressFamily">The address family.</param>
        /// <exception cref="InvalidOperationException">Address family " + addressFamily + " invalid</exception>
        internal static void ThrowInvalidAddressFamilyException(AddressFamily addressFamily)
        {
            throw new InvalidOperationException("The address family " + addressFamily + " is invalid.");
        }

        /// <summary>
        /// Throws an argument null exception.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw new ArgumentNullException(GetArgumentName(argument));
        }

        /// <summary>
        /// Throws an argument out of range exception.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument));
        }

        /// <summary>
        /// Throws an argument out of range exception.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static void ThrowArgumentOutOfRangeException(ExceptionDescription description)
        {
            throw new ArgumentOutOfRangeException(GetDescription(description));
        }

        /// <summary>
        /// Throws an invalid operation exception.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <exception cref="InvalidOperationException"></exception>
        internal static void ThrowInvalidOperationException(ExceptionDescription description)
        {
            throw new InvalidOperationException(GetDescription(description));
        }

        /// <summary>
        /// Throws a not implemented exception.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal static void ThrowNotImplementedException()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Throws a not implemented exception.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal static void ThrowNotImplementedException(ExceptionArgument argument)
        {
            throw new NotImplementedException(GetArgumentName(argument) + " is not yet implemented.");
        }

        /// <summary>
        /// Throws a not implemented exception.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal static void ThrowNotImplementedException(ExceptionDescription description)
        {
            throw new NotImplementedException(GetDescription(description));
        }

        /// <summary>
        /// Converts an ExceptionArgument enum value to the argument name string.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns><see cref="string" />.</returns>
        private static string GetArgumentName(ExceptionArgument argument)
        {
            string argumentName;
            switch (argument)
            {
                case ExceptionArgument.buffer:
                {
                    argumentName = "buffer";
                    break;
                }
                case ExceptionArgument.index:
                {
                    argumentName = "index";
                    break;
                }
                case ExceptionArgument.linkLayer:
                {
                    argumentName = "linkLayers";
                    break;
                }
                case ExceptionArgument.startIndex:
                {
                    argumentName = "startIndex";
                    break;
                }
                case ExceptionArgument.value:
                {
                    argumentName = "value";
                    break;
                }
                default:
                {
                    return String.Empty;
                }
            }

            return argumentName;
        }

        /// <summary>
        /// Converts an ExceptionDescription enum value to the description string.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns><see cref="string" />.</returns>
        private static string GetDescription(ExceptionDescription description)
        {
            string result;

            switch (description)
            {
                case ExceptionDescription.PacketAsPayloadPacket:
                {
                    result = "A packet cannot have itself as its payload.";
                    break;
                }

                case ExceptionDescription.TotalLengthBelowMinimumHeaderLength:
                {
                    result = "The total length is below the minimum header length.";
                    break;
                }

                case ExceptionDescription.UrgentPointerSet:
                {
                    result = "Options with the urgent pointer set are not yet implemented.";
                    break;
                }

                default:
                {
                    return String.Empty;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// The convention for this enum is using the argument name as the enum name.
    /// </summary>
    internal enum ExceptionArgument
    {
        buffer,
        index,
        linkLayer,
        startIndex,
        value
    }

    /// <summary>
    /// The convention for this enum is using the description name as the enum name.
    /// </summary>
    internal enum ExceptionDescription
    {
        PacketAsPayloadPacket,
        TotalLengthBelowMinimumHeaderLength,
        UrgentPointerSet
    }
}