using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Services.ServicesImplementation;

namespace asp_net_po_schedule_management_server
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // strefa dodawania serwisów i ich implementacji
            services.AddSingleton<IFirstEndpointService, FirstEndpointServiceImplementation>();
            // zmienia ścieżki routingu z wielkich liter na małe
            services.AddRouting(options => {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            // umożliwia mapowanie endpointów na podstawie annotacji w kontrolerach
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}