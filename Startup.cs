/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: Startup.cs
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Middleware;
using asp_net_po_schedule_management_server.ConfigureServices;


namespace asp_net_po_schedule_management_server
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }
        
        //--------------------------------------------------------------------------------------------------------------
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        public void ConfigureServices(IServiceCollection services)
        {
            Configuration.GetSection("ServerConfiguration").Bind(new GlobalConfigurer());
            
            services.AddJwtJsonServiceCollection();
            services.AddRestServicesCollection();
            services.AddMiddlewareServicesCollection(this.GetType().Assembly);
            services.AddDatabaseServicesCollection(Configuration);
            services.AddSshAndSmtpServicesCollection();
            services.AddRoutingServicesCollection();
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbSeeder seeder)
        {
            // ustawianie polityki cors
            if (env.IsDevelopment()) {
                app.UseCors("AngularDevClient");
            } else {
                app.UseCors("AngularClient");
            }
            
            seeder.Seed().Wait(); // seedowanie (umieszczanie) początkowych danych do encji bazy danych

            if (!env.IsDevelopment()) { // przekierowanie na adres szyfrowany SSL (tylko na produkcji)
                app.UseHttpsRedirection();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c => {  
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Schedule Management Server");  
            });  
            
            app.UseStaticFiles(); // zezwól na używanie plików statycznych (logo aplikacji do emailów)
            
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