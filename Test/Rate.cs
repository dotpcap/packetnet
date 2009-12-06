using System;

namespace Test
{
    public class Rate
    {
        private TimeSpan Elapsed;
        private string EventType;
        private int EventCount;

        public Rate(DateTime Start, DateTime End,
                    int EventCount, string EventType)
        {
            Elapsed = End - Start;
            this.EventCount = EventCount;
            this.EventType = EventType;
        }

        public override string ToString ()
        {
            return String.Format(" {0,10} {1} at a rate of {2,12} / second ({3} seconds elapsed)",
                                 EventCount,
                                 EventType,
                                 (((Double)EventCount / (Double)Elapsed.Ticks) * TimeSpan.TicksPerSecond).ToString("n"),
                                 Elapsed);
        }
    }
}
