using Utils;

namespace XSProfiler
{
    public class ProfilerAnalizer
    {
        //static readonly BaseRepository<TimeProfile> TimeRepo = new BaseRepository<TimeProfile>();
        public static List<DataPoint> GetCCU(int from, int to = int.MaxValue)
        {
            var all = WebProf.TimeRepo.GetAll()
                .Where(x => x.Begin > from && x.Begin < to).ToList(); //&& MainConstants.IsRelease(x.Server)
            var points = all.Select(
                x => new DataPoint(x.End.GlobalSecondToDateTime().ToString(),
                    x.PeersConnected)).ToList();
            return points;
        }

        public static List<ErrorsInfo> GetServerErrors(int n = 500)
        {
            var all = WebProf.ErrorRepo.GetAll().ToList();
            var table = all.GroupBy(x => x.Stack.SubstringFirst(700))
                .Select(e => new ErrorsInfo(e))
                .OrderByDescending(x => x.Time).Take(n).ToList();
            return table;
        }

        public static Dictionary<string, List<ProfileTimes>> GetTimes(
            string srv, int from, int to = int.MaxValue)
        {
            var all = WebProf.TimeRepo.Where(x => x.Begin > from && x.Begin < to);
            if (!string.IsNullOrEmpty(srv))
                all = all.Where(x => x.Server == srv);
            var times = TimeProfile.Group(all.ToArray());
            var splited = times.Times.Select(x => new ProfileTimes(x)).ToList();
            var byP = splited.GroupBy(x => x.Parent).ToDictionary(x => x.Key,
                x => x.OrderByDescending(y => y.Time).ToList());
            var top = byP.TryGetValueOrDefault("") ?? new List<ProfileTimes>();
            top.ForEach(x => SetTopTime(x, x.Time, 0));
            var sum = top.Sum(x => x.Time);
            if (sum == 0) sum = 1;
            splited.ForEach(x => x.TimeP = 100f * x.Time / sum);
            return byP;

            void SetTopTime(ProfileTimes t, float time, int lvl)
            {
                t.Level = lvl;
                t.TopTime = time;
                if (lvl > 20)
                    return;
                if (byP.TryGetValue(t.Metod, out var mm))
                    mm.ForEach(x => SetTopTime(x, time, lvl + 1));
            }
        }

        public static List<ProfileTimes> GetServerTimes(string srv, int lastTime)
        {
            //var sum = times.Times.Sum(x => x.IsTopLevel());
            //var byM = splited.GroupBy(x => x.Metod)
            //    .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.Time).ToList());
            var byP = GetTimes(srv, lastTime);
            var res = byP.Values.SelectMany(x => x)
                .OrderByDescending(x => x.TopTime)
                .ThenByDescending(x => x.Time).ToList();
            var n = 0;
            res.ForEach(x =>
            {
                x.Metod = (x.Level == 0 ? "=== " : new string('.', x.Level)) + x.Metod;
                x.N = ++n;
                //x.TimeP = 100f * x.Time / sum;
            });
            return res;
        }
    }
}
