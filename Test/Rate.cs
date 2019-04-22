using System;

namespace Test
{
    /// <summary>
    /// Compute a rate given a start and end DateTime and an event count
    /// </summary>
    public class Rate
    {
        private readonly TimeSpan _elapsed;
        private readonly int _eventCount;
        private readonly string _eventType;

        public Rate
        (
            DateTime start,
            DateTime end,
            int eventCount,
            string eventType)
        {
            _elapsed = end - start;
            _eventCount = eventCount;
            _eventType = eventType;
        }

        /// <value>
        /// Returns the rate in terms of events per second
        /// </value>
        public double RatePerSecond => _eventCount / (double) _elapsed.Ticks * TimeSpan.TicksPerSecond;

        public override string ToString()
        {
            return $" {_eventCount,10} {_eventType} at a rate of {RatePerSecond,12:n} / second ({_elapsed} seconds elapsed)";
        }
    }
}