using System;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.DbConfig
{
    public sealed class ApplicationDbContext : DbContext
    {
        private IConfiguration _configuration;

        //--------------------------------------------------------------------------------------------------------------
        
        #region Database entities mapping

        // zmapowane tabele bazy danych
        public DbSet<Person> Persons { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> Tokens { get; set; }
        public DbSet<ResetPasswordOtp> ResetPasswordOpts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Cathedral> Cathedrals { get; set; }
        public DbSet<StudyType> StudyTypes { get; set; }
        public DbSet<StudySpecialization> StudySpecializations { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<StudyRoom> StudyRooms { get; set; }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options) {
            _configuration = configuration;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region MySequel invokers

        //--------------------------------------------------------------------------------------------------------------
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // konfiguracja połączenia do bazy danych
            optionsBuilder
                .UseMySql(
                    _configuration.GetConnectionString("MySequelConnection"),
                    new MySqlServerVersion(GlobalConfigurer.DbDriverVersion))
                .UseLoggerFactory(LoggerFactory.Create(factory => factory
                    .AddConsole()
                    .AddFilter(level => level >= LogLevel.Information)
                ))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Asynchroniczne wywoływanie metod przed zapisaniem każdej encji do bazy danych.
        /// </summary>
        /// <param name="cancellationToken">default</param>
        /// <returns>wywołanie metody bazowej</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAutoInjectionSeqelDates();
            AddAutoInjectionSequelArtificianIndex();
            return await base.SaveChangesAsync(cancellationToken);
        }

        //--------------------------------------------------------------------------------------------------------------

        #region MySequel injectors

        /// <summary>
        /// Autmatyczne wstawianie pól bazowych dla każdej tabeli (data stworzenia oraz aktualizacji).
        /// </summary>
        private void AddAutoInjectionSeqelDates()
        {
            DateTime d = DateTime.Now;
            // znazienie encji ze statusem dodane lub zmodyfikowane dziedziczących klucz główny
            IEnumerable<EntityEntry> entitiesWithPrimaryKey = ChangeTracker.Entries()
                .Where(x => x.Entity is PrimaryKeyEntityInjection
                            && (x.State == EntityState.Added || x.State == EntityState.Modified));
            
            foreach (var entityEntry in entitiesWithPrimaryKey) {
                DateTime formatedDateTime = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
                if (entityEntry.State == EntityState.Added) {
                    ((PrimaryKeyEntityInjection)entityEntry.Entity).CreatedDate = formatedDateTime;
                }

                ((PrimaryKeyEntityInjection) entityEntry.Entity).UpdatedDate = formatedDateTime;
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Automatyczne wstawianie pola sztucznego indeksu (potrzebny do front-endu,
        /// generowany w metodzie statycznej klasy "ApplicationUtils").
        /// </summary>
        private void AddAutoInjectionSequelArtificianIndex()
        {
            // znazienie encji ze statusem dodane dziedziczących klucz główny oraz indeks sztuczny
            IEnumerable<EntityEntry> entitiesWithPrimaryKeyAndArtificianIndex = ChangeTracker.Entries()
                .Where(x => x.Entity is PrimaryKeyWithClientIdentifierInjection && x.State == EntityState.Added);
            
            foreach (var entityEntry in entitiesWithPrimaryKeyAndArtificianIndex) {
                ((PrimaryKeyWithClientIdentifierInjection)entityEntry.Entity).DictionaryHash = 
                    ApplicationUtils.DictionaryHashGenerator();
            }
        }

        #endregion
    }
}