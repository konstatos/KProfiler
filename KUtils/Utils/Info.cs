using System;

/// <summary>
/// Класс для загрузки локализации описания объектов
/// (c) konstatos
/// </summary>
[Serializable]
public class Info
{
    private const char Separator = ';';
    public const char NewLine = '\\';
    public string Name;
    public string Description;
    public string? Back;

    public Info() { }

    public Info(string name, string description, string? back = null)
    {
        Name = name;
        Description = description;
        Back = back;
    }

    public Info(string[] t)
    {
        Name = t[1];
        Description = t.Length > 2 ? t[2].Replace(NewLine, '\n') : "---";
        Back = t.Length > 3 ? t[3].Replace(NewLine, '\n') : null;
    }

    public string FullInfo(string sep = ". ") =>
        Name + sep + Description;
    public string FullInfoFormat(string color) =>
        $"<color=#{color}>{Name}</color>\n{Description}";
    public string FullInfoFormat(int size, string color = "ffd359") =>
        $"<color=#{color}><b><size={size}>{Name}</size></b></color>\n{Description}";
    public string FullInfoFormat() =>
        $"<color=#ffd359><b>{Name}</b></color>\n{Description}";

    public override string ToString() => FullInfo();
    public string Save() => Name + Separator + Description;
    public static Info Parse(string source)
    {
        var s = source.Split(Separator);
        return new Info()
        {
            Name = s[0],
            Description = s[1]
        };
    }
}
