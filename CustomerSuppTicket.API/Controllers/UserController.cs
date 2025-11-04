namespace CustomerSuppTicket.API.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using CustomerSuppTicket.Common.DTOs;
    using CustomerSuppTicket.Common.ViewModels;
    using CustomerSuppTicket.Common.Intefaces.Services;
    using Microsoft.IdentityModel.Tokens;

    public static class UserController
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            var keyString = app.Configuration.GetValue<string>("jwtAuthKey");
            var key = Encoding.ASCII.GetBytes(keyString);

            app.MapPost("/api/user/register", async (UserDto userDto, IUserService service) =>
            {
                var created = await service.CreateAsync(userDto);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, created.Username),
                }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
            });

            app.MapPost("/api/user/login", async (LoginViewModel login, IUserService service) =>
            {
                bool ok = await service.LoginAsync(new UserDto
                {
                    Username = login.Username,
                    Password = login.Password
                });

                if (!ok)
                    return Results.Unauthorized();

                var keyString2 = app.Configuration.GetValue<string>("jwtAuthKey");
                var key2 = Encoding.ASCII.GetBytes(keyString2);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, login.Username),
                }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key2),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
            });
        }
    }

}
