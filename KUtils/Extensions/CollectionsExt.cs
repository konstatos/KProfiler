using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionsExt
{
    #region IEnumerable
    public static T GetMin<T>(this IEnumerable<T> e, Func<T, float> getParametr) //where T : class
    {
        var res = default(T);
        var max = float.MaxValue;
        foreach (var v in e)
        {
            var p = getParametr(v);
            if (p < max)
            {
                max = p;
                res = v;
            }
        }
        return res;
    }

    public static T GetMin<T>(this List<T> List, Func<T, float> getParametr)
    {
        float min = float.MaxValue, dist;
        T res = default(T);
        for (var i = List.Count - 1; i >= 0; i--)
        {
            var v = List[i];
            dist = getParametr(v);
            if (dist < min)
            {
                min = dist;
                res = v;
            }
        }
        return res;
    }

    public static T GetMin<T>(this List<T> List, Func<T, bool> filtr, Func<T, float> getParametr)
    {
        float min = float.MaxValue, dist;
        T res = default(T);
        for (var i = List.Count - 1; i >= 0; i--)
        {
            var v = List[i];
            if (!filtr(v)) continue;
            dist = getParametr(v);
            if (dist < min)
            {
                min = dist;
                res = v;
            }
        }
        return res;
    }

    //public static T GetMax<T>(this IEnumerable<T> List,
    //  Func<T, float> TestFunc)
    //{
    //    float x = float.MinValue, f;
    //    T res = default(T);
    //    foreach (var v in List)
    //        if ((f = TestFunc(v)) > x)
    //        {
    //            x = f;
    //            res = v;
    //        }
    //    return res;
    //}

    public static int IndexOf<T>(this IList<T> e, Func<T, bool> getParametr)
    {
        for (var i = 0; i < e.Count; i++)
            if (getParametr(e[i]))
                return i;
        return -1;
    }

    public static int IndexOfMax<T>(this IEnumerable<T> e, Func<T, float> getParametr)
    {
        var max = float.MinValue;
        var i = 0;
        var res = 0;
        foreach (var v in e)
        {
            var p = getParametr(v);
            if (p > max)
            {
                max = p;
                res = i;
            }
            ++i;
        }
        return res;
    }

    public static int MaxOrDefault<T>(this IEnumerable<T> e, Func<T, int> getParametr, int defolt = 0) =>
        e.Any() ? e.Max(getParametr) : defolt;
    public static int MaxOrDefault<T>(this List<T> e, Func<T, int> getParametr, int defolt = 0) =>
        e.Count > 0 ? e.Max(getParametr) : defolt;

    #endregion

    #region List
    public static T TryGet<T>(this List<T> l, int index) =>
        index.IsBetween(0, l.Count - 1) ? l[index] : l.LastOrDefault();
    public static T TryGet<T>(this T[] l, int index) =>
        index.IsBetween(0, l.Length - 1) ? l[index] : l.LastOrDefault();

    public static int IndexOf<T>(this List<T> l, Func<T, bool> find)
    {
        for (var i = 0; i < l.Count; i++)
            if (find(l[i])) return i;
        return -1;
    }

    public static bool Replace<T>(this List<T> l, Func<T, bool> find, T neu)
    {
        for (var i = 0; i < l.Count; i++)
            if (find(l[i]))
            {
                l[i] = neu;
                return true;
            }
        return false;
    }

    public static bool AddIfNoExists<T>(this List<T> l, T add)
    {
        if (l.Contains(add))
            return false;
        l.Add(add);
        return true;
    }

    public static T GetAdd<T>(this List<T> l, Predicate<T> find, Func<T> getDefolt)
    {
        var x = l.Find(find);
        if (x == null)
            //if (getDefolt == null) l.Add(x = default(T));
            //else 
            l.Add(x = getDefolt());
        return x;
    }

    public static List<T> TakeLast<T>(this List<T> l, int n)
    {
        if (l.Count < n) return l;
        var res = new List<T>();
        for (var i = l.Count - n; i < l.Count; i++)
            res.Add(l[i]);
        return res;
    }

    #endregion

    #region Dictionary
    /// <summary>
    /// Ввиду плохого поведения стандартного линк-ToDictionary, 
    /// благополучно кидающего ексепшен без указания, что дублируется и даже типов данных
    /// </summary>  
    public static Dictionary<TKey, TElement> ToDictionaryLog<TSource, TKey, TElement>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
    {
        var d = new Dictionary<TKey, TElement>();
        foreach (var item in source)
        {
            try
            {
                d.Add(keySelector(item), elementSelector(item));
            }
            catch (Exception e)
            {
                CLog.Error($"ToDictionaryLog: {keySelector(item)} {e}");
            }
        }
        return d;
    }

    public static T TryGetValueOrDefault<TKey, T>(this Dictionary<TKey, T> d, TKey key)
    {
        if (key != null)
            return d.TryGetValue(key, out var res) ? res : default(T);
        CLog.Error("TryGetValueOrDefault: key=null in " + d);
        //throw new Exception("TryGetValueOrDefault: key=null in " + d); 
        return default(T);
    }

    public static T TryGetValueOrDefaultLog<TKey, T>(this Dictionary<TKey, T> d, TKey key)
    {
        CLog.Assert(key != null, () => "TryGetValueOrDefault: key=null in " + d);
        if (key == null) return default(T);
        if (d.TryGetValue(key, out var res)) return res;
        CLog.Error($"TryGetValueOrDefault {typeof(TKey).Name}-{typeof(T).Name}: " +
            $"{key} not found in {d.Count} - [{d.Keys.Take(5).ToStringJoin()}...]");
        return default(T);
    }
    public static T GetAddValue<TKey, T>(this Dictionary<TKey, T> d, TKey key, Func<T> getDefolt)
    {
        return d.TryGetValue(key, out var res) ? res : (d[key] = getDefolt());
    }
    public static int AddCount<T>(this Dictionary<T, int> a, T to) =>
        a.ContainsKey(to) ? a[to]++ : a[to] = 1;
    public static float AddCount<T>(this Dictionary<T, float> a, T to, float add) =>
        a.ContainsKey(to) ? a[to] += add : a[to] = add;
    #endregion

    #region Array
    public static T[] AppendArray<T>(this T[] arr, int newLength)
    {
        if (arr.Length >= newLength) return arr;
        var w = new T[newLength];
        for (var i = 0; i < arr.Length; i++) w[i] = arr[i];
        return w;
    }

    public static T Find<T>(this T[] arr, Func<T, bool> test)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            var a = arr[i];
            if (test(a)) return a;
        }
        return default(T);
    }

    public static bool Exists<T>(this T[] arr, Func<T, bool> test)
    {
        for (var i = arr.Length - 1; i >= 0; i--)
        {
            var a = arr[i];
            if (test(a)) return true;
        }
        return false;
    }

    /// <summary> Immutable - нужно присваивать результат </summary> 
    public static T[] RemoveAt<T>(this T[] source, int index)
    {
        var dest = new T[source.Length - 1];
        if (index > 0)
            Array.Copy(source, 0, dest, 0, index);

        if (index < source.Length - 1)
            Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        return dest;
    }
    #endregion
}