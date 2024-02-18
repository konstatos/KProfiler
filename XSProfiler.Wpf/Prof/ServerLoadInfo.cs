using Utils;

namespace XSProfiler.Wpf
{
    public class ServerLoadInfo
    {
        public string Server { get; set; }
        public TimeSpan Time { get; private set; }
        public float Load { get; set; }
        public TimeSpan AllTime { get; set; }
        public TimeSpan Handlers { get; set; }
        public int Players { get; set; }
        public int Peers { get; set; }
        public int CCU { get; set; }
        public DateTime Last { get; set; }

        public ServerLoadInfo(TimeProfile profile)
        {
            Server = profile.Server;
            var time = profile.End - profile.Begin;
            Time = TimeSpan.FromSeconds(time);
            var all = profile.Times.Sum(x => x.IsIndependent());
            Handlers = TimeSpan.FromTicks(profile.AllTime);
            AllTime = TimeSpan.FromTicks(all);
            Load = 100f * all / time / 10_000_000f;
            Players = profile.Players;
            Peers = profile.Peers;
            CCU = profile.PeersConnected;
            Last = profile.End.GlobalSecondToDateTime();
        }
    }
}