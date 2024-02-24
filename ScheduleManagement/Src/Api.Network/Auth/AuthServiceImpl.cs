using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Email;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Jwt;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.S3;
using ScheduleManagement.Api.Util;

namespace ScheduleManagement.Api.Network.Auth;

public class AuthServiceImpl(
	IMapper mapper,
	ApplicationDbContext dbContext,
	IJwtAuthManager jwtAuthManager,
	IPasswordHasher<Person> passwordHasher,
	IMailboxProxyService mailboxProxyService,
	ILogger<AuthServiceImpl> logger,
	IMailSenderService mailSenderService) : IAuthService
{
	public async Task<LoginResponseDto> Login(LoginRequestDto user)
	{
		var findPerson = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Department)
			.FirstOrDefaultAsync(p => p.Login == user.Login || p.Email == user.Login);

		if (findPerson == null)
		{
			throw new RestApiException("Podano zły login lub hasło. Spróbuj ponownie.", HttpStatusCode.Unauthorized);
		}
		var verificatrionRes = passwordHasher
			.VerifyHashedPassword(findPerson, findPerson.Password, user.Password);

		if (verificatrionRes == PasswordVerificationResult.Failed)
		{
			throw new RestApiException("Podano zły login lub hasło. Spróbuj ponownie.", HttpStatusCode.Unauthorized);
		}
		string bearerRefreshToken;

		var findRefreshToken = await dbContext.Tokens.FirstOrDefaultAsync(t => t.PersonId == findPerson.Id);
		if (findRefreshToken == null)
		{
			bearerRefreshToken = jwtAuthManager.RefreshTokenGenerator();
			var refreshToken = new RefreshToken
			{
				Token = bearerRefreshToken,
				PersonId = findPerson.Id
			};
			await dbContext.Tokens.AddAsync(refreshToken);
			await dbContext.SaveChangesAsync();
		}
		else
		{
			bearerRefreshToken = findRefreshToken.Token;
		}
		var resDto = mapper.Map<LoginResponseDto>(findPerson);
		resDto.BearerToken = jwtAuthManager.BearerHandlingService(findPerson);
		resDto.RefreshBearerToken = bearerRefreshToken;
		resDto.ConnectedWithDepartment = findPerson.Department!.Name;

		if (findPerson.ProfileImageUuid != null)
		{
			resDto.ProfileImageUrl = $"{ApiConfig.S3.Url}/{S3Bucket.Profiles}/{findPerson.ProfileImageUuid}.jpg";
		}
		logger.LogInformation("Successfully logged user: {}", findPerson);
		return resDto;
	}

	public async Task<RefreshTokenResponseDto> RefreshToken(RefreshTokenRequestDto dto)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		ClaimsPrincipal principal;
		try
		{
			principal = tokenHandler.ValidateToken(
				dto.BearerToken,
				JwtAuthManagerImpl.GetBasicTokenValidationParameters(false),
				out var validatedToken
			);
			if (validatedToken is not JwtSecurityToken jwtToken || !jwtToken.Header.Alg
				    .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new RestApiException("Niepoprawny token.", HttpStatusCode.ExpectationFailed);
			}
		}
		catch (System.Exception)
		{
			throw new RestApiException("Nieoczekiwany błąd podczas odczytywania tokenu.",
				HttpStatusCode.ExpectationFailed);
		}
		var findRefreshToken = await dbContext.Tokens
			.Include(p => p.Person)
			.FirstOrDefaultAsync(t => t.Token == dto.RefreshBearerToken && t.PersonId == t.Person.Id);
		if (findRefreshToken == null)
		{
			throw new RestApiException("Nie znaleziono tokenu odświeżającego.", HttpStatusCode.Forbidden);
		}
		logger.LogInformation("Successfully refresh token for user: {}", findRefreshToken.Person);
		return new RefreshTokenResponseDto
		{
			BearerToken = jwtAuthManager.BearerHandlingRefreshTokenService(principal.Claims.ToArray()),
			RefreshBearerToken = findRefreshToken.Token,
			TokenExpirationDate = DateTime.UtcNow.Add(ApiConfig.Jwt.ExpiredTimestamp),
			TokenRefreshInSeconds = ApiConfig.Jwt.ExpiredTimestamp.TotalSeconds
		};
	}

	public async Task<RegisterUpdateUserResponseDto> Register(RegisterUpdateUserRequestDto user, string customPassword)
	{
		var nameWithoutDiacritics = StringUtils.RemoveAccents(user.Name);
		var surnameWithoutDiacritics = StringUtils.RemoveAccents(user.Surname);
		var randomNumbers = RandomUtils.RandomNumberGenerator();

		var shortcut = string.Concat(nameWithoutDiacritics.AsSpan(0, 3), surnameWithoutDiacritics.AsSpan(0, 3));
		var defValues = new RegisterUserGeneratedValues
		{
			Shortcut = shortcut,
			Password = RandomUtils.GenerateUserFirstPassword(),
			Login = shortcut.ToLower() + randomNumbers,
			Email = $"{nameWithoutDiacritics.ToLower()}.{surnameWithoutDiacritics.ToLower()}" +
			        $"{randomNumbers}@{ApiConfig.Smtp.EmailDomain}",
			EmailPassword = RandomUtils.GenerateUserFirstPassword()
		};
		mailboxProxyService.AddNewEmailAccount(defValues.Email, defValues.EmailPassword);

		var findRoleId = await dbContext.Roles.FirstOrDefaultAsync(role => role.Name == user.Role);
		if (findRoleId == null)
		{
			throw new RestApiException("Podana rola nie istnieje w systemie.", HttpStatusCode.NotFound);
		}
		if (!customPassword.Equals(string.Empty))
		{
			defValues.Password = customPassword;
		}
		var findDepartment = await dbContext.Departments
			.FirstOrDefaultAsync(d => d.Name.Equals(user.DepartmentName, StringComparison.OrdinalIgnoreCase));

		var findCathedral = await dbContext.Cathedrals
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c =>
				c.Name.Equals(user.CathedralName, StringComparison.OrdinalIgnoreCase) &&
				c.Department.Name.Equals(findDepartment!.Name, StringComparison.OrdinalIgnoreCase));

		var newPerson = new Person
		{
			Name = StringUtils.CapitalisedLetter(user.Name),
			Surname = StringUtils.CapitalisedLetter(user.Surname),
			Nationality = StringUtils.CapitalisedLetter(user.Nationality),
			City = StringUtils.CapitalisedLetter(user.City),
			Shortcut = defValues.Shortcut,
			Email = defValues.Email,
			Login = defValues.Login,
			Password = defValues.Password,
			EmailPassword = defValues.EmailPassword,
			RoleId = findRoleId.Id,
			IfRemovable = user.IfRemovable,
			DepartmentId = findDepartment!.Id,
			CathedralId = findCathedral?.Id
		};
		if (!UserRole.Administrator.Equals(user.Role))
		{
			if (UserRole.Student.Equals(user.Role))
			{
				var findAllSpecializations = dbContext.StudySpecializations
					.Include(s => s.StudyType)
					.Where(s => user.StudySpecsOrSubjects.Any(id => id == s.Id))
					.AsEnumerable();
				newPerson.StudySpecializations = findAllSpecializations.ToList();
			}
			else
			{
				var findAllStudySubjects = dbContext.StudySubjects
					.Where(b => user.StudySpecsOrSubjects.Any(id => id == b.Id))
					.AsEnumerable();
				newPerson.Subjects = findAllStudySubjects.ToList();
			}
		}
		newPerson.Password = passwordHasher.HashPassword(newPerson, defValues.Password);
		await dbContext.Persons.AddAsync(newPerson);

		if (customPassword.Equals(string.Empty))
		{
			await mailSenderService.SendEmail(new UserEmailOptions<NewUserToUserViewModel>
			{
				ToEmails = [newPerson.Email],
				Subject = "Nowy użytkownik systemu",
				DataModel = new NewUserToUserViewModel
				{
					UserName = $"{newPerson.Name} {newPerson.Surname}",
					Login = newPerson.Login,
					Password = defValues.Password,
					UserRole = newPerson.Role.Name
				}
			}, LiquidTemplate.NewUserToUser);
		}
		await dbContext.SaveChangesAsync();
		var resDto = mapper.Map<RegisterUpdateUserResponseDto>(newPerson);

		logger.LogInformation("Successfully created account with details: {}", resDto);
		return resDto;
	}

	public Task<MessageContentResDto> Logout(string refreshToken, ClaimsPrincipal claimsPrincipal)
	{
		throw new NotImplementedException();
	}
}
