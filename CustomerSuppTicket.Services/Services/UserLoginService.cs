using Microsoft.AspNetCore.Identity;
using CustomerSuppTicket.Common.DTOs;
namespace CustomerSuppTicket.Services.Services
{

    public class UserLoginService
    {
        private readonly PasswordHasher<UserDto> _passwordHasher = new PasswordHasher<UserDto>();

        public string getHasedPassword(UserDto user, string password)
        {
           return _passwordHasher.HashPassword(user, password);
        }

        // Verify login credentials
        public bool Login(UserDto user, string password)
        {
            if (user == null) return false;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }
    }

}
