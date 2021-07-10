using System;

namespace Rmis.Yandex.Schedule
{
    public class YandexSchedule
    {
        public DateTimeOffset arrival { get; set; }

        public YandexStation from { get; set; }

        public YandexThread thread { get; set; }
        
        public DateTimeOffset departure { get; set; }

        public YandexStation to { get; set; }

        public string start_date { get; set; }
    
        public string GetKey()
        {
            return $"{this.thread.number.Substring(0, 3)}_{this.start_date}";
        }
    }

}