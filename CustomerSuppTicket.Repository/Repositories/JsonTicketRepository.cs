using System.Text.Json;
using CustomerSuppTicket.Entity.Models;

namespace CustomerSuppTicket.Repository.Repositories
{
    public class JsonTicketRepository : ITicketRepository
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };

        public JsonTicketRepository(string filePath)
        {
            _filePath = filePath;
            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
        }

        private async Task<List<Ticket>> ReadAllAsync()
        {
            await _lock.WaitAsync();
            try
            {
                using var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (stream.Length == 0) return new List<Ticket>();
                return await JsonSerializer.DeserializeAsync<List<Ticket>>(stream, _opts) ?? new List<Ticket>();
            }
            finally { _lock.Release(); }
        }

        private async Task WriteAllAsync(List<Ticket> tickets)
        {
            await _lock.WaitAsync();
            try
            {
                using var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await JsonSerializer.SerializeAsync(stream, tickets, _opts);
            }
            finally { _lock.Release(); }
        }

        public async Task<Ticket> AddAsync(Ticket ticket)
        {
            var list = await ReadAllAsync();
            if (ticket.Id == Guid.Empty) ticket.Id = Guid.NewGuid();
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.UpdatedAt = ticket.CreatedAt;
            list.Add(ticket);
            await WriteAllAsync(list);
            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            var list = await ReadAllAsync();
            return list;
        }

        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            var list = await ReadAllAsync();
            return list.FirstOrDefault(t => t.Id == id);
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            var list = await ReadAllAsync();
            var idx = list.FindIndex(t => t.Id == ticket.Id);
            if (idx == -1) return;
            ticket.UpdatedAt = DateTime.UtcNow;
            list[idx] = ticket;
            await WriteAllAsync(list);
        }
    }
}
