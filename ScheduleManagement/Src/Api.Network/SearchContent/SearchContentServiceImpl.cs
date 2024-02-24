using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Network.User;

namespace ScheduleManagement.Api.Network.SearchContent;

public class SearchContentServiceImpl(ApplicationDbContext dbContext) : ISearchContentService
{
	public async Task<List<SearchMassiveQueryResDto>> GetAllItemsFromServerQuery(SearchMassiveQueryReqDto query)
	{
		if (string.IsNullOrEmpty(query.SearchQuery))
		{
			return [];
		}
		var responseData = new List<SearchMassiveQueryResDto>();

		var findStudyGroups = await dbContext.StudyGroups
			.Include(g => g.Department)
			.Include(g => g.ScheduleSubjects)
			.Include(g => g.StudySpecialization)
			.Where(g =>
				(g.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
				 g.StudySpecialization.StudyDegree.Name.Contains(query.SearchQuery,
					 StringComparison.OrdinalIgnoreCase) ||
				 g.StudySpecialization.StudyType.Name.Contains(query.SearchQuery,
					 StringComparison.OrdinalIgnoreCase) ||
				 g.StudySpecialization.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase))
				&& query.IsGroupsActive)
			.ToListAsync();

		foreach (var studyGroup in findStudyGroups)
		{
			dynamic queryParams = new ExpandoObject();
			queryParams.deptId = studyGroup.Department.Id;
			queryParams.specId = studyGroup.StudySpecialization.Id;
			queryParams.groupId = studyGroup.Id;

			responseData.Add(new SearchMassiveQueryResDto
			{
				TypeName = "Grupa dziekańska",
				DepartmentName = $"{studyGroup.Department.Name} ({studyGroup.Department.Alias})",
				FullName =
					$"{studyGroup.Name}, {studyGroup.StudySpecialization.Name} ({studyGroup.StudySpecialization.Alias})",
				PathParam = "groups",
				PathQueryParams = queryParams
			});
		}
		var findStudyTeachers = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Cathedral)
			.Include(p => p.Department)
			.Where(p =>
				(p.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
				 p.Surname.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase))
				&& query.IsTeachersActive && !p.Role.Name.Equals(UserRole.Student))
			.ToListAsync();

		foreach (var person in findStudyTeachers)
		{
			dynamic queryParams = new ExpandoObject();
			queryParams.deptId = person.Department!.Id;
			queryParams.cathId = person.Cathedral!.Id;
			queryParams.employeerId = person.Id;

			responseData.Add(new SearchMassiveQueryResDto
			{
				TypeName = $"Użytkownik, {person.Role.Name}",
				DepartmentName = $"{person.Department.Name} ({person.Department.Alias})",
				FullName = $"{person.Surname} {person.Name} ({person.Cathedral.Name})",
				PathParam = "employeers",
				PathQueryParams = queryParams
			});
		}
		var findStudyRooms = await dbContext.StudyRooms
			.Include(p => p.RoomType)
			.Include(p => p.Cathedral)
			.Include(p => p.Department)
			.Where(p =>
				(p.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
				 p.Description.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
				 p.RoomType.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase))
				&& query.IsRoomsActive)
			.ToListAsync();

		foreach (var studyRoom in findStudyRooms)
		{
			dynamic queryParams = new ExpandoObject();
			queryParams.deptId = studyRoom.Department.Id;
			queryParams.cathId = studyRoom.Cathedral.Id;
			queryParams.roomId = studyRoom.Id;

			responseData.Add(new SearchMassiveQueryResDto
			{
				TypeName = "Sala zajęciowa",
				DepartmentName = $"{studyRoom.Department.Name} ({studyRoom.Department.Alias})",
				FullName = $"{studyRoom.Name}, {studyRoom.Description} ({studyRoom.Cathedral.Name})",
				PathParam = "rooms",
				PathQueryParams = queryParams
			});
		}
		return responseData;
	}
}
