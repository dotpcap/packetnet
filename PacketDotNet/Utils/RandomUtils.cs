using System;
using System.Collections.Generic;
using System.Net;

namespace PacketDotNet.Utils
{
    /// <summary>
    /// Random utility methods
    /// </summary>
    public class RandomUtils
    {
        /// <summary>
        /// Generate a random ip address
        /// </summary>
        /// <param name="version">
        /// A <see cref="IpVersion"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Net.IPAddress"/>
        /// </returns>
        public static System.Net.IPAddress GetIPAddress(IpVersion version)
        {
            var rnd = new Random();
            byte[] randomAddressBytes;

            if(version == IpVersion.IPv4)
            {
                randomAddressBytes = new byte[IPv4Fields.AddressLength];
                rnd.NextBytes(randomAddressBytes);
            } else if(version == IpVersion.IPv6)
            {
                randomAddressBytes = new byte[IPv6Fields.AddressLength];
                rnd.NextBytes(randomAddressBytes);
            } else
            {
                throw new System.InvalidOperationException("Unknown version of " + version);
            }

            return new System.Net.IPAddress(randomAddressBytes);
        }

        /// <summary>
        /// Checks equality on the values (not references) of two byte arrays
        /// </summary>
        public static bool ByteArrayEquals(byte[] arrayA, byte[] arrayB)
        {
            if(arrayA.Length != arrayB.Length)
                return false;

            for(int i=0; i < arrayA.Length; i++)
            {
                if(arrayA[i] != arrayB[i])
                    return false;
            }

            return true;
        }
    }
}
