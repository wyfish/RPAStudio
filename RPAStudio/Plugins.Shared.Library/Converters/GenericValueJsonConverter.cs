using Newtonsoft.Json;
using Plugins.Shared.Library.Librarys;
using System;
using System.ComponentModel;

namespace Plugins.Shared.Library.Converters
{
    class GenericValueJsonConverter : JsonConverter
    {
        private TypeConverter _converter = TypeDescriptor.GetConverter(typeof(GenericValue));

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GenericValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object value = reader.Value;
            return this._converter.ConvertFrom(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text = this._converter.ConvertToString(value);
            writer.WriteValue(text);
        }
    }
}
