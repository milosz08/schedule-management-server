using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Email;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Util;

namespace ScheduleManagement.Api.Network.ResetPassword;

public class ResetPasswordServiceImpl(
	ApplicationDbContext dbContext,
	IMailSenderService mailSenderService,
	ILogger<ResetPasswordServiceImpl> logger,
	IPasswordHasher<Person> passwordHasher) : IResetPasswordService
{
	public async Task<MessageContentResDto> SendPasswordResetEmailToken(string userEmail)
	{
		var findUser = await dbContext.Persons.FirstOrDefaultAsync(person => person.Email == userEmail);
		if (findUser == null)
		{
			throw new RestApiException("Nie znaleziono użytkownika.", HttpStatusCode.NotFound);
		}
		var otpToken = RandomUtils.RandomStringGenerator(8);
		var resetPasswordOpt = new ResetPasswordOtp
		{
			Email = findUser.Email,
			Otp = otpToken,
			OtpExpired = DateTime.UtcNow.Add(ApiConfig.OtpExpiredTimestamp),
			PersonId = findUser.Id
		};
		await mailSenderService.SendEmail(new UserEmailOptions<ResetPasswordViewModel>
		{
			ToEmails = [findUser.Email],
			Subject = "Resetowanie hasła",
			DataModel = new ResetPasswordViewModel
			{
				UserName = $"{findUser.Name} {findUser.Surname}",
				ExpiredInMinutes = ApiConfig.OtpExpiredTimestamp.Minutes,
				Token = otpToken
			}
		}, LiquidTemplate.ResetPassword);

		await dbContext.ResetPasswordOpts.AddAsync(resetPasswordOpt);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully send password reset email token to: {}", findUser.Email);
		return new MessageContentResDto
		{
			Message = $"Token resetujący został wysłany na adres email {findUser.Email}."
		};
	}

	public async Task<SetNewPasswordViaEmailResponse> ValidateResetEmailToken(string token)
	{
		var findResetOtp = await dbContext.ResetPasswordOpts
			.Include(p => p.Person)
			.Include(p => p.Person.Role)
			.FirstOrDefaultAsync(otp => otp.Otp == token);

		if (findResetOtp == null)
		{
			throw new RestApiException("Nieprawidłowy token.", HttpStatusCode.Forbidden);
		}
		if (findResetOtp.OtpExpired < DateTime.UtcNow)
		{
			throw new RestApiException("Token uległ przedawnieniu.", HttpStatusCode.Forbidden);
		}
		logger.LogInformation("Successfully validated reset password email token for user: {}", findResetOtp.Person);
		return new SetNewPasswordViaEmailResponse
		{
			Email = findResetOtp.Email,
			Token = token
		};
	}

	public async Task<MessageContentResDto> ChangePasswordViaEmailToken(SetResetPasswordRequestDto dto,
		string token)
	{
		ResetPasswordOtp? findResetOtp;
		try
		{
			findResetOtp = await dbContext.ResetPasswordOpts
				.Include(p => p.Person)
				.FirstOrDefaultAsync(otp => otp.Otp == token && !otp.IfUsed);
			if (findResetOtp == null)
			{
				throw new RestApiException("Nieprawidłowy token.", HttpStatusCode.Forbidden);
			}
			if (findResetOtp.IfUsed)
			{
				throw new RestApiException("Token został już wykorzystany.", HttpStatusCode.Forbidden);
			}
			if (findResetOtp.OtpExpired < DateTime.UtcNow)
			{
				throw new RestApiException("Token uległ przedawnieniu.", HttpStatusCode.Forbidden);
			}
		}
		catch (System.Exception)
		{
			throw new RestApiException("Wadliwy token dostępu.", HttpStatusCode.ExpectationFailed);
		}
		var findPerson = await dbContext.Persons.FirstOrDefaultAsync(person => person.Id == findResetOtp.Person.Id);
		if (findPerson == null)
		{
			throw new RestApiException("Nie znaleziono użytkownika.", HttpStatusCode.NotFound);
		}
		findResetOtp.IfUsed = true;
		findPerson.FirstAccess = false;
		findPerson.Password = passwordHasher.HashPassword(findPerson, dto.NewPassword);

		dbContext.Persons.Update(findPerson);

		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully change password via email token for user: {}", findResetOtp.Person);
		return new MessageContentResDto
		{
			Message = $"Nowe hasło dla użytkownika {findPerson.Name} {findPerson.Surname} zostało ustawione."
		};
	}

	public async Task<MessageContentResDto> ChangePasswordViaAccount(ChangePasswordRequestDto dto,
		ClaimsPrincipal claimsPrincipal)
	{
		var username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";
		var findPerson = await dbContext.Persons.FirstOrDefaultAsync(p => p.Login.Equals(username));
		if (findPerson == null)
		{
			throw new RestApiException("Nie znaleziono użytkownika w bazie danych.", HttpStatusCode.NotFound);
		}
		if (dto.OldPassword.Equals(dto.NewPassword))
		{
			throw new RestApiException(
				"Nowe hasło nie może być takie same jak hasło poprzednie.", HttpStatusCode.Forbidden);
		}
		var verificationPassword =
			passwordHasher.VerifyHashedPassword(findPerson, findPerson.Password, dto.OldPassword);
		if (verificationPassword == PasswordVerificationResult.Failed)
		{
			throw new RestApiException("Podano złe hasło pierwotne.", HttpStatusCode.Unauthorized);
		}
		findPerson.Password = passwordHasher.HashPassword(findPerson, dto.NewPassword);
		findPerson.FirstAccess = false;

		dbContext.Persons.Update(findPerson);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully change password via account for user: {}", findPerson);
		return new MessageContentResDto
		{
			Message = $"Hasło dla użytkownika {findPerson.Name} {findPerson.Surname} zostało pomyślnie zmienione."
		};
	}
}
