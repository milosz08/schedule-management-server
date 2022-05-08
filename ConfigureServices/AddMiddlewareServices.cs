using System.Reflection;
using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Middleware;
using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Validators;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddMiddlewareServices
    {
        // separacja serwisów odpowiedzialnych za usługi middleware
        public static IServiceCollection AddMiddlewareServicesCollection(
            this IServiceCollection services, Assembly assembly)
        {
            // strefa dodawnia middleware'ów
            services.AddScoped<ExceptionsHandlingMiddleware>();
            
            services.AddAutoMapper(assembly);
            
            // strefa dodawania walidatorów modeli DTO
            services.AddScoped<IValidator<UserQueryRequestDto>, UserQueryValidator>();
            
            return services;
        }
    }
}