using AutoMapper;

using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Dto.AuthDtos;


namespace asp_net_po_schedule_management_server.DbConfig
{
    public sealed class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<RegisterNewUserRequestDto, Person>();
            CreateMap<Person, RegisterNewUserResponseDto>();
        }
    }
}