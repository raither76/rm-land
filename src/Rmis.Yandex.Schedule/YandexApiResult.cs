using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rmis.Yandex.Schedule
{
    public class YandexApiResult<TBody>
    {
        public Pagination pagination { get; set; }

        public List<TBody> segments { get; set; }
    }
}