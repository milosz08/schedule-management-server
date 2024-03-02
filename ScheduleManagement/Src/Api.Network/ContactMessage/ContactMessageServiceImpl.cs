using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Email;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;
using ScheduleManagement.Api.Util;

namespace ScheduleManagement.Api.Network.ContactMessage;

public class ContactMessageServiceImpl(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher,
	ILogger<ContactMessageServiceImpl> logger,
	IMapper mapper,
	IMailSenderService mailSenderService)
	: AbstractExtendedCrudService(dbContext, passwordHasher), IContactMessageService
{
	public async Task<MessageContentResDto> CreateMessage(ContactMessagesReqDto dto, ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";

		var resMessage = "Zgłoszenie zostało pomyślnie wysłane.";
		Entity.Department? findDepartment = null;
		List<Entity.StudyGroup> findStudyGroups = [];
		Person? findPerson = null;
		List<string> senderEmails = [];

		var findIssueType = await dbContext.ContactFormIssueTypes
			.FirstOrDefaultAsync(i => i.Name.Equals(dto.IssueType, StringComparison.OrdinalIgnoreCase));

		if (findIssueType == null)
		{
			throw new RestApiException("Nie znaleziono typu zgłoszenia z podaną nazwą.", HttpStatusCode.NotFound);
		}
		if (!dto.IssueType.Contains("inne", StringComparison.OrdinalIgnoreCase))
		{
			findDepartment = await dbContext.Departments
				.FirstOrDefaultAsync(d => d.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));

			if (findDepartment == null)
			{
				throw new RestApiException("Nie znaleziono wydziału z podaną nazwą.", HttpStatusCode.NotFound);
			}
			findStudyGroups = await dbContext.StudyGroups
				.Include(g => g.Department)
				.Where(g => g.Department.Id == findDepartment.Id && dto.Groups.Any(sg => sg == g.Id))
				.ToListAsync();

			if (findStudyGroups.Count == 0)
			{
				throw new RestApiException("Należy wybrać przynajmniej jedną grupę.", HttpStatusCode.NotFound);
			}
			senderEmails.AddRange(await dbContext.Persons
				.Include(p => p.Role)
				.Include(p => p.Department)
				.Where(p => p.Role.Name.Equals(UserRole.Administrator) ||
				            (p.Role.Name.Equals(UserRole.Editor) && p.Department!.Id == findDepartment.Id))
				.Select(p => p.Email)
				.ToListAsync()
			);
		}
		else
		{
			senderEmails.AddRange(await dbContext.Persons
				.Include(p => p.Role)
				.Where(p => p.Role.Name.Equals(UserRole.Administrator))
				.Select(p => p.Email)
				.ToListAsync()
			);
		}
		if (dto.IsAnonymous)
		{
			dto.Name = StringUtils.CapitalisedLetter(dto.Name!);
			dto.Surname = StringUtils.CapitalisedLetter(dto.Surname!);
		}
		else
		{
			findPerson = await dbContext.Persons.FirstOrDefaultAsync(p => p.Login.Equals(userLogin));
			if (findPerson == null)
			{
				throw new RestApiException("Próba identyfikacji osoby zakończona niepowodzeniem.",
					HttpStatusCode.NotFound);
			}
			dto.Name = null;
			dto.Surname = null;
			dto.Email = null;
			senderEmails.Add(findPerson.Email);
			resMessage += $" Kopia zgłoszenia została również wysłana na podany adres email: {findPerson.Email}.";
		}
		var stringifyGroups = string.Join(",", findStudyGroups.Select(g => g.Name));
		var generateMessageId = RandomUtils.RandomNumberGenerator(8);
		await mailSenderService.SendEmail(new UserEmailOptions<ContactFormMessageCopyViewModel>
		{
			ToEmails = senderEmails.Distinct().ToList(),
			Subject = $"Nowe zgłoszenie {generateMessageId}",
			DataModel = new ContactFormMessageCopyViewModel
			{
				UserName = findPerson == null ? dto.Name! : findPerson.Name,
				MessageId = generateMessageId,
				IssueType = findIssueType.Name,
				DepartmentName = findDepartment == null ? "brak" : findDepartment.Name,
				GroupNames = stringifyGroups == "" ? "brak" : stringifyGroups,
				Description = dto.Description
			}
		}, LiquidTemplate.ContactFormMessageCopy);
		var contactMessage = new Entity.ContactMessage
		{
			AnonName = dto.Name,
			AnonSurname = dto.Surname,
			AnonEmail = dto.Email,
			PersonId = findPerson?.Id,
			DepartmentId = findDepartment?.Id,
			ContactFormIssueType = findIssueType,
			Description = dto.Description,
			IfAnonymous = dto.IsAnonymous,
			StudyGroups = findStudyGroups,
			MessageIdentifier = generateMessageId
		};
		await dbContext.ContactMessages.AddAsync(contactMessage);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully created new message: {}", contactMessage);
		return new MessageContentResDto
		{
			Message = resMessage
		};
	}

	public async Task<AvailableDataResponseDto<string>> GetAllContactMessageIssueTypes(string? issueTypeName)
	{
		issueTypeName ??= string.Empty;

		var findAllIssueTypes = await dbContext.ContactFormIssueTypes
			.Where(i => i.Name.Contains(issueTypeName, StringComparison.OrdinalIgnoreCase) ||
			            issueTypeName.Equals(string.Empty))
			.Select(i => i.Name)
			.ToListAsync();

		return new AvailableDataResponseDto<string>
		{
			DataElements = findAllIssueTypes
		};
	}

	public async Task<PaginationResponseDto<ContactMessagesQueryResponseDto>> GetAllMessagesBaseClaims(
		SearchQueryRequestDto searchQuery, ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";
		var userRole = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Role))?.Value ?? "";

		var findPerson = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Department)
			.FirstOrDefaultAsync(p =>
				p.Role.Name.Equals(userRole, StringComparison.OrdinalIgnoreCase) && p.Login.Equals(userLogin));

		if (findPerson == null)
		{
			throw new RestApiException("Nie znaleziono użytkownika na podstawie tokenu autoryzacji.",
				HttpStatusCode.NotFound);
		}
		var contactMessagesBaseQuery = dbContext.ContactMessages
			.Include(m => m.Person)
			.Include(m => m.Department)
			.Include(m => m.ContactFormIssueType)
			.Where(m =>
				(searchQuery.SearchPhrase == null || m.Person!.Surname
					.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase)) &&
				((m.Department!.Id == findPerson.Department!.Id && findPerson.Role.Name.Equals(UserRole.Editor)) ||
				 (m.Person!.Login.Equals(userLogin) &&
				  (findPerson.Role.Name.Equals(UserRole.Teacher) || findPerson.Role.Name.Equals(UserRole.Student))) ||
				 UserRole.IsAdministrator(findPerson)));

		if (!string.IsNullOrEmpty(searchQuery.SortBy))
		{
			PaginationConfig.ConfigureSorting(new Dictionary<string, Expression<Func<Entity.ContactMessage, object>>>
			{
				{ nameof(Entity.ContactMessage.Id), d => d.Id },
				{ "Surname", d => d.Person == null ? d.AnonSurname! : d.Person.Surname },
				{ "IssueType", d => d.ContactFormIssueType.Name },
				{ nameof(Entity.ContactMessage.CreatedDate), d => d.CreatedDate },
				{ nameof(Entity.ContactMessage.IfAnonymous), d => d.IfAnonymous }
			}, searchQuery, ref contactMessagesBaseQuery);
		}
		var allmessages = mapper.Map<List<ContactMessagesQueryResponseDto>>(PaginationConfig
			.ConfigureAdditionalFiltering(contactMessagesBaseQuery, searchQuery));

		return new PaginationResponseDto<ContactMessagesQueryResponseDto>(
			allmessages, contactMessagesBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
	}

	public async Task<SingleContactMessageResponseDto> GetContactMessageDetails(long messId,
		ClaimsPrincipal claimsPrincipal)
	{
		var userLogin = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name))?.Value ?? "";
		var userRole = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Role))?.Value ?? "";

		var findPerson = await FindPersonHelper(userLogin);
		var findContactMessage = await dbContext.ContactMessages
			.Include(m => m.Person)
			.Include(m => m.Department)
			.Include(m => m.StudyGroups)
			.Include(m => m.ContactFormIssueType)
			.FirstOrDefaultAsync(m => m.Id == messId);

		if (findContactMessage == null)
		{
			throw new RestApiException("Nie znaleziono wiadomości z podanym id", HttpStatusCode.NotFound);
		}
		if ((findContactMessage.IfAnonymous || findContactMessage.ContactFormIssueType.Name.Contains("inne")) &&
		    !userRole.Equals(UserRole.Administrator))
		{
			throw new RestApiException("Brak autoryzacji do pozyskania wiadomości.", HttpStatusCode.Forbidden);
		}
		if ((findContactMessage.Department!.Id != findPerson.Department!.Id && userRole.Equals(UserRole.Editor)) ||
		    (findContactMessage.Person!.Login != findPerson.Login &&
		     (userRole.Equals(UserRole.Teacher) || userRole.Equals(UserRole.Student))))
		{
			throw new RestApiException("Brak autoryzacji do pozyskania wiadomości.", HttpStatusCode.Forbidden);
		}
		var response = mapper.Map<SingleContactMessageResponseDto>(findContactMessage);
		if (findContactMessage.Department != null)
		{
			response.DepartmentName = $"{findContactMessage.Department.Name} ({findContactMessage.Department.Alias})";
			response.Groups = findContactMessage.StudyGroups.Select(g => g.Name).ToList();
		}
		return response;
	}

	protected override async Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader)
	{
		var findPerson = await FindPersonHelper(userCredentialsHeader.Login);
		var findAllRemovingMess = await dbContext.ContactMessages
			.Include(m => m.Person)
			.Include(m => m.Department)
			.Where(m =>
				items.ElementsIds.Any(id => m.Id == id) &&
				((m.Person!.Login.Equals(userCredentialsHeader.Login) &&
				  (findPerson.Role.Name.Equals(UserRole.Student) || findPerson.Role.Name.Equals(UserRole.Teacher))) ||
				 (m.Department!.Id == findPerson.Department!.Id && findPerson.Role.Name.Equals(UserRole.Editor))
				 || UserRole.IsAdministrator(findPerson)))
			.ToListAsync();

		var message = "Nie usunięto żadnych wiadomości.";
		if (findAllRemovingMess.Count != 0)
		{
			message = $"Pomyślnie usunięto wybrane wiadomości. " +
			          $"Liczba usuniętych wiadomości: {findAllRemovingMess.Count}.";
		}
		dbContext.ContactMessages.RemoveRange(findAllRemovingMess);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} messages", findAllRemovingMess.Count);
		return new MessageContentResDto
		{
			Message = message
		};
	}

	protected override async Task<MessageContentResDto> OnDeleteAll(UserCredentialsHeaderDto userCredentialsHeader)
	{
		var findPerson = await FindPersonHelper(userCredentialsHeader.Login);
		var findAllRemovingMess = await dbContext.ContactMessages
			.Include(m => m.Person)
			.Include(m => m.Department)
			.Where(m =>
				(m.Person!.Login.Equals(userCredentialsHeader.Login) &&
				 (findPerson.Role.Name.Equals(UserRole.Student) || findPerson.Role.Name.Equals(UserRole.Teacher))) ||
				(m.Department!.Id == findPerson.Department!.Id && findPerson.Role.Name.Equals(UserRole.Editor)) ||
				UserRole.IsAdministrator(findPerson))
			.ToListAsync();

		var count = findAllRemovingMess.Count;
		dbContext.ContactMessages.RemoveRange(findAllRemovingMess);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} messages", count);
		return new MessageContentResDto
		{
			Message = "Pomyślnie usunięto wszystkie wiadomości."
		};
	}

	private async Task<Person> FindPersonHelper(string login)
	{
		var findPerson = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Department)
			.FirstOrDefaultAsync(p => p.Login.Equals(login));
		if (findPerson == null)
		{
			throw new RestApiException("Nie znaleziono użytkownika.", HttpStatusCode.NotFound);
		}
		return findPerson;
	}
}
