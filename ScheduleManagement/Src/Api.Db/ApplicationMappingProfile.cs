using AutoMapper;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Network.Auth;
using ScheduleManagement.Api.Network.Cathedral;
using ScheduleManagement.Api.Network.ContactMessage;
using ScheduleManagement.Api.Network.Department;
using ScheduleManagement.Api.Network.ScheduleSubject;
using ScheduleManagement.Api.Network.StudyGroup;
using ScheduleManagement.Api.Network.StudyRoom;
using ScheduleManagement.Api.Network.StudySpec;
using ScheduleManagement.Api.Network.StudySubject;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Db;

public class ApplicationMappingProfile : Profile
{
	public ApplicationMappingProfile()
	{
		AddPersonMappers();
		AddCathedralMappers();
		AddStudySpecMappers();
		AddStudyRoomMappers();
		AddStudySubjectMappers();
		AddStudyGroupMappers();
		AddDepartmentMappers();
		AddContactMessageMapper();
		AddNameIdElementMapper();
		AddBaseScheduleMapper();
	}

	private void AddPersonMappers()
	{
		CreateMap<Person, RegisterUpdateUserResponseDto>()
			.ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name))
			.ForMember(dist => dist.DepartmentData, from => from
				.MapFrom(dir => $"{dir.Department!.Name} ({dir.Department.Alias})"))
			.ForMember(dist => dist.CathedralData, from => from
				.MapFrom(dir => $"{dir.Cathedral!.Name} ({dir.Cathedral.Alias})"));

		CreateMap<Person, LoginResponseDto>()
			.ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name))
			.ForMember(dist => dist.NameWithSurname, from => from.MapFrom(dir => $"{dir.Name} {dir.Surname}"));

		CreateMap<Person, UserResponseDto>()
			.ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name))
			.ForMember(dist => dist.NameWithSurname, from => from.MapFrom(dir => $"{dir.Surname} {dir.Name}"));

		CreateMap<Person, DashboardDetailsResDto>()
			.ForMember(dist => dist.DepartmentFullName,
				from => from.MapFrom(dir => $"{dir.Department!.Name} ({dir.Department.Alias})"));

		CreateMap<Person, UserDetailsEditResDto>()
			.ForMember(dist => dist.Role, from => from.MapFrom(dir => dir.Role.Name))
			.ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department!.Name))
			.ForMember(dist => dist.CathedralName, from => from.MapFrom(dir => dir.Cathedral!.Name));
	}

	private void AddCathedralMappers()
	{
		CreateMap<Cathedral, CathedralResponseDto>()
			.ForMember(dist => dist.DepartmentFullName,
				from => from.MapFrom(dir => $"{dir.Department.Name} ({dir.Department.Alias})"));

		CreateMap<Cathedral, CathedralQueryResponseDto>()
			.ForMember(dist => dist.DepartmentAlias, from => from.MapFrom(dir => dir.Department.Alias))
			.ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name));

		CreateMap<CathedralRequestDto, Cathedral>();

		CreateMap<Cathedral, CathedralEditResDto>()
			.ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name));
	}

	private void AddStudySpecMappers()
	{
		CreateMap<StudySpecialization, StudySpecResponseDto>()
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

		CreateMap<StudySpecialization, NameIdElementDto>()
			.ForMember(dist => dist.Name, from => from.MapFrom(dir => $"{dir.Name} ({dir.StudyType.Alias})"));

		CreateMap<StudySpecialization, StudySpecializationEditResDto>()
			.ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name))
			.ForMember(dist => dist.StudyType, from => from.MapFrom(dir => new List<long> { dir.StudyType.Id }))
			.ForMember(dist => dist.StudyDegree, from => from.MapFrom(dir => new List<long> { dir.StudyDegree.Id }));
	}

	private void AddStudyRoomMappers()
	{
		CreateMap<StudyRoom, StudyRoomResponseDto>()
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

		CreateMap<StudyRoom, StudyRoomEditResDto>()
			.ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name))
			.ForMember(dist => dist.CathedralName, from => from.MapFrom(dir => dir.Cathedral.Name))
			.ForMember(dist => dist.RoomTypeName,
				from => from.MapFrom(dir => $"{dir.RoomType.Name} ({dir.RoomType.Alias})"));
	}

	private void AddStudySubjectMappers()
	{
		CreateMap<StudySubject, StudySubjectResponseDto>()
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

		CreateMap<StudySubject, StudySubjectEditResDto>()
			.ForMember(dist => dist.DepartmentName, from => from.MapFrom(dir => dir.Department.Name))
			.ForMember(dist => dist.StudySpecName, from => from.MapFrom(dir =>
				$"{dir.StudySpecialization.Name} ({dir.StudySpecialization.StudyType.Alias} " +
				$"{dir.StudySpecialization.StudyDegree.Alias})"));
	}

	private void AddStudyGroupMappers()
	{
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
	}

	private void AddDepartmentMappers()
	{
		CreateMap<Department, DepartmentQueryResponseDto>();
		CreateMap<Department, SearchQueryResponseDto>();
		CreateMap<DepartmentRequestResponseDto, Department>();
		CreateMap<Department, DepartmentEditResDto>();
	}

	private void AddContactMessageMapper()
	{
		CreateMap<ContactMessage, ContactMessagesQueryResponseDto>()
			.ForMember(dist => dist.NameWithSurname, from => from.MapFrom(dir =>
				dir.Person == null ? $"{dir.AnonSurname} {dir.AnonName}" : $"{dir.Person.Surname} {dir.Person.Name}"))
			.ForMember(dist => dist.IssueType, from => from.MapFrom(dir => dir.ContactFormIssueType.Name))
			.ForMember(dist => dist.CreatedDate, from => from.MapFrom(dir => dir.CreatedDate.ToString("g")));

		CreateMap<ContactMessage, SingleContactMessageResponseDto>()
			.ForMember(dist => dist.NameWithSurname, from => from.MapFrom(dir =>
				dir.Person == null ? $"{dir.AnonSurname} {dir.AnonName}" : $"{dir.Person.Surname} {dir.Person.Name}"))
			.ForMember(dist => dist.IssueType, from => from.MapFrom(dir => dir.ContactFormIssueType.Name))
			.ForMember(dist => dist.CreatedDate, from => from.MapFrom(dir => dir.CreatedDate.ToString("g")))
			.ForMember(dist => dist.Email,
				from => from.MapFrom(dir => dir.Person == null ? dir.AnonEmail : dir.Person.Email));
	}

	private void AddNameIdElementMapper()
	{
		CreateMap<StudyType, NameIdElementDto>()
			.ForMember(dist => dist.Name, from => from.MapFrom(dir => $"{dir.Name} ({dir.Alias})"));

		CreateMap<StudyDegree, NameIdElementDto>()
			.ForMember(dist => dist.Name, from => from.MapFrom(dir => $"{dir.Name} ({dir.Alias})"));

		CreateMap<Person, NameIdElementDto>()
			.ForMember(dist => dist.Name, from => from.MapFrom(dir => $"{dir.Surname} {dir.Name}"));

		CreateMap<StudySpecialization, NameIdElementDto>()
			.ForMember(dist => dist.Name, from => from.MapFrom(dir =>
				$"{dir.Name} ({dir.StudyType.Alias}) ({dir.StudyDegree.Alias})"));

		CreateMap<StudySubject, NameIdElementDto>()
			.ForMember(dist => dist.Name, from => from.MapFrom(dir =>
				$"{dir.Name} ({dir.StudySpecialization.Alias} {dir.StudySpecialization.StudyType.Alias} " +
				$"{dir.StudySpecialization.StudyDegree.Alias})"));

		CreateMap<Semester, NameIdElementDto>();
		CreateMap<StudyRoom, NameIdElementDto>();
		CreateMap<Department, NameIdElementDto>();
		CreateMap<Cathedral, NameIdElementDto>();
		CreateMap<StudyDegree, NameIdElementDto>();
	}

	private void AddBaseScheduleMapper()
	{
		CreateMap<BaseScheduleResData, ScheduleGroups>();
		CreateMap<BaseScheduleResData, ScheduleTeachers>();
		CreateMap<BaseScheduleResData, ScheduleRooms>();
	}
}
