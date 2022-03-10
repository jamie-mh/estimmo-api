using Microsoft.Extensions.Configuration;

namespace Estimmo.Api.Options
{
    public class JwtOptions
    {
        public const string Section = "Jwt";

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int ExpirationInHours { get; set; }

        public static JwtOptions FromConfiguration(IConfiguration configuration)
        {
            return configuration.GetSection(Section).Get<JwtOptions>();
        }
    }
}
