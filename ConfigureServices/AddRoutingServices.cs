using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Utils;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddRoutingServices
    {
        /// <summary>
        /// Separacja serwisów odpowiedzialnych za usługi routingu w aplikacji.
        /// </summary>
        public static IServiceCollection AddRoutingServicesCollection(this IServiceCollection services)
        {
            // zmienia ścieżki routingu z wielkich liter na małe
            services.AddRouting(options => {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            // zezwolenie na politykę CORS
            services.AddCors(options => {
                options.AddPolicy("AngularClient", builder =>
                    builder.WithOrigins(GlobalConfigurer.ClientOrigin)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
                options.AddPolicy("AngularDevClient", builder =>
                    builder.WithOrigins(GlobalConfigurer.DevClientOrigin)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });
            
            return services;
        }
    }
}