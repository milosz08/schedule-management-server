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
	public async Task<LoginResponseDto> Login(LoginRequestDto reqDto)
	{
		var findPerson = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Department)
			.FirstOrDefaultAsync(p => p.Login == reqDto.Login || p.Email == reqDto.Login);

		if (findPerson == null)
		{
			throw new RestApiException("Podano zły login lub hasło. Spróbuj ponownie.", HttpStatusCode.Unauthorized);
		}
		var verificatrionRes = passwordHasher
			.VerifyHashedPassword(findPerson, findPerson.Password, reqDto.Password);

		if (verificatrionRes == PasswordVerificationResult.Failed)
		{
			throw new RestApiException("Podano zły login lub hasło. Spróbuj ponownie.", HttpStatusCode.Unauthorized);
		}
		var bearerRefreshToken = jwtAuthManager.RefreshTokenGenerator();
		var refreshToken = new RefreshToken
		{
			Token = bearerRefreshToken,
			PersonId = findPerson.Id
		};
		await dbContext.Tokens.AddAsync(refreshToken);
		await dbContext.SaveChangesAsync();

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

	public async Task<LoginResponseDto> TokenLogin(TokenLoginRequestDto reqDto)
	{
		var principal = ValidateTokens(reqDto.AccessToken);
		var userLogin = principal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";

		var findRefreshToken = await dbContext.Tokens
			.Include(p => p.Person).ThenInclude(person => person.Department)
			.Include(p => p.Person.Role)
			.FirstOrDefaultAsync(t => t.Token == reqDto.RefreshToken && t.Person.Login.Equals(userLogin));
		if (findRefreshToken == null)
		{
			throw new RestApiException("Nie odnaleziono aktywnej sesji.", HttpStatusCode.Forbidden);
		}
		var findPerson = findRefreshToken.Person;

		var resDto = mapper.Map<LoginResponseDto>(findPerson);
		resDto.BearerToken =
			jwtAuthManager.BearerHandlingRefreshTokenService(principal.Claims.Where(c => !c.Type.Equals("aud")));
		resDto.RefreshBearerToken = reqDto.RefreshToken;
		resDto.ConnectedWithDepartment = findPerson.Department!.Name;

		if (findPerson.ProfileImageUuid != null)
		{
			resDto.ProfileImageUrl = $"{ApiConfig.S3.Url}/{S3Bucket.Profiles}/{findPerson.ProfileImageUuid}.jpg";
		}
		logger.LogInformation("Successfully logged user via token: {}", findPerson);
		return resDto;
	}

	public async Task<RefreshTokenResponseDto> RefreshToken(RefreshTokenRequestDto reqDto)
	{
		var principal = ValidateTokens(reqDto.ExpiredAccessToken);
		var userLogin = principal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";

		var findRefreshToken = await dbContext.Tokens
			.Include(p => p.Person)
			.FirstOrDefaultAsync(t => t.Token == reqDto.RefreshToken && t.Person.Login.Equals(userLogin));
		if (findRefreshToken == null)
		{
			throw new RestApiException("Nie znaleziono tokenu odświeżającego.", HttpStatusCode.Forbidden);
		}
		var resDto = new RefreshTokenResponseDto
		{
			AccessToken = jwtAuthManager.BearerHandlingRefreshTokenService(principal.Claims.ToArray()),
			RefreshToken = findRefreshToken.Token
		};
		logger.LogInformation("Successfully refresh token for user: {}", findRefreshToken.Person);
		return resDto;
	}

	public async Task<RegisterUpdateUserResponseDto> Register(RegisterUpdateUserRequestDto reqDto,
		string customPassword)
	{
		var nameWithoutDiacritics = StringUtils.RemoveAccents(reqDto.Name);
		var surnameWithoutDiacritics = StringUtils.RemoveAccents(reqDto.Surname);
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

		var findRoleId = await dbContext.Roles.FirstOrDefaultAsync(role => role.Name == reqDto.Role);
		if (findRoleId == null)
		{
			throw new RestApiException("Podana rola nie istnieje w systemie.", HttpStatusCode.NotFound);
		}
		if (!customPassword.Equals(string.Empty))
		{
			defValues.Password = customPassword;
		}
		var findDepartment = await dbContext.Departments
			.FirstOrDefaultAsync(d => d.Name.Equals(reqDto.DepartmentName, StringComparison.OrdinalIgnoreCase));

		var findCathedral = await dbContext.Cathedrals
			.Include(c => c.Department)
			.FirstOrDefaultAsync(c =>
				c.Name.Equals(reqDto.CathedralName, StringComparison.OrdinalIgnoreCase) &&
				c.Department.Name.Equals(findDepartment!.Name, StringComparison.OrdinalIgnoreCase));

		var newPerson = new Person
		{
			Name = StringUtils.CapitalisedLetter(reqDto.Name),
			Surname = StringUtils.CapitalisedLetter(reqDto.Surname),
			Nationality = StringUtils.CapitalisedLetter(reqDto.Nationality),
			City = StringUtils.CapitalisedLetter(reqDto.City),
			Shortcut = defValues.Shortcut,
			Email = defValues.Email,
			Login = defValues.Login,
			Password = defValues.Password,
			EmailPassword = defValues.EmailPassword,
			RoleId = findRoleId.Id,
			IsRemovable = reqDto.IsRemovable,
			DepartmentId = findDepartment!.Id,
			CathedralId = findCathedral?.Id
		};
		if (!UserRole.Administrator.Equals(reqDto.Role))
		{
			if (UserRole.Student.Equals(reqDto.Role))
			{
				var findAllSpecializations = dbContext.StudySpecializations
					.Include(s => s.StudyType)
					.Where(s => reqDto.StudySpecsOrSubjects.Any(id => id == s.Id))
					.AsEnumerable();
				newPerson.StudySpecializations = findAllSpecializations.ToList();
			}
			else
			{
				var findAllStudySubjects = dbContext.StudySubjects
					.Where(b => reqDto.StudySpecsOrSubjects.Any(id => id == b.Id))
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

	public async Task<MessageContentResDto> Logout(string refreshToken, ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";

		var findRefreshToken = await dbContext.Tokens
			.Include(p => p.Person).ThenInclude(person => person.Department)
			.FirstOrDefaultAsync(t => t.Token == refreshToken && t.Person.Login.Equals(userLogin));
		if (findRefreshToken == null)
		{
			throw new RestApiException("Nie odnaleziono aktywnej sesji.", HttpStatusCode.Forbidden);
		}
		dbContext.Tokens.Remove(findRefreshToken);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully logout user: {}", findRefreshToken.Person);
		return new MessageContentResDto
		{
			Message = "Zostałeś pomyślnie wylogowany z aplikacji."
		};
	}

	private static ClaimsPrincipal ValidateTokens(string accessToken)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		ClaimsPrincipal principal;
		try
		{
			principal = tokenHandler.ValidateToken(
				accessToken,
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
		return principal;
	}
}
