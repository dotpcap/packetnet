using System;

namespace PacketDotNet.Utils
{
    public static class LazyExtensions
    {
        /// <summary>
        /// Evaluates the specified lazy function, if necessary.
        /// </summary>
        /// <param name="lazy">The lazy.</param>
        public static void Evaluate<T>(this Lazy<T> lazy)
        {
            if (lazy.IsValueCreated)
                return;


            var _ = lazy.Value;
        }
    }
}