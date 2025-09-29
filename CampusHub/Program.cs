using CampusHub.Application.Interfaces;
using CampusHub.Application.Services;
using CampusHub.Presentation.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using CampusHub.Infrastructure.Data;
using CampusHub.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies; 

var builder = WebApplication.CreateBuilder(args);

// Add Razor Components (Blazor UI)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => options.DetailedErrors = true); 

// Add Controllers (API layer)
builder.Services.AddControllers();

// ADD AUTHENTICATION - Add this section
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

// ADD AUTHORIZATION - Add this section
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IYearLevelRepository, YearLevelRepository>();
builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IMarketplaceRepository, MarketplaceRepository>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDropdownService, DropdownService>();
builder.Services.AddScoped<IMarketplaceService, MarketplaceService>();

// Register HttpClient that points to the same app (hosted together)
builder.Services.AddScoped(sp =>
{
    var navManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navManager.BaseUri) };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ADD AUTHENTICATION MIDDLEWARE - Add these in correct order
app.UseAuthentication();
app.UseAuthorization();

// Controllers (API endpoints)
app.MapControllers();

// Blazor UI
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();