using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddDatabaseServices
    {
        /// <summary>
        /// Separacja serwisów odpowiedzialnych za usługi łączenia z bazą danych.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseServicesCollection(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Dodawanie kontekstu bazy danych
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("MySequelConnection"),
                    new MySqlServerVersion(GlobalConfigurer.DbDriverVersion)
                ));
            
            // dodawanie seedera jako serwisu
            services.AddScoped<ApplicationDbSeeder>();
            
            return services;
        }
    }
}