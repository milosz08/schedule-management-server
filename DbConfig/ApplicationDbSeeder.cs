using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Dto.Requests;


namespace asp_net_po_schedule_management_server.DbConfig
{
    public sealed class ApplicationDbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        private readonly string _name = GlobalConfigurer.InitialCredentials.AccountName;
        private readonly string _surname = GlobalConfigurer.InitialCredentials.AccountSurname;

        //--------------------------------------------------------------------------------------------------------------
        
        public ApplicationDbSeeder(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Funkcja seedująca (wstrzykująca) dane do bazy danych.
        /// </summary>
        public async Task Seed()
        {
            if (_context.Database.CanConnect()) {
                // wywoływane jeśli przy migracji nie ma żadnych elementów w encji "roles" (auto-seedowanie)
                if (!_context.Roles.Any()) {
                    await _context.Roles.AddRangeAsync(InsertRoles());
                    await _context.SaveChangesAsync();
                }
                await InsertDefaultAdminData();
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Seeders

        /// <summary>
        /// Umieszczanie wszystkich możliwych ról (na podstawie enuma) w liście.
        /// </summary>
        /// <returns>wszystkie możliwe role jakie znajdują się w enumie</returns>
        private IEnumerable<Role> InsertRoles()
        {
            List<Role> allRoles = new List<Role>();
            IEnumerable<string> possibleRoles = System.Enum.GetNames(typeof(AvailableRoles));
            foreach (var singleRole in possibleRoles) {
                allRoles.Add(new Role() { Name = singleRole });
            }
            return allRoles;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Dodawanie domyślnego użytkownika (jako administratora systemu, jeśli nie ma w tabeli na
        /// podstawie pliku appsettings.json).
        /// </summary>
        private async Task InsertDefaultAdminData()
        {
            // dodaj domyślnego użytkownika (dane podane w pliku appsettings.json)
            Person findPerson = await _context.Persons
                .FirstOrDefaultAsync(p => p.Name == _name && p.Surname == _surname);
            if (findPerson == null) {
                await _authService.UserRegister(new RegisterNewUserRequestDto()
                {
                    Name = _name,
                    Surname = _surname,
                    Nationality = "Polska",
                    City = "Gliwice",
                    IfRemovable = false,
                    Role = AvailableRoles.ADMINISTRATOR.ToString()
                }, GlobalConfigurer.InitialCredentials.AccountPassword);
            }
        }
    }
}