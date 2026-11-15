using System;

public static class TimeExt
{
    #region Params

    public const long TicksInGlobalSecond = 10000000L;

    // Текущие времена
    public static float TimeFromStartup;

    /// <summary> Глобальная секунда </summary>
    public static int Now; // => DateTime.UtcNow.Ticks / TicksInGlobalSecond;
    public static long NowTick => DateTime.UtcNow.Ticks; // todo убрать, т.к. не держит смену времени на устройствах

    private static long UtcNowSecond => DateTime.UtcNow.Ticks / TicksInGlobalSecond;

    private static long GlobalStartConst = new DateTime(2020, 1, 1).Ticks / TicksInGlobalSecond;
    private static long GlobalStart = GlobalStartConst;

    #region Время
    public const int Minute = 60; // todo 60
    public const int Hour = Minute * 60; // todo 60
    public const int Day = Hour * 24;
    public const int Month = Day * 30;
    public const int Week = Day * 7;
    /// <summary> для отсчета глобальной секунды от 2000 года </summary>
    //public const long Year2000 = 2000L * 730485L * 24L * 3600L;
    //public const int Years50 = 365 * 24 * 60 * 60 * 50; 
    #endregion

    #endregion

    #region After
    public static int After(this int seconds) => Now + seconds;
    //public static long AfterTick(this long seconds) => DateTime.UtcNow.Ticks + seconds * TickInGlobalSecond;
    public static int After(this float seconds) => Now + (int)seconds;
    #endregion

    #region SetNow
    public static void SetNow(int now) => GlobalStart = UtcNowSecond - now;
    public static void SetNow()
    {
        Now = (int)(UtcNowSecond - GlobalStart);
        //CLog.Info($"Now={Now}, S={GlobalStart}, SC={GlobalStartConst}, UtcNow={UtcNowSecond}");
    }
    #endregion

    #region To
    public static int TicksToGlobalSecond(this long ticks) => (int)(ticks / TicksInGlobalSecond - GlobalStart);
    public static int ToGlobalSecond(this DateTime t) => t.Ticks.TicksToGlobalSecond();
    /// <summary> Получает глобальную секунду, которая будет через seconds секунд </summary>
    public static DateTime GlobalSecondToDateTime() => Now.GlobalSecondToDateTime();
    public static DateTime GlobalSecondToDateTime(this int seconds) => new DateTime((GlobalStart + seconds) * TicksInGlobalSecond);
    public static TimeSpan GlobalSecondToTimeSpan(this int seconds) => new TimeSpan(seconds * TicksInGlobalSecond);
    public static int GlobalSecondsFromNow(this int seconds) => seconds - Now;
    //public static int GlobalSecondsFromNow(this float seconds) => seconds - (float)Now; 
    #endregion

    #region ToTimeString

    static string ч = "ч", м = "м", д = "д";

    public static string ToTimeStringH(this int s) =>
        s > Hour ? $"{s / Hour}{ч}" : $"{s / Minute}{м}";
    public static string ToTimeStringD(this int s) =>
        s > Day ? $"{s / Day}{д}" : $"{s / Hour}{ч}";
    public static string ToTimeStringDHM(this int s) =>
        $"{s / Day}{д} {(s % Day) / Hour:D2}:{(s % Hour) / Minute:D2}";
    /// <summary> 0:00 </summary> 
    public static string ToTimeStringHM(this float t) => ToTimeStringHM((int)t);
    /// <summary> 0:00 </summary> 
    public static string ToTimeStringHM(this int t)
    {
        var s = t < 0 ? 0 : t;
        return $"{s / Hour}:{(s % Hour) / Minute:D2}";
    }
    /// <summary> 0:00:00 </summary> 
    public static string ToTimeStringHMS(this float t) => ToTimeStringHMS((int)t);
    /// <summary> 0:00:00 </summary> 
    public static string ToTimeStringHMS(this int t)
    {
        return t <= 0 ? "0:00:00" : $"{t / Hour}:{(t % Hour) / Minute:D2}:{t % Minute:D2}";
    }
    /// <summary> 0:00 </summary> 
    public static string ToTimeStringHS(this float t) => ToTimeStringHS((int)t);
    /// <summary> 0:00 </summary> 
    public static string ToTimeStringHS(this int t)
    {
        if (t <= 0) return "0:00";
        var h = t / Hour;
        if (h > 0) return $"{t / Hour}:{(t % Hour) / Minute:D2}";
        return $"{t / Minute}:{t % Minute:D2}";
    }

    #endregion

    #region Debug
    public static string DebugInfo() =>
        $"SC={GlobalStartConst} => S={GlobalStart} (delta={GlobalStartConst - GlobalStart}) + Now={Now} ({(UtcNowSecond - GlobalStart) / int.MaxValue}%)";

    #endregion
}