using System;

namespace Test
{
    /// <summary>
    ///     Compute a rate given a start and end DateTime and an event count
    /// </summary>
    public class Rate
    {
        private readonly Int32 _eventCount;
        private readonly String _eventType;
        private TimeSpan _elapsed;

        public Rate(DateTime start, DateTime end,
            Int32 eventCount, String eventType)
        {
            this._elapsed = end - start;
            this._eventCount = eventCount;
            this._eventType = eventType;
        }

        /// <value>
        ///     Returns the rate in terms of events per second
        /// </value>
        public Double RatePerSecond => (this._eventCount / (Double) this._elapsed.Ticks) * TimeSpan.TicksPerSecond;

        public override String ToString()
        {
            return String.Format(" {0,10} {1} at a rate of {2,12} / second ({3} seconds elapsed)", this._eventCount,
                this._eventType, this.RatePerSecond.ToString("n"), this._elapsed);
        }
    }
}