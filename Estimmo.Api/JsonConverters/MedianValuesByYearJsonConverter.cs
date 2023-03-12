using Estimmo.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Estimmo.Api.JsonConverters
{
    public class MedianValuesByYearJsonConverter : JsonConverter<IEnumerable<IValueStatsByYear>>
    {
        public override IEnumerable<IValueStatsByYear> Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<IValueStatsByYear> value,
            JsonSerializerOptions options)
        {
            var dictionary = value
                .GroupBy(v => v.Type)
                .ToDictionary(v => (short) v.Key, v => v.ToDictionary(v2 => v2.Year, v2 => v2.Median));

            var json = JsonSerializer.Serialize(dictionary);
            writer.WriteRawValue(json);
        }
    }
}
