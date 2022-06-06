using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class ScheduleSubjectsServiceImplementation : IScheduleSubjectsService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _helper;
        private readonly ApplicationDbContext _context;
            
        public readonly TimeSpan _startTime = TimeSpan.ParseExact("07:00", "h\\:mm", new CultureInfo("pl-PL"));
        public readonly TimeSpan _endTime = TimeSpan.ParseExact("22:00", "h\\:mm", new CultureInfo("pl-PL"));
        public readonly TimeSpan _minInterval = TimeSpan.ParseExact("00:05", "h\\:mm", new CultureInfo("pl-PL"));
        
        //--------------------------------------------------------------------------------------------------------------
        
        public ScheduleSubjectsServiceImplementation(ApplicationDbContext context, ServiceHelper helper, IMapper mapper)
        {
            _context = context;
            _helper = helper;
            _mapper = mapper;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        #region Add new schedule activity/subject

        /// <summary>
        /// Metoda służąca do dodawania nowej aktywności w planie zajęć. Metoda posiada walizację: brak możliwości
        /// wprowadzenia nakładających się zajęć w tej samej grupie, sprawdzanie godzin oraz wyszukiwanie elementów w
        /// bazie danych (jeśli nie istnieją, metoda wyrzuci wyjątek).
        /// </summary>
        /// <param name="dto">obiekt transferowy z danymi</param>
        /// <exception cref="BasicServerException">w przypadku braku zasobu/niedozwolonej operacji</exception>
        public async Task AddNewScheduleActivity(ScheduleActivityReqDto dto)
        {
            // wyszukaj wszystkie grupy dziekańskie na podstawie parametrów, jeśli nie znajdzie rzuć wyjątek
            List<StudyGroup> findStudyGroups = await _context.StudyGroups
                .Include(g => g.Department)
                .Include(g => g.StudySpecialization)
                .Where(g => (g.Id == dto.StudyGroupId || dto.IfAddForAllGroups) && g.Department.Id == dto.DeptId &&
                            g.StudySpecialization.Id == dto.StudySpecId)
                .ToListAsync();
            if (findStudyGroups.Count == 0) {
                throw new BasicServerException("Nie znaleziono grup dziekańskich na podstawie podanych parametrów.", 
                    HttpStatusCode.NotFound);
            }

            // sprawdź, czy godzina rozpoczęcia jest mniejsza niż zakończenia i czy mieści się w podanym zakresie oraz
            // czy jest podaną wielokrotnością stałej czasowej
            TimeSpan startHour, endHour;
            try {
                startHour = TimeSpan.ParseExact(dto.HourStart, "h\\:mm", new CultureInfo("pl-PL"));
                endHour = TimeSpan.ParseExact(dto.HourEnd, "h\\:mm", new CultureInfo("pl-PL"));
                if (startHour >= endHour) {
                    throw new BasicServerException("Godzina rozpoczęcia musi być mniejsza od godziny zakończenia.",
                        HttpStatusCode.ExpectationFailed);
                }
                if (startHour < _startTime || endHour > _endTime) {
                    throw new BasicServerException("Nieprawidłowy zakres godzin.", HttpStatusCode.ExpectationFailed);
                }
                string formattedDividedHour = (startHour / _minInterval).ToString(CultureInfo.CurrentCulture);
                if (formattedDividedHour.Contains(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator)) {
                    throw new BasicServerException("Czas odbywania zajęć musi być wielokrotnością pięciu minut.", 
                        HttpStatusCode.ExpectationFailed);
                }
            } catch (FormatException ex) {
                throw new BasicServerException("Niepoprawne wartości godzinowe.", HttpStatusCode.ExpectationFailed);
            }

            // znajdź i dopasuj dzień tygodnia z parametrów, jeśli nie znajdzie rzuć wyjątek
            Weekday findWeekday = await _context.Weekdays.FirstOrDefaultAsync(w => w.Id == dto.WeekDayId);
            if (findWeekday == null) {
                throw new BasicServerException("Nie znaleziono dnia tygodnia na podstawie parametrów.", 
                    HttpStatusCode.NotFound);
            }

            // znajdź i dopasuj typ przedmiotu z parametrów, jeśli nie znajdzie rzuć wyjątek
            ScheduleSubjectType findSubjectType = await _context.ScheduleSubjectTypes
                .FirstOrDefaultAsync(t => t.Name.Equals(dto.SubjectTypeName, StringComparison.OrdinalIgnoreCase));
            if (findSubjectType == null) {
                throw new BasicServerException("Nie znaleziono typu przedmiotu na podstawie podanych parametrów.", 
                    HttpStatusCode.NotFound);
            }

            // znajdź i dopasuj przedmiot z parametrów, jeśli nie znajdzie rzuć wyjątek
            StudySubject findStudySubject = await _context.StudySubjects
                .Include(s => s.StudySpecialization)
                .FirstOrDefaultAsync(s => s.StudySpecialization.Id == dto.StudySpecId &&
                                          s.Name.Equals(dto.SubjectOrActivityName, StringComparison.OrdinalIgnoreCase));
            if (findStudySubject == null) {
                throw new BasicServerException("Nie znaleziono przedmiotu na podstawie podanych parametrów.", 
                    HttpStatusCode.NotFound);
            }

            // znajdź i dopasuj wszystkie sale zajęciowe na podstawie tablicy wartości id w parametrach, jeśli nie
            // znajdzie, przypisz pustą tablicę
            List<StudyRoom> findAllStudyRooms = await _context.StudyRooms
                .Include(r => r.Department)
                .Where(r => dto.SubjectRooms.Any(rdto => rdto == r.Id) && r.Department.Id == dto.DeptId)
                .ToListAsync();

            // znajdź i dopasuj wszystkich nauczycieli na podstawie tablicy wartości id w parametrach, jeśli nie
            // znajdzie, przypisz pustą tablicę
            List<Person> findAllTeachers = await _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Department)
                .Where(p => dto.SubjectTeachers.Any(pdto => pdto == p.Id) && p.Department.Id == dto.DeptId &&
                            p.Role.Name != AvailableRoles.STUDENT)
                .ToListAsync();

            // dodanie do listy występowań aktywności nowych encji
            List<WeekScheduleOccur> allScheduleOccurs = new List<WeekScheduleOccur>();
            foreach (string weekData in dto.WeeksData) {
                List<int> onlyYearAndWeekNumber = weekData
                    .Substring(weekData.IndexOf("(", StringComparison.OrdinalIgnoreCase) + 1)
                    .Replace(")", "").Split(", ").Select(v => int.Parse(v)).ToList();
                allScheduleOccurs.Add(new WeekScheduleOccur()
                {
                    WeekNumber = (byte) onlyYearAndWeekNumber[1],
                    Year = onlyYearAndWeekNumber[0],
                    OccurDate = _helper.FindDayBasedDayIdAndWeekNumber(onlyYearAndWeekNumber[0], 
                        onlyYearAndWeekNumber[1], findWeekday.Number),
                });
            }

            // walidacja encji przed dodaniem jej do bazy danych pod względem występowania duplikatów i konfliktów
            // (nakładanie się przedmiotów w grupie, itp.)
            List<string> allOccursConvert = allScheduleOccurs.Select(o => $"{o.Year},{o.WeekNumber}").ToList();
            List<ScheduleSubject> findDuplicatRooms = await _context.ScheduleSubjects
                .Include(sb => sb.Weekday)
                .Include(sb => sb.StudyRooms)
                .Include(sb => sb.StudyGroups)
                .Include(sb => sb.WeekScheduleOccurs)
                .ToListAsync();

            foreach (ScheduleSubject sb in findDuplicatRooms) {
                if (sb.StudyGroups.Any(sgb => findStudyGroups.Any(sg => sgb.Id == sg.Id))) {
                    List<string> convertOccured = sb.WeekScheduleOccurs.Select(o => $"{o.Year},{o.WeekNumber}").ToList();
                    // aktywność nakładająca się na inną aktywność
                    bool hours = sb.StartTime < TimeSpan.Parse(dto.HourStart) || TimeSpan.Parse(dto.HourStart) < sb.EndTime
                                 || sb.StartTime < TimeSpan.Parse(dto.HourEnd) || TimeSpan.Parse(dto.HourEnd) < sb.EndTime;
                    // wyszukaj duplikatów mających taki sam dzień tygodnia, rok studiów, godziny oraz występowanie
                    if (sb.Weekday.Id == dto.WeekDayId && sb.StudyYear.Equals(dto.StudyYear) && hours &&
                        (convertOccured.Intersect(allOccursConvert).Any() || convertOccured.IsNullOrEmpty() && 
                            allOccursConvert.IsNullOrEmpty())) {
                        throw new BasicServerException(
                            "Termin ma już przypisaną aktywność. Proszę dodać aktywność dla innego terminu lub " +
                            "usunąć kolidację.", HttpStatusCode.Forbidden);
                    }    
                }
            }

            ScheduleSubject addingScheduleSubject = new ScheduleSubject()
            {
                ScheduleSubjectTypeId = findSubjectType.Id,
                WeekScheduleOccurs = allScheduleOccurs,
                StudySubjectId = findStudySubject.Id,
                ScheduleTeachers = findAllTeachers,
                StudyRooms = findAllStudyRooms,
                StudyGroups = findStudyGroups,
                WeekdayId = findWeekday.Id,
                StudyYear = dto.StudyYear,
                StartTime = startHour,
                EndTime = endHour,
            };

            await _context.ScheduleSubjects.AddAsync(addingScheduleSubject);
            await _context.SaveChangesAsync();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all schedule subjects base group ID
        
        /// <summary>
        /// Metoda odpowiadająca za pobieranie wszystkich przedmiotów na planie zajęć wybranej grupy dziekańskiej. Metoda
        /// dodatkowo jest parametryzowana filtrem umożliwiającym filtrowanie wyników na podstawie daty (występowania)
        /// przyrównując do aktualnej daty (nawigacja w planie zajęć).
        /// </summary>
        /// <param name="dto">obiekt transferowy z parametrami zapytania</param>
        /// <param name="filter">obiekt filtracyjny z parametrami filtracji</param>
        /// <returns>opakowane dane przygotowane pod wyświetlanie w formie tabeli dni tygodnia</returns>
        /// <exception cref="BasicServerException">w przypadku błędych parametrów planu</exception>
        public async Task<ScheduleDataRes<ScheduleGroups>> GetAllScheduleSubjectsBaseGroup(ScheduleGroupQuery dto, 
            ScheduleFilteringData filter)
        {
            // wyszukiwanie pasującej grupy dziekańskiej, w przypadku zwrócenia wartości null, wyrzucenie wyjątku
            StudyGroup findStudyGroup = await _context.StudyGroups
                .Include(g => g.Semester)
                .Include(g => g.Department)
                .Include(g => g.StudySpecialization)
                .Include(g => g.StudySpecialization.StudyType)
                .Include(g => g.StudySpecialization.StudyDegree)
                .FirstOrDefaultAsync(g => g.Id == dto.GroupId && g.Department.Id == dto.DeptId && 
                                          g.StudySpecialization.Id == dto.SpecId);
            if (findStudyGroup == null) {
                throw new BasicServerException("Błędne parametry planu.", HttpStatusCode.NotFound);
            }

            ScheduleDataRes<ScheduleGroups> response = new ScheduleDataRes<ScheduleGroups>();
            response.TraceDetails = new List<string>()
            {
                "Grupy",
                findStudyGroup.Department.Name,
                findStudyGroup.StudySpecialization.StudyDegree.Name,
                findStudyGroup.StudySpecialization.StudyType.Name,
                findStudyGroup.StudySpecialization.Name,
                findStudyGroup.Semester.Name,
            };
            response.ScheduleHeaderData =
                $"Plan zajęć - {findStudyGroup.Name}, rok {filter.SelectedYears}";
            
            // przefiltrowanie dni tygodnia
            List<Weekday> allWeekdays = await _context.Weekdays
                .Where(d => findStudyGroup.StudySpecialization.StudyType.Alias == StudySpecTypes.ST
                    ? d.Number > 0 && d.Number < 6
                    : d.Number > 4 && d.Number < 8)
                .Select(d => d).ToListAsync();
            
            int dayIncrement = findStudyGroup.StudySpecialization.StudyType.Alias == StudySpecTypes.ST ? 0 : 4;
            foreach (Weekday weekday in allWeekdays) {
                ScheduleCanvasData<ScheduleGroups> singleDay = new ScheduleCanvasData<ScheduleGroups>();
                
                List<ScheduleSubject> findAllScheduleSubjects = await _context.ScheduleSubjects
                    .Include(s => s.Weekday)
                    .Include(s => s.StudyRooms)
                    .Include(s => s.StudySubject)
                    .Include(s => s.WeekScheduleOccurs)
                    .Include(s => s.ScheduleSubjectType)
                    .Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Cathedral)
                    .Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Department)
                    .Where(s => s.Weekday.Id == weekday.Id && s.StudyGroups.Any(stg => stg.Id == dto.GroupId))
                    .ToListAsync();
                
                List<ScheduleGroups> allScheduleSubjectsBaseGroup = new List<ScheduleGroups>();
                
                foreach (ScheduleSubject scheduleSubject in findAllScheduleSubjects) {
                    BaseScheduleResData baseData = _helper.ScheduleSubjectFilledFieldData(scheduleSubject, filter);
                    ScheduleGroups scheduleGroups = _mapper.Map<ScheduleGroups>(baseData);

                    scheduleGroups.TeachersAliases = _helper.MappingScheduleTeachers(scheduleSubject);
                    scheduleGroups.RoomsAliases = _helper.MappingScheduleRooms(scheduleSubject);

                    _helper.FilteringScheduleSubject(scheduleSubject, filter, ref allScheduleSubjectsBaseGroup, 
                        ref scheduleGroups);
                }

                _helper.AddBaseValuesForSingleDay(ref singleDay, weekday, filter, ref dayIncrement);
                singleDay.WeekdayData = allScheduleSubjectsBaseGroup;
                
                response.ScheduleCanvasData.Add(singleDay);
            }

            response.CurrentChooseWeek = filter.WeekInputOptions;
            return response;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Get all schedule subjects base teacher ID

        /// <summary>
        /// Metoda odpowiadająca za pobieranie wszystkich przedmiotów na planie zajęć wybranego pracownika. Metoda
        /// dodatkowo jest parametryzowana filtrem umożliwiającym filtrowanie wyników na podstawie daty (występowania)
        /// przyrównując do aktualnej daty (nawigacja w planie zajęć).
        /// </summary>
        /// <param name="dto">obiekt transferowy z parametrami zapytania</param>
        /// <param name="filter">obiekt filtracyjny z parametrami filtracji</param>
        /// <returns>opakowane dane przygotowane pod wyświetlanie w formie tabeli dni tygodnia</returns>
        /// <exception cref="BasicServerException">w przypadku błędych parametrów planu</exception>
        public async Task<ScheduleDataRes<ScheduleTeachers>> GetAllScheduleSubjectsBaseTeacher(ScheduleTeacherQuery dto,
            ScheduleFilteringData filter)
        {
            Person findTeacher = await _context.Persons
                .Include(t => t.Role)
                .Include(t => t.Department)
                .Include(t => t.Cathedral)
                .Where(t => t.Role.Name != AvailableRoles.STUDENT)
                .FirstOrDefaultAsync(t => t.Id == dto.EmployeerId && t.Department.Id == dto.DeptId &&
                                          t.Cathedral.Id == dto.CathId);
            if (findTeacher == null) {
                throw new BasicServerException("Błędne parametry planu.", HttpStatusCode.NotFound);
            }
            
            ScheduleDataRes<ScheduleTeachers> response = new ScheduleDataRes<ScheduleTeachers>();
            response.TraceDetails = new List<string>()
            {
                "Pracownicy",
                findTeacher.Department.Name,
                findTeacher.Cathedral.Name,
            };
            response.ScheduleHeaderData =
                $"Plan zajęć - {findTeacher.Surname} {findTeacher.Name}, rok {filter.SelectedYears}";
            
            // iteracja po dniach tygodnia
            int dayIncrement = 0;
            foreach (Weekday weekday in await _context.Weekdays.Select(d => d).ToListAsync()) {
                ScheduleCanvasData<ScheduleTeachers> singleDay = new ScheduleCanvasData<ScheduleTeachers>();
                
                List<ScheduleSubject> findAllScheduleTeachers = await _context.ScheduleSubjects
                    .Include(s => s.Weekday)
                    .Include(s => s.StudyRooms)
                    .Include(s => s.StudyGroups)
                    .Include(s => s.StudySubject)
                    .Include(s => s.WeekScheduleOccurs)
                    .Include(s => s.ScheduleSubjectType)
                    .Include(s => s.StudyRooms).ThenInclude(ste => ste.Cathedral)
                    .Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Department)
                    .Include(s => s.StudyGroups).ThenInclude(ste => ste.StudySpecialization)
                    .Where(s => s.Weekday.Id == weekday.Id && s.ScheduleTeachers.Any(st => st.Id == dto.EmployeerId))
                    .ToListAsync();

                List<ScheduleTeachers> allScheduleSubjectsBaseTeacher = new List<ScheduleTeachers>();
                
                foreach (ScheduleSubject scheduleSubject in findAllScheduleTeachers) {
                    BaseScheduleResData baseData = _helper.ScheduleSubjectFilledFieldData(scheduleSubject, filter);
                    ScheduleTeachers scheduleTeachers = _mapper.Map<ScheduleTeachers>(baseData);

                    scheduleTeachers.StudyGroupAliases = _helper.MappingScheduleGroups(scheduleSubject);
                    scheduleTeachers.RoomsAliases = _helper.MappingScheduleRooms(scheduleSubject);
                    
                    _helper.FilteringScheduleSubject(scheduleSubject, filter, ref allScheduleSubjectsBaseTeacher, 
                        ref scheduleTeachers);
                }
                
                _helper.AddBaseValuesForSingleDay(ref singleDay, weekday, filter, ref dayIncrement);
                singleDay.WeekdayData = allScheduleSubjectsBaseTeacher;
                
                response.ScheduleCanvasData.Add(singleDay);
            }
            
            response.CurrentChooseWeek = filter.WeekInputOptions;
            return response;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Get all schedule subjects base room ID

        /// <summary>
        /// Metoda odpowiadająca za pobieranie wszystkich przedmiotów na planie zajęć wybranej sali zajęciowej. Metoda
        /// dodatkowo jest parametryzowana filtrem umożliwiającym filtrowanie wyników na podstawie daty (występowania)
        /// przyrównując do aktualnej daty (nawigacja w planie zajęć).
        /// </summary>
        /// <param name="dto">obiekt transferowy z parametrami zapytania</param>
        /// <param name="filter">obiekt filtracyjny z parametrami filtracji</param>
        /// <returns>opakowane dane przygotowane pod wyświetlanie w formie tabeli dni tygodnia</returns>
        /// <exception cref="BasicServerException">w przypadku błędych parametrów planu</exception>
        public async Task<ScheduleDataRes<ScheduleRooms>> GetAllScheduleSubjectsBaseRoom(ScheduleRoomQuery dto,
            ScheduleFilteringData filter)
        {
            StudyRoom findStudyRoom = await _context.StudyRooms
                .Include(r => r.RoomType)
                .Include(r => r.Department)
                .Include(r => r.Cathedral)
                .FirstOrDefaultAsync(r => r.Id == dto.RoomId && r.Department.Id == dto.DeptId &&
                                          r.Cathedral.Id == dto.CathId);
            if (findStudyRoom == null) {
                throw new BasicServerException("Błędne parametry planu.", HttpStatusCode.NotFound);
            }
                
            ScheduleDataRes<ScheduleRooms> response = new ScheduleDataRes<ScheduleRooms>();
            response.TraceDetails = new List<string>()
            {
                "Sale zajęciowe",
                findStudyRoom.Department.Name,
                findStudyRoom.Cathedral.Name
            };

            string ifRoomDescription = findStudyRoom.Description != string.Empty ? $"({findStudyRoom.Description})" : "";
            response.ScheduleHeaderData =
                $"Plan zajęć - {findStudyRoom.Name} {ifRoomDescription}, rok {filter.SelectedYears}";
            
            // iteracja po dniach tygodnia
            int dayIncrement = 0;
            foreach (Weekday weekday in await _context.Weekdays.Select(d => d).ToListAsync()) {
                ScheduleCanvasData<ScheduleRooms> singleDay = new ScheduleCanvasData<ScheduleRooms>();
                
                List<ScheduleSubject> findAllScheduleStudyRooms = await _context.ScheduleSubjects
                    .Include(s => s.Weekday)
                    .Include(s => s.StudyGroups)
                    .Include(s => s.StudySubject)
                    .Include(s => s.ScheduleTeachers)
                    .Include(s => s.WeekScheduleOccurs)
                    .Include(s => s.ScheduleSubjectType)
                    .Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Cathedral)
                    .Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Department)
                    .Include(s => s.StudyGroups).ThenInclude(ste => ste.StudySpecialization)
                    .Where(s => s.Weekday.Id == weekday.Id && s.StudyRooms.Any(st => st.Id == dto.RoomId))
                    .ToListAsync();
                
                List<ScheduleRooms> allScheduleSubjectsBaseRoom = new List<ScheduleRooms>();
                
                foreach (ScheduleSubject scheduleSubject in findAllScheduleStudyRooms) {
                    BaseScheduleResData baseData = _helper.ScheduleSubjectFilledFieldData(scheduleSubject, filter);
                    ScheduleRooms scheduleRooms = _mapper.Map<ScheduleRooms>(baseData);

                    scheduleRooms.StudyGroupAliases = _helper.MappingScheduleGroups(scheduleSubject);
                    scheduleRooms.TeachersAliases = _helper.MappingScheduleTeachers(scheduleSubject);
                    
                    _helper.FilteringScheduleSubject(scheduleSubject, filter, ref allScheduleSubjectsBaseRoom, 
                        ref scheduleRooms);
                }
                
                _helper.AddBaseValuesForSingleDay(ref singleDay, weekday, filter, ref dayIncrement);
                singleDay.WeekdayData = allScheduleSubjectsBaseRoom;
                
                response.ScheduleCanvasData.Add(singleDay);
            }
            
            response.CurrentChooseWeek = filter.WeekInputOptions;
            return response;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Schedule subject details popup informations

        /// <summary>
        /// Metoda zwracająca szczegółowe dane na temat przedmiotu na podstawie ID przedmiotu przekazywanego w
        /// parametrach zapytania.
        /// </summary>
        /// <param name="schedSubjId">id przedmiotu na planie zajęć w bazie danych</param>
        /// <returns>obiekt transferowy z informacjami o przedmiocie</returns>
        /// <exception cref="BasicServerException">w przypadku błędego ID przedmiotu</exception>
        public async Task<ScheduleSubjectDetailsResDto> GetScheduleSubjectDetails(long schedSubjId)
        {
            ScheduleSubject findSubject = await _context.ScheduleSubjects
                .Include(s => s.StudyRooms)
                .Include(s => s.StudySubject)
                .Include(s => s.ScheduleTeachers)
                .Include(s => s.WeekScheduleOccurs)
                .Include(s => s.ScheduleSubjectType)
                .Include(s => s.StudySubject.Department)
                .FirstOrDefaultAsync(s => s.Id == schedSubjId);
            if (findSubject == null) {
                throw new BasicServerException("Nie znaleziono przedmiotu z planu zajęć z podanym ID",
                    HttpStatusCode.NotFound);
            }
            
            return new ScheduleSubjectDetailsResDto()
            {
                SubjectName = $"{findSubject.StudySubject.Name}",
                SubjectTypeColor = findSubject.ScheduleSubjectType.Color,
                SubjectHours = ApplicationUtils.FormatTime(findSubject),
                Teachers = string.Join(", ", findSubject.ScheduleTeachers.Select(t => $"{t.Name} {t.Surname}")),
                TypeAndRoomsName = $"{findSubject.ScheduleSubjectType.Name}, " +
                                   $"sala: {string.Join(", ", findSubject.StudyRooms.Select(r => r.Name))}",
                DepartmentName = $"{findSubject.StudySubject.Department.Name}, " +
                                 $"({findSubject.StudySubject.Department.Alias})",
                SubjectOccur = ApplicationUtils.ConvertScheduleOccur(findSubject),
            };
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete massive

        /// <summary>
        /// Metoda usuwająca wybrane przedmioty z planu zajęć (na podstawie wartości id w ciele zapytania).
        /// </summary>
        /// <param name="scheduleSubjects">wszystkie numery ID elementów do usunięcia</param>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteMassiveScheduleSubjects(MassiveDeleteRequestDto scheduleSubjects, 
            UserCredentialsHeaderDto credentials)
        {
            // wyszukaj przedmiot na podstawie pierwszego id w tabeli, jeśli nie znajdzie rzuć wyjątek
            ScheduleSubject findScheduleSubject = await _context.ScheduleSubjects
                .Include(s => s.StudySubject).ThenInclude(sb => sb.Department)
                .FirstOrDefaultAsync(d =>  scheduleSubjects.ElementsIds.Any(sb => sb == d.Id));
            if (findScheduleSubject == null) {
                throw new BasicServerException("Nie znaleziono przedmiotu na podstawie id", HttpStatusCode.NotFound);
            }
            
            // sprawdź, czy usunięcie jest realizowane z konta edytora, jeśli przejdź dalej
            if ((findScheduleSubject.StudySubject.Department.Id != credentials.Person.Department.Id && 
                 credentials.Person.Role.Name == AvailableRoles.EDITOR)) {
                // sprawdź, czy usunięcie jest realizowane z konta administratora jeśli nie wyrzuć wyjątek
                if (credentials.Person.Role.Name != AvailableRoles.ADMINISTRATOR) {
                    throw new BasicServerException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora " +
                                                   "lub próba usunięcia chronionego zasobu z rangą edytora.",
                        HttpStatusCode.Forbidden);
                }
            }
            
            // filtrowanie sal zajęciowych po ID znajdujących się w tablicy
            _context.ScheduleSubjects.RemoveRange(_context.ScheduleSubjects
                .Where(r => scheduleSubjects.ElementsIds.Any(id => id == r.Id)));
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}