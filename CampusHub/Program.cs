using CampusHub.Application.Interfaces;
using CampusHub.Application.Services;
using CampusHub.Infrastructure.Data;
using CampusHub.Infrastructure.Repositories;
using CampusHub.Presentation.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDropdownService, DropdownService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IYearLevelRepository, YearLevelRepository>();
builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMarketplaceRepository, MarketplaceRepository>();



// Application Services
builder.Services.AddScoped<IMarketplaceService, MarketplaceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
