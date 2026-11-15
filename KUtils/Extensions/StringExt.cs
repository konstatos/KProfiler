using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public enum EStringFormat { Names, Values, All, Props }

public struct Int2
{
    public int Id;
    public int N;

    public Int2(string data) : this()
    {
        var ids = data.Split(',');
        if (ids.Length < 2)
            CLog.Error("TryParseToInt2: " + data);
        Id = ids[0].TryParseToInt();
        N = ids.Length > 1 ? ids[1].TryParseToInt() : 0;
    }

    public string ToCsv() => $"{Id},{N}";
}

public struct Int3
{
    public int N1, N2, N3;

    public Int3(string data) : this()
    {
        var ids = data.Split(',');
        if (ids.Length < 3)
            CLog.Error("TryParseToInt3: " + data);
        N1 = ids[0].TryParseToInt();
        N2 = ids.Length > 1 ? ids[1].TryParseToInt() : 0;
        N3 = ids.Length > 2 ? ids[2].TryParseToInt() : 0;
    }

    public override string ToString() => $"{N1},{N2},{N3}";
}

/// <summary>
/// Расширения для строк
/// (c) Konstatos
/// </summary>
public static class StringExt
{
    #region Format
    public static string TryFormat(this string text, params object[] format)
    {
        try
        {
            return string.Format(text, format);
        }
        catch (Exception e)
        {
            CLog.Error($"TryFormat {text}: {e}");
            return text;
        }
    }

    public static string JsonFormatRemoveLines(this string text) =>
        text.Replace("{\r\n ", "{").Replace("\r\n}", "}").Replace("{\n ", "{").Replace("\n}", "}");
    public static string JsonFormatAddLines(this string text) =>
        text.Replace(":[{", ":[\n{").Replace("},", "},\n");
    #endregion

    #region Other

    private static readonly Regex MatchEmail =
        new Regex("^([a-z0-9_-]+\\.)*[a-z0-9_-]+@[a-z0-9_-]+(\\.[a-z0-9_-]+)*\\.[a-z]{2,6}$");
    public static bool IsValidEmail(this string s) => MatchEmail.IsMatch(s);
    public static bool IsValidPassword(this string s) => s.Length > 4;

    public static string SubstringFirst(this string s, int count) =>
        s.Length > count ? s.Substring(0, count - 1) + '…' : s;

    public static string SplitUpperCased(this string inputString)
    {
        var newstring = string.Empty;
        for (var i = 0; i < inputString.Length; i++)
        {
            if (char.IsUpper(inputString[i]))
                newstring += " ";
            newstring += inputString[i].ToString();
        }
        return newstring;
    }

    #endregion

    #region Color
    private static readonly Regex MatchColorAll = new Regex("<.?color(.{0,10})>");
    private static readonly Regex MatchColorAndSize = new Regex("<.?color(.{0,10})>|<.?size=..?.?>");
    private static readonly Regex MatchColorBegin = new Regex("<color=#(.{0,8})>");
    private static readonly string MatchColorEnd = "</color>";

    public static string RemoveColorTags(string s) =>
        MatchColorAndSize.Replace(s, "");
    public static string ReplaceColors(string res)
    {
        res = res.Replace(MatchColorEnd, "]");
        res = MatchColorBegin.Replace(res, "[");
        return res;
    }

    #endregion

    #region InTag
    public static string InColorY(this string text) => "<color=#FFFF00>" + text + "</color>";
    public static string InColor(this string text, string color) => $"<color=#{color}>{text}</color>";
    public static string InColor(this char text, string color) => $"<color=#{color}>{text}</color>";
    public static string InSize(this string text, int size) => $"<size={size}>{text}</size>";
    public static string InColorBack(this string text) => text?.InColor("ff0");
    public static string InColorW(this string text) => text?.InColor("fff");
    public static string InColorWS(this string text) => text?.InColor("fc0");
    public static string InTitle(this string text) => "<color=#fc0><b>" + text + "</b></color>";
    #endregion

    #region Split
    private static readonly char[] separatorRow = { '\n', '\r' };
    private static readonly char[] separatorCell = { '\t' }; //, ';'

    public static Int2 TryParseToInt2(this string data) => new Int2(data);
    public static Int3 TryParseToInt3(this string data) => new Int3(data);

    public static List<string[]> SplitTable(this string data)
    {
        var t = data.Split(separatorRow, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split(separatorCell));
        return t.ToList();
    }
    public static List<string[]> SplitTable(this string data, char row, char col)
    {
        var t = data.Split(row).Select(x => x.Split(col));
        return t.ToList();
    }

