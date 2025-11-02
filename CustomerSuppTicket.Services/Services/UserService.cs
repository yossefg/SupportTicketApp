using CustomerSuppTicket.Common.DTOs;
using CustomerSuppTicket.Common.Intefaces.Repositories;
using CustomerSuppTicket.Common.Intefaces.Services;
using CustomerSuppTicket.Entity.Models;
using Microsoft.AspNetCore.Identity;

namespace CustomerSuppTicket.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private UserLoginService userLoginService;
        public UserService(IUserRepository repo )
        {
            _repo = repo;
            userLoginService = new UserLoginService();
        }

        public async Task<UserDto> CreateAsync(UserDto dto)
        {
            string hashedPassword = userLoginService.getHasedPassword(dto, dto.Password);
            var entity = new UserEntity
            {
               Username = dto.Username,
               Password = hashedPassword,
               Email = dto.Email
            };
            var added = await _repo.AddAsync(entity);


            return MapToDto(added);
        }

        public async Task<UserDto?> getUserByUserName(string userName)
        {
            var user = await _repo.GetByUsernameAsync(userName);
            if (user == null) return null;
            return MapToDto(user);
        }

        public async Task<bool> LoginAsync(UserDto dto)
        {
            var user = await _repo.GetByUsernameAsync(dto.Username);
            if (user == null) return false;
            return userLoginService.Login(MapToDto(user), dto.Password);
        }

        private UserDto MapToDto(UserEntity t) => new UserDto
        {
            Username = t.Username,
            Password = t.Password,
            Email = t.Email
        };
    }
}
