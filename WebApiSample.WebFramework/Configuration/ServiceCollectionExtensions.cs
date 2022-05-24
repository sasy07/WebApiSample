using System.Text;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace WebApiSample.WebFramework.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services , JwtSettings siteSettings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero, // default : 5 min 
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(siteSettings.SecretKey)),
                RequireExpirationTime = true,
                ValidateAudience = true, // default : false
                ValidAudience = siteSettings.Audience,
                ValidateIssuer = true, // default : false
                ValidIssuer = siteSettings.Issuer
            };
        });
    }
}