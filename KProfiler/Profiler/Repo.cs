using Utils;

namespace XSProfiler
{
    public class WebProf
    {
        public static Repo<ErrorLog> ErrorRepo = new Repo<ErrorLog>();
        public static Repo<TimeProfile> TimeRepo = new Repo<TimeProfile>();
    }

    public class Repo<T>
    {
        public void DeleteMany(Func<T, bool> value) { }
        public IEnumerable<T> GetAll() => new List<T>();
        public IEnumerable<T> Where(Func<T, bool> value) => new List<T>();
    }
}