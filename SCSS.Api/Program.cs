using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CustomerSupportTicketDbContext>(opt => opt.UseInMemoryDatabase("TicketsDB"));
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseCors("AllowAll");

app.MapPost("/api/tickets", async (CustomerSupportTicket t, CustomerSupportTicketDbContext db) =>
{
    t.CreatedAt = DateTime.UtcNow;
    db.Tickets.Add(t);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tickets/{t.Id}", t);
});

app.MapGet("/api/tickets", async (CustomerSupportTicketDbContext db) => await db.Tickets.ToListAsync());
app.MapGet("/api/tickets/{id:int}", async (int id, CustomerSupportTicketDbContext db) =>
    await db.Tickets.FindAsync(id) is CustomerSupportTicket t ? Results.Ok(t) : Results.NotFound());
app.MapPut("/api/tickets/{id:int}", async (int id, CustomerSupportTicket updated, CustomerSupportTicketDbContext db) =>
{
    var ticket = await db.Tickets.FindAsync(id);
    if (ticket is null) return Results.NotFound();
    ticket.Status = updated.Status;
    ticket.Resolution = updated.Resolution;
    await db.SaveChangesAsync();
    return Results.Ok(ticket);
});

app.Run();

record CustomerSupportTicket(int Id, string CustomerName, string Email, string Subject, string Description, string Status, string? Resolution, DateTime CreatedAt);
class CustomerSupportTicketDbContext : DbContext
{
    public CustomerSupportTicketDbContext(DbContextOptions<CustomerSupportTicketDbContext> options) : base(options) { }
    public DbSet<CustomerSupportTicket> Tickets => Set<CustomerSupportTicket>();
}
