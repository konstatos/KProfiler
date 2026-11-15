using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    //public enum EMatrixTest
    //{
    //    OptSound,
    //    OptLight,
    //    OptQuality,
    //    OptHP,
    //    OptFPS,
    //}

    public class MatrixTest
    {
        public int Default;
        public bool IsAcitive;
        public string Name = null!;
        public int[] Values = null!;
        public List<int> Active = null!;

        public override string ToString() =>
            $"{Name}: {Default} ({Values.ToStringJoin()} / {Active.ToStringJoin()})";
    }

    [Serializable]
    public class MatrixTestResult
    {
        public string Test = null!;
        public int Value;

        public override string ToString() => Test + "=" + Value;
    }

    public class MatrixTestResults
    {
        public int Donat;
        public int Created;
        public int DaysInGame;
        public int TargetValue;
        public Dictionary<string, int> AB = null!;
    }

    public static class MatrixTestsExt
    {
        public static Dictionary<string, int> GetNewAB(this List<MatrixTest> activeTests)
        {
            var alt = new Dictionary<string, int>();
            foreach (var v in activeTests)
            {
                var id = v.Name;
                if (v.Active.Count > 0)
                    alt[id] = v.Active.RandomS();
            }
            return alt;
        }

        public static List<MatrixTestResult>? GetNewABList(this List<MatrixTest> activeTests)
        {
            var alt = new List<MatrixTestResult>();
            foreach (var v in activeTests)
            {
                var id = v.Name;
                if (v.Values.Length > 0)
                    alt.Add(new MatrixTestResult() { Test = id, Value = v.Values.RandomS() });
            }
            return alt;
        }

        public static void CalcAciveAB(this List<MatrixTest> activeTests,
            IEnumerable<MatrixTestResults> results,
            int dopDefaults = 2, int resultsForDop = 5)
        {
            var byAb = GetResultsMatrix(results);
            foreach (var t in activeTests)
            {
                for (int i = 0; i < dopDefaults; i++)
                    t.Active.Add(t.Default);
                if (!byAb.TryGetValue(t.Name, out var ab)) continue;
                if (ab.Count < resultsForDop) continue;
                //var res = ab.Select(x => new { Val = x.Key, Target = x.Value.Average(x => x.TargetValue) })
                //    .OrderBy(x => x.Target).ToList();                
                var res = ab.Select(x => new
                {
                    Val = x.Key,
                    x.Value.Count,
                    Target = (float)x.Value.Average(x => x.TargetValue),
                }).GetMin(x => -x.Target);
                var n = res.Count / resultsForDop;
                for (int i = 0; i < dopDefaults; i++)
                    t.Active.Add(res.Val);
            }
        }

        /// <summary> тест - вариант - результаты </summary>
        private static Dictionary<string, Dictionary<int, List<MatrixTestResults>>> GetResultsMatrix(IEnumerable<MatrixTestResults> results)
        {
            var rr = results.OrderBy(x => x.TargetValue).ToList();
            var byAb = new Dictionary<string, Dictionary<int, List<MatrixTestResults>>>();
            foreach (var r in rr)
            {
                if (r.AB == null) continue;
                foreach (var t in r.AB)
                {
                    var ab = byAb.GetAddValue(t.Key, () => new Dictionary<int, List<MatrixTestResults>>());
                    var val = ab.GetAddValue(t.Value, () => new List<MatrixTestResults>());
                    val.Add(r);
                }
            }
            return byAb;
        }

        public static string GetActiveAbString(this List<MatrixTest> activeTests) =>
            activeTests.Select(x => @$"{x.Name} {x.Default} -> {x.Active.GroupBy(y => y)
                .Select(y => $"{y.Key}({y.Count()})").ToStringJoin()}").ToStringJoin("\n");

        public static string GetResultsAbString(this List<MatrixTest> allTests,
            IEnumerable<MatrixTestResults> results)
        {
            var sb = new StringBuilder();
            var byAb = GetResultsMatrix(results);
            sb.AppendLine("Тест\tТип\tРазы\tЦель\tДни\tДонат\tПоследняя регистрация");
            foreach (var ab in byAb)
            {
                var t = allTests.Find(x => x.Name == ab.Key);
                foreach (var val in ab.Value)
                {
                    var s = t == null ? "-" :
                        val.Key == t.Default ? "def" :
                        t.IsAcitive ? "" : "off";
                    var v = val.Value;
                    var target = v.Average(x => x.TargetValue);
                    var enter = v.Max(x => x.Created).GlobalSecondToDateTime().ToShortDateString();
                    var days = v.Average(x => x.DaysInGame);
                    var donat = v.Average(x => x.Donat);
                    sb.AppendLine($"{ab.Key} {val.Key}\t{s}\t{v.Count}\t{target}\t{days}\t{donat}\t{enter}");
                }
            }
            return sb.ToString();
        }
    }
}
