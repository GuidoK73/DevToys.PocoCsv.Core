using System;

namespace DevToys.PocoCsv.UnitTests
{
    public class StopWatch
    {
        private DateTime _Start = new();

        public StopWatch() { }

        public TimeSpan Duration { get; private set; } = new TimeSpan();

        public void Start()
        {
            Duration = new TimeSpan();
            _Start = DateTime.Now;
        }

        public void Stop() => Duration = Elapsed;

        public override string ToString() => Duration.ToString();

        private TimeSpan Elapsed => (DateTime.Now - _Start);
    }
}
