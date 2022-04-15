using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Middleware;
using asp_net_po_schedule_management_server.Dto.AuthDtos;
using asp_net_po_schedule_management_server.Dto.Validators;
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
            Configuration.GetSection("ServerConfiguration").Bind(new GlobalConfigurer());
            services.AddControllers()
                .AddFluentValidation()
                // ignorowanie serializacji JSONów w przypadku zapętlonych referencji (wstawianie nulla)
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            
            // strefa autentykacji i blokowania tras oraz odblokowywania przez JWT
            services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManagerImplementation());
            JwtAuthenticationManagerImplementation.ImplementsJwtOnStartup(services);
            services.AddScoped<IPasswordHasher<Person>, PasswordHasher<Person>>();
            
            // strefa dodawania serwisów i ich implementacji
            services.AddScoped<IAuthService, AuthServiceImplementation>();

            // strefa dodawnia middleware'ów
            services.AddScoped<ExceptionsHandlingMiddleware>();
            services.AddAutoMapper(this.GetType().Assembly);
            
            // strefa dodawania customowych walidatorów Dtos'ów
            services.AddScoped<IValidator<ChangePasswordRequestDto>, ChangePasswordRequestDtoValidator>();
            
            // Dodawanie kontekstu bazy danych
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("MySequelConnection"),
                    new MySqlServerVersion(GlobalConfigurer.DbDriverVersion)
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
            seeder.Seed(); // seedowanie (umieszczanie) początkowych danych do encji bazy danych

            if (!env.IsDevelopment()) { // przekierowanie na adres szyfrowany SSL (tylko na produkcji)
                app.UseHttpsRedirection();
            }
            
            // przechwytywanie globalnych wyjątków aplikacji i wyświetlanie ich w formie zserializowanego JSONa
            app.UseMiddleware<ExceptionsHandlingMiddleware>();

            app.UseAuthentication();
            app.UseRouting();

            // ustawianie polityki cors
            app.UseCors(options => options
                .SetIsOriginAllowed(url => env.IsDevelopment() 
                    ? url == $"http://localhost:{GlobalConfigurer.AngularPort}"     // dla wersji developerskiej
                    : url == $"https://{GlobalConfigurer.AngularProductionUrl}")    // dla wersji produkcyjnej
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
            
            app.UseAuthorization();
            
            // umożliwia mapowanie endpointów na podstawie annotacji w kontrolerach
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}