namespace CustomerSuppTicket.Repository.Repositories
{
 using CustomerSuppTicket.Entity.Models;
 public interface ITicketRepository
 {
 Task<IEnumerable<Ticket>> GetAllAsync();
 Task<Ticket?> GetByIdAsync(Guid id);
 Task<Ticket> AddAsync(Ticket ticket);
 Task UpdateAsync(Ticket ticket);
 }
}