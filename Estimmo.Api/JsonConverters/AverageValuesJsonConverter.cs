using Estimmo.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Estimmo.Api.JsonConverters
{
    public class AverageValuesJsonConverter : JsonConverter<IEnumerable<IAverageValue>>
    {
        public override IEnumerable<IAverageValue> Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<IAverageValue> value,
            JsonSerializerOptions options)
        {
            var dictionary = value.ToDictionary(v => (short) v.Type, v => v.Value);
            var json = JsonSerializer.Serialize(dictionary);
            writer.WriteRawValue(json);
        }
    }
}
