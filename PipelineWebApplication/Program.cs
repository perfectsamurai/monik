global using monitoring.Models;
global using monitoring.Data;
global using monitoring.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace monitoring
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<MonitoringSystemContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("MonitoringSystemContext") ?? throw new InvalidOperationException("Connection string 'MonitoringContext' not found.")));
            // Add services to the container.
            // установка конфигурации подключения
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new PathString("/Account/Login");

                });
            builder.Services.AddControllersWithViews();

            var app = builder.Build();





            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();    // аутентификация
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.UseEndpoints(endpoints =>
            {
                // Другие маршруты...

                endpoints.MapControllerRoute(
                    name: "exportToExcel",
                    pattern: "Dynamogram/ExportToExcel",
                    defaults: new { controller = "Dynamogram", action = "ExportToExcel" }
                );
            });

            app.Run();
        }
    }
}