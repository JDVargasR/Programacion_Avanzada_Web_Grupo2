using APW.Architecture;
using APW.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();
builder.Services.AddSession();

builder.Services.AddScoped<IWrapperServiceProvider, WrapperServiceProvider>();
builder.Services.AddScoped<IRestProvider, RestProvider>();

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
