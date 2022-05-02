using System;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.Jwt
{
    public sealed class JwtAuthenticationManagerImplementation : IJwtAuthenticationManager
    {
        // sekretny klucz prywatny do JWT
        private static byte[] tokenKey = Encoding.ASCII.GetBytes(GlobalConfigurer.JwtKey);
        
        //--------------------------------------------------------------------------------------------------------------
        
        // funkcja generująca JWT dla użytkownika na podstawie jego nazwy. Token jest ważny n-czasu, w
        // zależności od przechowywanej wartości w zmiennej TOKEN_EXPIRED_HOURS
        public string BearerHandlingService(Person person)
        {
            return JwtDeploymentDescriptor(new[]
            {
                new Claim(ClaimTypes.Name, person.Login),
                new Claim(ClaimTypes.Role, person.Role.Name),
            }, GlobalConfigurer.JwtExpiredTimestamp);
        }

        //--------------------------------------------------------------------------------------------------------------
        
        // metoda odpowiadająca za generowanie tokenu JWT używanego do odzyskiwania hasła. Token ważny w
        // zależności od ważności token OPT.
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
        
        // metoda odpowiadająca za generowanie nowego tokenu JWT na podstawie tokenu odświeżania
        public string BearerHandlingRefreshTokenService(Claim[] claims)
        {
            return JwtDeploymentDescriptor(claims, GlobalConfigurer.JwtExpiredTimestamp);
        }

        //--------------------------------------------------------------------------------------------------------------

        // metoda odpowiadająca za generowanie tokenu używanego do odświeżania tokenu JWT
        public string RefreshTokenGenerator()
        {
            byte[] randomNumbers = new byte[32];
            RandomNumberGenerator.Create().GetBytes(randomNumbers);
            return Convert.ToBase64String(randomNumbers);
        }
        
        //--------------------------------------------------------------------------------------------------------------

        // metoda inicjalizująca deskpryptor wdrożenia dla JWT
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

        //--------------------------------------------------------------------------------------------------------------
        
        // metoda statyczna ustawiająca konfigurację autentykacji w aplikacji (używana w klasie Startup.cs)
        public static void ImplementsJwtOnStartup(IServiceCollection services)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.RequireHttpsMetadata = false; // na developmencie false, na produkcji true <- ważne!!
                options.SaveToken = true; // czy klucz ma być przechowywany
                options.TokenValidationParameters = GetBasicTokenValidationParameters();
            });
        }

        //--------------------------------------------------------------------------------------------------------------
        
        // metoda statyczna zwracająca obiekt konfiguracji parametrów walidacji tokenu JWT
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
    }
}