using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class EnumExt
{
    #region Common

    public static List<T> GetEnumMembers<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    #endregion

    #region Test
    private static readonly HashSet<string> NotEnum = new HashSet<string>()
    {
        "No",
    };

    /// <summary> Очень тестовый метод </summary>
    public static void ForAllEnumsGettersIn(Type type,
        Action<Type, MethodInfo, object, object> onGet)
    {
        foreach (var m in type.GetMethods())
        {
            if (m.Name == "GetRandomReplica") continue;
            if (m.Name == "GetAllReplicas") continue;
            if (m.Name == "GetReplikaRecord") continue;
            if (m.Name == "GetReplika") continue;
            if (m.Name == "GetBossFindMessages") continue;
            if (m.Name == "GetBadReplika") continue;
            if (m.Name == "LoadInfoHero") continue;
            if (m.Name == "LoadInfoHeroDisc") continue;
            if (m.Name == "LoadInfoTutor") continue; // todo
            if (m.Name == "LoadStatInnerInfo") continue; // todo
            if (m.Name == "LoadMapData") continue; // todo

            var pp = m.GetParameters();
            if (pp.Length != 1) continue;
            var p = pp[0].ParameterType;
            if (!p.IsEnum) continue;

            var ee = p.GetEnumValues();
            var list = (System.Collections.IList)ee;
            for (var i = 1; i < list.Count; i++)
            {
                var e = list[i];
                var es = e.ToString();
                if (NotEnum.Contains(es)) continue;
                if (es.StartsWith("Z") || es.StartsWith("S") || es.StartsWith("X") || es.StartsWith("Terra") ||  // todo
                    es.StartsWith("Rating")) continue;
                var info = m.Invoke(null, new[] { e });
                if (info == null) throw new Exception("Not info: " + m.Name);
                onGet(p, m, e, info);
            }
        }
    }
    #endregion
}
