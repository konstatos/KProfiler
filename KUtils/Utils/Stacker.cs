using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

public class Stacker
{
    public class StackItem
    {
        public String Module;       // модуль, в котором находится данная функция
        public String ClassName;    // имя класса, в котором находится данная функция
        public String MethodName;   // имя данной функции, из которой будет вызвана следующая функция
        public String Params;       // сюда положим текстовый список параметров данной функции
        public String FileName;     // файл, в котором находится данная функция
        public Int32 Line;          // строка, в которой вызвана следующая функция

        public override string ToString()
        {
            return string.Format("{0}.{1}({2}) {3}", ClassName, MethodName, Params,
                (FileName != null ? FileName + " (" + Line.ToString() + ")" : ""));
        }
    }

    public static string GetStackString5Lines() => GetStackString(4, 5);
    public static string GetStackString2Lines() => GetStackString(4, 2);
    // получить коллстек функций для текущего потока в виде строки
    public static string GetStackString(int skip = 2, int take = 20, int skiplast = 0)
    {
        StringBuilder hStrBld = new StringBuilder(Environment.NewLine);
        StackItem[] stack = GetStack(skip, take, skiplast);
        foreach (StackItem item in stack)
            hStrBld.Append($"*> {item}{Environment.NewLine}");
        return hStrBld.ToString();
    }

    // получить коллстек функций
    public static StackItem[] GetStack(int skip = 2, int take = 20, int skiplast = 0)
    {
        List<StackItem> stackList = new List<StackItem>();
        StackTrace st = new StackTrace(skip, true);
        var n = Math.Min(take, st.FrameCount - skiplast);
        for (Int32 i = 0; i < n; ++i)
        {
            StackItem item = GetStackItem(st.GetFrame(i));
            if (item != null)
                stackList.Add(item);
        }
        return stackList.ToArray();
    }

    // сформировать данные о фрейме стека
    private static StackItem GetStackItem(StackFrame sf)
    {
        if (sf == null)
            return null;
        MethodBase method = sf.GetMethod();
        if (method == null || method.ReflectedType == null)
            return null;

        // получить информацию о данном фрейме стека
        StackItem item = new StackItem();
        item.Module = method.Module.Assembly.FullName;
        item.ClassName = method.ReflectedType.Name;
        item.MethodName = method.Name;
        item.FileName = sf.GetFileName();
        item.Line = sf.GetFileLineNumber();

        // получить параметры данного метода
        StringBuilder parms = new StringBuilder();
        ParameterInfo[] paramsInfo = method.GetParameters();
        for (Int32 i = 0; i < paramsInfo.Length; i++)
        {
            ParameterInfo currParam = paramsInfo[i];
            parms.Append(currParam.ParameterType.Name);
            parms.Append(" ");
            parms.Append(currParam.Name);
            if (i != paramsInfo.Length - 1)
                parms.Append(", ");
        }

        item.Params = parms.ToString();
        return item;
    }
}