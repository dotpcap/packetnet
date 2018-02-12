using System;

namespace Test
{
    /// <summary>
    ///     Compute a rate given a start and end DateTime and an event count
    /// </summary>
    public class Rate
    {
        private readonly Int32 EventCount;
        private readonly String EventType;
        private TimeSpan Elapsed;

        public Rate(DateTime Start, DateTime End,
            Int32 EventCount, String EventType)
        {
            this.Elapsed = End - Start;
            this.EventCount = EventCount;
            this.EventType = EventType;
        }

        /// <value>
        ///     Returns the rate in terms of events per second
        /// </value>
        public Double RatePerSecond => (this.EventCount / (Double) this.Elapsed.Ticks) * TimeSpan.TicksPerSecond;

        public override String ToString()
        {
            return String.Format(" {0,10} {1} at a rate of {2,12} / second ({3} seconds elapsed)", this.EventCount,
                this.EventType, this.RatePerSecond.ToString("n"), this.Elapsed);
        }
    }
}