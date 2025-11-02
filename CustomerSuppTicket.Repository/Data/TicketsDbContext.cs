using Microsoft.EntityFrameworkCore;
using CustomerSuppTicket.Entity.Models;

namespace CustomerSuppTicket.Repository.Data
{
    public class TicketsDbContext : DbContext
    {
        public TicketsDbContext(DbContextOptions<TicketsDbContext> options) : base(options) { }
        public DbSet<TicketEntity> Tickets => Set<TicketEntity>();
        public DbSet<UserEntity> Users => Set<UserEntity>();    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketEntity>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Name).IsRequired();
                b.Property(t => t.Email).IsRequired();
                b.Property(t => t.Summary).IsRequired();
            });
            modelBuilder.Entity<UserEntity>(b =>
            {
                b.Property(u => u.Username).IsRequired();
                b.Property(u => u.Email).IsRequired();
                b.Property(u => u.Password).IsRequired();
            });
        }
    }
}
