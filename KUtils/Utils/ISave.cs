using System.Collections.Generic;
/// <summary>
/// Универсальный механизм сохранения моделей для переопределения на клиенте и сервере
/// (c) konstatos
/// </summary>
public interface ISave
{
    void SaveString(string path, string json);
    string LoadString(string path);
    List<string> LoadResources(string path);
    string LoadResource(string path);

    void Save<T>(string path, T obj);
    T Load<T>(string path);
    bool Exists(string path);
    void Delete(string path);

    string ToJson(object o);
    T FromJson<T>(string s);
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