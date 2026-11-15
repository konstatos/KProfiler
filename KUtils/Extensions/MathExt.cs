using System;
using System.Globalization;

public static class MathExt
{
    #region Power2
    //public static int NearestPowerOf2(this int x)
    //{
    //    return 1 << (sizeof(int) * 8 - BitOperations.LeadingZeroCount(x - 1));
    //}

    public static int CeilPower2b(this int x)
    {
        return x < 2 ? 1 : (int)Math.Pow(2, (int)Math.Log(x - 1, 2) + 1);
    }

    public static int FloorPower2(this int x)
    {
        return x < 1 ? 1 : (int)Math.Pow(2, (int)Math.Log(x, 2));
    }

    public static int CeilPower2(this int x)
    {
        if (x < 0) { return 0; }
        --x;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        x |= x >> 32;
        x |= x >> 64;
        x |= x >> 128;
        return x + 1;
    } 
    #endregion

    #region Math

    public static int Kv(this int x) => x * x;
    public static float Kv(this float x) => x * x;
    public static int Min(int v1, int v2, int v3) =>
        v1 < v2 ? v1 < v3 ? v1 : v3 : v2 < v3 ? v2 : v3;

    /// <summary> Сложный процент = x * (1 + percentage)n </summary>
    public static double CompoundInterest(this double x, double percentage, int n) =>
        x * Math.Pow(1 + percentage, n);

    /// <summary> Сложный процент = x * (1 + percentage)n </summary>
    public static int CompoundInterest(this int x, double percentage, int n) =>
        (int)Math.Round(CompoundInterest((float)x, percentage, n));

    /// <summary> Сложный процент для умножения на число = (1 + percentage)n </summary>
    public static double CompoundInterest(double percentage, int n) =>
        Math.Pow(1 + percentage, n);

    ///// <summary> Умножает V1 и V2 на y </summary>
    //public static void Mul(this Atribute2<int> x, double y)
    //{
    //    x.V1 = (int)(x.V1 * y);
    //    x.V2 = (int)(x.V2 * y);
    //}

    public static bool IsInRadius(this int x, int y, int radius) => x >= y - radius && x <= y + radius;
    public static bool IsInRadius(this long x, long y, long radius) => x >= y - radius && x <= y + radius;
    public static bool IsInRadius(this double x, double y, double radius) => x >= y - radius && x <= y + radius;
    public static bool IsInRadius(this float x, float y, float radius) => x >= y - radius && x <= y + radius;

    /// <summary> Умножает x на x </summary>
    public static float Kvadrat(this float x) => x * x;

    public static bool IsBetween(this int x, int min, int max) => x >= min && x <= max;
    public static bool IsBetweenNoMax(this int x, int min, int max) => x >= min && x < max;
    public static bool IsBetween(this float x, float min, float max) => x >= min && x <= max;
    public static int Clamp(this int x, int min, int max) => x > min ? x < max ? x : max : min;
    public static float Clamp(this float x, float min, float max) => x > min ? x < max ? x : max : min;
    public static int Abs(this int i) => i < 0 ? -i : i;
    public static float Abs(this float i) => i < 0 ? -i : i;
    public static int Max(this int x, int min) => x < min ? min : x;
    public static int Min(this int x, int max) => x > max ? max : x;
    public static float Max(this float x, float min) => x < min ? min : x;
    public static float Min(this float x, float max) => x > max ? max : x;

    private const float _tolerance = 0.1f;
    public static bool FloatEquals(this float x, float y) => x - _tolerance < y && x + _tolerance > y;

    #endregion

    #region Round

    //public static int CeilToInt(this float arg) => (int)Math.Ceiling(arg);
    public static float Round(this float arg, int decil) => (float)Math.Round(arg, decil, MidpointRounding.AwayFromZero);
    public static double Round(this double arg, int decil) => Math.Round(arg, decil, MidpointRounding.AwayFromZero);
    public static int Round(this float arg) => arg > 0 ? (int)(arg + 0.5f) : (int)(arg - 0.5f);
    public static int Round(this double arg) => arg > 0 ? (int)(arg + 0.5f) : (int)(arg - 0.5f);
    public static int CeilToInt(this float f) => (int)Math.Ceiling(f);
    public static int CeilToInt(this double f) => (int)Math.Ceiling(f);
    //public static string RoundToStr(this float arg, int decimals) => Math.Round(arg, decimals);

    #endregion

    #region TryParse

    public static T TryParseToEnum<T>(this string source) where T : struct
    {
        Enum.TryParse(source, out T e);
        return e;
    }

    public static T TryParseToEnum<T>(this string source, bool ignoreCase) where T : struct
    {
        Enum.TryParse(source, ignoreCase, out T e);
        return e;
    }

    public static byte TryParseToByte(this string source, byte def = 0) =>
        byte.TryParse(source, out var i) ? i : def;

    public static int TryParseToInt(this string source, int def = 0) =>
        int.TryParse(source, out var i) ? i : def;

    /// <summary> InvariantCulture: 0.00 </summary> 
    public static float TryParseToFloat(this string source, float def = 0)
    {
        return float.TryParse(source, NumberStyles.Float,
            NumberFormatInfo.InvariantInfo, out var i) ? float.IsNaN(i) ? def : i : def;
    }

    #endregion
}