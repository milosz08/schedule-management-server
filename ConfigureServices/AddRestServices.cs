/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: AddRestServices.cs
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

using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Services.Helpers;
using asp_net_po_schedule_management_server.Services.ServicesImplementation;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddRestServices
    {
        /// <summary>
        /// Separacja serwisów odpowiedzialnych za usługi restowe.
        /// </summary>
        public static IServiceCollection AddRestServicesCollection(this IServiceCollection services)
        {
            // serwisy restowe z interfejsami
            services.AddScoped<IAuthService, AuthServiceImplementation>();
            services.AddScoped<IFilesService, FilesServiceImplementation>();
            services.AddScoped<IResetPasswordService, ResetPasswordServiceImplementation>();
            services.AddScoped<IUsersService, UsersServiceImplementation>();
            services.AddScoped<IDepartmentsService, DepartmentsServiceImplementation>();
            services.AddScoped<ICathedralService, CathedralsServiceImplementation>();
            services.AddScoped<IHelperService, HelperServiceImplementation>();
            services.AddScoped<IStudySpecService, StudySpecServiceImplementation>();
            services.AddScoped<IStudyRoomsService, StudyRoomsServiceImplementation>();
            services.AddScoped<IStudySubjectService, StudySubjectServiceImplementation>();
            services.AddScoped<IStudyGroupService, StudyGroupServiceImplementation>();
            services.AddScoped<IScheduleSubjectsService, ScheduleSubjectsServiceImplementation>();
            services.AddScoped<ITimeManagementService, TimeManagementServiceImplementation>();
            services.AddScoped<ISearchContentService, SearchContentServiceImplementation>();
            services.AddScoped<IContactMessagesService, ContactMessagesServiceImplementation>();

            // serwisy typu "singleton" (bez interfejsów)
            services.AddTransient<ServiceHelper>();
            
            return services;
        }
    }
}