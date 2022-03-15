using System;
using System.Text.Json.Serialization;

namespace Estimmo.Api.Entities
{
    public class TokenPayload
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("validTo")]
        public DateTime ValidTo { get; set; }
    }
}
