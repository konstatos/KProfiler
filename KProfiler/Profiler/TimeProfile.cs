using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public class TimeProfile : IEntity
    {
        private const int TicksInMs = 10000;
        public string Server;
        public string User;
        /// <summary> ticks </summary>
        public long AllTime;
        /// <summary> s </summary>
        public int Begin, End;
        public int Players;
        public int Peers;
        public int PeersConnected;
        public List<TimeSample> Times = new List<TimeSample>();

        public static TimeProfile Group(TimeProfile[] profiles)
        {
            if (profiles.Length == 0)
                return new TimeProfile()
                {
                    Server = "-"
                };

            //var f = profiles[0];
            var t = new Dictionary<string, TimeSample>();

            foreach (var pp in profiles)
                foreach (var p in pp.Times)
                {
                    var ts = t.GetAddValue(p.Param, () => new TimeSample() { Param = p.Param });
                    ts.Time += p.Time;
                    ts.Count += p.Count;
                    ts.Exc += p.Exc;
                }
            return new TimeProfile()
            {
                Server = profiles.Last().Name,
                Begin = profiles.Min(x => x.Begin),
                End = profiles.Max(x => x.End),
                AllTime = profiles.Sum(x => x.AllTime),
                Peers = profiles.Max(x => x.Peers),
                PeersConnected = profiles.Max(x => x.PeersConnected),
                Times = t.Select(x => x.Value).ToList(),
                Players = profiles.Max(x => x.Players),
            };
        }

        public string GetInfo()
        {
            return $"========= {Begin.GlobalSecondToDateTime()}-{End.GlobalSecondToDateTime()} =========\n" +
                $"{GetInfoTitle()}\n{GetLog()}";
        }

        public string GetLog() =>
            Times.OrderByDescending(x => x.Time)
                .Select(x => $"{x.Count,7}\t{(float)x.Time / TicksInMs,10:f3}\t{x.Param}").ToStringJoin("\n");

        public string GetInfoTitle()
        {
            var all = AllTime / TicksInMs;
            var time = End - Begin;
            var sum = "";
            try
            {
                sum = (Times.Sum(x => (long)x.Time) / TicksInMs).ToString();
            }
            catch (Exception e)
            {
                sum = e.Message;
            }
            return $"Load={0.1f * all / time:f5}%, " +
                $"Total={time.GlobalSecondToTimeSpan()}, " +
                $"All={all}ms, Handlers={sum}ms, " +
                $"peers={PeersConnected}/{Peers}, players={Players}";
        }
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