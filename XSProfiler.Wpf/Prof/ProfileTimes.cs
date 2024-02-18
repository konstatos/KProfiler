using Utils;

namespace XSProfiler
{
    public class ProfileTimes
    {
        public string Parent;
        public string Param;
        public float TopTime;
        public int Level;

        public int N { get; set; }
        public string Metod { get; set; }
        public float Time { get; set; }
        public float TimeP { get; set; }
        public int Count { get; set; }
        public int Exc { get; set; }

        public ProfileTimes(TimeSample x)
        {
            var s = x.Param.Split('!');
            if (s.Length > 1)
            {
                Metod = s[1];
                Parent = s[0];
            }
            else
            {
                Metod = s[0];
                Parent = "";
            }
            Param = x.Param;
            Time = x.Time / 10_000_000f;
            Count = x.Count;
            Exc = x.Exc;
        }

        public override string ToString() => Param;
    }
}
