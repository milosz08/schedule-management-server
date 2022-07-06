/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: AddMiddlewareServices.cs
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

using FluentValidation;
using System.Reflection;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Middleware;
using asp_net_po_schedule_management_server.Dto.Validators;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddMiddlewareServices
    {
        /// <summary>
        /// Separacja serwisów odpowiedzialnych za usługi middleware.
        /// </summary>
        public static IServiceCollection AddMiddlewareServicesCollection(
            this IServiceCollection services, Assembly assembly)
        {
            // strefa dodawnia middleware'ów
            services.AddScoped<ExceptionsHandlingMiddleware>();
            
            services.AddAutoMapper(assembly);
            
            // strefa dodawania walidatorów modeli DTO
            services.AddScoped<IValidator<SearchQueryRequestDto>, UserQueryValidator>();
            
            services.Configure<FormOptions>(o => {  
                o.ValueLengthLimit = int.MaxValue;  
                o.MultipartBodyLengthLimit = int.MaxValue;  
                o.MemoryBufferThreshold = int.MaxValue;  
            }); 
            
            services.AddSwaggerGen();
            
            return services;
        }
    }
}