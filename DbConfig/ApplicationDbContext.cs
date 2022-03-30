using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using asp_net_po_schedule_management_server.Entities;

namespace asp_net_po_schedule_management_server.DbConfig
{
    public sealed class ApplicationDbContext : DbContext
    {
        private IConfiguration _configuration;
        
        // zmapowane tabele bazy danych
        public DbSet<Person> Persons { get; set; }
        
        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql(_configuration.GetConnectionString("MySequelConnection"), new MySqlServerVersion("5.7.35"))
                .UseLoggerFactory(LoggerFactory.Create(b => b
                    .AddConsole()
                    .AddFilter(level => level >= LogLevel.Information)
                ))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
    }
}