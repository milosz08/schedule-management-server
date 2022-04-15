using System;
using System.Text;
using System.Security.Claims;
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
        
        // funkcja generująca JWT dla użytkownika na podstawie jego nazwy. Token jest ważny n-czasu, w
        // zależności od przechowywanej wartości w zmiennej TOKEN_EXPIRED_HOURS
        public string BearerHandlingService(Person person)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            byte[] tokenKey = Encoding.ASCII.GetBytes(GlobalConfigurer.JwtKey);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.Name, person.Login),
                    new Claim(ClaimTypes.Role, person.Role.Name)
                }),
                Expires = DateTime.UtcNow.AddMinutes(GlobalConfigurer.JwtExpiredMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            SecurityToken generatedToken = handler.CreateToken(descriptor);
            return handler.WriteToken(generatedToken);
        }
        
        
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
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    // ustawianie klucza symetrycznego używanego do walidacji
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalConfigurer.JwtKey)),
                    ValidateAudience = false, // nie waliduj audiencji (można zaimplementować ale po co xd)
                    ValidateIssuer = false,   // nie waliduj wystawcy (można zaimplementować ale po co xd)
                };
            });
        }
    }
}