using System;

using System.Net;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;


namespace asp_net_po_schedule_management_server.Services.Helpers
{
    /// <summary>
    /// Klasa serwisu przechowująca pomocnicze metody używane w innnych serwisach aplikacji. Klasa jest dodana do
    /// zakresu całej aplikacji, przez co możliwe jest wskrzykiwanie jej w konstruktorze.
    /// </summary>
    public class ServiceHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Person> _passwordHasher;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public ServiceHelper(ApplicationDbContext context, IPasswordHasher<Person> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Pagination and filtering results

        /// <summary>
        /// Metoda odpowiadająca za logikę paginacji rezultatów i pierwotnego zapytania. Metoda filtruje zapytania z
        /// zapytania pierwotnego zwracają ilość rezultatów na podstawie kwerendy zapytania.
        /// </summary>
        /// <param name="baseQuery">bazowy rezultat zapytania</param>
        /// <param name="query">parametry filtrowania i paginacji</param>
        /// <typeparam name="T">typ encji na której dokonywana jest paginacja</typeparam>
        /// <returns>przefitrowana generyczna lista wynikowa</returns>
        public List<T> PaginationAndAdditionalFiltering<T>(IQueryable<T> baseQuery, SearchQueryRequestDto query)
        {
            return baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();
        }
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda odpowiadająca za sortowanie rosnące/malejące wyników zapytania. Kolumna podlegająca sortowaniu
        /// wybierana jest na podstawie generycznej ekpresji typów (bazowana na parametrach zapytania i słowniku
        /// przechowującym kolumny podlegające sortowaniu).
        /// </summary>
        /// <param name="colSelect">wybrana kolumna</param>
        /// <param name="query">parametry zapytania</param>
        /// <param name="baseQuery">wynik przed filtracją (generyczny, przekazywana referencja)</param>
        /// <typeparam name="T">typ encji na której metoda dokonuje filtracji</typeparam>
        public void PaginationSorting<T>(Dictionary<string, Expression<Func<T, object>>> colSelect,
            SearchQueryRequestDto query, ref IQueryable<T> baseQuery)
        {
            Expression<Func<T, object>> selectColumn = colSelect[query.SortBy];
            baseQuery = query.SortDirection == SortDirection.ASC
                ? baseQuery.OrderBy(selectColumn)
                : baseQuery.OrderByDescending(selectColumn);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Extract credentials on delete content

        /// <summary>
        /// Metoda eksportująca nagłówki autoryzacji i wartość claim reprezentującą login użytkownika (zaszytą w
        /// JWT) i opakowująca wszystkie dane w obiekt.
        /// </summary>
        /// <returns>Obiekt z danymi autoryzacji: login, nazwa użytkownika i hasło</returns>
        /// <exception cref="BasicServerException">Brak nagłówków autoryzacji</exception>
        public UserCredentialsHeaderDto ExtractedUserCredentialsFromHeader(HttpContext context, HttpRequest request)
        {
            Claim userLogin = context.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            IHeaderDictionary headers = request.Headers;
            if (headers.TryGetValue("User-Name", out var username) 
                && headers.TryGetValue("User-Password", out var password)) {
                return new UserCredentialsHeaderDto()
                {
                    Login = userLogin?.Value,
                    Username = username.First(),
                    Password = password.First(),
                };
            }
            throw new BasicServerException("Brak nagłówków autoryzacji.", HttpStatusCode.Forbidden);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Checking credentials on massive content deleting
        
        /// <summary>
        /// Metoda walidująca użytkownika na podstawie wprowadzonego hasła oraz loginu pozyskanego z tokenu JWT
        /// oraz podanego przez użytkownika.
        /// </summary>
        /// <param name="credentials">obiekt przechowujący wartości autoryzacji</param>
        /// <returns>Zwróci true, jeśli autoryzacja przebiegła prawidłowo.</returns>
        /// <exception cref="BasicServerException">W przypadku błędu serwera wyrzuci wyjątek</exception>
        public async Task CheckIfUserCredentialsAreValid(UserCredentialsHeaderDto credentials)
        {
            Person findPerson = await _context.Persons
                .FirstOrDefaultAsync(p => p.Login == credentials.Login && p.Login == credentials.Username);

            // jeśli użytkownik nie istnieje w systemie
            if (findPerson == null) {
                throw new BasicServerException("Użytkownik z podanym loginem/nazwą nie istnieje w systemie.", 
                    HttpStatusCode.Forbidden);
            }
            
            // weryfikacja hasła
            PasswordVerificationResult verificatrionRes = _passwordHasher
                .VerifyHashedPassword(findPerson, findPerson.Password, credentials.Password);
            if (verificatrionRes == PasswordVerificationResult.Failed) {
                throw new BasicServerException("Nieprawidłowe hasło. Spróbuj ponownie.", HttpStatusCode.Unauthorized);
            }
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Generate values for registered user

        /// <summary>
        /// Metoda generująca podstawowe dane zarejestrowanego użytkownika na podstawie podstawowych danych
        /// wprowadzonych przez administratora systemu.
        /// </summary>
        /// <param name="user">obiekt transferowy z podstawowymi danymi</param>
        /// <returns>obiekt z wygenerowanymi danymi użytkownika</returns>
        public RegisterUserGeneratedValues GenerateUserDetails(RegisterNewUserRequestDto user)
        {
            string nameWithoutDiacritics = ApplicationUtils.RemoveAccents(user.Name);
            string surnameWithoutDiacritics = ApplicationUtils.RemoveAccents(user.Surname);
            string randomNumbers = ApplicationUtils.RandomNumberGenerator();
           
            string shortcut = nameWithoutDiacritics.Substring(0, 3) + surnameWithoutDiacritics.Substring(0, 3);

            return new RegisterUserGeneratedValues()
            {
                Shortcut = shortcut,
                Password = ApplicationUtils.GenerateUserFirstPassword(),
                Login = shortcut.ToLower() + randomNumbers,
                Email = $"{nameWithoutDiacritics.ToLower()}.{surnameWithoutDiacritics.ToLower()}" +
                        $"{randomNumbers}@{GlobalConfigurer.UserEmailDomain}",
                EmailPassword = ApplicationUtils.GenerateUserFirstPassword(),
            };
        }

        #endregion
    }
}