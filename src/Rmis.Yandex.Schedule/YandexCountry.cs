using System.Collections.Generic;

namespace Rmis.Yandex.Schedule
{
    public class YandexCountry
    {
        public YandexCode codes { get; set; }
        
        public string title { get; set; }

        public List<YandexRegion> regions { get; set; }
    }
}