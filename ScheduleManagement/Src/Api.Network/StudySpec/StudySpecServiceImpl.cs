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

namespace ScheduleManagement.Api.Network.StudySpec;

public class StudySpecServiceImpl(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher,
	ILogger<StudySpecServiceImpl> logger,
	IMapper mapper)
	: AbstractExtendedCrudService(dbContext, passwordHasher), IStudySpecService
{
	public async Task<IEnumerable<StudySpecResponseDto>> CreateStudySpecialization(StudySpecRequestDto dto)
	{
		var findDepartment = await dbContext.Departments
			.FirstOrDefaultAsync(d => d.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));
		if (findDepartment == null)
		{
			throw new RestApiException("Nie znaleziono wydziału z podaną nazwą", HttpStatusCode.NotFound);
		}
		var findAllStudyTypes = dbContext.StudyTypes
			.Where(t => dto.StudyType.Any(id => id == t.Id)).ToList();
		if (findAllStudyTypes.Count == 0)
		{
			throw new RestApiException("Nie znaleziono podanych id typów kierunków", HttpStatusCode.NotFound);
		}
		var findAllStudyDegrees = dbContext.StudyDegrees
			.Where(d => dto.StudyDegree.Any(id => id == d.Id)).ToList();
		if (findAllStudyDegrees.Count == 0)
		{
			throw new RestApiException("Nie znaleziono podanych id stopni studiów", HttpStatusCode.NotFound);
		}
		var findSpecialization = await dbContext.StudySpecializations
			.Include(s => s.Department)
			.Include(s => s.StudyType)
			.Include(s => s.StudyDegree)
			.FirstOrDefaultAsync(s =>
				(s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) ||
				 s.Alias.Equals(dto.Alias, StringComparison.OrdinalIgnoreCase)) &&
				s.Department.Name.Equals(dto.DepartmentName, StringComparison.Ordinal) &&
				dto.StudyType.Any(v => v == s.StudyType.Id) && dto.StudyDegree.Any(v => v == s.StudyDegree.Id));

		if (findSpecialization != null)
		{
			throw new RestApiException(
				"Podany kierunek studiów istnieje już w wybranej jednostce.", HttpStatusCode.ExpectationFailed);
		}
		var createdSpecializations = new List<StudySpecialization>();
		foreach (var studyType in findAllStudyTypes)
		{
			createdSpecializations.AddRange(findAllStudyDegrees.Select(studyDegree => new StudySpecialization
			{
				Name = dto.Name,
				Alias = dto.Alias,
				DepartmentId = findDepartment.Id,
				StudyTypeId = studyType.Id,
				StudyDegreeId = studyDegree.Id
			}));
		}
		await dbContext.StudySpecializations.AddRangeAsync(createdSpecializations);
		await dbContext.SaveChangesAsync();

		var resDto = createdSpecializations.Select(mapper.Map<StudySpecResponseDto>);

		logger.LogInformation("Successfully created new study specialization: {}", resDto);
		return resDto;
	}

	public async Task<List<StudySpecResponseDto>> UpdateStudySpecialization(StudySpecRequestDto dto, long specId)
	{
		var findStudySpec = await dbContext.StudySpecializations
			.Include(s => s.StudyType)
			.Include(s => s.Department)
			.Include(s => s.StudyDegree)
			.FirstOrDefaultAsync(s => s.Id == specId);

		if (findStudySpec == null)
		{
			throw new RestApiException("Nie znaleziono kierunku studiów z podanym id", HttpStatusCode.NotFound);
		}
		if (findStudySpec.Name.Equals(dto.Name) && findStudySpec.Alias.Equals(dto.Alias))
		{
			throw new RestApiException("Należy wprowadzić wartości różne od poprzednich.",
				HttpStatusCode.ExpectationFailed);
		}
		findStudySpec.Name = dto.Name;
		findStudySpec.Alias = dto.Alias;

		await dbContext.SaveChangesAsync();

		var resDto = mapper.Map<StudySpecResponseDto>(findStudySpec);

		logger.LogInformation("Successfully updated study specialization with id: {}: {}", specId, resDto);
		return [resDto];
	}

	public PaginationResponseDto<StudySpecQueryResponseDto> GetAllStudySpecializations(
		SearchQueryRequestDto searchQuery)
	{
		var studySpecsBaseQuery = dbContext.StudySpecializations
			.Include(s => s.Department)
			.Include(s => s.StudyType)
			.Include(s => s.StudyDegree)
			.Where(s => searchQuery.SearchPhrase == null ||
			            s.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(searchQuery.SortBy))
		{
			PaginationConfig.ConfigureSorting(new Dictionary<string, Expression<Func<StudySpecialization, object>>>
			{
				{ nameof(StudySpecialization.Id), s => s.Id },
				{ nameof(StudySpecialization.Name), s => s.Name },
				{ "DepartmentAlias", s => s.Department.Alias },
				{ "SpecTypeAlias", s => s.StudyType.Alias },
				{ "SpecDegree", s => s.StudyDegree.Alias }
			}, searchQuery, ref studySpecsBaseQuery);
		}
		var allDepts = mapper.Map<List<StudySpecQueryResponseDto>>(PaginationConfig
			.ConfigureAdditionalFiltering(studySpecsBaseQuery, searchQuery));

		return new PaginationResponseDto<StudySpecQueryResponseDto>(
			allDepts, studySpecsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
	}

	public async Task<List<NameIdElementDto>> GetAllStudySpecsScheduleBaseDept(long deptId, long degreeId)
	{
		var studySpecsBaseDept = await dbContext.StudySpecializations
			.Include(s => s.Department)
			.Include(s => s.StudyType)
			.Include(s => s.StudyDegree)
			.Where(s => s.Department.Id == deptId && s.StudyDegree.Id == degreeId)
			.ToListAsync();

		studySpecsBaseDept.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));

		return studySpecsBaseDept
			.Select(s => new NameIdElementDto(s.Id, $"{s.Name} ({s.StudyType.Alias})"))
			.ToList();
	}

	public async Task<AvailableDataResponseDto<NameIdElementDto>> GetAvailableStudySpecsBaseDept(string deptName)
	{
		var findAllStudySpecs = await dbContext.StudySpecializations
			.Include(s => s.Department)
			.Include(s => s.StudyType)
			.Include(s => s.StudyDegree)
			.Where(s => s.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase))
			.ToListAsync();

		return new AvailableDataResponseDto<NameIdElementDto>
		{
			DataElements = findAllStudySpecs
				.Select(s => new NameIdElementDto(s.Id, $"{s.Name} ({s.StudyType.Alias})"))
				.ToList()
		};
	}

	public async Task<StudySpecializationEditResDto> GetStudySpecializationDetails(long specId)
	{
		var findStudySpecialization = await dbContext.StudySpecializations
			.Include(s => s.Department)
			.Include(s => s.StudyType)
			.Include(s => s.StudyDegree)
			.FirstOrDefaultAsync(s => s.Id == specId);

		if (findStudySpecialization == null)
		{
			throw new RestApiException("Nie znaleziono wybranego kierunku.", HttpStatusCode.NotFound);
		}
		return mapper.Map<StudySpecializationEditResDto>(findStudySpecialization);
	}

	public async Task<SearchQueryResponseDto> GetAllStudySpecializationsInDepartment(string? specName, string? deptName)
	{
		deptName ??= string.Empty;
		specName ??= string.Empty;

		var findAllStudySpecializations = await dbContext.StudySpecializations
			.Include(s => s.Department)
			.Include(s => s.StudyType)
			.Include(s => s.StudyDegree)
			.Where(s => s.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase)
			            && s.Name.Contains(specName, StringComparison.OrdinalIgnoreCase))
			.Select(s => $"{s.Name} ({s.StudyType.Alias} {s.StudyDegree.Alias})")
			.ToListAsync();

		findAllStudySpecializations.Sort();

		if (findAllStudySpecializations.Count > 0)
		{
			return new SearchQueryResponseDto(findAllStudySpecializations);
		}
		var findAllElements = await dbContext.StudySpecializations
			.Include(s => s.Department)
			.Include(s => s.StudyType)
			.Include(s => s.StudyDegree)
			.Where(s => s.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase))
			.Select(s => $"{s.Name} ({s.StudyType.Alias} {s.StudyDegree.Alias}")
			.ToListAsync();

		findAllElements.Sort();
		return new SearchQueryResponseDto(findAllStudySpecializations);
	}

	protected override async Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader)
	{
		if (UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
				HttpStatusCode.Forbidden);
		}
		var message = "Nie usunięto żadnego kierunku.";
		var toRemoved = dbContext.StudySpecializations.Where(s => items.ElementsIds.Any(id => id == s.Id));
		if (toRemoved.Any())
		{
			message = $"Pomyślnie usunięto wybrane kierunki. Liczba usuniętych kierunków: {toRemoved.Count()}.";
		}
		dbContext.StudySpecializations.RemoveRange(toRemoved);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} study specializations", toRemoved.Count());
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
		var count = dbContext.StudySpecializations.Count();

		dbContext.StudySpecializations.RemoveRange();
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} study specializations", count);
		return new MessageContentResDto
		{
			Message = "Pomyślnie usunięto wszystkie specjalizacje."
		};
	}
}
