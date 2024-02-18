namespace Utils
{
    public class TimeSample
    {
        public string Param;
        public string Over;
        public long Time;
        public int Count;
        public int Exc;

        public TimeSample() { }

        public long IsIndependent()
        {
            switch (Param)
            {
                //case "_EverySecond": return t.Time;
                case "TimeService.EverySecond()":
                case "MasterClientPeer.OnOperationRequest(operationRequest, sendParameters)":
                case "ConfigChangeListenerService.Update()":
                case "SaveDBToGlobal": return Time;
                default: return 0;
            }
        }

        public long IsTopLevel()
        {
            switch (Param)
            {
                //case "_EverySecond":
                case "MasterClientPeer.OnOperationRequest(operationRequest, sendParameters)":
                case "TimeService.EverySecond()": return Time;
                default: return 0;
            }
        }
        //public TimeSample(string v, Stopwatch value)
        //{
        //    Param = v;
        //    Time = (int)value.ElapsedMilliseconds;
        //}
    }

    //public class TimeProfile : IEntity
    //{
    //    long AllTiks;
    //    int Interval = 30;
    //    int Next;
    //    int Start = TimeExt.Now;
    //    const int TicksInMs = 10000;
    //    const int TicksInS = TicksInMs * 1000;
    //    Stopwatch Stopwatch = new Stopwatch();
    //    Dictionary<OperationCode, Stopwatch> Stopwatches = new Dictionary<OperationCode, Stopwatch>();
    //    //public Func<int> GetSubscribers;
    //    //public Func<int> GetSubscribersConnected; 

    //    public void ShowTime()
    //    {
    //        var sum = Stopwatches.Sum(x => x.Value.ElapsedMilliseconds);
    //        var log = Stopwatches.OrderByDescending(x => x.Value.ElapsedTicks)
    //            .Select(x => $"{x.Key,30}:\t{x.Value.ElapsedMilliseconds}")
    //            .ToStringJoin("\n");
    //        var time = TimeExt.Now - Start;
    //        CLog.ActualInfo($"========= ShowTime =========\nTotal={time.GlobalSecondToTimeSpan()}, " +
    //            $"ms={AllTiks / TicksInMs}, handlers={sum}, load={100f * AllTiks / time / TicksInS:f5}%, " +
    //            //$"peers={Subscribers.Values.Count(x => x.Connected)}/{Subscribers.Count}, " +
    //            $"players={PlayerRepository.Cache.Count}\n{log}");

    //        Repos.I.Times.AddAsync();
    //    }

    //    public void RequestEnd(Dictionary<byte, object> parameters, BaseHandler handler, Stopwatch sw)
    //    {
    //        sw.Stop();
    //        AllTiks += Stopwatch.ElapsedTicks;
    //        CLog.ActualInfo($"<---- Request: " + //Connection={ConnectionId}, " +
    //            $"Operation={handler.ControlCode} ({Stopwatch.ElapsedTicks} ticks/{sw.ElapsedMilliseconds} ms): " +
    //            $"{string.Join(", ", parameters.Select(x => (EParameterCode)x.Key + "=" + (x.Value is byte[]? (x.Value as byte[]).Length + "b" : x.Value.ToJsonMin())))}");
    //        Stopwatch.Stop();
    //        if (TimeExt.Now > Next)
    //        {
    //            Next = TimeExt.Now + Interval;
    //            ShowTime();
    //        }
    //    }

    //    public Stopwatch RequestBegin(BaseHandler handler)
    //    {
    //        Stopwatch.Restart();
    //        var sw = Stopwatches.GetAddValue(handler.ControlCode, () => new Stopwatch());
    //        sw.Start();
    //        return sw;
    //    }
    //}
}