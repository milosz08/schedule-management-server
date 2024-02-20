using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;
using ScheduleManagement.Api.Util;

namespace ScheduleManagement.Api.Network.StudySubject;

public class StudySubjectServiceImpl(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher,
	ILogger<StudySubjectServiceImpl> logger,
	IMapper mapper)
	: AbstractExtendedCrudService(dbContext, passwordHasher), IStudySubjectService
{
	public async Task<StudySubjectResponseDto> CreateStudySubject(StudySubjectRequestDto dto)
	{
		var findSpecialization = await dbContext.StudySpecializations
			.Include(d => d.Department)
			.Include(d => d.StudyType)
			.Include(d => d.StudyDegree)
			.FirstOrDefaultAsync(s =>
				string.Equals(s.Name + " (" + s.StudyType.Alias + " " + s.StudyDegree.Alias + ")",
					dto.StudySpecName, StringComparison.OrdinalIgnoreCase) &&
				s.Department.Name.Equals(dto.DepartmentName, StringComparison.InvariantCulture));

		if (findSpecialization == null)
		{
			throw new RestApiException(
				"Nie znaleziono kierunku/wydziału z podaną nazwą", HttpStatusCode.NotFound);
		}
		var findSubject = await dbContext.StudySubjects
			.Include(s => s.Department)
			.Include(s => s.StudySpecialization)
			.FirstOrDefaultAsync(s =>
				s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) &&
				s.StudySpecialization.Name.Equals(dto.StudySpecName, StringComparison.OrdinalIgnoreCase)
				&& s.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));

		if (findSubject != null)
		{
			throw new RestApiException(
				"Podany przedmiot istnieje już na wybranym kierunku.", HttpStatusCode.ExpectationFailed);
		}
		var studySubject = new Entity.StudySubject
		{
			Name = dto.Name,
			Alias = $"{StringUtils.CreateSubjectAlias(dto.Name)}/{findSpecialization.Alias}" +
			        $"/{findSpecialization.Department.Alias}",
			DepartmentId = findSpecialization.DepartmentId,
			StudySpecializationId = findSpecialization.Id
		};
		await dbContext.AddAsync(studySubject);
		await dbContext.SaveChangesAsync();

		var resDto = mapper.Map<StudySubjectResponseDto>(studySubject);

		logger.LogInformation("Successfully created new study subject: {}", resDto);
		return resDto;
	}

	public async Task<StudySubjectResponseDto> UpdateStudySubject(StudySubjectRequestDto dto, long subjId)
	{
		var findStudySubject = await dbContext.StudySubjects
			.Include(s => s.Department)
			.Include(s => s.StudySpecialization).ThenInclude(sp => sp.Department)
			.FirstOrDefaultAsync(s => s.Id == subjId);

		if (findStudySubject == null)
		{
			throw new RestApiException("Nie znaleziono przedmiotu z podanym id", HttpStatusCode.NotFound);
		}
		if (dto.Name.Equals(findStudySubject.Name))
		{
			throw new RestApiException("Należy wprowadzić wartości różne od poprzednich.",
				HttpStatusCode.ExpectationFailed);
		}
		findStudySubject.Name = dto.Name;
		findStudySubject.Alias = $"{StringUtils.CreateSubjectAlias(dto.Name)}" +
		                         $"/{findStudySubject.StudySpecialization.Alias}" +
		                         $"/{findStudySubject.StudySpecialization.Department.Alias}";

		await dbContext.SaveChangesAsync();
		var resDto = mapper.Map<StudySubjectResponseDto>(findStudySubject);

		logger.LogInformation("Successfully updated study subject with ID: {}: {}", subjId, resDto);
		return resDto;
	}

	public PaginationResponseDto<StudySubjectQueryResponseDto> GetAllStudySubjects(SearchQueryRequestDto searchQuery)
	{
		var studySubjectsBaseQuery = dbContext.StudySubjects
			.Include(s => s.Department)
			.Include(s => s.StudySpecialization)
			.Include(s => s.StudySpecialization.StudyType)
			.Include(s => s.StudySpecialization.StudyDegree)
			.Where(s => searchQuery.SearchPhrase == null
			            || s.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(searchQuery.SortBy))
		{
			PaginationConfig.ConfigureSorting(new Dictionary<string, Expression<Func<Entity.StudySubject, object>>>
			{
				{ nameof(Entity.StudyRoom.Id), s => s.Id },
				{ nameof(Entity.StudyRoom.Name), s => s.Name },
				{ "DepartmentAlias", s => s.Department.Alias },
				{ "SpecTypeAlias", s => s.StudySpecialization.Alias }
			}, searchQuery, ref studySubjectsBaseQuery);
		}
		var allDepts = mapper.Map<List<StudySubjectQueryResponseDto>>(PaginationConfig
			.ConfigureAdditionalFiltering(studySubjectsBaseQuery, searchQuery));

		return new PaginationResponseDto<StudySubjectQueryResponseDto>(
			allDepts, studySubjectsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
	}

	public SearchQueryResponseDto GetAllStudySubjectsBaseDeptAndSpec(string? subjcName, long deptId, long studySpecId)
	{
		subjcName ??= string.Empty;

		var findAllSubjects = dbContext.StudySubjects
			.Include(s => s.Department)
			.Include(s => s.StudySpecialization)
			.Where(s => s.Department.Id == deptId && s.StudySpecialization.Id == studySpecId &&
			            s.Name.Contains(subjcName, StringComparison.OrdinalIgnoreCase))
			.Select(s => s.Name)
			.ToList();
		findAllSubjects.Sort();

		if (findAllSubjects.Count > 0)
		{
			return new SearchQueryResponseDto(findAllSubjects);
		}
		var findAllElements = dbContext.StudySubjects
			.Include(s => s.Department)
			.Where(s => s.Department.Id == deptId && s.StudySpecialization.Id == studySpecId)
			.Select(s => s.Name)
			.ToList();
		findAllElements.Sort();

		return new SearchQueryResponseDto(findAllElements);
	}

	public async Task<StudySubjectEditResDto> GetStudySubjectBaseDbId(long subjId)
	{
		var findStudySubject = await dbContext.StudySubjects
			.Include(s => s.Department)
			.Include(s => s.StudySpecialization).ThenInclude(sp => sp.StudyType)
			.Include(s => s.StudySpecialization).ThenInclude(sp => sp.StudyDegree)
			.FirstOrDefaultAsync(s => s.Id == subjId);
		if (findStudySubject == null)
		{
			throw new RestApiException("Nie znaleziono szukanego przedmiotu.", HttpStatusCode.NotFound);
		}
		return mapper.Map<StudySubjectEditResDto>(findStudySubject);
	}

	public async Task<AvailableDataResponseDto<NameIdElementDto>> GetAvailableSubjectsBaseDept(string deptName)
	{
		var findAllStudySubjects = await dbContext.StudySubjects
			.Include(s => s.Department)
			.Include(s => s.StudySpecialization)
			.Include(s => s.StudySpecialization.StudyType)
			.Include(s => s.StudySpecialization.StudyDegree)
			.Where(s => deptName.Equals(s.Department.Name, StringComparison.OrdinalIgnoreCase))
			.ToListAsync();

		return new AvailableDataResponseDto<NameIdElementDto>
		{
			DataElements = findAllStudySubjects.Select(mapper.Map<NameIdElementDto>).ToList()
		};
	}

	protected override async Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);
		}
		var message = "Nie usunięto żadnego przedmiotu.";
		var toRemoved = dbContext.StudySubjects.Where(s => items.ElementsIds.Any(id => id == s.Id));
		if (toRemoved.Any())
		{
			message = $"Pomyślnie usunięto wybrane przedmioty. Liczba usuniętych przedmiotów: {toRemoved.Count()}.";
		}
		dbContext.StudySubjects.RemoveRange(toRemoved);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} schedule subjects", toRemoved.Count());
		return new MessageContentResDto
		{
			Message = message
		};
	}

	protected override async Task<MessageContentResDto> OnDeleteAll(UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);
		}
		var count = dbContext.StudySubjects.Count();

		dbContext.StudySubjects.RemoveRange();
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} schedule subjects", count);
		return new MessageContentResDto
		{
			Message = "Pomyślnie usunięto wszystkie przedmioty."
		};
	}
}
