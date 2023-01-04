using BurgerShop.Data;
using BurgerShop.Services.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace BurgerShop.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMvcWithOptions(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddMvcOptions(options =>
                {
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                        _ => "Введите все необходимые поля");
                });

            services.AddDistributedMemoryCache();
            services.AddSession(optons =>
            {
                optons.IdleTimeout = TimeSpan.FromMinutes(10);
            });

            return services;
        }

        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
        {
            services.AddSingleton<IClaimsPrincipalGenerator, ClaimsPrincipalGenerator>();
            services.AddSingleton<IPasswordHandler, PasswordHandler>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/auth/login";
                    options.LogoutPath = "/auth/logout";
                });
            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddDbConnection(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton(_ => 
                new UserContext(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton(_ =>
                new MenuContext(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
