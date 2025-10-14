using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.S3;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ScheduleManagement.Api.Network.Profile;

public class ProfileServiceImpl(
	ApplicationDbContext dbContext,
	ILogger<ProfileServiceImpl> logger,
	IS3Service s3Service) : IProfileService
{
	private static readonly string[] AcceptableImageTypes = ["image/jpeg", "image/jpg", "image/png"];

	public async Task<ProfileResDto> CreateUserCustomAvatar(IFormFile image, ClaimsPrincipal claimsPrincipal)
	{
		var findPerson = await GetPersonFromDb(claimsPrincipal);
		if (image == null || image.Length == 0)
			throw new RestApiException("Dodawany obraz nie istnieje lub jest uszkodzony. Spróbuj ponownie.",
				HttpStatusCode.ExpectationFailed);

		if (Array.IndexOf(AcceptableImageTypes, image.ContentType) == -1)
			throw new RestApiException("Akceptowane rozszerzenia pliku to: png, jpg, jpeg.",
				HttpStatusCode.ExpectationFailed);

		var imageId = Guid.NewGuid().ToString();
		using (var loadedImage = await Image.LoadAsync(image.OpenReadStream()))
		{
			loadedImage.Mutate(x => x.Resize(200, 200));
			var encoder = new JpegEncoder { Quality = 100 };
			using (var outputStream = new MemoryStream())
			{
				await loadedImage.SaveAsJpegAsync(outputStream, encoder);
				outputStream.Seek(0, SeekOrigin.Begin);

				if (findPerson.ProfileImageUuid != null)
					await s3Service.DeleteFileFromBucket(S3Bucket.Profiles, $"{findPerson.ProfileImageUuid}.jpg");

				await s3Service.PutFileFromRequest(S3Bucket.Profiles, $"{imageId}.jpg", outputStream.ToArray());
			}
		}

		findPerson.ProfileImageUuid = imageId;
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully add profile image for user: {}", findPerson);
		return new ProfileResDto
		{
			Message = $"Zdjęcie profilowe użytkownika {findPerson.Name} {findPerson.Surname} zostało ustawione.",
			ResourceUrl = $"{ApiConfig.S3.Url}/{S3Bucket.Profiles}/{imageId}.jpg"
		};
	}

	public async Task<MessageContentResDto> RemoveUserCustomAvatar(ClaimsPrincipal claimsPrincipal)
	{
		var findPerson = await GetPersonFromDb(claimsPrincipal);
		if (findPerson.ProfileImageUuid == null)
			throw new RestApiException("Użytkownik nie posiada zdjęcia profilowego.", HttpStatusCode.NotFound);

		await s3Service.DeleteFileFromBucket(S3Bucket.Profiles, $"{findPerson.ProfileImageUuid}.jpg");

		findPerson.ProfileImageUuid = null;
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed profile image from user: {}", findPerson);
		return new MessageContentResDto
		{
			Message = $"Zdjęcie profilowe użytkownika {findPerson.Name} {findPerson.Surname} zostało usunięte."
		};
	}

	private async Task<Person> GetPersonFromDb(ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";
		if (userLogin == null) throw new RestApiException("Dostęp do zasobu zabroniony.", HttpStatusCode.Forbidden);

		var findPerson = await dbContext.Persons.FirstOrDefaultAsync(p => p.Login == userLogin);
		if (findPerson == null)
			throw new RestApiException("Podany użytkownik nie istenieje w systemie.", HttpStatusCode.NotFound);

		return findPerson;
	}
}
