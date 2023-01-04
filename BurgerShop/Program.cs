using BurgerShop.Configuration;
using BurgerShop.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMvcWithOptions()
    .AddDbConnection(builder.Configuration)
    .AddAuthenticationAndAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Init data if not exists
using (var scope = app.Services.CreateScope())
{    
    await DbInitializer.Initialize();
}

// Add midlewares to the HTTP request pipeline
{
    app.UseHttpsRedirection();
    app.UseFileServer();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();
}

// Routing
app.MapControllers();

app.Run();