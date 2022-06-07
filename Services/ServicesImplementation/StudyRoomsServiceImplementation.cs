using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class StudyRoomsServiceImplementation : IStudyRoomsService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _helper;
        private readonly ApplicationDbContext _context;

        //--------------------------------------------------------------------------------------------------------------

        public StudyRoomsServiceImplementation(ServiceHelper helper, ApplicationDbContext context, IMapper mapper)
        {
            _helper = helper;
            _mapper = mapper;
            _context = context;
        }

        //--------------------------------------------------------------------------------------------------------------

        #region Create study room

        /// <summary>
        /// Metoda umożliwiająca dodanie nowego pokoju do bazy danych. Pokój ten jest zmapowany bezpośrednio z
        /// wydziałem, katedrą oraz jego typem. 
        /// </summary>
        /// <param name="dto">obiekt transferowy przechowujące dane nowej sali</param>
        /// <returns>dane nowej sali uzupełnione o dodatkowe informacje</returns>
        /// <exception cref="BasicServerException">
        /// Jeśli nie znajdzie katedry/wydziału/typu lub jeśli zajdzie próba wprowadzenia duplikatu.
        /// </exception>
        public async Task<StudyRoomResponseDto> CreateStudyRoom(StudyRoomRequestDto dto)
        {
            //wyszukanie wydziału pasującego do katedry, jeśli nie znajdzie wyrzuci wyjątek
            Cathedral findCathedral = await _context.Cathedrals
                .Include(c => c.Department)
                .AsSingleQuery()
                .FirstOrDefaultAsync(c => c.Name.Equals(dto.CathedralName, StringComparison.OrdinalIgnoreCase)
                                          && c.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));
            if (findCathedral == null) {
                throw new BasicServerException("Nie znaleziono wydziału/katedry z podaną nazwą", HttpStatusCode.NotFound);
            }
            
            //wyszukanie katedry pasującego do wyniku wyszukiwania, jeśli nie znajdzie wyrzuci wyjątek
            RoomType findRoomType = await _context.RoomTypes
                .FirstOrDefaultAsync(r => string
                    .Equals(r.Name + " (" + r.Alias + ")", dto.RoomTypeName, StringComparison.OrdinalIgnoreCase));
            if (findRoomType == null) {
                throw new BasicServerException("Nie znaleziono typu sali z podanym aliasem.", HttpStatusCode.NotFound);
            }

            // przy próbie dodania duplikatu, wyrzuć wyjątek
            StudyRoom findExistingRoom = await _context.StudyRooms
                .Include(r => r.Department)
                .Include(r => r.Cathedral)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) &&
                                          r.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase) &&
                                          r.Cathedral.Name.Equals(dto.CathedralName, StringComparison.OrdinalIgnoreCase));
            
            if (findExistingRoom != null) {
                throw new BasicServerException("Podana sala istnieje już w wybranej jednostce",
                    HttpStatusCode.ExpectationFailed);
            }

            StudyRoom createStudyRoom = new StudyRoom()
            {
                Name = dto.Name.ToUpper(),
                Description = dto.Description,
                Capacity = dto.Capacity,
                CathedralId = findCathedral.Id,
                DepartmentId = findCathedral.Department.Id,
                RoomTypeId = findRoomType.Id,
            };

            await _context.AddAsync(createStudyRoom);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudyRoomResponseDto>(createStudyRoom);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Update study room

        /// <summary>
        /// Metoda odpowiedzialna za aktualizowanie danych wybranej sali (na podstawie ciała zapytania i parametrów).
        /// </summary>
        /// <param name="dto">obiekt z danymi do zamiany</param>
        /// <param name="roomId">id sali zajęciowej podlegającej zamianie</param>
        /// <returns>zamienione dane w postaci obiektu transferowego</returns>
        /// <exception cref="BasicServerException">jeśli nie znajdzie sali/próba wprowadzenia tych samych danych</exception>
        public async Task<StudyRoomResponseDto> UpdateStudyRoom(StudyRoomRequestDto dto, long roomId)
        {
            // wyszukaj salę zajęciową do zamiany na podstawie id, jeśli nie znajdzie rzuć wyjątek
            StudyRoom findStudyRoom = await _context.StudyRooms
                .Include(r => r.RoomType)
                .Include(r => r.Cathedral)
                .Include(r => r.Department)
                .FirstOrDefaultAsync(r => r.Id == roomId);
            if (findStudyRoom == null) {
                throw new BasicServerException("Nie znaleziono sali zajęciowej z podanym id", HttpStatusCode.NotFound);
            }
            
            string roomTypeWithAlias = $"{findStudyRoom.RoomType.Name} ({findStudyRoom.RoomType.Alias})";
            // sprawdź, czy nie zachodzi próba dodania niezaktualizowanych wartości
            if (findStudyRoom.Name == dto.Name && findStudyRoom.Description == dto.Description &&
                findStudyRoom.Capacity == dto.Capacity && roomTypeWithAlias == dto.RoomTypeName) {
                throw new BasicServerException("Należy wprowadzić wartości różne od poprzednich.", 
                    HttpStatusCode.ExpectationFailed);
            }

            // wyszukaj inny typ sali zajęciowej, jeśli nie znajdzie rzuć wyjątek
            RoomType findRoomType = await _context.RoomTypes
                .FirstOrDefaultAsync(t => t.Name + " (" + t.Alias + ")" == dto.RoomTypeName);
            if (findRoomType == null) {
                throw new BasicServerException("Nie znaleziono typu sali na podstawie nazwy", HttpStatusCode.NotFound);
            }
            
            findStudyRoom.Name = dto.Name;
            findStudyRoom.Description = dto.Description;
            findStudyRoom.Capacity = dto.Capacity;
            findStudyRoom.RoomType.Id = findRoomType.Id;
            
            await _context.SaveChangesAsync();
            
            return _mapper.Map<StudyRoomResponseDto>(findStudyRoom);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Get all study rooms
        
        /// <summary>
        /// Metoda zwracająca wszystkie sale zajęciowe opakowane w obiekt paginacji i fitrowania rezultatów na podstawie
        /// przekazywanych parametrów zapytania. Umożliwia sortowanie po kolumnach (kluczach) w trybach ASC/DES.
        /// </summary>
        /// <param name="searchQuery">parametry zapytania (filtrowania wyników)</param>
        /// <returns>opakowane dane wynikowe w obiekt paginacji</returns>
        public PaginationResponseDto<StudyRoomQueryResponseDto> GetAllStudyRooms(SearchQueryRequestDto searchQuery)
        {
            // wyszukiwanie użytkowników przy pomocy parametru SearchPhrase
            IQueryable<StudyRoom> studyRoomsBaseQuery = _context.StudyRooms
                .Include(r => r.Department)
                .Include(r => r.Cathedral)
                .Include(r => r.RoomType)
                .Where(r => searchQuery.SearchPhrase == null ||
                            r.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

            // sortowanie (rosnąco/malejąco) dla kolumn
            if (!string.IsNullOrEmpty(searchQuery.SortBy)) {
                _helper.PaginationSorting(new Dictionary<string, Expression<Func<StudyRoom, object>>>
                {
                    { nameof(StudyRoom.Id), r => r.Id },
                    { nameof(StudyRoom.Name), r => r.Name },
                    { nameof(StudyRoom.Capacity), r => r.Capacity },
                    { "DepartmentAlias", r => r.Department.Alias },
                    { "CathedralAlias", r => r.Cathedral.Alias },
                    { "RoomTypeAlias", r => r.RoomType.Alias }
                }, searchQuery, ref studyRoomsBaseQuery);
            }
            
            List<StudyRoomQueryResponseDto> allDepts = _mapper.Map<List<StudyRoomQueryResponseDto>>(_helper
                .PaginationAndAdditionalFiltering(studyRoomsBaseQuery, searchQuery));
            
            return new PaginationResponseDto<StudyRoomQueryResponseDto>(
                allDepts, studyRoomsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all study room base cathedral database id

        /// <summary>
        /// Metoda zwracająca wszystkie sale zajęciowe w postaci tupli (name, id) na podstawie id wydziału i katedry.
        /// Używana dla punktu końcowego niechronionego, przy wyświetlaniu planu.
        /// </summary>
        /// <returns>wszystkie znalezione sale zajęciowe</returns>
        public async Task<List<NameWithDbIdElement>> GetAllStudyRoomsScheduleBaseCath(long deptId, long cathId)
        {
            List<StudyRoom> studyRoomBaseDeptAndCath = await _context.StudyRooms
                .Include(r => r.Department)
                .Include(r => r.Cathedral)
                .Where(r => r.Department.Id == deptId && (r.Cathedral.Id == cathId))
                .ToListAsync();
            studyRoomBaseDeptAndCath.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));
            return studyRoomBaseDeptAndCath.Select(d => new NameWithDbIdElement(d.Id, d.Name)).ToList();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all study room base department id
        
        /// <summary>
        /// Metoda zwracająca wszystkie sale zajęciowe w postaci tupli (name, id) na podstawie id wydziału.
        /// </summary>
        /// <param name="deptId">id wydziału</param>
        /// <returns>wszystkie znalezione sale zajęciowe</returns>
        public async Task<List<NameWithDbIdElement>> GetAllStudyRoomsScheduleBaseDeptName(long deptId)
        {
            List<NameWithDbIdElement> studyRoomBaseDeptAndCath = await _context.StudyRooms
                .Include(r => r.Department)
                .Where(r => r.Department.Id == deptId)
                .Select(d => new NameWithDbIdElement(d.Id, d.Name))
                .ToListAsync();
            studyRoomBaseDeptAndCath.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));
            return studyRoomBaseDeptAndCath;
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Get study room data base study room id

        /// <summary>
        /// Metoda pobierająca zawartość sali zajęciowej z bazy danych na podstawie przekazywanego parametru id w
        /// parametrach zapytania HTTP. Metoda używana głównie w celu aktualizacji istniejących treści w serwisie.
        /// </summary>
        /// <param name="roomId">id sali zajęciowej</param>
        /// <returns>obiekt transferowy z danymi konkretnej sali zajęciowej</returns>
        /// <exception cref="BasicServerException">w przypadku nieznalezienia sali z podanym id</exception>
        public async Task<StudyRoomEditResDto> GetStudyRoomBaseDbId(long roomId)
        {
            // wyszukaj katedrę na podstawie parametru ID w bazie danych, jeśli nie znajdzie rzuć 404.
            StudyRoom findStudyRoom = await _context.StudyRooms
                .Include(r => r.RoomType)
                .Include(r => r.Cathedral)
                .Include(r => r.Department)
                .FirstOrDefaultAsync(r => r.Id == roomId);
            if (findStudyRoom == null) {
                throw new BasicServerException("Nie znaleziono sali z podanym numerem id.", HttpStatusCode.NotFound);
            }

            return _mapper.Map<StudyRoomEditResDto>(findStudyRoom);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete massive

        /// <summary>
        /// Metoda usuwająca wybrane sale zajęciowe z bazy danych (na podstawie wartości id w ciele zapytania).
        /// </summary>
        /// <param name="studyRooms">wszystkie numery ID elementów do usunięcia</param>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteMassiveStudyRooms(MassiveDeleteRequestDto studyRooms, UserCredentialsHeaderDto credentials)
        {
            // sprawdź, czy usunięcie jest realizowane z konta administratora, jeśli nie wyrzuć wyjątek
            if (credentials.Person.Role.Name != AvailableRoles.ADMINISTRATOR) {
                throw new BasicServerException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
                    HttpStatusCode.Forbidden);
            }
            
            // filtrowanie sal zajęciowych po ID znajdujących się w tablicy
            _context.StudyRooms.RemoveRange(_context.StudyRooms
                .Where(r => studyRooms.ElementsIds.Any(id => id == r.Id)));
            await _context.SaveChangesAsync();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Delete all

        /// <summary>
        /// Metoda usuwająca z bazy danych wszystkie sale zajęciowe.
        /// </summary>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteAllStudyRooms(UserCredentialsHeaderDto credentials)
        {
            // sprawdź, czy usunięcie jest realizowane z konta administratora, jeśli nie wyrzuć wyjątek
            if (credentials.Person.Role.Name != AvailableRoles.ADMINISTRATOR) {
                throw new BasicServerException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
                    HttpStatusCode.Forbidden);
            }
            
            _context.StudyRooms.RemoveRange();
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}