using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Shared.Library.Librarys
{
    public static class JsonParser
    {
        public static object DeserializeArgument(object value, Type argumentType)
        {
            return JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(value)).ToObject(argumentType);
        }
    }
}