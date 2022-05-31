using System;

using System.Net;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;


namespace asp_net_po_schedule_management_server.Services.Helpers
{
    /// <summary>
    /// Klasa serwisu przechowująca pomocnicze metody używane w innnych serwisach aplikacji. Klasa jest dodana do
    /// zakresu całej aplikacji, przez co możliwe jest wskrzykiwanie jej w konstruktorze.
    /// </summary>
    public class ServiceHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Person> _passwordHasher;

        private readonly int _singleGridBlockHeight = 96;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public ServiceHelper(ApplicationDbContext context, IPasswordHasher<Person> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Pagination and filtering results

        /// <summary>
        /// Metoda odpowiadająca za logikę paginacji rezultatów i pierwotnego zapytania. Metoda filtruje zapytania z
        /// zapytania pierwotnego zwracają ilość rezultatów na podstawie kwerendy zapytania.
        /// </summary>
        /// <param name="baseQuery">bazowy rezultat zapytania</param>
        /// <param name="query">parametry filtrowania i paginacji</param>
        /// <typeparam name="T">typ encji na której dokonywana jest paginacja</typeparam>
        /// <returns>przefitrowana generyczna lista wynikowa</returns>
        public List<T> PaginationAndAdditionalFiltering<T>(IQueryable<T> baseQuery, SearchQueryRequestDto query)
        {
            return baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();
        }
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda odpowiadająca za sortowanie rosnące/malejące wyników zapytania. Kolumna podlegająca sortowaniu
        /// wybierana jest na podstawie generycznej ekpresji typów (bazowana na parametrach zapytania i słowniku
        /// przechowującym kolumny podlegające sortowaniu).
        /// </summary>
        /// <param name="colSelect">wybrana kolumna</param>
        /// <param name="query">parametry zapytania</param>
        /// <param name="baseQuery">wynik przed filtracją (generyczny, przekazywana referencja)</param>
        /// <typeparam name="T">typ encji na której metoda dokonuje filtracji</typeparam>
        public void PaginationSorting<T>(Dictionary<string, Expression<Func<T, object>>> colSelect,
            SearchQueryRequestDto query, ref IQueryable<T> baseQuery)
        {
            Expression<Func<T, object>> selectColumn = colSelect[query.SortBy];
            baseQuery = query.SortDirection == SortDirection.ASC
                ? baseQuery.OrderBy(selectColumn)
                : baseQuery.OrderByDescending(selectColumn);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Extract credentials on delete content

        /// <summary>
        /// Metoda eksportująca nagłówki autoryzacji i wartość claim reprezentującą login użytkownika (zaszytą w
        /// JWT) i opakowująca wszystkie dane w obiekt.
        /// </summary>
        /// <returns>Obiekt z danymi autoryzacji: login, nazwa użytkownika i hasło</returns>
        /// <exception cref="BasicServerException">Brak nagłówków autoryzacji</exception>
        public UserCredentialsHeaderDto ExtractedUserCredentialsFromHeader(HttpContext context, HttpRequest request)
        {
            Claim userLogin = context.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            IHeaderDictionary headers = request.Headers;
            if (headers.TryGetValue("User-Name", out var username) 
                && headers.TryGetValue("User-Password", out var password)) {
                return new UserCredentialsHeaderDto()
                {
                    Login = userLogin?.Value,
                    Username = username.First(),
                    Password = password.First(),
                };
            }
            throw new BasicServerException("Brak nagłówków autoryzacji.", HttpStatusCode.Forbidden);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Checking credentials on massive content deleting
        
        /// <summary>
        /// Metoda walidująca użytkownika na podstawie wprowadzonego hasła oraz loginu pozyskanego z tokenu JWT
        /// oraz podanego przez użytkownika.
        /// </summary>
        /// <param name="credentials">obiekt przechowujący wartości autoryzacji</param>
        /// <returns>Zwróci true, jeśli autoryzacja przebiegła prawidłowo.</returns>
        /// <exception cref="BasicServerException">W przypadku błędu serwera wyrzuci wyjątek</exception>
        public async Task CheckIfUserCredentialsAreValid(UserCredentialsHeaderDto credentials)
        {
            Person findPerson = await _context.Persons
                .FirstOrDefaultAsync(p => p.Login == credentials.Login && p.Login == credentials.Username);

            // jeśli użytkownik nie istnieje w systemie
            if (findPerson == null) {
                throw new BasicServerException("Użytkownik z podanym loginem/nazwą nie istnieje w systemie.", 
                    HttpStatusCode.Forbidden);
            }
            
            // weryfikacja hasła
            PasswordVerificationResult verificatrionRes = _passwordHasher
                .VerifyHashedPassword(findPerson, findPerson.Password, credentials.Password);
            if (verificatrionRes == PasswordVerificationResult.Failed) {
                throw new BasicServerException("Nieprawidłowe hasło. Spróbuj ponownie.", HttpStatusCode.Unauthorized);
            }
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Generate values for registered user

        /// <summary>
        /// Metoda generująca podstawowe dane zarejestrowanego użytkownika na podstawie podstawowych danych
        /// wprowadzonych przez administratora systemu.
        /// </summary>
        /// <param name="user">obiekt transferowy z podstawowymi danymi</param>
        /// <returns>obiekt z wygenerowanymi danymi użytkownika</returns>
        public RegisterUserGeneratedValues GenerateUserDetails(RegisterNewUserRequestDto user)
        {
            string nameWithoutDiacritics = ApplicationUtils.RemoveAccents(user.Name);
            string surnameWithoutDiacritics = ApplicationUtils.RemoveAccents(user.Surname);
            string randomNumbers = ApplicationUtils.RandomNumberGenerator();
           
            string shortcut = nameWithoutDiacritics.Substring(0, 3) + surnameWithoutDiacritics.Substring(0, 3);

            return new RegisterUserGeneratedValues()
            {
                Shortcut = shortcut,
                Password = ApplicationUtils.GenerateUserFirstPassword(),
                Login = shortcut.ToLower() + randomNumbers,
                Email = $"{nameWithoutDiacritics.ToLower()}.{surnameWithoutDiacritics.ToLower()}" +
                        $"{randomNumbers}@{GlobalConfigurer.UserEmailDomain}",
                EmailPassword = ApplicationUtils.GenerateUserFirstPassword(),
            };
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Return study year base month
        
        /// <summary>
        /// Metoda zwracająca rok studiów na podstawie aktualnego roku.
        /// </summary>
        /// <returns>rok studiów składający się z dwóch dat</returns>
        public string StudyYearBaseCurrentMonth()
        {
            int currentYear = DateTime.Now.Year;
            if (DateTime.Now.Month > 1 && DateTime.Now.Month < 10) {
                return $"{currentYear - 1}/{currentYear}";
            }
            return $"{currentYear}/{currentYear + 1}";
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Find day based day id and weeknumber with year

        /// <summary>
        /// Metoda znajdująca dzień (w postaci obiektu daty) na podstawie roku, numeru tygodnia oraz dnia tygodnia.
        /// </summary>
        /// <param name="year">rok</param>
        /// <param name="weekNumber">numer tygodnia</param>
        /// <param name="dayOfWeek">dzień tygodnia</param>
        /// <returns>znaleziony dzień (w postaci obiektu daty)</returns>
        public DateTime FindDayBasedDayIdAndWeekNumber(int year, int weekNumber, int dayOfWeek)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Tuesday - jan1.DayOfWeek;
        
            DateTime firstMonday = jan1.AddDays(daysOffset);
        
            Calendar cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        
            int weekNum = weekNumber;
            if (firstWeek <= 1) {
                weekNum -= 1;
            }

            return firstMonday.AddDays(weekNum * 7 + dayOfWeek - 2);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Computed position from top and height of subject schedule element

        /// <summary>
        /// Metoda obliczająca ilość pikseli od góry elementu canvas oraz wielkości pojedynczego bloku zajęciowego.
        /// Używane przede wszystkim we front-endzie do wizualizacji planu na obiekcie canvas.
        /// </summary>
        /// <param name="hourStart">godzina rozpoczęcia zajęć</param>
        /// <param name="hourEnd">godzina zakończenia zajęć</param>
        /// <returns></returns>
        public (int pxFromTop, int pxHegith) ComputedPositionFromTopAndHeight(TimeSpan hourStart, TimeSpan hourEnd)
        {
            int heightOf5minutes = _singleGridBlockHeight / (60 / 5); // wysokość jednego 5 minutowego bloku
            
            // obliczanie pozycji kafelka od góry canvasu
            int allMinutesFromTop = (int) (hourStart - TimeSpan.Parse("07:00")).TotalMinutes;
            int pxFromTop = (heightOf5minutes * allMinutesFromTop) / 5;
            
            // obliczanie wysokości kafelka
            int fullLength = ((int) (hourEnd - hourStart).TotalMinutes) / 5;
            int pxHegith = heightOf5minutes * fullLength;
            
            return (pxFromTop, pxHegith);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Schedule Subjects helpers

        /// <summary>
        /// Metoda pomocnicza, tworząca obiekt bazowy filtrowanych przedmiotów na planie zajęć.
        /// </summary>
        /// <param name="subj">iterowany przedmiot</param>
        /// <returns>stworzony obiekt z parametrami przedmiotu przekazywanego w argumencie</returns>
        public BaseScheduleResData ScheduleSubjectFilledFieldData(ScheduleSubject subj, ScheduleFilteringData filter)
        {
            var test = new BaseScheduleResData()
            {
                ScheduleSubjectId = subj.Id,
                SubjectWithTypeAlias = ApplicationUtils.CreateSubjectAlias(subj),
                SubjectTypeHexColor = subj.ScheduleSubjectType.Color,
                SubjectTime = $"{subj.StartTime.ToString("hh\\:mm")} - {subj.EndTime.ToString("hh\\:mm")}",
                PositionFromTop = ComputedPositionFromTopAndHeight(subj.StartTime, subj.EndTime).pxFromTop,
                ElementHeight = ComputedPositionFromTopAndHeight(subj.StartTime, subj.EndTime).pxHegith,
                SubjectOccuredData = ApplicationUtils.ConvertScheduleOccur(subj),
                IfNotShowingOccuredDates = !filter.WeekInputOptions.Equals("pokaż wszystko", StringComparison.OrdinalIgnoreCase),
            };
            return test;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda pomocnicza służąca do mapowania grup dziekańskich z encji na obiekt zapytania HTTP.
        /// </summary>
        /// <param name="scheduleSubject">obiekt przedmiotu do zmapowania</param>
        /// <returns>zmapoowany subobiekt</returns>
        public List<ScheduleMultipleValues<ScheduleGroupQuery>> MappingScheduleGroups(ScheduleSubject scheduleSubject)
        {
            return scheduleSubject.StudyGroups
                .Select(g => new ScheduleMultipleValues<ScheduleGroupQuery>(
                    g.Name,
                    new ScheduleGroupQuery(g.Department.Id, g.StudySpecialization.Id, g.Id))
                ).ToList();
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda pomocnicza służąca do mapowania pracowników z encji na obiekt zapytania HTTP.
        /// </summary>
        /// <param name="scheduleSubject">obiekt przedmiotu do zmapowania</param>
        /// <returns>zmapoowany subobiekt</returns>
        public List<ScheduleMultipleValues<ScheduleTeacherQuery>> MappingScheduleTeachers(ScheduleSubject scheduleSubject)
        {
            return scheduleSubject.ScheduleTeachers
                .Select(t => new ScheduleMultipleValues<ScheduleTeacherQuery>(
                    t.Shortcut,
                    new ScheduleTeacherQuery(t.Department.Id, t.Cathedral.Id, t.Id))
                ).ToList();
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda pomocnicza służąca do mapowania sal zajęciowych z encji na obiekt zapytania HTTP.
        /// </summary>
        /// <param name="scheduleSubject">obiekt przedmiotu do zmapowania</param>
        /// <returns>zmapoowany subobiekt</returns>
        public List<ScheduleMultipleValues<ScheduleRoomQuery>> MappingScheduleRooms(ScheduleSubject scheduleSubject)
        {
            return scheduleSubject.StudyRooms
                .Select(r => new ScheduleMultipleValues<ScheduleRoomQuery>(
                    r.Name,
                    new ScheduleRoomQuery(r.Department.Id, r.Cathedral.Id, r.Id))
                ).ToList();
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda pomocniczna filtrująca przedmioty w planie zajęć na podstawie parametrów filtracji w obiekcie
        /// transferowym.
        /// </summary>
        /// <param name="scheduleSubject">iterowany przedmiot z planu</param>
        /// <param name="filter">obiekt filtracji</param>
        /// <param name="allElements">wszystkie przedmioty</param>
        /// <param name="element">pojedynczy przedmiot</param>
        /// <typeparam name="T">parametr pojedynczego elementu (grupy, pracownika, sali)</typeparam>
        public void FilteringScheduleSubject<T>(ScheduleSubject scheduleSubject, ScheduleFilteringData filter, 
            ref List<T> allElements, ref T element)
        {
            bool ifShow = false;
            if (!filter.WeekInputOptions.Equals("pokaż wszystko", StringComparison.OrdinalIgnoreCase)) {
                List<int> options = filter.WeekInputOptions
                    .Substring(filter.WeekInputOptions.IndexOf("(", StringComparison.OrdinalIgnoreCase) + 1)
                    .Replace(")", "").Split(", ").Select(v => int.Parse(v)).ToList();
                ifShow = scheduleSubject.WeekScheduleOccurs
                    .Any(o => o.Year == options[0] && o.WeekNumber == options[1]);
                if (scheduleSubject.WeekScheduleOccurs.IsNullOrEmpty()) {
                    ifShow = true;
                }
            } else if(scheduleSubject.StudyYear.Equals(filter.SelectedYears)) {
                ifShow = true;
            }

            // pokaż wybrane/pokaż wszystkie
            if (ifShow) {
                allElements.Add(element);
            }
        }
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Current DateTime object from year and weekOfYear

        /// <summary>
        /// Metoda pomocnicza zwracająca datę pierszego dnia tygodnia na podstawie numeru tygodnia oraz roku.
        /// </summary>
        /// <param name="year">rok</param>
        /// <param name="weekOfYear">numer tygodnia</param>
        /// <returns>pierwsza data pierszego dnia tygodnia (poniedziałek)</returns>
        public DateTime FirstDateOfWeekBasedWeekNumber(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;
            DateTime firstThursday = jan1.AddDays(daysOffset);
            Calendar cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int weekNum = weekOfYear;
            if (firstWeek == 1) {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Add base values for single day

        /// <summary>
        /// Metoda pomocnicza dodająca podstawowe wartości do modelu odczytane z bazy danych na podstawie parametrów
        /// zapytania.
        /// </summary>
        /// <param name="canvasData">zawartość wszystkich treści</param>
        /// <param name="weekday">dzień tygodnia</param>
        /// <param name="filter">obiekt filtracji</param>
        /// <param name="dayIncrement">zmienna inkrementująca dzień tygodnia</param>
        /// <typeparam name="T">typ zawartości treści (grupy, pracownicy, sale zajęciowe)</typeparam>
        public void AddBaseValuesForSingleDay<T>(ref ScheduleCanvasData<T> canvasData, Weekday weekday,
            ScheduleFilteringData filter, ref int dayIncrement)
        {
            canvasData.WeekdayNameWithId = new NameWithDbIdElement(weekday.Id, weekday.Alias);
            canvasData.IfNotShowingOccuredDates = filter.WeekInputOptions
                .Equals("pokaż wszystko", StringComparison.OrdinalIgnoreCase);
            if (!filter.WeekInputOptions.Equals("pokaż wszystko", StringComparison.OrdinalIgnoreCase)) {
                List<int> options = filter.WeekInputOptions
                    .Substring(filter.WeekInputOptions.IndexOf("(", StringComparison.OrdinalIgnoreCase) + 1)
                    .Replace(")", "").Split(", ").Select(v => int.Parse(v)).ToList();
                canvasData.WeekdayDateTime = FirstDateOfWeekBasedWeekNumber(options[0], options[1])
                    .AddDays(dayIncrement++).ToString("dd\\.MM");
            } else {
                canvasData.WeekdayDateTime = filter.WeekInputOptions;
            }
        }

        #endregion
    }
}