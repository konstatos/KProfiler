
/// <summary>
/// Универсальный механизм сохранения моделей для переопределения на клиенте и сервере
/// (c) konstatos
/// </summary>
public static class Saver
{
    public static ISave I; // = new SaveDebug();

    //public static Func<object, string> DToJson;
    //public static Func<string, Type, object> DFromJson;
    //public static Func<string, string> OpenResource = LoadTextFile;
    //public static Func<string, List<string>> LoadAll = LoadTextFiles;

    public static T DeepClone<T>(this T o) => I.FromJson<T>(I.ToJson(o));
    public static string ToJson(this object o) => I.ToJson(o);
    public static T FromJson<T>(this string s) => I.FromJson<T>(s);
}

//public class SaveDebug : ISave
//{
//    public void SaveString(string path, string json) => File.WriteAllText(path, json);
//    public void Save<T>(string path, T obj) => SaveString(path, obj.ToJson());
//    public string LoadString(string path) => File.ReadAllText(path);
//    public T Load<T>(string path) => LoadString(path).FromJson<T>();
//    public bool Exists(string path) => File.Exists(path); // + ".*"
//    public void Delete(string path) => File.Delete(path);

//    public string LoadResource(string path)
//    {
//        //try
//        //{
//        var ff = Directory.GetFiles(Path.GetDirectoryName(path), Path.GetFileName(path) + "*");
//        var f = ff.GetMin(x => x.Length);
//        if (f != null) return File.ReadAllText(f);
//        var er = "!!! No file: " + path;
//        CLog.Error(er);
//        return er;
//        //}
//        //catch (Exception e)
//        //{
//        //    var er = $"NoFile: {path} - {e.Message}";
//        //    CLog.Error(er);
//        //    return er;
//        //}
//    }

//    public List<string> LoadResources(string path)
//    {
//        var l = new List<string>();
//        foreach (var f in Directory.GetFiles(Path.GetDirectoryName(path)))
//        {
//            if (f.EndsWith(".meta")) continue;
//            var s = File.ReadAllText(f);
//            l.Add(s);
//        }
//        return l;
//    }
//}