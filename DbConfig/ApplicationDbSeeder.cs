using System;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
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
        private const string _studyTypes = "study-types.mocked.json";
        private const string _departments = "departments.mocked.json";
        private const string _cathedrals = "cathedrals.mocked.json";
        private const string _studyDegrees = "study-degrees.mocked.json";
        private const string _roles = "roles.mocked.json";
        private const string _semesters = "semesters.mocked.json";
        private const string _weekdays = "weekdays.mocked.json";
        private const string _scheduleTypes = "schedule-types.mocked.json";

        #endregion

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
                await InsertAllRoles();
                await InsertAllSemesters();
                await InsertAllWeekdays();
                await InsertAllScheduleSubjectTypes();
                await InsertInitialDepartment();
                await InsertInitialCathedral();
                await InsertDefaultAdminData();
                await InsertDefaultStudyTypes();
                await InsertStudyRoomsTypes();
                await InsertStudyDegreesTypes();
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Seeders

        /// <summary>
        /// Umieszczanie początkowego wydziału (na podstawie wartości w pliku json).
        /// </summary>
        private async Task InsertInitialDepartment()
        {
            if (!_context.Departments.Any()) {
                List<Department> initialDepartments = ApplicationUtils
                    .ConvertJsonToList<Department>(_departments, _hostingEnvironment);
                initialDepartments[0].IfRemovable = false;
                await _context.Departments.AddAsync(initialDepartments[0]);
                await _context.SaveChangesAsync();
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Umieszczanie początkowej katedry i przypisanie do wcześniej stworzonego wydziału (na podstawie wartości
        /// w pliku json).
        /// </summary>
        private async Task InsertInitialCathedral()
        {
            if (!_context.Cathedrals.Any()) {
                List<Cathedral> initialCathedrals = ApplicationUtils
                    .ConvertJsonToList<Cathedral>(_cathedrals, _hostingEnvironment);
                initialCathedrals[0].DepartmentId = _context.Departments.First().Id;
                initialCathedrals[0].IfRemovable = false;
                await _context.Cathedrals.AddAsync(initialCathedrals[0]);
                await _context.SaveChangesAsync();
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Dodawanie domyślnego użytkownika (jako administratora systemu, jeśli nie ma w tabeli na
        /// podstawie pliku appsettings.json). Przypisuje mu również domyślny wydział wraz z katedrą.
        /// </summary>
        private async Task InsertDefaultAdminData()
        {
            // dodaj domyślnego użytkownika (dane podane w pliku appsettings.json)
            Person findPerson = await _context.Persons
                .FirstOrDefaultAsync(p => p.Name == _name && p.Surname == _surname);
            List<Cathedral> initialCathedrals = ApplicationUtils
                .ConvertJsonToList<Cathedral>(_cathedrals, _hostingEnvironment);
            Cathedral findCathedral = await _context.Cathedrals
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Name.Equals(initialCathedrals[0].Name, StringComparison.OrdinalIgnoreCase));
            if (findPerson == null) {
                await _authService.UserRegister(new RegisterUpdateUserRequestDto()
                {
                    Name = _name,
                    Surname = _surname,
                    Nationality = "Polska",
                    City = "Gliwice",
                    IfRemovable = false,
                    Role = AvailableRoles.ADMINISTRATOR,
                    DepartmentName = findCathedral.Department.Name,
                    CathedralName = findCathedral.Name,
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
                await _context.StudyTypes.AddRangeAsync(ApplicationUtils
                    .ConvertJsonToList<StudyType>(_studyTypes, _hostingEnvironment));
                await _context.SaveChangesAsync();
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Umieszczanie wszystkich typów sal zajęciowych (pobierane z zamockowanych danych w formacie json).
        /// </summary>
        /// <exception cref="BasicServerException"></exception>
        private async Task InsertStudyRoomsTypes()
        {
            if (!_context.RoomTypes.Any()) {
                await _context.RoomTypes.AddRangeAsync(ApplicationUtils
                    .ConvertJsonToList<RoomType>(_studyRoomsTypes, _hostingEnvironment));
                await _context.SaveChangesAsync();
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Umieszczanie wszystkich stopni studiów (pobierane z zamockowanych danych w formacie json).
        /// </summary>
        /// <exception cref="BasicServerException"></exception>
        private async Task InsertStudyDegreesTypes()
        {
            if (!_context.StudyDegrees.Any()) {
                await _context.StudyDegrees.AddRangeAsync(ApplicationUtils
                    .ConvertJsonToList<StudyDegree>(_studyDegrees, _hostingEnvironment));
                await _context.SaveChangesAsync();
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Umieszczanie wszystkich ról w systemie (pobierane z zamockowanych danych w formacie json).
        /// </summary>
        private async Task InsertAllRoles()
        {
            if (!_context.Roles.Any()) {
                await _context.Roles.AddRangeAsync(ApplicationUtils
                    .ConvertJsonToList<Role>(_roles, _hostingEnvironment));
                await _context.SaveChangesAsync();
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Umieszczanie wszystkich semstrów (pobierane z zamockowanych danych w formacie json).
        /// </summary>
        private async Task InsertAllSemesters()
        {
            if (!_context.Semesters.Any()) {
                await _context.Semesters.AddRangeAsync(ApplicationUtils
                    .ConvertJsonToList<Semester>(_semesters, _hostingEnvironment));
                await _context.SaveChangesAsync();
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Umieszczanie wszystkich dni tygodnia (pobierane z zamockowanych danych w formacie json).
        /// </summary>
        private async Task InsertAllWeekdays()
        {
            if (!_context.Weekdays.Any()) {
                await _context.Weekdays.AddRangeAsync(ApplicationUtils
                    .ConvertJsonToList<Weekday>(_weekdays, _hostingEnvironment));
                await _context.SaveChangesAsync();
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Umieszczanie wszystkich typów zajęć (pobierane z zamockowanych danych w formacie json).
        /// </summary>
        private async Task InsertAllScheduleSubjectTypes()
        {
            if (!_context.ScheduleSubjectTypes.Any()) {
                await _context.ScheduleSubjectTypes.AddRangeAsync(ApplicationUtils
                    .ConvertJsonToList<ScheduleSubjectType>(_scheduleTypes, _hostingEnvironment));
                await _context.SaveChangesAsync();
            }
        }
        
        #endregion
    }
}