using AutoMapper;

using System.Net;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class StudyRoomsServiceImplementation : IStudyRoomsService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        //--------------------------------------------------------------------------------------------------------------

        public StudyRoomsServiceImplementation(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        public async Task<CreateStudyRoomResponseDto> CreateStudyRoom(CreateStudyRoomRequestDto dto)
        {
            //wyszukanie wydziału pasującego do katedry, jeśli nie znajdzie wyrzuci wyjątek
            Department findDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.ToLower() == dto.DepartmentName.ToLower());
            if (findDepartment == null) {
                throw new BasicServerException("Nie znaleziono wydziału z podaną nazwą", HttpStatusCode.NotFound);
            }

            //wyszukanie katedry pasującego do wyniku wyszukiwania, jeśli nie znajdzie wyrzuci wyjątek
            Cathedral findCathedral = await _context.Cathedrals
                .FirstOrDefaultAsync(c => c.Name.ToLower() == dto.CathedralName.ToLower());
            if (findCathedral == null) {
                throw new BasicServerException("Nie znaleziono katedry z podaną nazwą.", HttpStatusCode.NotFound);
            }

            //wyszukanie katedry pasującego do wyniku wyszukiwania, jeśli nie znajdzie wyrzuci wyjątek
            RoomType findRoomType = await _context.RoomTypes
                .FirstOrDefaultAsync(r => r.Alias.ToLower() == dto.RoomType.ToLower());
            if (findRoomType == null) {
                throw new BasicServerException("Nie znaleziono typu sali z podanym aliasem.", HttpStatusCode.NotFound);
            }

            StudyRoom findExistingRoom = await _context.StudyRooms
                .Include(r => r.Department)
                .Include(r => r.Cathedral)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Name.ToLower() == dto.Name.ToLower()
                                          && r.Department.Name.ToLower() == dto.DepartmentName.ToLower()
                                          && r.Cathedral.Name.ToLower() == dto.CathedralName.ToLower());

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
                DepartmentId = findDepartment.Id,
                RoomTypeId = findRoomType.Id,
            };

            await _context.AddAsync(createStudyRoom);
            await _context.SaveChangesAsync();

            return _mapper.Map<CreateStudyRoomResponseDto>(createStudyRoom);
        }

        #endregion
    }
}