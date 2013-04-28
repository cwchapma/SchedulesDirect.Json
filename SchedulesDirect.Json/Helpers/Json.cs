using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulesDirect.Json.Helpers
{
    static class MyJson
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include };

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, jsonSettings);
        }

        public static T Deserialize<T>(string s)
        {
            return JsonConvert.DeserializeObject<T>(s, jsonSettings);
        }
    }
}
