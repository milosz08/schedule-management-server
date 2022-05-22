using System.IO;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Exceptions;


namespace asp_net_po_schedule_management_server.DbConfig
{
    /// <summary>
    /// Klasa odpowiadająca za wstawianie początkowych wartości do bazy danych (seedowanie). Wartości te są pobierane
    /// z zamockowanych danych w formacie JSON, odpowiednio deserializowane na obiekty i mapowane obiektowo-relacyjnie
    /// przez Entity Framework, po czym są wstawiane do bazy danych.
    /// </summary>
    public sealed class ApplicationDbSeeder
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        //--------------------------------------------------------------------------------------------------------------
        
        private readonly string _name = GlobalConfigurer.InitialCredentials.AccountName;
        private readonly string _surname = GlobalConfigurer.InitialCredentials.AccountSurname;

        #region Mocked data files

        private const string _studyRoomsTypes = "study-room.mocked.json";

        //--------------------------------------------------------------------------------------------------------------
        
        public ApplicationDbSeeder(ApplicationDbContext context, IAuthService authService,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _authService = authService;
            _hostingEnvironment = hostingEnvironment;
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
                await InsertDefaultStudyTypes();
                await InsertStudyRoomsTypes();
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
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda dodająca domyślne typy studiów do bazy danych (jeśli takowe się w niej jeszcze nie znajdują).
        /// </summary>
        private async Task InsertDefaultStudyTypes()
        {
            if (!_context.StudyTypes.Any()) {
                StudyType[] studyType = new StudyType[]
                {
                    new StudyType()
                    {
                        Name = "stacjonarne",
                        Alias = "ST",
                    },
                    new StudyType()
                    {
                        Name = "niestacjonarne",
                        Alias = "NS/Z",
                    },
                };
                await _context.StudyTypes.AddRangeAsync(studyType);
                await _context.SaveChangesAsync();
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="BasicServerException"></exception>
        private async Task InsertStudyRoomsTypes()
        {
            if (!_context.RoomTypes.Any()) {
                string roomTypesPath = Path.Combine(_hostingEnvironment.WebRootPath, "cdn/mocked", _studyRoomsTypes);
                string jsonString = File.ReadAllText(roomTypesPath);
                List<RoomType> allRoomTypes = JsonSerializer.Deserialize<List<RoomType>>(jsonString);
                if (allRoomTypes == null || allRoomTypes.Count < 0) {
                    throw new BasicServerException($"Nieprawidłowy format pliku json: {_studyRoomsTypes}",
                        HttpStatusCode.InternalServerError);
                }
                await _context.RoomTypes.AddRangeAsync(allRoomTypes);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}