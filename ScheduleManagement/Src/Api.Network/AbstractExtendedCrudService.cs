using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network;

public abstract class AbstractExtendedCrudService(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher)
	: IBaseCrudService
{
	public async Task<MessageContentResDto> Delete2WayFactorSelected(DeleteSelectedRequestDto items,
		HttpContext context, HttpRequest request)
	{
		return await OnDeleteSelected(items, await ValidatePassword(context, request));
	}

	public async Task<MessageContentResDto> Delete2WayFactorAll(HttpContext context, HttpRequest request)
	{
		return await OnDeleteAll(await ValidatePassword(context, request));
	}

	private async Task<UserCredentialsHeaderDto> ValidatePassword(HttpContext context, HttpRequest request)
	{
		var userLogin = context.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
		var headers = request.Headers;
		var searchPerson = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Department)
			.FirstOrDefaultAsync(p => p.Login.Equals(userLogin == null ? string.Empty : userLogin.Value));
		if (searchPerson == null)
		{
			throw new RestApiException("Nie znaleziono użytkownika na podstawie tokenu.", HttpStatusCode.NotFound);
		}
		if (!headers.TryGetValue("User-Name", out var username) ||
		    !headers.TryGetValue("User-Password", out var password))
		{
			throw new RestApiException("Brak nagłówków autoryzacji.", HttpStatusCode.Forbidden);
		}
		var verificationResult =
			passwordHasher.VerifyHashedPassword(searchPerson, searchPerson.Password, password.First() ?? "");
		if (verificationResult == PasswordVerificationResult.Failed)
		{
			throw new RestApiException("Podano nieprawidłowe hasło. Spróbuj ponownie.", HttpStatusCode.Forbidden);
		}
		return new UserCredentialsHeaderDto
		{
			Login = userLogin?.Value ?? "",
			Username = username.First() ?? "",
			Password = password.First() ?? "",
			Person = searchPerson
		};
	}

	protected abstract Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader);

	protected abstract Task<MessageContentResDto> OnDeleteAll(UserCredentialsHeaderDto userCredentialsHeader);
}
