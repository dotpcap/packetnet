using System;

namespace Test
{
    /// <summary>
    /// Compute a rate given a start and end DateTime and an event count
    /// </summary>
    public class Rate
    {
        private TimeSpan Elapsed;
        private String EventType;
        private Int32 EventCount;

        public Rate(DateTime Start, DateTime End,
                    Int32 EventCount, String EventType)
        {
            Elapsed = End - Start;
            this.EventCount = EventCount;
            this.EventType = EventType;
        }

        /// <value>
        /// Returns the rate in terms of events per second
        /// </value>
        public Double RatePerSecond
        {
            get
            {
                return ((Double)EventCount / (Double)Elapsed.Ticks) * TimeSpan.TicksPerSecond;
            }
        }

        public override String ToString ()
        {
            return String.Format(" {0,10} {1} at a rate of {2,12} / second ({3} seconds elapsed)",
                                 EventCount,
                                 EventType,
                                 RatePerSecond.ToString("n"),
                                 Elapsed);
        }
    }
}
