using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddJwtJsonServices
    {
        /// <summary>
        /// Separacja serwisów odpowiedzialnych za tokeny JWT i obsługę formatu JSON.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtJsonServiceCollection(this IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation()
                // ignorowanie serializacji JSONów w przypadku zapętlonych referencji (wstawianie nulla)
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            
            // strefa autentykacji i blokowania tras oraz odblokowywania przez JWT
            services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManagerImplementation());
            
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.RequireHttpsMetadata = false; // na developmencie false, na produkcji true <- ważne!!
                options.SaveToken = true; // czy klucz ma być przechowywany
                options.TokenValidationParameters = JwtAuthenticationManagerImplementation
                    .GetBasicTokenValidationParameters();
            });
            
            services.AddScoped<IPasswordHasher<Person>, PasswordHasher<Person>>();
            
            return services;
        }
    }
}