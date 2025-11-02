namespace CustomerSuppTicket.Common.Intefaces.Repositories
{
    using CustomerSuppTicket.Entity.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserRepository
    {
        Task<UserEntity> AddAsync(UserEntity user);
        Task<UserEntity?> GetByUsernameAsync(string username); 
    }
}
