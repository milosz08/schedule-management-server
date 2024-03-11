using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.Helper;

public class HelperServiceImpl(ApplicationDbContext dbContext, IMapper mapper) : IHelperService
{
	public List<int> GetAvailablePaginationTypes()
	{
		return PaginationConfig.AllowedPageSizes.ToList();
	}

	public async Task<AvailableDataResponseDto<NameIdElementDto>> GetAvailableStudyTypes()
	{
		return new AvailableDataResponseDto<NameIdElementDto>
		{
			DataElements = await dbContext.StudyTypes.Select(t => mapper.Map<NameIdElementDto>(t)).ToListAsync()
		};
	}

	public async Task<AvailableDataResponseDto<NameIdElementDto>> GetAvailableStudyDegreeTypes()
	{
		return new AvailableDataResponseDto<NameIdElementDto>
		{
			DataElements = await dbContext.StudyDegrees.Select(d => mapper.Map<NameIdElementDto>(d)).ToListAsync()
		};
	}

	public async Task<AvailableDataResponseDto<NameIdElementDto>> GetAvailableSemesters()
	{
		return new AvailableDataResponseDto<NameIdElementDto>
		{
			DataElements = await dbContext.Semesters.Select(s => mapper.Map<NameIdElementDto>(s)).ToListAsync()
		};
	}

	public async Task<List<NameIdElementDto>> GetAvailableStudyDegreeBaseAllSpecs(long deptId)
	{
		var findAllDegreesBaseAllSpecs = await dbContext.StudySpecializations
			.Include(s => s.StudyDegree)
			.Include(s => s.Department)
			.Where(s => s.Department.Id == deptId)
			.Select(r => r.StudyDegree)
			.ToListAsync();

		var removeDuplicates = findAllDegreesBaseAllSpecs.Distinct().ToList();
		removeDuplicates
			.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase));

		return removeDuplicates
			.Select(mapper.Map<NameIdElementDto>)
			.ToList();
	}

	public async Task<List<NameIdElementDto>> GetAvailableSemBaseStudyGroups(long deptId, long studySpecId)
	{
		var findAllSemestersBaseSpec = await dbContext.StudyGroups
			.Include(g => g.StudySpecialization)
			.Include(g => g.Department)
			.Include(g => g.Semester)
			.Where(g => g.StudySpecialization.Id == studySpecId && g.Department.Id == deptId)
			.Select(g => g.Semester)
			.ToListAsync();

		var removeDuplicates = findAllSemestersBaseSpec.Distinct().ToList();
		removeDuplicates
			.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase));

		return removeDuplicates
			.Select(mapper.Map<NameIdElementDto>)
			.ToList();
	}

	public async Task<ConvertToTupleResponseDto> ConvertNamesToTuples(ConvertNamesToTuplesRequestDto dto)
	{
		var findStudyGroup = await dbContext.StudyGroups
			.Include(g => g.Department)
			.Include(g => g.StudySpecialization)
			.FirstOrDefaultAsync(g =>
				g.Name.Equals(dto.StudyGroupName, StringComparison.OrdinalIgnoreCase) &&
				g.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase) &&
				string.Equals(g.StudySpecialization.Name + " (" + g.StudySpecialization.StudyType.Alias + " " +
				              g.StudySpecialization.StudyDegree.Alias + ")",
					dto.StudySpecName, StringComparison.OrdinalIgnoreCase));

		if (findStudyGroup == null)
		{
			throw new RestApiException("Nie znaleziono grupy z podanymi parametrami.", HttpStatusCode.NotFound);
		}
		return new ConvertToTupleResponseDto(
			findStudyGroup.Department,
			findStudyGroup.StudySpecialization,
			findStudyGroup);
	}

	public async Task<ConvertToTupleResponseDto> ConvertIdsToTuples(ConvertIdsToTuplesRequestDto dto)
	{
		if (dto.StudySpecId == null || dto.StudyGroupId == null || dto.DepartmentId == null)
		{
			throw new RestApiException("Niepoprawne parametry planu.", HttpStatusCode.NotFound);
		}
		var findStudyGroup = await dbContext.StudyGroups
			.Include(g => g.Department)
			.Include(g => g.StudySpecialization)
			.FirstOrDefaultAsync(g => g.Id == dto.StudyGroupId && g.Department.Id == dto.DepartmentId &&
			                          g.StudySpecialization.Id == dto.StudySpecId);

		if (findStudyGroup == null)
		{
			throw new RestApiException("Nie znaleziono grupy z podanymi parametrami.", HttpStatusCode.NotFound);
		}
		return new ConvertToTupleResponseDto(
			findStudyGroup.Department,
			findStudyGroup.StudySpecialization,
			findStudyGroup);
	}

	public async Task<AvailableDataResponseDto<string>> GetAvailableSubjectTypes(string? subjTypeName)
	{
		subjTypeName ??= string.Empty;

		var findAllScheduleSubjectTypes = await dbContext.ScheduleSubjectTypes
			.Where(t => t.Name.Contains(subjTypeName, StringComparison.OrdinalIgnoreCase) ||
			            subjTypeName.Equals(string.Empty))
			.Select(t => t.Name)
			.ToListAsync();

		return new AvailableDataResponseDto<string>
		{
			DataElements = findAllScheduleSubjectTypes
		};
	}

	public async Task<AvailableDataResponseDto<string>> GetAvailableRoomTypes()
	{
		return new AvailableDataResponseDto<string>
		{
			DataElements = await dbContext.RoomTypes.Select(r => $"{r.Name} ({r.Alias})").ToListAsync()
		};
	}

	public async Task<AvailableDataResponseDto<string>> GetAvailableRoles()
	{
		return new AvailableDataResponseDto<string>
		{
			DataElements = await dbContext.Roles.Select(r => r.Name).ToListAsync()
		};
	}
}