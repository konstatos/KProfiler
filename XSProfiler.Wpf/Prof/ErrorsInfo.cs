using Utils;

namespace XSProfiler
{
    public class ErrorsInfo
    {
        public List<ErrorLog> Logs;
        public ErrorLog Last;

        public string Time { get; }
        public string Metod { get; }
        public string Error { get; }
        public string Stack { get; }
        public string Args { get; }
        public int Count { get; }
        public string Server { get; }
        public int Version { get; }

        public ErrorsInfo(string time, string metod, string error,
            string stack, string args, int count, string server, int version)
        {
            Time = time;
            Metod = metod;
            Error = error;
            Stack = stack;
            Args = args;
            Count = count;
            Server = server;
            Version = version;
        }

        public ErrorsInfo(IEnumerable<ErrorLog> logs)
        {
            Logs = logs.ToList();
            var x = Last = Logs.Last();
            Time = x.Time.GlobalSecondToDateTime().ToString();
            Metod = x.Name;
            Error = x.Exception.SubstringFirst(200);
            Stack = x.Stack;
            Args = x.Arguments.ToStringJoin();
            Count = Logs.Sum(y => y.Count + 1);
            Server = x.OwnerId;
            Version = x.Version;
        }

        public string GetInfo()
        {
            return $"Случаев: {Count}\n" +
                $"Последний случай: {Time}\n" +
                $"Метод: {Metod}\n" +
                $"Аргументы: {Args}\n" +
                $"Исключение: {Last.Exception}\n" +
                $"Сервер: {Server} v{Version}\n\n" +
                $"Стэк:\n {Stack}";
        }
    }
}
