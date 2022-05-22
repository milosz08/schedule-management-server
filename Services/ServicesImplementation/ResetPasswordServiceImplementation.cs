using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Ssh.SmtpEmailService;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class ResetPasswordServiceImplementation : IResetPasswordService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IJwtAuthenticationManager _manager;
        private readonly IPasswordHasher<Person> _passwordHasher;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public ResetPasswordServiceImplementation(ApplicationDbContext context, IJwtAuthenticationManager manager,
            ISmtpEmailService smtpEmailService, IPasswordHasher<Person> passwordHasher)
        {
            _context = context;
            _manager = manager;
            _passwordHasher = passwordHasher;
            _smtpEmailService = smtpEmailService;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Send Password Reset Token Via Email

        /// <summary>
        /// Metoda wysyłająca token resetujący hasło na podany adres email (w parametrach zapytania) oraz dodająca
        /// token do bazy danych (token ważny przez X minut).
        /// </summary>
        /// <param name="userEmail">adres email użytkownika</param>
        /// <returns>podstawowa wiadomość serwera</returns>
        /// <exception cref="BasicServerException">brak odnalezienia szukanego zasobu (użytkownika)</exception>
        public async Task<PseudoNoContentResponseDto> SendPasswordResetTokenViaEmail(string userEmail)
        {
            Person findUser = await _context.Persons.FirstOrDefaultAsync(person => person.Email == userEmail);
            // jeśli nie znajdzie użytkownika w bazie, rzuć wyjątek
            if (findUser == null) {
                throw new BasicServerException("Nie znaleziono użytkownika.", HttpStatusCode.NotFound);
            }
            
            string otpToken = ApplicationUtils.DictionaryHashGenerator(8);
            ResetPasswordOtp resetPasswordOpt = new ResetPasswordOtp()
            {
                Email = findUser.Email,
                Otp = otpToken,
                OtpExpired = DateTime.UtcNow.Add(GlobalConfigurer.OptExpired),
                PersonId = findUser.Id,
            };
            
            // konfiguracja wysyłanej wiadomości email i wysłanie jej do klienta
            await _smtpEmailService.SendResetPassword(new UserEmailOptions()
            {
                ToEmails = new List<string>() {findUser.Email},
                Placeholders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{userName}}", $"{findUser.Name} {findUser.Surname}"),
                    new KeyValuePair<string, string>("{{token}}", otpToken),
                    new KeyValuePair<string, string>("{{expiredInMinutes}}", GlobalConfigurer.OptExpired.Minutes.ToString()),
                    new KeyValuePair<string, string>("{{serverTime}}", ApplicationUtils.GetCurrentUTCdateString()),
                },
            });

            // dodaj token do bazy danych
            await _context.ResetPasswordOpts.AddAsync(resetPasswordOpt);

            await _context.SaveChangesAsync();
            return new PseudoNoContentResponseDto()
            {
                Message = $"Token resetujący został wysłany na adres email {findUser.Email}."
            };
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Reset Password Via Email Token

        /// <summary>
        /// Metoda walidująca wysłany token resetujący hasło na adres email (na podstawie parametru zapytania).
        /// </summary>
        /// <param name="emailToken">wartość tokenu otrzymanego w wiadomości email</param>
        /// <returns>bearer token i adres email z którego dokonano zmiany hasła</returns>
        /// <exception cref="BasicServerException">nieprawidłowy token/przedawniony token</exception>
        public async Task<SetNewPasswordViaEmailResponse> ResetPasswordViaEmailToken(string emailToken)
        {
            ResetPasswordOtp findResetOtp = await _context.ResetPasswordOpts
                .Include(p => p.Person)
                .Include(p => p.Person.Role)
                .FirstOrDefaultAsync(otp => otp.Otp == emailToken);
            
            // jeśli token nie istnieje rzuć wyjątek 403 forbidden
            if (findResetOtp == null) {
                throw new BasicServerException("Nieprawidłowy token.", HttpStatusCode.Forbidden);
            }
            // jeśli token uległ przedawnieniu, rzuć wyjątek 403 forbidden
            if (findResetOtp.OtpExpired < DateTime.UtcNow) {
                throw new BasicServerException("Token uległ przedawnieniu.", HttpStatusCode.Forbidden);
            }
            
            return new SetNewPasswordViaEmailResponse()
            {
                Email = findResetOtp.Email,
                BearerToken = _manager.BearerHandlingResetPasswordTokenService(findResetOtp.Person, emailToken),
            };
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region User Reset Password

        /// <summary>
        /// Metoda umożliwiająca zresetowanie hasła na podstawie parametrów wygenerowanego tokena JWT.
        /// </summary>
        /// <param name="dto">reprezentacja danych od klienta (hasło, token itp.)</param>
        /// <param name="resetToken">token służący do odświeżania JWT</param>
        /// <param name="userLogin">login użytkownika</param>
        /// <returns>prosta wiadomość serwera</returns>
        /// <exception cref="BasicServerException">brak poświadczeń/brak zasobu w bazie danych</exception>
        public async Task<PseudoNoContentResponseDto> UserResetPassword(
            SetResetPasswordRequestDto dto, Claim resetToken, Claim userLogin)
        {
            ResetPasswordOtp findResetOtp;
            try {
                // sprawdzenie, czy użytkownik istnieje
                findResetOtp = await _context.ResetPasswordOpts
                    .Include(p => p.Person)
                    .FirstOrDefaultAsync(otp => otp.Otp == resetToken.Value 
                                                && otp.Person.Login == userLogin.Value && !otp.IfUsed);
            
                // jeśli token nie istnieje rzuć wyjątek 403 forbidden
                if (findResetOtp == null) {
                    throw new BasicServerException("Nieprawidłowy token.", HttpStatusCode.Forbidden);
                }
                // jeśli token został już wykorzystany
                if (findResetOtp.IfUsed) {
                    throw new BasicServerException("Token został już wykorzystany.", HttpStatusCode.Forbidden);
                }
                // jeśli token uległ przedawnieniu, rzuć wyjątek 403 forbidden
                if (findResetOtp.OtpExpired < DateTime.UtcNow) {
                    throw new BasicServerException("Token uległ przedawnieniu.", HttpStatusCode.Forbidden);
                }
            }
            catch (Exception ex) {
                throw new BasicServerException("Wadliwy token dostępu.", HttpStatusCode.ExpectationFailed);
            }
            
            // modyfikuj hasło użytkownika (na postawie hasza słownikowego)
            Person findPerson = await _context.Persons
                .FirstOrDefaultAsync(person => person.DictionaryHash == findResetOtp.Person.DictionaryHash);
            if (findPerson == null) {
                throw new BasicServerException("Nie znaleziono użytkownika.", HttpStatusCode.NotFound);
            }
            
            // zapisz jako wykorzystane i zapisz nowe hasło
            findResetOtp.IfUsed = true;
            findPerson.Password = _passwordHasher.HashPassword(findPerson, dto.newPassword);
            _context.Persons.Update(findPerson);
            
            await _context.SaveChangesAsync();
            return new PseudoNoContentResponseDto()
            {
                Message = $"Nowe hasło dla użytkownika {findPerson.Name} {findPerson.Surname} zostało ustawione.",
            };
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region ChangePassword

        /// <summary>
        /// Metoda odpowiadająca za zmianę hasła przez użytkownika.
        /// </summary>
        /// <param name="dto">reprezentacja danych od klienta (hasło, token itp.)</param>
        /// <param name="userId">id użytkownika</param>
        /// <param name="userLogin">login użytkownika (pobierane z JWT claimów)</param>
        /// <returns>prosta wiadomość serwera</returns>
        /// <exception cref="BasicServerException">brak poświadczeń/brak zasobu w bazie danych</exception>
        public async Task<PseudoNoContentResponseDto> UserChangePassword(
            ChangePasswordRequestDto dto, string userId, Claim userLogin)
        {
            Person findPerson = await _context.Persons.FirstOrDefaultAsync(p => p.DictionaryHash == userId);
            if (findPerson == null) {
                throw new BasicServerException($"Nie znaleziono użytkownika w bazie danych.", HttpStatusCode.NotFound);
            }

            // jeśli login zapisany w tokenie JWT nie jest zgody ze znalezionym użytkownikiem
            if (findPerson.Login != userLogin.Value) {
                throw new BasicServerException("Brak poświadczeń do edycji zasobu.", HttpStatusCode.Forbidden);
            }

            // jeśli użytkownik podał takie same nowe hasło jak poprzednie
            if (dto.OldPassword == dto.NewPassword) {
                throw new BasicServerException(
                    "Nowe hasło nie może być takie same jak hasło poprzednie.", HttpStatusCode.Forbidden);
            }
            
            PasswordVerificationResult verificationPassword = _passwordHasher
                .VerifyHashedPassword(findPerson, findPerson.Password, dto.OldPassword);
            if (verificationPassword == PasswordVerificationResult.Failed) {
                throw new BasicServerException("Podano złe hasło pierwotne.", HttpStatusCode.Unauthorized);
            }
            
            findPerson.Password = _passwordHasher.HashPassword(findPerson, dto.NewPassword);
            findPerson.FirstAccess = false;
            _context.Persons.Update(findPerson);
            await _context.SaveChangesAsync();
            
            return new PseudoNoContentResponseDto()
            {
                Message = $"Hasło dla użytkownika {findPerson.Name} {findPerson.Surname} zostało pomyślnie zmienione."
            };
        }

        #endregion
    }
}