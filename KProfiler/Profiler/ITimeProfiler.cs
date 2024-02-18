using System;

namespace Utils
{
    public interface ITimeProfiler
    {
        TimeProfiler.TimeProf Begin(string n);
        TimeProfile CalcTime(int peers, int peersConnected);
        void End(string name);
        TimeProfiler.TimeProf Get(string name);
        TimeProfiler.TimeProf RequestBegin(string n);
        void RequestEnd(TimeProfiler.TimeProf sw);
        void Sample(string name, Action action);
    }
}