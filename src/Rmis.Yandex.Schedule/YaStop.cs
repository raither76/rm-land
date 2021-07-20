using System;

namespace Rmis.Yandex.Schedule
{
    public class YaStop
    {
        public double duration { get; set; }

        public double? stop_time { get; set; }

        public YandexStation station { get; set; }

        public string arrival { get; set; }
        
        public string departure { get; set; }
    }
}