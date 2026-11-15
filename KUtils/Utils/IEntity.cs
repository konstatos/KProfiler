using System.ComponentModel;

public class IEntity
{
    //public string _id
    //{
    //    get => __id;
    //    set
    //    {
    //        if (__id != null) CLog.Error($"{GetType().Name} {Name} _id {__id}->{value}");
    //        __id = value;
    //    }
    //}
    public string _id;
    //[DisplayName("Название")]
    public string Name;
    public int Version;

    public void NewId()
    {
        _id = RandomExt.GetGuid();
    }

    public override string ToString()
    {
        return $"{GetType().Name} {Name} ({_id})";
    }
}
