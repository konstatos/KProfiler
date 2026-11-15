using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

public static class RandomExt
{
    #region Random  
    // копия того, что в UnityComponentsExt, т.к. UnityEngine.Random не работает вне юнити, но быстрее, чем System.Random

    public static readonly Random Rnd = new Random((int)DateTime.Now.Ticks);

    public static T RandomS<T>(this IEnumerable<T> List) => List.ElementAt(Rnd.Next(List.Count()));
    public static T RandomS<T>(this ICollection<T> list) => list.ElementAt(Rnd.Next(list.Count));
    public static T RandomOrDeafultS<T>(this ICollection<T> list) => list.Count > 0 ? list.ElementAt(Rnd.Next(list.Count)) : default(T);
    public static T RandomS<T>(this IList<T> list, Random r) => list[r.Next(list.Count)];
    public static T RandomS<T>(this T[] list) => list[Rnd.Next(list.Length)];
    public static T RandomOrDeafultS<T>(this T[] list) => list?.Length > 0 ? list[Rnd.Next(list.Length)] : default(T);
    public static T RandomS<T>(this IList<T> List) => List[Rnd.Next(List.Count)];
    public static T RandomS<T>(this IList<T> list, int max) => list[Rnd.Next(Math.Min(list.Count, max))];
    public static T RandomS<T>(this IList<T> list, int min, int max) => list[Math.Min(Rnd.Next(min, max), list.Count - 1)];
    public static T RandomS2<T>(this IList<T> list) => list[Rnd.Next(Rnd.Next(list.Count) + 1)];
    public static T RandomS2<T>(this IList<T> list, int max) => list[Rnd.Next(Rnd.Next(Math.Min(list.Count, max)) + 1)];


    /// <summary> Возвращает случайный объект из листа и убирает его оттуда </summary>
    public static T PeekRandom<T>(this IList<T> list)
    {
        var r = Rnd.Next(list.Count);
        var e = list[r];
        list.RemoveAt(r);
        return e;
    }

    public static T Peek<T>(this IList<T> list, int index)
    {
        var e = list[index];
        list.RemoveAt(index);
        return e;
    }

    public static List<T> GetRandomN<T>(this IList<T> all, int n)
    {
        var rnd = new List<T>();
        for (int i = 0; i < n; i++)
            rnd.Add(all.RandomS());
        return rnd;
    }

    ///// <summary> Попадание random(100) в вероятность от 0 до procent </summary>
    //public static bool RandomProcent(int procent) => UnityEngine.Random.Range(0, 100) < procent;

    #endregion

    public static int Rand100() => Rnd.Next(100);
    /// <summary> 50/50 </summary> 
    public static bool Rand2() => Rnd.NextDouble() > 0.5;
    /// <summary> до - искл </summary> 
    public static int RndRnd(int max) => Rnd.Next(Rnd.Next(max + 1));
    /// <summary> до - искл </summary> 
    public static int Rand(int max) => Rnd.Next(max);
    /// <summary> От - вкл, до - искл </summary> 
    public static int Rand(int min, int max) =>
        min > max ? Rnd.Next(max, min) : Rnd.Next(min, max);
    public static double Rand(double min, double max) =>
        Rnd.NextDouble() * (max - min) + min;
    /// <summary> 0-1 </summary> 
    public static double Rand() => Rnd.NextDouble();
    public static string GetGuid() => Guid.NewGuid().ToString("N");

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Rnd.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
