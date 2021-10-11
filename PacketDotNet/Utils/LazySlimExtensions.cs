/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Utils
{
    public static class LazySlimExtensions
    {
        /// <summary>
        /// Evaluates the specified lazy function, if necessary.
        /// </summary>
        /// <param name="lazy">The lazy.</param>
        public static void Evaluate<T>(this LazySlim<T> lazy) where T : class
        {
            if (lazy.IsValueCreated)
                return;


            var _ = lazy.Value;
        }
    }
}