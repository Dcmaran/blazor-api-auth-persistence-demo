using BlazorAuthAPI.Auth;
using BlazorAuthAPI.Components;
using BlazorAuthAPI.Models;
using BlazorAuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<UserState>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<CryptographyService>();
builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();  // Certifique-se de usar a autenticação antes da autorização
app.UseAuthorization();

app.MapControllers();  // Mapeia os controllers para serem acessados via rota

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
