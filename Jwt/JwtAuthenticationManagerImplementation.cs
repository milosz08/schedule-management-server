using System;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;


namespace asp_net_po_schedule_management_server.Jwt
{
    public sealed class JwtAuthenticationManagerImplementation : IJwtAuthenticationManager
    {
        private readonly string _token;

        public JwtAuthenticationManagerImplementation(string token)
        {
            _token = token;
        }

        // funkcja generująca JWT dla użytkownika na podstawie jego nazwy. Token jest ważny n-czasu, w
        // zależności od przechowywanej wartości w zmiennej TOKEN_EXPIRED_HOURS
        public string BearerHandlingService(string username)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            byte[] tokenKey = Encoding.ASCII.GetBytes(_token);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(GlobalConfigurer.TOKEN_EXPIRED_HOURS),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            SecurityToken generatedToken = handler.CreateToken(descriptor);
            return handler.WriteToken(generatedToken);
        }
        
        // metoda statyczna ustawiająca konfigurację autentykacji w aplikacji (używana w klasie Startup.cs)
        public static void ImplementsJwtOnStartup(IServiceCollection services, string jwtKey)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.RequireHttpsMetadata = true; // na developmencie false, na produkcji true <- ważne!!
                options.SaveToken = true; // czy klucz ma być przechowywany
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true, // czy walidacja ma się odbywać przy pomocy klucza
                    // ustawianie klucza symetrycznego używanego do walidacji
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}