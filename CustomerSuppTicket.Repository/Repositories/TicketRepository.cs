using Microsoft.EntityFrameworkCore;
using CustomerSuppTicket.Entity.Models;
using CustomerSuppTicket.Repository.Data;

namespace CustomerSuppTicket.Repository.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketsDbContext _db;
        public TicketRepository(TicketsDbContext db)
        {
            _db = db;
        }

        public async Task<Ticket> AddAsync(Ticket ticket)
        {
            if (ticket.Id == Guid.Empty) ticket.Id = Guid.NewGuid();
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.UpdatedAt = ticket.CreatedAt;
            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();
            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _db.Tickets.ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            return await _db.Tickets.FindAsync(id);
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
            _db.Tickets.Update(ticket);
            await _db.SaveChangesAsync();
        }
    }
}