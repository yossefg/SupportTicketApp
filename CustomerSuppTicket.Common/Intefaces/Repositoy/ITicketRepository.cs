namespace CustomerSuppTicket.Common.Intefaces.Repositoy
{
    using CustomerSuppTicket.Entity.Models;
    public interface ITicketRepository
    {
        Task<IEnumerable<TicketEntity>> GetAllAsync();
        Task<TicketEntity?> GetByIdAsync(Guid id);
        Task<TicketEntity> AddAsync(TicketEntity ticket);
        Task UpdateAsync(TicketEntity ticket);
        Task UpdateBulkAsync(List<TicketEntity> tickets);
    }
}