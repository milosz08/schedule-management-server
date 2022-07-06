/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: AddRoutingServices.cs
 * Project name | Nazwa Projektu: asp-net-po-schedule-management-server
 *
 * Klient | Client: <https://github.com/Milosz08/Angular_PO_Schedule_Management_Client>
 * Serwer | Server: <https://github.com/Milosz08/ASP.NET_PO_Schedule_Management_Server>
 *
 * RestAPI for the Angular application to manage schedule for sample university. Written with the ASP.NET Core
 * and Entity Framework with mySQL database. Project for the teaching course "Objected Oriented Programming".
 *
 * RestAPI dla aplikacji Angular do zarządzania planem zajęć przykładowej uczelni wyższej. Napisane w oparciu o
 * ASP.NET Core oraz Entity Framework z bazą danych mySQL. Projekt wykonany na zajęcia "Programowanie Obiektowe".
 */

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