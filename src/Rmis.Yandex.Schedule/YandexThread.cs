namespace Rmis.Yandex.Schedule
{
    public class YandexThread
    {
        public string number { get; set; }

        public string title { get; set; }

        public YandexCarrier carrier { get; set; }

        public string transport_type { get; set; }

        /// <summary>
        /// Получение первых 3-х цифрм номера маршрута, для соответствия с тем что в реестре.
        /// </summary>
        /// <returns></returns>
        public string GetNormalizedNumber()
        {
            return this.number.Substring(0, 3);
        }
    }
}