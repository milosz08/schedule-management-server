using System;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.Jwt
{
    public sealed class JwtAuthenticationManagerImplementation : IJwtAuthenticationManager
    {
        // sekretny klucz prywatny do JWT
        private static byte[] tokenKey = Encoding.ASCII.GetBytes(GlobalConfigurer.JwtKey);
        
        //--------------------------------------------------------------------------------------------------------------

        #region JWT deployment descriptor

        /// <summary>
        /// Funkcja generująca JWT dla użytkownika na podstawie jego nazwy. Token jest ważny n-czasu, w
        /// zależności od przechowywanej wartości w zmiennej TOKEN_EXPIRED_HOURS.
        /// </summary>
        /// <param name="person">reprezentacja instancji osoby</param>
        /// <returns>bearer token w formie ciągu znaków</returns>
        public string BearerHandlingService(Person person)
        {
            return JwtDeploymentDescriptor(new[]
            {
                new Claim(ClaimTypes.Name, person.Login),
                new Claim(ClaimTypes.Role, person.Role.Name),
            }, GlobalConfigurer.JwtExpiredTimestamp);
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda odpowiadająca za generowanie tokenu JWT używanego do odzyskiwania hasła. Token ważny w
        /// zależności od ważności token OPT.
        /// </summary>
        /// <param name="person">reprezentacja instancji osoby</param>
        /// <param name="otpToken">token OTP (tzw. jednorazowe hasło) ważny przez X czasu</param>
        /// <returns>bearer token w formie ciągu znaków</returns>
        public string BearerHandlingResetPasswordTokenService(Person person, string otpToken)
        {
            return JwtDeploymentDescriptor(new[]
            {
                new Claim(ClaimTypes.Name, person.Login),
                new Claim(ClaimTypes.Role, person.Role.Name),
                new Claim(ClaimTypes.Rsa, otpToken),
            }, GlobalConfigurer.OptExpired);
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda odpowiadająca za generowanie nowego tokenu JWT na podstawie tokenu odświeżania.
        /// </summary>
        /// <param name="claims">claimy z tokenu JWT</param>
        /// <returns>deskryptor tokenu bearer</returns>
        public string BearerHandlingRefreshTokenService(Claim[] claims)
        {
            return JwtDeploymentDescriptor(claims, GlobalConfigurer.JwtExpiredTimestamp);
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda inicjalizująca deskpryptor wdrożenia dla JWT.
        /// </summary>
        /// <param name="claims">claimy tokena bearer</param>
        /// <param name="tokenExpired">czas przedawnienia tokenu</param>
        /// <returns></returns>
        private string JwtDeploymentDescriptor(Claim[] claims, TimeSpan tokenExpired)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(tokenExpired),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            return handler.WriteToken(handler.CreateToken(descriptor));
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Utils

        /// <summary>
        /// Metoda odpowiadająca za generowanie tokenu używanego do odświeżania tokenu JWT.
        /// </summary>
        /// <returns></returns>
        public string RefreshTokenGenerator()
        {
            byte[] randomNumbers = new byte[32];
            RandomNumberGenerator.Create().GetBytes(randomNumbers);
            return Convert.ToBase64String(randomNumbers);
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda statyczna zwracająca obiekt konfiguracji parametrów walidacji tokenu JWT
        /// </summary>
        /// <param name="ifValidateLifetime">flaga, czy funkcja ma walidować token pod względem przedawnienia</param>
        /// <returns>instancję parametrów tokena</returns>
        public static TokenValidationParameters GetBasicTokenValidationParameters(bool ifValidateLifetime = true)
        {
            return new TokenValidationParameters()
            {
                // ustawianie klucza symetrycznego używanego do walidacji
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = ifValidateLifetime,
            };
        }

        #endregion
    }
}