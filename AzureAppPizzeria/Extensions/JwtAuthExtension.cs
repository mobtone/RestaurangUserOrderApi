using AzureAppPizzeria.Data.Configurations;
using AzureAppPizzeria.Data.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace AzureAppPizzeria.Extensions
{
    public static class JwtAuthExtension
    {
        public static SymmetricSecurityKey SigningKey { get; private set; }
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();

            configuration.Bind("JwtSettings", jwtSettings); //bindning av Jwtsettings från appsettings.json

            if (string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException(
                    "Jwt key is not configured. Ensure it is set in Key Vault as 'JwtSettings--Key' (double hyphen for nesting) or in user secrets for local development, and that the application has access.");
            }
            //Gör så att en ny signeringsnyckel skapas varje gång appen startas
            SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));


            services.AddSingleton(jwtSettings); //detta objekt innehåller nyckeln från Key Vault

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false; //Sätts till true i produktion
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = SigningKey,
                        ClockSkew = TimeSpan.Zero
                    };

                    //här anpassas svaret för 401 Unauthorized (när autentisering misslyckas eller token saknas)
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            Console.WriteLine("OnChallenge triggered: User is not authenticated.");
                            context.HandleResponse();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var responsePayload = new
                                { message = "Du måste vara inloggad för att kunna använda denna funktion" };
                            return context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload));
                        },
                        OnForbidden = context => //om användaren är autentiserad men inte auktoriserad (ex. fel roll)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            var responsePayload = new { message = "Du har inte behörighet att utföra denna åtgärd" };
                            return context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload));
                        },
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Headers.ContainsKey("Authorization"))
                            {
                                var token = context.Request.Headers["Authorization"].ToString();
                                Console.WriteLine($"Token received: {token}");
                                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
                                {
                                    context.Token = token.Replace("Bearer ", "");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No Authorization header found.");
                            }

                            return Task.CompletedTask;
                        }

                        //det går också att använda OnAuthenticationFailed om jag vill logga eller anpassa specifika fel
                        // OnAuthenticationFailed = context => {
                        //     Console.WriteLine("Authentication failed: " + context.Exception.Message);
                        //     return Task.CompletedTask;
                        // }
                    };
                });
        }

        public static string GenerateJwtToken(ApplicationUser user, IList<string> roles, JwtSettings jwtSettings)
        {
            if (string.IsNullOrEmpty(jwtSettings.Key)) 
            {
                throw new InvalidOperationException("JWT Key is missing in JwtSettings when trying to generate token.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(jwtSettings.DurationInHours);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}