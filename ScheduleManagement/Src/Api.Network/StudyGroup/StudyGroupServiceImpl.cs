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

namespace ScheduleManagement.Api.Network.StudyGroup;

public class StudyGroupServiceImpl(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher,
	ILogger<StudyGroupServiceImpl> logger,
	IMapper mapper)
	: AbstractExtendedCrudService(dbContext, passwordHasher), IStudyGroupService
{
	public async Task<List<CreateStudyGroupResponseDto>> CreateStudyGroup(CreateStudyGroupRequestDto dto)
	{
		var findStudySpec = await dbContext.StudySpecializations
			.Include(s => s.Department)
			.Include(s => s.StudyType)
			.Include(s => s.StudyDegree)
			.FirstOrDefaultAsync(s =>
				string.Equals(s.Name + " (" + s.StudyType.Alias + " " + s.StudyDegree.Alias + ")",
					dto.StudySpecName, StringComparison.OrdinalIgnoreCase) &&
				s.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));

		if (findStudySpec == null)
		{
			throw new RestApiException("Nie znaleziono pasującego wydziału/kierunku", HttpStatusCode.NotFound);
		}
		var findAllSemesters = await dbContext.Semesters
			.Where(s => dto.Semesters.Any(id => id == s.Id))
			.ToListAsync();

		if (findAllSemesters.Count == 0)
		{
			throw new RestApiException("Nie znaleziono pasujących semestrów.", HttpStatusCode.NotFound);
		}

		List<Entity.StudyGroup> createdAllStudyGrops = [];

		foreach (var semester in findAllSemesters)
		{
			var findAllAlreadyCreated = await dbContext.StudyGroups
				.Include(g => g.Semester)
				.Include(g => g.StudySpecialization)
				.Where(g => g.Semester.Id == semester.Id && g.StudySpecialization.Id == findStudySpec.Id)
				.ToListAsync();

			for (var i = findAllAlreadyCreated.Count; i < dto.CountOfGroups + findAllAlreadyCreated.Count; i++)
			{
				var createdStudyGroup = new Entity.StudyGroup
				{
					Name = $"{findStudySpec.StudyDegree.Alias} {findStudySpec.Alias} " +
					       $"{findStudySpec.StudyType.Alias} {semester.Alias[4..]}/{i + 1}",
					DepartmentId = findStudySpec.Department.Id,
					StudySpecializationId = findStudySpec.Id,
					SemesterId = semester.Id
				};
				var findExistingStudyGroup = await dbContext.StudyGroups
					.FirstOrDefaultAsync(g => g.Name.Equals(createdStudyGroup.Name));

				if (findExistingStudyGroup != null)
				{
					throw new RestApiException("Podana grupa istnieje już w systemie.",
						HttpStatusCode.ExpectationFailed);
				}
				createdAllStudyGrops.Add(createdStudyGroup);
			}
		}
		await dbContext.StudyGroups.AddRangeAsync(createdAllStudyGrops);
		await dbContext.SaveChangesAsync();

		var resDto = createdAllStudyGrops
			.Select(mapper.Map<CreateStudyGroupResponseDto>)
			.ToList();

		logger.LogInformation("Successfully created new study group: {}", resDto);
		return resDto;
	}

	public PaginationResponseDto<StudyGroupQueryResponseDto> GetAllStudyGroups(SearchQueryRequestDto searchQuery)
	{
		var studyGroupsBaseQuery = dbContext.StudyGroups
			.Include(r => r.Department)
			.Include(r => r.Semester)
			.Include(r => r.StudySpecialization)
			.Where(r => searchQuery.SearchPhrase == null ||
			            r.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(searchQuery.SortBy))
		{
			PaginationConfig.ConfigureSorting(new Dictionary<string, Expression<Func<Entity.StudyGroup, object>>>
			{
				{ nameof(Entity.StudyGroup.Id), r => r.Id },
				{ nameof(Entity.StudyGroup.Name), r => r.Name },
				{ "DepartmentAlias", r => r.Department.Alias },
				{ "SpecTypeAlias", r => r.StudySpecialization.Alias }
			}, searchQuery, ref studyGroupsBaseQuery);
		}
		var allGroups = mapper.Map<List<StudyGroupQueryResponseDto>>(PaginationConfig
			.ConfigureAdditionalFiltering(studyGroupsBaseQuery, searchQuery));

		return new PaginationResponseDto<StudyGroupQueryResponseDto>(
			allGroups, studyGroupsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
	}

	public async Task<List<NameIdElementDto>> GetAvailableGroupsBaseStudySpecAndSem(long studySpecId, long semId)
	{
		var findAllStudyGroups = await dbContext.StudyGroups
			.Include(s => s.Department)
			.Include(s => s.StudySpecialization)
			.Include(s => s.Semester)
			.Where(s => s.StudySpecialization.Id == studySpecId && s.Semester.Id == semId)
			.ToListAsync();

		return findAllStudyGroups
			.Select(mapper.Map<NameIdElementDto>)
			.ToList();
	}

	public async Task<SearchQueryResponseDto> GetGroupsBaseStudySpec(string? groupName, string? deptName,
		string? studySpecName)
	{
		groupName ??= string.Empty;
		deptName ??= string.Empty;
		studySpecName ??= string.Empty;

		var findAllMatchStudyGroups = await dbContext.StudyGroups
			.Include(s => s.Department)
			.Include(s => s.StudySpecialization)
			.Where(s =>
				string.Equals(
					s.StudySpecialization.Name + " (" + s.StudySpecialization.StudyType.Alias + " " +
					s.StudySpecialization.StudyDegree.Alias + ")", studySpecName, StringComparison.OrdinalIgnoreCase)
				&& s.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase)
				&& s.Name.Contains(groupName, StringComparison.OrdinalIgnoreCase))
			.Select(s => s.Name)
			.ToListAsync();

		findAllMatchStudyGroups.Sort();

		if (findAllMatchStudyGroups.Count > 0)
		{
			return new SearchQueryResponseDto(findAllMatchStudyGroups);
		}
		var findAllElements = await dbContext.StudyGroups
			.Include(s => s.Department)
			.Include(s => s.StudySpecialization)
			.Where(s =>
				s.StudySpecialization.Name.Equals(studySpecName, StringComparison.OrdinalIgnoreCase) &&
				s.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase))
			.Select(s => s.Name)
			.ToListAsync();

		findAllElements.Sort();

		return new SearchQueryResponseDto(findAllElements);
	}

	public async Task<List<NameIdElementDto>> GetAllStudyGroupsBaseDept(string deptName)
	{
		var studyGroupsBaseDept = await dbContext.StudyGroups
			.Include(g => g.Department)
			.Where(g => g.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase))
			.ToListAsync();

		return studyGroupsBaseDept
			.Select(mapper.Map<NameIdElementDto>)
			.ToList();
	}

	protected override async Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (!UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);
		}
		var message = "Nie usunięto żadnej grupy.";
		var toRemoved = dbContext.StudyGroups.Where(s => items.Ids.Any(id => id == s.Id));
		if (toRemoved.Any())
		{
			message = $"Pomyślnie usunięto wybrane grupy. Liczba usuniętych grup: {toRemoved.Count()}.";
		}
		dbContext.StudyGroups.RemoveRange(toRemoved);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} study groups", toRemoved.Count());
		return new MessageContentResDto
		{
			Message = message
		};
	}

	protected override async Task<MessageContentResDto> OnDeleteAll(UserCredentialsHeaderDto userCredentialsHeaderDto)
	{
		if (!UserRole.IsAdministrator(userCredentialsHeaderDto.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);
		}
		var count = dbContext.StudyGroups.Count();

		dbContext.StudyGroups.RemoveRange();
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} study groups", count);
		return new MessageContentResDto
		{
			Message = "Pomyślnie usunięto wszystkie sale."
		};
	}
}
