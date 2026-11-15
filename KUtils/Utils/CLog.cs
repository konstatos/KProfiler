//#undef DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Класс для логирования из библиотеки, работает по подписке из клиентов
/// (c) Konstatos
/// </summary>
public static class CLog
{
    public static event Action<string>? OnInfo;
    public static event Action<string>? OnLocal;
    public static event Action<string>? OnWarning;
    public static event Action<string>? OnError;
    //public static event Action<string> OnFatalError;

    private const string DEBUG = "DEBUG";
    private static readonly HashSet<string> Onces = new HashSet<string>();

    public static void DropSubscribers()
    {
        OnInfo = null;
        OnLocal = null;
        OnWarning = null;
        OnError = null;
        //OnFatalError = null;
    }

    //public static void Add(object s) => OnInfo?.Invoke(s?.ToString());
    //public static void FatalError(string s) => OnFatalError?.Invoke(s);

    public static void Error(string s, Exception e) => OnError?.Invoke($"{s}: {e.Message}\n{e.StackTrace}");
    public static void Error(string s) => OnError?.Invoke(s);

    [Conditional(DEBUG)]
    public static void Warning(string s) => OnWarning?.Invoke(s);
    [Conditional(DEBUG)]
    public static void KnownError(string s) => OnWarning?.Invoke(s);
    [Conditional(DEBUG)]
    public static void ActualInfo(string s) => OnWarning?.Invoke(s);

    [Conditional(DEBUG)]
    public static void Info(string s) => OnInfo?.Invoke(s);
    [Conditional(DEBUG)]
    public static void Assert(bool test, Func<string> getInfo)
    {
        if (!test) OnError?.Invoke(getInfo());
    }
    [Conditional(DEBUG)]
    public static void Assert(bool test, string info)
    {
        if (!test) OnError?.Invoke(info);
    }
    [Conditional(DEBUG)]
    public static void Assert(bool test, string info, object? o)
    {
        if (!test) OnError?.Invoke(info + o);
    }
    [Conditional(DEBUG)]
    public static void WriteOnce(string s)
    {
        if (Onces.Contains(s)) return;
        Onces.Add(s);
        ActualInfo(s);
    }
}

namespace Utils
{
    public enum ELogType
    {
        Error,
        KnownError,
        Warning,
        Message,
        Info,
        No
    }

    public enum ELogShow { No, User, Error, Warning, Message }

    public class CLogSaver
    {
        public static CLogSaver I = new CLogSaver();

        public List<string> Log = new List<string>();

        public CLogSaver()
        {
            CLog.OnError += s => OnError(s, " Error");
            CLog.OnWarning += s => OnError(s, "");
            //CLog.OnInfo += s => OnError(s, "");
            //CLog.OnFatalError += s => OnError(s, " FatalError");
        }

        public void Init() { }

        private void OnError(string s, string type) =>
            Log.Add($"{DateTime.UtcNow:HH:mm:ss}{type}: {s}");
    }
}