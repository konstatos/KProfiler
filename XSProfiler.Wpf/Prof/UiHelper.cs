using System.Diagnostics;
using System.Windows.Controls;

namespace XSProfiler.Wpf
{
    public static class UiHelper
    {
        public static Button AddMenu(this StackPanel menu, string text, Action action)
        {
            var i = new Button() { Content = text };
            menu.Children.Add(i);
            i.Click += (s, e) =>
            {
                //LogInit("");
                var sw = Stopwatch.StartNew();
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    CLog.Error("AddMenu: " + ex.ToString());
                }
                //Log($"\nЗавершено за {sw.ElapsedMilliseconds / 1000f} c");
            };
            return i;
        }
    }
}
