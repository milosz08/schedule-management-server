using System.ComponentModel.DataAnnotations;

namespace ScheduleManagement.Api.Network.StudySpec;

public sealed class StudySpecRequestDto
{
    [Required(ErrorMessage = "Pole nazwy kierunku nie może być puste")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Pole aliasu nazwy kierunku nie może być puste")]
    public string Alias { get; set; }

    [Required(ErrorMessage = "Pole nazwy przypisanego wydziału do kierunku nie może być puste")]
    public string DepartmentName { get; set; }

    [Required(ErrorMessage = "Pole typu/typów kierunku do kierunku nie może być puste")]
    public List<long> StudyType { get; set; }

    [Required(ErrorMessage = "Pole stopnia/stopni studiów nie może być puste")]
    public List<long> StudyDegree { get; set; }
}

public sealed class StudySpecResponseDto
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string DepartmentFullName { get; set; }
    public string StudyTypeFullName { get; set; }
    public string StudyDegreeFullName { get; set; }
}

public sealed class StudySpecQueryResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string SpecTypeName { get; set; }
    public string SpecTypeAlias { get; set; }
    public string DepartmentName { get; set; }
    public string DepartmentAlias { get; set; }
    public string StudyDegree { get; set; }
    public string StudyDegreeAlias { get; set; }
}

public sealed class StudySpecializationEditResDto
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string DepartmentName { get; set; }
    public List<long> StudyType { get; set; }
    public List<long> StudyDegree { get; set; }
}
