using Microsoft.EntityFrameworkCore;
using CustomerSuppTicket.Entity.Models;

namespace CustomerSuppTicket.Repository.Data
{
    public class TicketsDbContext : DbContext
    {
        public TicketsDbContext(DbContextOptions<TicketsDbContext> options) : base(options) { }
        public DbSet<Ticket> Tickets => Set<Ticket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Name).IsRequired();
                b.Property(t => t.Email).IsRequired();
                b.Property(t => t.Summary).IsRequired();
            });
        }
    }
}
