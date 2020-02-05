using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace MaitlandsInterfaceFramework.Core.Converters
{
    public class MillisecondEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - epoch).TotalMilliseconds.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return epoch.AddMilliseconds((long)reader.Value);
        }
    }
}
