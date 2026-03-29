using APW.Architecture;
using APW.Architecture.Services;
using APW.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using APW.Architecture;
using APW.Architecture.Services;
using APW.Web.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<IWrapperServiceProvider, WrapperServiceProvider>();
builder.Services.AddScoped<IRestProvider, RestProvider>();

// Servicio de Exportar
builder.Services.AddHttpClient<IExportImportService, ExportImportService>();

// Servicio de IA
builder.Services.AddHttpClient<IAiEnrichmentService, AiEnrichmentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Authentication", action = "Login" });

app.MapControllerRoute(
    name: "registro",
    pattern: "registro",
    defaults: new { controller = "Authentication", action = "Registro" });

app.MapControllerRoute(
    name: "logout",
    pattern: "logout",
    defaults: new { controller = "Authentication", action = "Logout" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}/{id?}");

app.Run();