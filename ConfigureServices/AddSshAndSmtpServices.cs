using Microsoft.Extensions.DependencyInjection;

using asp_net_po_schedule_management_server.Ssh.SshInterceptor;
using asp_net_po_schedule_management_server.Ssh.SshEmailService;
using asp_net_po_schedule_management_server.Ssh.SmtpEmailService;


namespace asp_net_po_schedule_management_server.ConfigureServices
{
    public static class AddSshAndSmtpServices
    {
        /// <summary>
        /// Separacja serwisów odpowiedzialnych za usługi socketu SSH i obsługę protokołu SMTP.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSshAndSmtpServicesCollection(this IServiceCollection services)
        {
            // serwis dla socketu SSH
            services.AddScoped<ISshInterceptor, SshInterceptorImplementation>();
            
            // serwisy dla poczty
            services.AddScoped<ISshEmailService, SshEmailServiceImplementation>();
            services.AddScoped<ISmtpEmailService, SmtpEmailServiceImplementation>();
            
            return services;
        }
    }
}