using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Services.ServicesImplementation;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddRestServices
    {
        // separacja serwisów odpowiedzialnych za usługi restowe
        public static IServiceCollection AddRestServicesCollection(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthServiceImplementation>();
            services.AddScoped<IFilesService, FilesServiceImplementation>();
            services.AddScoped<IResetPasswordService, ResetPasswordServiceImplementation>();
            services.AddScoped<IUsersService, UsersServiceImplementation>();
            
            return services;
        }
    }
}