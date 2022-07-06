/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: AddJwtJsonServices.cs
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