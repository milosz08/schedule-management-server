using AutoMapper;

using asp_net_po_schedule_management_server.Entities;

using asp_net_po_schedule_management_server.Dto.Responses;
using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;


namespace asp_net_po_schedule_management_server.DbConfig
{
    public sealed class ApplicationMappingProfile : Profile
    {
        /// <summary>
        /// Konstruktor odpowiadający za stworzenie map obiektów (przepisania wartości). 
        /// </summary>
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

            CreateMap<Cathedral, CreatedCathedralResponseDto>()
                .ForMember(dist => dist.DepartmentAlias, from => from.MapFrom(dir => dir.Department.Alias))
                .ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name));
            
            CreateMap<StudySpecialization, StudySpecResponseDto>()
                .ForMember(dist => dist.DepartmentAlias, from => from.MapFrom(dir => dir.Department.Alias))
                .ForMember(dist => dist.StudyTypeFullName, from => from.MapFrom(dir => dir.StudyType.Name))
                .ForMember(dist => dist.StudyType, from => from.MapFrom(dir => dir.StudyType.Alias));

            CreateMap<StudyRoom, CreateStudyRoomResponseDto>()
                .ForMember(dist => dist.DepartmentAlias, from => from.MapFrom(dir => dir.Department.Alias))
                .ForMember(dist => dist.CathedralAlias, from => from.MapFrom(dir => dir.Cathedral.Alias))
                .ForMember(dist => dist.RoomTypeName, from => from.MapFrom(dir => dir.RoomType.Name))
                .ForMember(dist => dist.RoomType, from => from.MapFrom(dir => dir.RoomType.Alias));
            
            CreateMap<CreateDepartmentRequestResponseDto, Department>();
            CreateMap<CreateCathedralRequestDto, Cathedral>();
            CreateMap<StudyType, StudySingleTypeDto>();
            CreateMap<Department, SearchQueryResponseDto>();
            CreateMap<RoomType, SingleRoomTypeDto>();
        }
    }
}