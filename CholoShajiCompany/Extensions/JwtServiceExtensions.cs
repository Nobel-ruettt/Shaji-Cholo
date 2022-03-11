using System;
using System.Text;
using CholoShajiCore.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CholoShajiCompany.Extensions
{
    public static class JwtServiceExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services,string jwtSecret)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = GetJwtOptionsTokenValidationParameters(jwtSecret);
            });
        }

        private static TokenValidationParameters GetJwtOptionsTokenValidationParameters(string jwtSecret)
        {
            var option = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero
            };
            return option;
        }
    }
}
