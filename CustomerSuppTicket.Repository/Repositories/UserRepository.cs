using Microsoft.EntityFrameworkCore;
using CustomerSuppTicket.Entity.Models;
using CustomerSuppTicket.Repository.Data;
using CustomerSuppTicket.Common.Intefaces.Repositories;

namespace CustomerSuppTicket.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TicketsDbContext _db;

        public UserRepository(TicketsDbContext db)
        {
            _db = db;
        }

        public async Task<UserEntity> AddAsync(UserEntity user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            return await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
