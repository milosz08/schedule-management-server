using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Services.ServicesImplementation;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddRestServices
    {
        /// <summary>
        /// Separacja serwisów odpowiedzialnych za usługi restowe.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRestServicesCollection(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthServiceImplementation>();
            services.AddScoped<IFilesService, FilesServiceImplementation>();
            services.AddScoped<IResetPasswordService, ResetPasswordServiceImplementation>();
            services.AddScoped<IUsersService, UsersServiceImplementation>();
            services.AddScoped<IDepartmentsService, DepartmentsServiceImplementation>();
            services.AddScoped<ICathedralService, CathedralsServiceImplementation>();
            services.AddScoped<IHelperService, HelperServiceImplementation>();
            services.AddScoped<IStudySpecService, StudySpecServiceImplementation>();
            services.AddScoped<IStudyRoomsService, StudyRoomsServiceImplementation>();
            
            return services;
        }
    }
}