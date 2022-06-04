using AutoMapper;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.DbConfig
{
    /// <summary>
    /// Klasa odpowiadająca za mapowanie obiektów na inne obiekty (przepisywanie wartości). 
    /// </summary>
    public sealed class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<Person, RegisterNewUserResponseDto>()
                .ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name))
                .ForMember(dist => dist.DepartmentData, from => from
                    .MapFrom(dir => $"{dir.Department.Name} ({dir.Department.Alias})"))
                .ForMember(dist => dist.CathedralData, from => from
                    .MapFrom(dir => $"{dir.Cathedral.Name} ({dir.Cathedral.Alias})"));

            CreateMap<Person, LoginResponseDto>()
                .ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name))
                .ForMember(dist => dist.NameWithSurname, from => from.MapFrom(dir => $"{dir.Name} {dir.Surname}"));

            CreateMap<Person, UserResponseDto>()
                .ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name))
                .ForMember(dist => dist.NameWithSurname, from => from.MapFrom(dir => $"{dir.Surname} {dir.Name}"));

            CreateMap<Person, DashboardDetailsResDto>()
                .ForMember(dist => dist.DepartmentFullName,
                    from => from.MapFrom(dir => $"{dir.Department.Name} ({dir.Department.Alias})"));

            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<Cathedral, CathedralResponseDto>()
                .ForMember(dist => dist.DepartmentFullName,
                    from => from.MapFrom(dir => $"{dir.Department.Name} ({dir.Department.Alias})"));

            CreateMap<Cathedral, CathedralQueryResponseDto>()
                .ForMember(dist => dist.DepartmentAlias, from => from.MapFrom(dir => dir.Department.Alias))
                .ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name));
            
            CreateMap<CathedralRequestDto, Cathedral>();
            
            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<StudySpecialization, CreateStudySpecResponseDto>()
                .ForMember(dist => dist.DepartmentFullName,
                    from => from.MapFrom(dir => $"{dir.Department.Name} ({dir.Department.Alias})"))
                .ForMember(dist => dist.StudyTypeFullName,
                    from => from.MapFrom(dir => $"{dir.StudyType.Name} ({dir.StudyType.Alias})"))
                .ForMember(dist => dist.StudyDegreeFullName,
                    from => from.MapFrom(dir => $"{dir.StudyDegree.Name} ({dir.StudyDegree.Alias})"));

            CreateMap<StudySpecialization, StudySpecQueryResponseDto>()
                .ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name))
                .ForMember(dist => dist.DepartmentAlias, from => from.MapFrom(dir => dir.Department.Alias))
                .ForMember(dist => dist.SpecTypeName, from => from.MapFrom(dir => dir.StudyType.Name))
                .ForMember(dist => dist.SpecTypeAlias, from => from.MapFrom(dir => dir.StudyType.Alias))
                .ForMember(dist => dist.StudyDegree, from => from.MapFrom(dir => dir.StudyDegree.Name))
                .ForMember(dist => dist.StudyDegreeAlias, from => from.MapFrom(dir => dir.StudyDegree.Alias));
            
            CreateMap<StudySpecialization, NameWithDbIdElement>()
                .ForMember(dist => dist.Name, from => from.MapFrom(dir => $"{dir.Name} ({dir.StudyType.Alias})"));
            
            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<StudyRoom, CreateStudyRoomResponseDto>()
                .ForMember(dist => dist.DepartmentFullName,
                    from => from.MapFrom(dir => $"{dir.Department.Name} ({dir.Department.Alias})"))
                .ForMember(dist => dist.CathedralFullName,
                    from => from.MapFrom(dir => $"{dir.Cathedral.Name} ({dir.Cathedral.Alias})"))
                .ForMember(dist => dist.RoomTypeFullName,
                    from => from.MapFrom(dir => $"{dir.RoomType.Name} ({dir.RoomType.Alias})"));

            CreateMap<StudyRoom, StudyRoomQueryResponseDto>()
                .ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name))
                .ForMember(dist => dist.DepartmentAlias, from => from.MapFrom(dir => dir.Department.Alias))
                .ForMember(dist => dist.CathedralName, from => from.MapFrom(dir => dir.Cathedral.Name))
                .ForMember(dist => dist.CathedralAlias, from => from.MapFrom(dir => dir.Cathedral.Alias))
                .ForMember(dist => dist.DeptWithCathAlias, from => from.MapFrom(dir =>
                    $"{dir.Department.Alias} / {dir.Cathedral.Alias}"));
            
            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<StudySubject, CreateStudySubjectResponseDto>()
                .ForMember(dist => dist.DepartmentFullName,
                    from => from.MapFrom(dir => $"{dir.Department.Name} ({dir.Department.Alias})"))
                .ForMember(dist => dist.StudySpecFullName,
                    from => from.MapFrom(dir => $"{dir.StudySpecialization.Name} ({dir.StudySpecialization.Alias})"));

            CreateMap<StudySubject, StudySubjectQueryResponseDto>()
                .ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name))
                .ForMember(dist => dist.DepartmentAlias, from => from.MapFrom(dir => dir.Department.Alias))
                .ForMember(dist => dist.SpecName, from => from.MapFrom(dir => dir.StudySpecialization.Name))
                .ForMember(dist => dist.SpecAlias, from => from.MapFrom(dir =>
                    $"{dir.StudySpecialization.Alias} ({dir.StudySpecialization.StudyType.Alias} " +
                    $"{dir.StudySpecialization.StudyDegree.Alias})"));
            
            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<StudyGroup, CreateStudyGroupResponseDto>()
                .ForMember(dist => dist.SemesterName, from => from.MapFrom(dir => dir.Semester.Name))
                .ForMember(dist => dist.DepartmentFullName,
                    from => from.MapFrom(dir => $"{dir.Department.Name} ({dir.Department.Alias})"))
                .ForMember(dist => dist.StudySpecFullName,
                    from => from.MapFrom(dir => $"{dir.StudySpecialization.Name} ({dir.StudySpecialization.Alias})"));
            
            CreateMap<StudyGroup, StudyGroupQueryResponseDto>()
                .ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name))
                .ForMember(dist => dist.DepartmentAlias, from => from.MapFrom(dir => dir.Department.Alias))
                .ForMember(dist => dist.StudySpecName, from => from.MapFrom(dir => dir.StudySpecialization.Name))
                .ForMember(dist => dist.StudySpecAlias, from => from.MapFrom(dir => dir.StudySpecialization.Alias));
            
            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<Department, DepartmentQueryResponseDto>();
            CreateMap<Department, SearchQueryResponseDto>();
            CreateMap<DepartmentRequestResponseDto, Department>();
            
            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<StudyType, NameWithDbIdElement>()
                .ForMember(dist => dist.Name, from => from.MapFrom(dir => $"{dir.Name} ({dir.Alias})"));

            CreateMap<StudyDegree, NameWithDbIdElement>()
                .ForMember(dist => dist.Name, from => from.MapFrom(dir => $"{dir.Name} ({dir.Alias})"));
            
            CreateMap<Person, NameWithDbIdElement>()
                .ForMember(dist => dist.Name, from => from.MapFrom(dir => $"{dir.Surname} {dir.Name}"));
            
            CreateMap<StudySpecialization, NameWithDbIdElement>()
                .ForMember(dist => dist.Name, from => from.MapFrom(dir => 
                    $"{dir.Name} ({dir.StudyType.Alias}) ({dir.StudyDegree.Alias})"));
            
            CreateMap<StudySubject, NameWithDbIdElement>()
                .ForMember(dist => dist.Name, from => from.MapFrom(dir => 
                    $"{dir.Name} ({dir.StudySpecialization.Alias} {dir.StudySpecialization.StudyType.Alias} " +
                    $"{dir.StudySpecialization.StudyDegree.Alias})"));

            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<BaseScheduleResData, ScheduleGroups>();
            CreateMap<BaseScheduleResData, ScheduleTeachers>();
            CreateMap<BaseScheduleResData, ScheduleRooms>();
            
            //----------------------------------------------------------------------------------------------------------
            
            CreateMap<Semester, NameWithDbIdElement>();
            CreateMap<StudyRoom, NameWithDbIdElement>();
            CreateMap<Department, NameWithDbIdElement>();
            CreateMap<Cathedral, NameWithDbIdElement>();
            CreateMap<StudyDegree, NameWithDbIdElement>();
        }
    }
}