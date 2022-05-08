using AutoMapper;

using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public class UsersServiceImplementation : IUsersService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public UsersServiceImplementation(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all users

        // metoda zwracająca wszystkich użytkowników na podstawie wyszukiwanej frazy (oraz paginacji rezultatów)
        public PaginationResponseDto<UserResponseDto> GetAllUsers(UserQueryRequestDto query)
        {
            // wyszukiwanie użytkowników przy pomocy parametru SearchPhrase
            IQueryable<Person> usersBaseQuery = _context.Persons
                .Include(p => p.Role)
                .Where(p => query.SearchPhrase == null || p.Surname.ToLower().Contains(query.SearchPhrase.ToLower()));

            // sortowanie użytkowników (rosnąco/malejąco)
            usersBaseQuery = query.SortDirection == SortDirection.ASC
                ? usersBaseQuery.OrderBy(p => p.Id)
                : usersBaseQuery.OrderByDescending(p => p.Id);
            
            // paginacja i dodatkowe filtrowanie
            List<Person> findAllUsers = usersBaseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            List<UserResponseDto> allUsers = _mapper.Map<List<UserResponseDto>>(findAllUsers);
            return new PaginationResponseDto<UserResponseDto>(
                allUsers, usersBaseQuery.Count(), query.PageSize, query.PageNumber);
        }

        #endregion
    }
}