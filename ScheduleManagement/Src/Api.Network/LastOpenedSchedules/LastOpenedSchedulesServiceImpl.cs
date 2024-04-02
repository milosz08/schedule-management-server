using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.LastOpenedSchedules;

public class LastOpenedSchedulesServiceImpl(ApplicationDbContext dbContext) : ILastOpenedSchedulesService
{
	public async Task<List<LastOpenedScheduleData>> GetAllLastOpenedSchedules(ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";
		var filteredSchedules = await dbContext.LastOpenedSchedules
			.Include(l => l.Person)
			.Include(l => l.StudyGroup).ThenInclude(g => g.Department)
			.Include(l => l.StudyGroup).ThenInclude(g => g.StudySpecialization)
			.Where(l => l.Person.Login.Equals(userLogin))
			.ToListAsync();

		return filteredSchedules
			.Select(schedule => new LastOpenedScheduleData
			{
				Id = schedule.Id,
				Name = schedule.StudyGroup.Name,
				DeptId = schedule.StudyGroup.DepartmentId,
				GroupId = schedule.StudyGroupId,
				SpecId = schedule.StudyGroup.StudySpecializationId
			}).ToList();
	}

	public async Task<MessageContentResDto> DeleteSelectedLastOpenedSchedules(DeleteSelectedRequestDto requestDto,
		ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";

		var lastOpenedSchedulesToRemove = await dbContext.LastOpenedSchedules
			.Include(l => l.Person)
			.Where(l => requestDto.Ids.Any(id => id == l.Id) && l.Person.Login.Equals(userLogin))
			.ToListAsync();

		var count = lastOpenedSchedulesToRemove.Count;
		dbContext.LastOpenedSchedules.RemoveRange(lastOpenedSchedulesToRemove);
		await dbContext.SaveChangesAsync();

		return new MessageContentResDto
		{
			Message = $"Pomyślnie usunięto {count} zapisanych planów."
		};
	}

	public async Task<MessageContentResDto> DeleteAllLastOpenedSchedules(ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";

		var lastOpenedSchedulesToRemove = await dbContext.LastOpenedSchedules
			.Include(l => l.Person)
			.Where(l => l.Person.Login.Equals(userLogin))
			.ToListAsync();

		var count = lastOpenedSchedulesToRemove.Count;
		dbContext.LastOpenedSchedules.RemoveRange(lastOpenedSchedulesToRemove);
		await dbContext.SaveChangesAsync();

		return new MessageContentResDto
		{
			Message = $"Pomyślnie usunięto {count} zapisanych planów."
		};
	}

	public async Task<LastOpenedScheduleResponseDto> AppendLastOpenedSchedule(LastOpenedScheduleRequestDto requestDto,
		ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";
		var loggedPerson = await dbContext.Persons.FirstOrDefaultAsync(p => p.Login.Equals(userLogin));

		var lastOpenedSchedule = await dbContext.LastOpenedSchedules
			.Include(l => l.StudyGroup)
			.FirstOrDefaultAsync(l =>
				l.StudyGroupId == requestDto.GroupId && l.StudyGroup.DepartmentId == requestDto.DeptId &&
				l.StudyGroup.StudySpecializationId == requestDto.SpecId);

		if (lastOpenedSchedule == null && loggedPerson != null)
		{
			var studyGroup = await dbContext.StudyGroups.FirstOrDefaultAsync(g => g.DepartmentId == requestDto.DeptId &&
				g.StudySpecializationId == requestDto.SpecId && g.Id == requestDto.GroupId);

			if (studyGroup != null)
			{
				var lastOpenedScheduleEntity = new LastOpenedSchedule
				{
					StudyGroup = studyGroup,
					Person = loggedPerson
				};
				await dbContext.LastOpenedSchedules.AddAsync(lastOpenedScheduleEntity);
				await dbContext.SaveChangesAsync();
			}
		}
		return new LastOpenedScheduleResponseDto
		{
			DeptId = requestDto.DeptId.ToString(),
			SpecId = requestDto.SpecId.ToString(),
			GroupId = requestDto.GroupId.ToString()
		};
	}
}
