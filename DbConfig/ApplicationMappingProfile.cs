using AutoMapper;

using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.DbConfig
{
    public sealed class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Person, RegisterNewUserResponseDto>()
                .ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name));
            CreateMap<Person, LoginResponseDto>()
                .ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name))
                .ForMember(dist => dist.NameWithSurname, from => from.MapFrom(dir => $"{dir.Name} {dir.Surname}"));
            CreateMap<Person, UserResponseDto>()
                .ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name))
                .ForMember(dist => dist.NameWithSurname, from => from.MapFrom(dir => $"{dir.Surname} {dir.Name}"));
        }
    }
}