using System;

namespace Utils
{
    public class Change
    {
        public string N;
        public object V;

        public Change() { }
        public Change(object t, string n)
        {
            GetFrom(t, n);
        }

        public void GetFrom(object t, string n)
        {
            N = n;
            V = t.GetType().GetField(N).GetValue(t);
        }

        public void SetTo(object t)
        {
            try
            {
                var f = t.GetType().GetField(N);
                if (f.FieldType == typeof(int))
                    V = Convert.ChangeType(V, typeof(int));
                else
                {
                    var vt = V.GetType();
                    if (vt.Name == "JObject")
                    {
                        var methodInfo = vt.GetMethod("ToObject", new Type[] { typeof(Type) });
                        V = methodInfo.Invoke(V, new object[] { f.FieldType });
                    }
                }
                //else if (f.FieldType.IsClass && f.FieldType != typeof(string))
                //    V = 
                f.SetValue(t, V);
            }
            catch (Exception e)
            {
                CLog.Error($"SetTo: {t} <- {this}", e);
            }
        }

        public override string ToString() => $"{N}={V}";
    }
}