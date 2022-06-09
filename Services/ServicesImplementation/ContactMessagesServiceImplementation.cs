using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Services.Helpers;
using asp_net_po_schedule_management_server.Ssh.SmtpEmailService;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class ContactMessagesServiceImplementation : IContactMessagesService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _helper;
        private readonly ApplicationDbContext _context;
        private readonly ISmtpEmailService _smtpEmailService;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public ContactMessagesServiceImplementation(ApplicationDbContext context, ISmtpEmailService smtpEmailService, 
            ServiceHelper helper, IMapper mapper)
        {
            _context = context;
            _smtpEmailService = smtpEmailService;
            _helper = helper;
            _mapper = mapper;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        #region Add new message

        /// <summary>
        /// Metoda umożliwiająca dodanie nowej wiadomości dla administratorów/edytorów systemu. Metoda na podstawie ciała
        /// zapytania dodaje wiadomość anonimową (niepowiązaną z osobą w systemie) lub wiadomość wysyłaną z konkretnego
        /// konta. Metoda również wysyła potwierdzenie na skrzynkę email (zarówno do użytkownika jak i do administratorów/
        /// moderatorów w zależności od typi wiadomości).
        /// </summary>
        /// <param name="dto">obiekt transferowy z danymi wiadomości</param>
        /// <param name="userIdentity">identyfikacja użytkownika na podstawie JWT</param>
        /// <returns>wiadomość odnosząca się do sukcesu wysłanej wiadomości</returns>
        /// <exception cref="BasicServerException">brak znalezionych elementów/nieautoryzowany dostęp</exception>
        public async Task<PseudoNoContentResponseDto> AddNewMessage(ContactMessagesReqDto dto, Claim userIdentity)
        {
            string emailAddress;
            string resMessage = "Zgłoszenie zostało pomyślnie wysłane.";
            Department findDepartment = null;
            List<StudyGroup> findStudyGroups = new List<StudyGroup>();
            Person findPerson = null;
            List<string> senderEmails = new List<string>();
            
            // wyszukaj typ zgłoszenia na podstawie nazwy, jeśli nie znajdzie rzuć wyjątek
            ContactFormIssueType findIssueType = await _context.ContactFormIssueTypes
                .FirstOrDefaultAsync(i => i.Name.Equals(dto.IssueType, StringComparison.OrdinalIgnoreCase));
            if (findIssueType == null) {
                throw new BasicServerException("Nie znaleziono typu zgłoszenia z podaną nazwą.", HttpStatusCode.NotFound);
            }

            // jeśli jest to zgłoszenie z uwzględnianiem wydziału i innych pokrewnych
            if (!dto.IssueType.Contains("inne", StringComparison.OrdinalIgnoreCase)) {
                // wyszukaj wydział na podstawie nazwy, jeśli nie znajdzie rzuć wyjątek
                findDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));
                if (findDepartment == null) {
                    throw new BasicServerException("Nie znaleziono wydziału z podaną nazwą.", HttpStatusCode.NotFound);
                }
                // wyszukaj wszystkie grupy dziekańskie na podstawie id, jeśli nie znajdzie zwróć pustą tablicę
                findStudyGroups = await _context.StudyGroups
                    .Include(g => g.Department)
                    .Where(g => g.Department.Id == findDepartment.Id && dto.Groups.Any(sg => sg == g.Id))
                    .ToListAsync();
                if (findStudyGroups.Count == 0) {
                    throw new BasicServerException("Należy wybrać przynajmniej jedną grupę.", HttpStatusCode.NotFound);
                }
                // dodaj adresy email wszystkich administratorów i moderatorów (z wybranego wydziału) systemu do wysyłki
                senderEmails.AddRange(await _context.Persons
                    .Include(p => p.Role)
                    .Include(p => p.Department)
                    .Where(p => p.Role.Name == AvailableRoles.ADMINISTRATOR ||
                                (p.Role.Name == AvailableRoles.EDITOR && p.Department.Id == findDepartment.Id))
                    .Select(p => p.Email)
                    .ToListAsync()
                );
            } else {
                // dodaj adresy email wszystkich administratorów systemu do wysyłki
                senderEmails.AddRange(await _context.Persons
                    .Include(p => p.Role)
                    .Where(p => p.Role.Name == AvailableRoles.ADMINISTRATOR)
                    .Select(p => p.Email)
                    .ToListAsync()
                );
            }
            
            // jeśli zgłoszenie jest robione bez zalogowania
            if (dto.IfAnonymous) {
                dto.Name = ApplicationUtils.CapitalisedLetter(dto.Name);
                dto.Surname = ApplicationUtils.CapitalisedLetter(dto.Surname);
                emailAddress = dto.Email;
            } else { // jeśli wiadomość wysyłana jest przy aktywnym zalogowaniu
                // wyszukaj osobę na podstawie claimów w JWT, jeśli nie znajdzie rzuć wyjątek
                findPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Login.Equals(userIdentity.Value));
                if (findPerson == null) {
                    throw new BasicServerException("Próba identyfikacji osoby zakończona niepowodzeniem.",
                        HttpStatusCode.NotFound);
                }
                emailAddress = findPerson.Email;
                // ustawienie wartości pustego stringa, jeśli nie podano parametru (nullable)
                dto.Name = null;
                dto.Surname = null;
                dto.Email = null;
                senderEmails.Add(findPerson.Email);
                resMessage += $" Kopia zgłoszenia została również wysłana na podany adres email: {emailAddress}.";
            }

            string stringifyGroups = string.Join(",", findStudyGroups.Select(g => g.Name));
            string generateMessageId = ApplicationUtils.RandomNumberGenerator(8);

            if (!dto.IfAnonymous) {
                // preparowanie oraz wysłanie wiadomości email z kopią wiadomości
                await _smtpEmailService.SendNewContactMessage(new UserEmailOptions()
                {
                    ToEmails = senderEmails.Distinct().ToList(), // usuwanie duplikatów
                    Placeholders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{messageId}}", generateMessageId),
                        new KeyValuePair<string, string>("{{userName}}", findPerson == null ? dto.Name : findPerson.Name),
                        new KeyValuePair<string, string>("{{issueType}}", findIssueType.Name),
                        new KeyValuePair<string, string>("{{departmentName}}", findDepartment == null ? "brak" : findDepartment.Name),
                        new KeyValuePair<string, string>("{{groupNames}}", stringifyGroups == "" ? "brak" : stringifyGroups),
                        new KeyValuePair<string, string>("{{description}}", dto.Description),
                        new KeyValuePair<string, string>("{{serverTime}}", ApplicationUtils.GetCurrentUTCdateString()),
                    },
                }, generateMessageId);
            }

            ContactMessage contactMessage = new ContactMessage()
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                PersonId = findPerson == null ? null : findPerson.Id,
                DepartmentId = findDepartment == null ? null : findDepartment.Id,
                ContactFormIssueType = findIssueType,
                Description = dto.Description,
                IfAnonymous = dto.IfAnonymous,
                StudyGroups = findStudyGroups,
                MessageIdentifier = generateMessageId,
            };

            // dodawanie do bazy danych
            await _context.ContactMessages.AddAsync(contactMessage);
            await _context.SaveChangesAsync();
            
            return new PseudoNoContentResponseDto()
            {
                Message = resMessage,
            };
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all contact message issue types base type name
        
        /// <summary>
        /// Metoda odpowiedzialna za pobieranie wszystkich typów jakie może przyjąć wiadomość od użytkownika. Metoda
        /// również filtruje typy na podstawie ich nazwy i zwraca w postaci obiektu transferowego.
        /// </summary>
        /// <param name="issueTypeName">nazwa typu na podstawie której zachodzi filtrowanie</param>
        /// <returns>przefiltrowane wyniki w obiekcie transferowym</returns>
        public async Task<AvailableDataResponseDto<string>> GetAllContactMessageIssueTypes(string issueTypeName)
        {
            // jeśli nie podano parametru, przypisz wartość pustego stringa
            if (issueTypeName == null) {
                issueTypeName = String.Empty;
            }
            List<string> findAllIssueTypes = await _context.ContactFormIssueTypes
                .Where(i => i.Name.Contains(issueTypeName, StringComparison.OrdinalIgnoreCase) || 
                            issueTypeName == string.Empty)
                .Select(i => i.Name)
                .ToListAsync();
            return new AvailableDataResponseDto<string>(findAllIssueTypes);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all messages base user claims

        /// <summary>
        /// Metoda zwracająca oraz filtrująca (opakowując w obiekt paginacji rezultatów) wszystkie wiadomości od
        /// użytkowników. Dodatkowo występuje filtracja wiadomości na podstawie tokenu JWT (administrator otrzymuje
        /// wszystkie wiadomości, edytor tylko te, które mają przypisany wydział taki sam jak jego, a reszta użytkowników
        /// otrzymuje tylko wiadomości wysłane przez siebie).
        /// </summary>
        /// <param name="searchQuery">parametr wyszukiwania wiadomości</param>
        /// <param name="userRole">claim z rolą użytkownika</param>
        /// <param name="userLogin">claim z loginem użytkownika</param>
        /// <returns>dane opakowane w obiekt pseudo HATEOAS (paginacja rezultatów)</returns>
        /// <exception cref="BasicServerException">nieznalezienie zasobu/nieautoryzowany dostęp</exception>
        public async Task<PaginationResponseDto<ContactMessagesQueryResponseDto>> GetAllMessagesBaseClaims(
            SearchQueryRequestDto searchQuery, Claim userRole, Claim userLogin)
        {
            // wyszukaj użytkownika na podstawie wartości claims w JWT, jeśli nie znajdzie rzuć wyjątek    
            Person findPerson = await _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Department)
                .FirstOrDefaultAsync(p => p.Role.Name.Equals(userRole.Value, StringComparison.OrdinalIgnoreCase) &&
                                          p.Login.Equals(userLogin.Value));
            if (findPerson == null) {
                throw new BasicServerException("Nie znaleziono użytkownika na podstawie tokenu autoryzacji.",
                    HttpStatusCode.NotFound);
            }
            
            // wyszukiwanie wiadomości użytkowników przy pomocy parametru SearchPhrase
            IQueryable<ContactMessage> contactMessagesBaseQuery = _context.ContactMessages
                .Include(m => m.Person)
                .Include(m => m.Department)
                .Include(m => m.ContactFormIssueType)
                .Where(m => (searchQuery.SearchPhrase == null ||
                            m.Person.Surname.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase)) &&
                            ((m.Department.Id == findPerson.Department.Id && findPerson.Role.Name == AvailableRoles.EDITOR) ||
                            (m.Person.Login.Equals(userLogin.Value) && (findPerson.Role.Name == AvailableRoles.TEACHER || 
                            findPerson.Role.Name == AvailableRoles.STUDENT)) || 
                            findPerson.Role.Name == AvailableRoles.ADMINISTRATOR));
            
            // sortowanie (rosnąco/malejąco) dla kolumn
            if (!string.IsNullOrEmpty(searchQuery.SortBy)) {
                _helper.PaginationSorting(new Dictionary<string, Expression<Func<ContactMessage, object>>>
                {
                    { nameof(ContactMessage.Id), d => d.Id },
                    { "Surname", d => d.Person == null ? d.Surname : d.Person.Surname },
                    { "IssueType", d => d.ContactFormIssueType.Name },
                    { nameof(ContactMessage.CreatedDate), d => d.CreatedDate },
                    { nameof(ContactMessage.IfAnonymous), d => d.IfAnonymous },
                }, searchQuery, ref contactMessagesBaseQuery);
            }
            
            List<ContactMessagesQueryResponseDto> allmessages = _mapper.Map<List<ContactMessagesQueryResponseDto>>(_helper
                .PaginationAndAdditionalFiltering(contactMessagesBaseQuery, searchQuery));
            
            return new PaginationResponseDto<ContactMessagesQueryResponseDto>(
                allmessages, contactMessagesBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
            
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Get message based id and claims
        
        /// <summary>
        /// Metoda zwracająca szczegółowe dane wybranej wiadomości (na podstawie parametru ID przekazywanego w
        /// parametrach) zapytania. Metoda również weryfikuje użytkownika i na tej podstawie zezwala na wysłanie treści
        /// lub wyrzuca wyjątek z brakiem poświadczeń do wyświetlania zasobu.
        /// </summary>
        /// <param name="messId">id wiadomości</param>
        /// <param name="userRole">claim roli użytkownika</param>
        /// <param name="userLogin">claim loginu użytkownika</param>
        /// <returns>szczegółowe dane wiadomości opakowane w obiekt transferowy</returns>
        /// <exception cref="BasicServerException">brak zasobu/nieautoryzowany dostęp</exception>
        public async Task<SingleContactMessageResponseDto> GetContactMessageBaseId(long messId, Claim userRole,
            Claim userLogin)
        {
            Person findPerson = await FindPersonHelper(userLogin.Value);
            // wyszukaj wiadomość na podstawie id, jeśli nie znajdzie zwróć wyjątek
            ContactMessage findContactMessage = await _context.ContactMessages
                .Include(m => m.Person)
                .Include(m => m.Department)
                .Include(m => m.StudyGroups)
                .Include(m => m.ContactFormIssueType)
                .FirstOrDefaultAsync(m => m.Id == messId);
            if (findContactMessage == null) {
                throw new BasicServerException("Nie znaleziono wiadomości z podanym id", HttpStatusCode.NotFound);
            }

            // dla wiadomości anonimowych
            if (findContactMessage.IfAnonymous || findContactMessage.ContactFormIssueType.Name.Contains("inne")) {
                // sprawdź, czy nie zachodzi próba odczytania anonimowej lub wiadomości oznaczonej jako inna z konta
                // innego niż administrator
                if (userRole.Value != AvailableRoles.ADMINISTRATOR) {
                    throw new BasicServerException("Brak autoryzacji do pozyskania wiadomości", HttpStatusCode.Forbidden);
                }
            } else {
                // sprawdź czy nie zachodzi próba odczytania chronionego zasobu (jeśli wiadomość jest anominowa a użytkownik
                // nie ma konta administratora, jeśli id wydziału nie jest równie przypisanemu wydziałowi dla edytora)
                if ((findContactMessage.Department.Id != findPerson.Department.Id && userRole.Value == AvailableRoles.EDITOR) ||
                    (findContactMessage.Person.Login != findPerson.Login &&
                     (userRole.Value == AvailableRoles.TEACHER || userRole.Value == AvailableRoles.STUDENT))) {
                    throw new BasicServerException("Brak autoryzacji do pozyskania wiadomości", HttpStatusCode.Forbidden);
                }
            }

            // mapowanie obiektu na obiekt transferowy i dodanie dodatkowych parametrów
            SingleContactMessageResponseDto response = _mapper.Map<SingleContactMessageResponseDto>(findContactMessage);
            if (findContactMessage.Department != null) {
                response.DepartmentName = $"{findContactMessage.Department.Name} ({findContactMessage.Department.Alias})";
                response.Groups = findContactMessage.StudyGroups.Select(g => g.Name).ToList();
            }

            return response;
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete content

        /// <summary>
        /// Usuwanie masywne (na podstawie identyfikatorów z tablicy). Dodatkowo metoda sprawdza, czy użytkownik ma
        /// dostęp do usunięcia danej wiadomości (na podstawie JWT).
        /// </summary>
        /// <param name="dto">obiekt transferowy z id elementów do usunięcia</param>
        /// <param name="credentials">dane logownania użytkownika</param>
        /// <exception cref="BasicServerException">brak zasobu/nieautoryzowany dostęp</exception>
        public async Task DeleteMassiveContactMess(MassiveDeleteRequestDto dto, UserCredentialsHeaderDto credentials)
        {
            Person findPerson = await FindPersonHelper(credentials.Login);
            // wyszukaj listę wszystkich wiadomości do usunięcia, jeśli nie najdzie rzuć wyjątek
            List<ContactMessage> findAllRemovingMess = await _context.ContactMessages
                .Include(m => m.Person)
                .Include(m => m.Department)
                .Where(m => dto.ElementsIds.Any(id => m.Id == id) &&
                            ((m.Person.Login.Equals(credentials.Login) && (findPerson.Role.Name == AvailableRoles.STUDENT || 
                                                                          findPerson.Role.Name == AvailableRoles.TEACHER)) ||
                            (m.Department.Id == findPerson.Department.Id && findPerson.Role.Name == AvailableRoles.EDITOR) ||
                            findPerson.Role.Name == AvailableRoles.ADMINISTRATOR))
                .ToListAsync();
            
            if (findAllRemovingMess.Count > 0) {
                _context.ContactMessages.RemoveRange(findAllRemovingMess);
                await _context.SaveChangesAsync();
            }
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Delete all

        /// <summary>
        /// Usuwanie wszystkich wiadomości (na podstawie roli: administrator usuwa wszystkie wiadomości w systemie,
        /// edytor usuwa wszystkie wiadomości z przypisanym do niego wydziałem, normalni użytkownicy usuwają wszystkie
        /// tylko swoje wiadomości).
        /// </summary>
        /// <param name="credentials">dane logownania użytkownika</param>
        /// <exception cref="BasicServerException">brak zasobu/nieautoryzowany dostęp</exception>
        public async Task DeleteAllContactMess(UserCredentialsHeaderDto credentials)
        {
            Person findPerson = await FindPersonHelper(credentials.Login);
            // wyszukaj listę wszystkich wiadomości do usunięcia, jeśli nie najdzie rzuć wyjątek
            List<ContactMessage> findAllRemovingMess = await _context.ContactMessages
                .Include(m => m.Person)
                .Include(m => m.Department)
                .Where(m => (m.Person.Login.Equals(credentials.Login) && ((findPerson.Role.Name == AvailableRoles.STUDENT || 
                                                                          findPerson.Role.Name == AvailableRoles.TEACHER)) ||
                            (m.Department.Id == findPerson.Department.Id && findPerson.Role.Name == AvailableRoles.EDITOR) ||
                            findPerson.Role.Name == AvailableRoles.ADMINISTRATOR))
                .ToListAsync();
            
            if (findAllRemovingMess.Count > 0) {
                _context.ContactMessages.RemoveRange(findAllRemovingMess);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Helper methods

        /// <summary>
        /// Metoda wyszukująca użytkownika w bazie danych na podstawie JWT. Jeśli nie znajdzie, wyrzuca wyjątek.
        /// </summary>
        /// <param name="login">login użytkownika z nagłówka zapytania</param>
        /// <returns>znaleziona encja użytkownika</returns>
        /// <exception cref="BasicServerException">nieznalezienie użytkownika</exception>
        private async Task<Person> FindPersonHelper(string login)
        {
            // wyszukaj osobę na podstawie JWT, jeśli nie znajdzie rzuć wyjątek
            Person findPerson = await _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Department)
                .FirstOrDefaultAsync(p => p.Login.Equals(login));
            if (findPerson == null) {
                throw new BasicServerException("Nie znaleziono użytkownika na podstawie JWT.", HttpStatusCode.NotFound);
            }
            return findPerson;
        }

        #endregion
    }
}