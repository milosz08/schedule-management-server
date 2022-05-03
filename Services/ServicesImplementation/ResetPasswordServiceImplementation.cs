using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Ssh.StmpEmailService;

using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public class ResetPasswordServiceImplementation : IResetPasswordService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStmpEmailService _stmpEmailService;
        private readonly IJwtAuthenticationManager _manager;
        private readonly IPasswordHasher<Person> _passwordHasher;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public ResetPasswordServiceImplementation(
            ApplicationDbContext context,
            IJwtAuthenticationManager manager,
            IStmpEmailService stmpEmailService,
            IPasswordHasher<Person> passwordHasher)
        {
            _context = context;
            _manager = manager;
            _passwordHasher = passwordHasher;
            _stmpEmailService = stmpEmailService;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Send Password Reset Token Via Email

        // metoda wysyłająca token resetujący hasło na podany adres email (w parametrach zapytania) oraz dodająca
        // token do bazy danych (token ważny przez X minut).
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
            await _stmpEmailService.SendResetPassword(new UserEmailOptions()
            {
                ToEmails = new List<string>() {findUser.Email},
                Placeholders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{userName}}", $"{findUser.Name} {findUser.Surname}"),
                    new KeyValuePair<string, string>("{{token}}", otpToken),
                },
            });
            
            ResetPasswordOtp findResetPasswordOtp = await _context.ResetPasswordOpts
                .AsNoTracking()
                .FirstOrDefaultAsync(otp => otp.Email == findUser.Email);
            
            // sprawdź, czy rekord istnieje, jeśli tak to zaktualizuj, jeśli nie to stwórz
            if (findResetPasswordOtp != null) {
                resetPasswordOpt.Id = findResetPasswordOtp.Id;
                _context.ResetPasswordOpts.Update(resetPasswordOpt);
            } else {
                await _context.ResetPasswordOpts.AddAsync(resetPasswordOpt);
            }

            await _context.SaveChangesAsync();
            return new PseudoNoContentResponseDto()
            {
                Message = $"Token resetujący został wysłany na adres email {findUser.Email}."
            };
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Reset Password Via Email Token

        // metoda walidująca wysłany token resetujący hasło na adres email (na podstawie parametru zapytania)
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

        // metoda umożliwiająca zresetowanie hasła na podstawie parametrów wygenerowanego tokena JWT
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

        // metoda odpowiadająca za zmianę hasła przez użytkownika
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