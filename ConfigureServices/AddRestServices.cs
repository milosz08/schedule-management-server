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

            // serwisy typu "singleton" (bez interfejsów)
            services.AddTransient<ServiceHelper>();
            
            return services;
        }
    }
}