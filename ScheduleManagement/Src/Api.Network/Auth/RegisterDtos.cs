﻿using System.ComponentModel.DataAnnotations;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Network.User;

namespace ScheduleManagement.Api.Network.Auth;

public class RegisterUpdateUserRequestDto
{
	[Required(ErrorMessage = "Imię nie może być puste")]
	public string Name { get; set; }

	[Required(ErrorMessage = "Nazwisko nie może być puste")]
	public string Surname { get; set; }

	[Required(ErrorMessage = "Narowodowść nie może być pusta")]
	public string Nationality { get; set; }

	[Required] public string City { get; set; }

	[Required(ErrorMessage = "Rola użytkownika nie może być pusta")]
	[ValidValues(UserRole.Editor, UserRole.Student, UserRole.Teacher, UserRole.Administrator)]
	public string Role { get; set; }

	[Required(ErrorMessage = "Nazwa wydziału nie może być pusta")]
	public string DepartmentName { get; set; }

	public string CathedralName { get; set; } = "";

	[Required(ErrorMessage = "Dodatkowa zawartość przedmiotów/kierunków nie może być pusta")]
	public List<long> StudySpecsOrSubjects { get; set; }

	public bool IsRemovable { get; set; } = true;
}

public sealed class RegisterUpdateUserResponseDto
{
	public string Name { get; set; }
	public string Surname { get; set; }
	public string Nationality { get; set; }
	public string City { get; set; }
	public string Role { get; set; }
	public string Email { get; set; }
	public string EmailPassword { get; set; }
	public string DepartmentData { get; set; }
	public string CathedralData { get; set; }
}

public sealed class RegisterUserGeneratedValues
{
	public string Password { get; set; }
	public string Shortcut { get; set; }
	public string Login { get; set; }
	public string Email { get; set; }
	public string EmailPassword { get; set; }
}
