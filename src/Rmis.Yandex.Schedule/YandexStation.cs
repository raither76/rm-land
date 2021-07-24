using System.Collections.Generic;

namespace Rmis.Yandex.Schedule
{
    public class YandexStation
    {
        public string code { get; set; }

        public string title { get; set; }

        public YandexCode codes { get; set; }

        public object longitude { get; set; }

        public object latitude { get; set; }

        public string transport_type { get; set; }
    }
}