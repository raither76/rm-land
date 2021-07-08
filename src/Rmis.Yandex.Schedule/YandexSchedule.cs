using System;

namespace Rmis.Yandex.Schedule
{
    public class YandexSchedule
    {
        public DateTimeOffset arrival { get; set; }

        public YandexStation from { get; set; }
        
        public DateTimeOffset departure { get; set; }

        public YandexStation to { get; set; }
    }

}