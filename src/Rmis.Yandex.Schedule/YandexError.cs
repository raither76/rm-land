namespace Rmis.Yandex.Schedule
{
    public class YandexError
    {
        public string request { get; set; }

        public string text { get; set; }

        public string error_code { get; set; }

        public int http_code { get; set; }
    }
}