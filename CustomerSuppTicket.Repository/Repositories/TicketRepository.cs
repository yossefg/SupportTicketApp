using Microsoft.EntityFrameworkCore;
using CustomerSuppTicket.Entity.Models;
using CustomerSuppTicket.Repository.Data;
using CustomerSuppTicket.Common.Intefaces.Repositoy;

namespace CustomerSuppTicket.Repository.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketsDbContext _db;
        public TicketRepository(TicketsDbContext db)
        {
            _db = db;
        }

        public async Task<TicketEntity> AddAsync(TicketEntity ticket)
        {
            if (ticket.Id == Guid.Empty) ticket.Id = Guid.NewGuid();
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.UpdatedAt = ticket.CreatedAt;
            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();
            return ticket;
        }

        public async Task<IEnumerable<TicketEntity>> GetAllAsync()
        {
            return await _db.Tickets.ToListAsync();
        }

        public async Task<TicketEntity?> GetByIdAsync(Guid id)
        {
            return await _db.Tickets.FindAsync(id);
        }

        public async Task UpdateAsync(TicketEntity ticket)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
            _db.Tickets.Update(ticket);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateBulkAsync(List<TicketEntity> tickets)
        {
            if (tickets == null || tickets.Count == 0)
                return;

            foreach (var ticket in tickets)
            {
                ticket.UpdatedAt = DateTime.UtcNow;
                _db.Tickets.Update(ticket);
            }

            await _db.SaveChangesAsync();
        }
    }
}