    public static string[] SplitList(this string data) =>
        data.Split(separatorRow, StringSplitOptions.RemoveEmptyEntries);
    public static string[] SplitList(this string data, char separator) => data.Split(separator);

    #endregion

    #region ToString

    public static string ToStringKb(this int t) => $"{t / 1024}kb";
    public static string ToRim(this int t)
    {
        switch (t)
        {
            case 0: return "-";
            case 1: return "I";
            case 2: return "II";
            case 3: return "III";
            case 4: return "IV";
            case 5: return "V";
            case 6: return "IV";
            case 7: return "IIV";
            case 8: return "IIIV";
            case 9: return "IX";
            case 10: return "X";
            default: return t.ToString();
        }
    }

    public static string ToStringFileName(this DateTime t) => t.ToString("yyyy-MM-dd_hh-mm-ss");
    //public static string ToStringPlusMinus(this int i) => i > 0 ? "+" + i : i.ToString();
    public static string ToStringPlusMinus(this int t) => t > 0 ? "+" + t : t.ToString();
    public static string ToStringPlusMinus(this float t, int round)
    {
        t = (float)Math.Round(t, round);
        return t > 0 ? "+" + t : t.ToString();
    }
    public static string ToStringPlusMinus(this float t)
    {
        t = (int)(t + 0.5f);
        return t > 0 ? "+" + t : t.ToString();
    }
    public static string ToStringProc(this float t) => $"{100 * t:f0}%";
    //public static string ToStringPlusMinusProc(this float t)
    //{
    //    t = (int)(100f * t + 0.5f);
    //    return t > 0 ? $"+{t}%" : $"{t}%";
    //}
    public static string ToDollarString(this int cost) => "$" + cost / 100f;
    public static string ToCostString(this int cost)
    {
        if (cost > 9999999) return cost / 1000000 + "M";
        if (cost > 9999) return cost / 1000 + "K";
        return cost.ToString();
    }

    public static string ToStringAll<T>(this T x, string separator = ", ") =>
        x == null ? "ToStringAll: NULL" :
        string.Join(separator, x.GetType().GetFields()
            .Where(f => !f.IsStatic).Select(y => $"{y.Name}: {ToString(x, y)}"));
    public static string ToString<T>(this T x, EStringFormat format, string separator = ", ")
    {
        switch (format)
        {
            case EStringFormat.Names:
                return string.Join(separator, x.GetType().GetFields().Select(y => y.Name));
            case EStringFormat.Values:
                return string.Join(separator, x.GetType().GetFields().Select(y => ToString(x, y)));
            case EStringFormat.Props:
                return string.Join(separator, x.GetType().GetProperties().Where(y => y.GetMethod != null).Select(y => ToString(x, y)));
            default: return x.ToStringAll(separator);
        }
    }
    private static string ToString<T>(T x, System.Reflection.FieldInfo y) => ObjToString(y.GetValue(x));
    private static string ToString<T>(T x, System.Reflection.PropertyInfo y) => ObjToString(y.GetValue(x));

    private static string ObjToString(object v)
    {
        if (v == null) return "[null]";
        if (v is ICollection) return $"[{((ICollection)v).Count}]";
        return v.ToString();
    }
    public static string ToStringJoin<T>(this IEnumerable<T> x, string separator = ", ") =>
        x == null ? "null" : string.Join(separator, x);

    #endregion

    #region 123->ABC
    private static readonly string ABC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static string ToAbc(this int i)
    {
        var n = ABC.Length;
        if (i < 0) i = 0;
        return i < n ? ABC[i].ToString() :
            ABC[i / n - 1] + ABC[i % n].ToString();
    }
    #endregion

    #region //
    //public static T FromStringAll<T>(this string[] x) where T : new()
    //{
    //    var t = new T();
    //    foreach (var p in typeof(T).GetProperties())
    //    {
    //        p.SetValue(t, )
    //    }
    //    x == null ? "FromStringAll: NULL" :
    //    string.Join(separator, .Select(y => $"{y.Name}: {ToString(x, y)}"));
    //}

    //public static string ToStringFields<T>(this T x, string separator = "\n") =>
    //    string.Join(separator, x.GetType().GetFields().Select(y => $"{y.Name}: {y.GetValue(x)}"));
    //public static string ToStringFieldNames<T>(this T x, string separator = "\t") =>
    //    string.Join(separator, x.GetType().GetFields().Select(y => y.Name));
    //public static string ToStringFieldValues<T>(this T x, string separator = "\t") =>
    //    string.Join(separator, x.GetType().GetFields().Select(y => y.GetValue(x))); 
    #endregion
}
