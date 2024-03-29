﻿using ScheduleManagement.Api.Entity;

namespace ScheduleManagement.Api.Network.Helper;

public sealed class ConvertNamesToTuplesRequestDto
{
	public string DepartmentName { get; set; }
	public string StudySpecName { get; set; }
	public string StudyGroupName { get; set; }
}

public sealed class ConvertNamesToDataSingleElement(long id, string name)
{
	public long Id { get; set; } = id;
	public string Name { get; set; } = name;
}

public sealed class ConvertToTupleResponseDto(
	Entity.Department deptData,
	StudySpecialization specData,
	Entity.StudyGroup groupData)
{
	public ConvertNamesToDataSingleElement DeptData { get; set; } = new(deptData.Id, deptData.Name.ToLower());
	public ConvertNamesToDataSingleElement StudySpecData { get; set; } = new(specData.Id, specData.Name.ToLower());
	public ConvertNamesToDataSingleElement StudyGroupData { get; set; } = new(groupData.Id, groupData.Name.ToLower());
}
