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
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Middleware;

using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Validators;

using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Services.ServicesImplementation;

using asp_net_po_schedule_management_server.Ssh.SshInterceptor;
using asp_net_po_schedule_management_server.Ssh.SshEmailService;


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
            services.AddScoped<IFilesService, FilesServiceImplementation>();

            // strefa dodawnia middleware'ów
            services.AddScoped<ExceptionsHandlingMiddleware>();
            services.AddAutoMapper(this.GetType().Assembly);
            
            // serwisy dla poczty i socketów SSH
            services.AddScoped<ISshInterceptor, SshInterceptorImplementation>();
            services.AddScoped<ISshEmailService, SshEmailServiceImplementation>();
            
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

            // zezwolenie na politykę CORS
            services.AddCors(options => {
                options.AddPolicy("AngularClient", builder =>
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:8383")
                    );
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbSeeder seeder)
        {
            // ustawianie polityki cors
            app.UseCors("AngularClient");
            
            seeder.Seed().Wait(); // seedowanie (umieszczanie) początkowych danych do encji bazy danych

            if (!env.IsDevelopment()) { // przekierowanie na adres szyfrowany SSL (tylko na produkcji)
                app.UseHttpsRedirection();
            }
            
            // przechwytywanie globalnych wyjątków aplikacji i wyświetlanie ich w formie zserializowanego JSONa
            app.UseMiddleware<ExceptionsHandlingMiddleware>();

            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();
            
            // umożliwia mapowanie endpointów na podstawie annotacji w kontrolerach
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}