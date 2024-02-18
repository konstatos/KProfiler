namespace XSProfiler
{
    public class DataPoint
    {
        public string Day { get; set; }
        public double Value { get; set; }

        public DataPoint() { }

        public DataPoint(string day, double value)
        {
            Day = day;
            Value = value;
        }

        public override string ToString() => Day + ": " + Value;
    }
}
