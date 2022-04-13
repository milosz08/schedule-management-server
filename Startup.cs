using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Middleware;
using asp_net_po_schedule_management_server.Services.ServicesImplementation;


namespace asp_net_po_schedule_management_server
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string jwtToken = Configuration.GetConnectionString("JwtKey");
            services.AddControllers();
            
            // strefa autentykacji i blokowania tras oraz odblokowywania przez JWT
            services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManagerImplementation(jwtToken));
            JwtAuthenticationManagerImplementation.ImplementsJwtOnStartup(services, jwtToken);
            
            // strefa dodawania serwisów i ich implementacji
            services.AddScoped<IUserService, UserServiceImplementation>();
            
            // strefa dodawnia middleware'ów
            services.AddScoped<ExceptionsHandlingMiddleware>();

            // Dodawanie kontekstu bazy danych
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("MySequelConnection"),
                    new MySqlServerVersion(GlobalConfigurer.DB_DRIVER)
                ));
            
            // dodawanie seedera jako serwisu
            services.AddScoped<ApplicationDbSeeder>();
            
            // zmienia ścieżki routingu z wielkich liter na małe
            services.AddRouting(options => {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbSeeder seeder)
        {
            // seedowanie początkowych danych do encji bazy danych
            seeder.Seed();
            
            // przechwytywanie globalnych wyjątków aplikacji i wyświetlanie ich w formie zserializowanego JSONa
            app.UseMiddleware<ExceptionsHandlingMiddleware>();
            
            if (!env.IsDevelopment()) {
                app.UseHttpsRedirection();
            }
            app.UseRouting();

            // ustawianie polityki cors
            app.UseCors(options => options
                .SetIsOriginAllowed(url => env.IsDevelopment() 
                    ? url == $"http://localhost:{GlobalConfigurer.ANGULAR_PORT}"    // dla wersji developerskiej
                    : url == $"https://{GlobalConfigurer.ANGULAR_PROD}")            // dla wersji produkcyjnej
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
            
            app.UseAuthorization();
            app.UseAuthentication();
            
            // umożliwia mapowanie endpointów na podstawie annotacji w kontrolerach
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}