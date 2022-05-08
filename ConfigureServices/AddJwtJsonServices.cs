using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddJwtJsonServices
    {
        // separacja serwisów odpowiedzialnych za tokeny JWT i obsługę formatu JSON
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
            JwtAuthenticationManagerImplementation.ImplementsJwtOnStartup(services);
            services.AddScoped<IPasswordHasher<Person>, PasswordHasher<Person>>();
            
            return services;
        }
    }
}