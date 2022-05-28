using System;

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
        public static IServiceCollection AddDatabaseServicesCollection(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Dodawanie kontekstu bazy danych
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("MySequelConnection"),
                    new MySqlServerVersion(GlobalConfigurer.DbDriverVersion),
                    opt => {
                        // zezwolenie na dynamiczną translację zapytań do bazy
                        opt.EnableStringComparisonTranslations();
                        // zezwolenie na próbę nawiązania ponownego połączenia do bazy danych w przypadku błędu
                        // po stronie serwera bazodanowego
                        opt.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    }
                ));
            
            // dodawanie seedera jako serwisu
            services.AddScoped<ApplicationDbSeeder>();
            
            return services;
        }
    }
}