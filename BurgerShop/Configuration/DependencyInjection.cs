using BurgerShop.Data;
using BurgerShop.Data.Repositories;
using BurgerShop.Services.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
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
            services.AddSingleton<IUserRepository>(_ => 
                new UserRepository(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IMenuRepository>(_ =>
                new MenuRepository(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IOrderRepository>(_ =>
                new OrderRepository(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IPurchaseRepository>(_ =>
                new PurchaseRepository(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IBurgerRepository>(_ =>
                new BurgerRepository(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IRecipesRepository>(_ =>
                new RecipesRepository(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}