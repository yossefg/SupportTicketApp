using CustomerSuppTicket.Common.DTOs;
using CustomerSuppTicket.Common.Intefaces.Repositories;
using CustomerSuppTicket.Common.Intefaces.Repositoy;
using CustomerSuppTicket.Common.Intefaces.Services;
using CustomerSuppTicket.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerSuppTicket.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)

        {
            _repo = repo;
        }

        public async Task<UserDto> CreateAsync(UserDto dto)
        {
            var entity = new UserEntity
            {
               Username = dto.Username,
               Password = dto.Password,
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

        private UserDto MapToDto(UserEntity t) => new UserDto
        {
            Username = t.Username,
            Password = t.Password,
            Email = t.Email
        };
    }
}
