using CustomerSuppTicket.Common.DTOs;
using System.Threading.Tasks;

namespace CustomerSuppTicket.Common.Intefaces.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="dto">User data transfer object</param>
        /// <returns>The created user DTO</returns>
        Task<UserDto> CreateAsync(UserDto dto);

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="dto">User data transfer object</param>
        /// <returns>The created user DTO</returns>
        Task<UserDto?> getUserByUserName(string userName);

        Task<bool> LoginAsync(UserDto dto);
    }
}
