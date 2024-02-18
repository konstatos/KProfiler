using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Utils
{
    public sealed class ProfilerAttribute : OnMethodBoundaryAspect
    {
        //public static Thread MainThread = Thread.CurrentThread;
        public static TimeProfiler.TimeProf Prof = new TimeProfiler.TimeProf(TimeProfiler.Profiler);
        public static TimeProfiler.TimeProf ProfExc = new TimeProfiler.TimeProf(TimeProfiler.ProfilerExc);
        static Dictionary<Thread, Stack<string>> Over = new Dictionary<Thread, Stack<string>>();
        //string name = "*";

        private string GetName(MethodBase m) => // {Thread.CurrentThread.ManagedThreadId}:
            $"{m.DeclaringType.Name}.{m.Name}({m.GetParameters().Select(x => x.Name).ToStringJoin()})";

        //public override bool CompileTimeValidate(MethodBase method)
        //{
        //    Name = GetName(method);
        //    return base.CompileTimeValidate(method);
        //}

        public override void OnEntry(MethodExecutionArgs args)
        {
            Prof.Start();
            //Thread.
            //var last = Over.Count > 0 ? Over.Peek() : null;
            //if (MainThread == Thread.CurrentThread)
            //{
            var name = GetName(args.Method);
            var o = GetOvers();
            var n = o.Count > 0 ? o.Peek() + '!' + name : name;
            o.Push(name);
            TimeProfiler.I.Begin(n);
            //}
            //else
            //{
            //    TimeProfiler.I.Begin(name);
            //}
            Prof.Stop();
        }

        private static Stack<string> GetOvers()
        {
            var t = Thread.CurrentThread;
            var o = Over.GetAddValue(t, () => new Stack<string>());
            return o;
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Prof.Start();
            var name = GetName(args.Method);
            //TimeProfiler.I.End(name);
            //CLog.Info($"OnExit: {name}"); // {TimeProfiler.I.Get(name)}");
            Stop(name);
            Prof.Stop();
        }

        public override void OnException(MethodExecutionArgs args)
        {
            ProfExc.Start();
            var name = GetName(args.Method);
            //TimeProfiler.I.End(name);
            TimeProfiler.I.OnException(name);
            ErrorLogger.I.LogError(name, args);
            //CLog.ActualInfo($"OnException in: {name} {args.Exception.Message}"); // {TimeProfiler.I.Get(name)}");
            //TimeProfiler.I.Begin(name + " " + args.Exception);
            Stop(name);
            ProfExc.Stop();
        }

        private void Stop(string name)
        {
            //if (MainThread == Thread.CurrentThread)
            var o = GetOvers();
            if (o.Count > 0)
            {
                o.Pop();
                if (o.Count > 0)
                    name = o.Peek() + '!' + name;
            }
            TimeProfiler.I.End(name);
        }
    }

    [Profiler]
    public class ProfilerAttributeDebug
    {
        public void Do()
        {

        }
        public void DoLong()
        {
            for (int i = 0; i < 1000000; i++)
            {

            }
        }
        public void DoSleep()
        {
            Thread.Sleep(100);
        }
        public void Do2()
        {
            Do();
        }
    }

    public class ErrorLog : IEntity
    {
        public int Time;
        public int Count;
        public string[] Arguments;
        public string Exception;
        public string Stack;
        public string OwnerId;
    }

    public class ErrorLogger
    {
        internal static bool IsRelease;
        internal static int GameVersion;
        internal static string Server;

        public static ErrorLogger I = new ErrorLogger();

        public readonly Dictionary<string, ErrorLog> Log =
            new Dictionary<string, ErrorLog>();

        public void Init(bool relese, int version, string server)
        {
            Server = server;
            IsRelease = relese;
            GameVersion = version;
        }

        public void LogError(string name, MethodExecutionArgs args)
        {
            var l = LogError(name, args.Exception);
            l.Arguments = args.Arguments?.Select(x => x?.ToString()).ToArray();
        }

        public ErrorLog LogError(string name, Exception e = null)
        {
            ErrorLog l = null;
            try
            {
                if (IsRelease)
                    CLog.ActualInfo($"Error: {name} {e?.Message}"); // {TimeProfiler.I.Get(name)}");
                else CLog.ActualInfo($"{name} {e}");
                var m = e == null ? Stacker.GetStackString(3, 5) : e.ToString();
                if (Log.TryGetValue(m, out l))
                {
                    l.Time = TimeExt.Now;
                    l.Count++;
                    return l;
                }
                l = new ErrorLog()
                {
                    Name = name,
                    Time = TimeExt.Now,
                    OwnerId = Server,
                    Version = GameVersion,
                };
                if (e != null)
                {
                    l.Stack = m;
                    l.Exception = e.Message;
                }
                l.NewId();
                Log[m] = l;
            }
            catch (Exception ex)
            {
                CLog.Error($"ErrorLogger: {name} {ex}");
            }
            return l;
        }
    }

    public class TimeProfiler
    {
        #region TimeProf
        public class TimeProf
        {
            //internal static Stack<string> Over = new Stack<string>();
            private Stopwatch Stopwatch = new Stopwatch();
            public string Name;
            private int Count;
            private int Exceptions;
            //string Name;
            //public TimeProf()
            //{
            //    Stopwatch = Stopwatch.StartNew();
            //}

            public TimeProf() { }
            public TimeProf(string name)
            {
                Name = name;
            }

            public void Start()
            {
                Stopwatch.Start();
                //var full = Over.Count > 0 ? Over.Peek() + '!' + n : n;
                //Name = n;
                //if (Name != null) Over.Push(Name);
                Count++;
            }
            public void Stop()
            {
                //if (Name != null) Over.Pop();
                Stopwatch.Stop();
            }
            public void OnException()
            {
                Exceptions++;
            }
            public void Clear()
            {
                Stopwatch.Reset();
                Exceptions = 0;
                Count = 0;
            }

            public TimeSample ToTimeSample() => new TimeSample
            {
                Param = Name,
                Count = Count,
                Exc = Exceptions,
                Time = (int)Stopwatch.ElapsedTicks,
            };
        }
        #endregion

        #region Params
        public static TimeProfiler I = new TimeProfiler();
        public const int TicksInMs = 10000;
        public const string Profiler = "PROFILER";
        public const string ProfilerExc = "PROFILER exceptions";
        public long AllTiks;
        private int Interval = 60;
        private int Next;
        private int Start = TimeExt.Now;
        private Stopwatch Stopwatch = new Stopwatch();
        private Dictionary<string, TimeProf> Stopwatches = new Dictionary<string, TimeProf>();

        //Dictionary<OperationCode, Stopwatch> Stopwatches = new Dictionary<OperationCode, Stopwatch>();
        //Dictionary<string, Stopwatch> StopwatchesCommon = new Dictionary<string, Stopwatch>();
        //Dictionary<string, int> Exceptions = new Dictionary<string, int>();
        //const int TicksInS = TicksInMs * 1000; 
        #endregion

        #region ShowTime
        private void Clear()
        {
            Start = TimeExt.Now;
            Next = TimeExt.Now + Interval;
            AllTiks = 0;
            Stopwatches.Clear();
            ProfilerAttribute.Prof = new TimeProf(Profiler);
            ProfilerAttribute.ProfExc = new TimeProf(ProfilerExc);
            //foreach (var sw in Stopwatches)
            //    sw.Value.Clear();
            //StopwatchesCommon.Clear();
        }

        public TimeProfile CalcTime(int peers, int peersConnected, int allPlayers)
        {
            if (Stopwatches.Count == 0) return null;
            Stopwatches[Profiler] = ProfilerAttribute.Prof;
            Stopwatches[ProfilerExc] = ProfilerAttribute.ProfExc;
            //var sws = Stopwatches.ToList();
            TimeProf[] tp = new TimeProf[Stopwatches.Count + 10];
            Stopwatches.Values.CopyTo(tp, 0);
            var sws = tp.Where(x => x != null).ToList();
            var t = new TimeProfile
            {
                Begin = Start,
                End = TimeExt.Now,
                AllTime = AllTiks,
                Players = allPlayers,
                Peers = peers,
                PeersConnected = peersConnected,
                Times = sws.Select(x => x.ToTimeSample()).ToList(), //x.Key)).ToList(),
                Server = ErrorLogger.Server, // + Environment.TickCount,
                User = Environment.UserName, // + Environment.TickCount,
                //Times = Stopwatches.Select(x => new TimeSample(x.Key.ToString(), x.Value))
                //    .Union(StopwatchesCommon.Select(x => new TimeSample(x.Key, x.Value))).ToList(),
            };
            t.NewId();
            Clear();
            return t;
        }
        #endregion

        //public static string GetServer() => Environment.MachineName;

        //public void Begin(string name)
        //{
        //    var sw = Stopwatches.GetAddValue(name, () => new Stopwatch());
        //    sw.Start();
        //}

        //[Conditional("DEBUG")]
        public TimeProf Begin(string n)
        {
            //var full = TimeProf.Over.Count > 0 ? TimeProf.Over.Peek() + '!' + n : n;
            //var full = TimeProf.Over.ToStringJoin("!") + '!' + n;
            var sw = Stopwatches.GetAddValue(n, () => new TimeProf(n));
            sw.Start();
            //CLog.ActualInfo($"Begin: {n} ({Stopwatches.Keys.ToStringJoin()})");
            return sw;
        }

        public void End(string name)
        {
            //Over.Pop();
            Get(name)?.Stop();
        }

        public TimeProf Get(string name) => Stopwatches.TryGetValueOrDefault(name);

        public void Sample(string name, Action action)
        {
            //CLog.ActualInfo($"Sample time {name} start...");
            var sw = Begin(name);
            try
            {
                action();
            }
            catch (Exception e)
            {
                CLog.Error(name + ": " + e.Message);
            }
            sw.Stop();
            //CLog.ActualInfo($"Sample time {name}: {sw.ElapsedMilliseconds} ms");
        }

        #region Request
        public TimeProf RequestBegin(string n)
        {
            Stopwatch.Restart();
            var sw = Begin(n);
            return sw;
        }

        public void RequestEnd(TimeProf sw)
        {
            sw.Stop();
            Stopwatch.Stop();
            AllTiks += Stopwatch.ElapsedTicks;
            //CLog.Info($"<---- Request: " + //Connection={ConnectionId}, " +
            //    $"Operation={handler.ControlCode}/{operationRequest.OperationCode} " +
            //    $"{string.Join(", ", operationRequest.Parameters.Select(x => (EParameterCode)x.Key + "=" + (x.Value is byte[]? (x.Value as byte[]).Length + "b" : x.Value.ToJsonMin())))}");
            // ({Stopwatch.ElapsedTicks} ticks/{sw.ElapsedMilliseconds} ms): " +
            //if (TimeExt.Now > Next)
            //{
            //    ShowTime();
            //}
        }
        #endregion

        internal void OnException(string name)
        {
            Stopwatches.TryGetValueOrDefaultLog(name)?.OnException();
        }
    }
}