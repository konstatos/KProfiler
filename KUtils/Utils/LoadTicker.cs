namespace Utils
{
    /// <summary>
    /// Добавлялка троеточий к тексту при загрузке,
    /// вызов каждый кадр добавляет троеточия до 3х и потом снова
    /// (c) konstatos
    /// </summary>
    public class LoadTicker
    {
        private int Count;
        private int Max = 4;
        private int Step = 60; // кадры в секунду

        public string GetText(string text)
        {
            return text.PadRight(GetTick());
        }

        private int GetTick()
        {
            return ++Count / Step % Max;
        }
    }
}