using System;
using AutoMapper;

using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.Net;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

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
        public async Task<CreateStudyRoomResponseDto> CreateStudyRoom(CreateStudyRoomRequestDto dto)
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

            return _mapper.Map<CreateStudyRoomResponseDto>(createStudyRoom);
        }

        #endregion
    }
}