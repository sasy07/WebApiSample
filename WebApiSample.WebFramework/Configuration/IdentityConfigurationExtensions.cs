using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WebApiSample.Data;
using WebApiSample.Entities;

namespace WebApiSample.WebFramework.Configuration;

public static class IdentityConfigurationExtensions
{
    public static void AddCustomIdentity(this IServiceCollection services, IdentitySettings settings)
    {
        services.AddIdentity<User, Role>(options =>
            {
                #region password settings

                options.Password.RequireDigit = settings.PasswordRequireDigit;
                options.Password.RequiredLength = settings.PasswordRequiredLength;
                options.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanumeric;
                options.Password.RequireLowercase = settings.PasswordRequireLowercase;
                options.Password.RequireUppercase = settings.PasswordRequireUppercase;

                #endregion

                #region user-name setting

                options.User.RequireUniqueEmail = settings.RequireUniqueEmail;

                #endregion

                #region sign-in settings

                // options.SignIn.RequireConfirmedEmail = false;
                // options.SignIn.RequireConfirmedPhoneNumber = false;

                #endregion

                #region lock account settings

                // options.Lockout.MaxFailedAccessAttempts = 5;
                // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                // options.Lockout.AllowedForNewUsers = false;

                #endregion
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}