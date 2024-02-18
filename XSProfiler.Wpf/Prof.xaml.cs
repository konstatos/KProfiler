using System.Windows.Controls;
using Utils;

namespace XSProfiler.Wpf
{
    /// <summary>
    /// Логика взаимодействия для Prof.xaml
    /// </summary>
    public partial class Prof : UserControl
    {
        private void Log(string s)
        {
            Dispatcher.Invoke(() => _tOut.Text += s + "\n");
        }
        private void LogInit(string s)
        {
            Dispatcher.Invoke(() => _tOut.Text = s); //.ReplaceDots()
        }

        public Prof()
        {
            InitializeComponent();
            TimeExt.SetNow();
            CLog.OnError += s => Log("ERROR: " + s);
            CLog.OnWarning += s => Log(s);
            CLog.OnLocal += s => Log(s);
            LogInit("Запущено");
            void AddMenu(string text, Action action) =>
                _mMenu.AddMenu(text, action);

            AddMenu("Профайлер", ShowProfileTree);
            AddMenu("Ошибки", ShowErrorTree);

            TimeExt.SetNow();
            var min = TimeExt.After(-TimeExt.Day);
            AddMenu("Удалить ошибки", () => WebProf.ErrorRepo.DeleteMany(x => x.Time < min));
            AddMenu("Удалить профайлер", () => WebProf.TimeRepo.DeleteMany(x => x.End < min));
            AddMenu("Удалить профайлер совсем", () => WebProf.TimeRepo.DeleteMany(x => true));
        }

        public void ShowProfileTree()
        {
            var pro = ProfilerAnalizer.GetTimes(_tSrv.Text, 0); //-Times.Hour.After()
            _tData.Items.Clear();
            var max = 20;
            if (pro.Count == 0)
            {
                Log("\nНет данных");
                return;
            }
            if (!pro.TryGetValue("", out var value))
            {
                Log("\nНет корня: " + pro.Keys.ToStringJoin());
                return;
            }
            Add(_tData.Items, value);
            void Add(ItemCollection items, List<ProfileTimes> lists, int m = 0)
            {
                foreach (var p in lists)
                {
                    var n = new TreeViewItem();
                    n.Header = $"{p.TimeP,8:f3}% {p.Time,8:f3}c {p.Count,8} {p.Metod}";
                    items.Add(n);
                    if (m < max && pro.TryGetValue(p.Metod, out var next))
                        Add(n.Items, next, m + 1);
                }
            }
            ShowCommonInfo();
        }

        private void ShowCommonInfo(int from = 0)
        {
            try
            {
                var all = WebProf.TimeRepo.GetAll().Where(x => x.Begin > from).ToList();
                var times = all.GroupBy(x => x.Server)
                    .Select(x => new ServerLoadInfo(TimeProfile.Group(x.ToArray())) { Server = x.Key }).ToList();
                _dData.ItemsSource = times;
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
        }

        private void ShowDetailInfo()
        {
            try
            {
                var br = "\n";
                var all = WebProf.TimeRepo.GetAll().ToList();
                //.Where(x => x.Begin > LastTime).ToList();
                var times = all.GroupBy(x => x.Server)
                    .Select(x => x.Key + " " + x.Last().User + br + TimeProfile.Group(x.ToArray())
                    .GetInfo().Replace("\n", br));
                Log(times.ToStringJoin(br + br));
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
        }

        public void ShowErrorTree()
        {
            var errors = ProfilerAnalizer.GetServerErrors();
            _tData.Items.Clear();
            var items = _tData.Items;
            foreach (var e in errors.OrderByDescending(x => x.Last.Time))
            {
                var n = new TreeViewItem();
                n.Header = $"{-e.Last.Time.GlobalSecondsFromNow() / TimeExt.Day,5}дн " +
                    $"{e.Count,5} {e.Metod}:  {e.Error}";
                items.Add(n);
                var info = new TextBox()
                {
                    Text = e.GetInfo(),
                };
                n.Items.Add(info);
            }
        }
    }
}
