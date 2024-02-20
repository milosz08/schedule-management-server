using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.S3;

namespace ScheduleManagement.Api.Network.Profile;

public class ProfileServiceImpl(
	ApplicationDbContext dbContext,
	ILogger<ProfileServiceImpl> logger,
	IS3Service s3Service) : IProfileService
{
	private static readonly string[] AcceptableImageTypes = ["image/jpeg"];

	public async Task<MessageContentResDto> CreateUserCustomAvatar(IFormFile image, ClaimsPrincipal claimsPrincipal)
	{
		var findPerson = await GetPersonFromDb(claimsPrincipal);
		if (image == null || image.Length == 0)
		{
			throw new RestApiException("Błąd podczas dodawania obrazu. Spróbuj ponownie.",
				HttpStatusCode.ExpectationFailed);
		}
		if (Array.IndexOf(AcceptableImageTypes, image.ContentType) == -1)
		{
			throw new RestApiException("Akceptowane rozszerzenia pliku to: .jpeg", HttpStatusCode.ExpectationFailed);
		}
		await s3Service.PutFileFromRequest(S3Bucket.Profiles, findPerson.Id.ToString(), image);

		findPerson.HasPicture = true;
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully add profile image for user: {}", findPerson);
		return new MessageContentResDto
		{
			Message = $"Zdjęcie profilowe użytkownika {findPerson.Name} {findPerson.Surname} zostało ustawione."
		};
	}

	public async Task<MessageContentResDto> RemoveUserCustomAvatar(ClaimsPrincipal claimsPrincipal)
	{
		var findPerson = await GetPersonFromDb(claimsPrincipal);

		await s3Service.DeleteFileFromBucket(S3Bucket.Profiles, findPerson.Id.ToString());

		findPerson.HasPicture = false;
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed profile image from user: {}", findPerson);
		return new MessageContentResDto
		{
			Message = $"Zdjęcie profilowe użytkownika {findPerson.Name} {findPerson.Surname} zostało usunięte."
		};
	}

	public async Task<(byte[], string)> GetUserCustomAvatar(ClaimsPrincipal claimsPrincipal)
	{
		var findPerson = await GetPersonFromDb(claimsPrincipal);
		return await s3Service.GetFileFromBucket(S3Bucket.Profiles, findPerson.Id.ToString());
	}

	private async Task<Person> GetPersonFromDb(ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";
		if (userLogin == null)
		{
			throw new RestApiException("Dostęp do zasobu zabroniony.", HttpStatusCode.Forbidden);
		}
		var findPerson = await dbContext.Persons.FirstOrDefaultAsync(p => p.Login == userLogin);
		if (findPerson == null)
		{
			throw new RestApiException("Podany użytkownik nie istenieje w systemie.", HttpStatusCode.NotFound);
		}
		return findPerson;
	}
}