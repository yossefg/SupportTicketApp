using System.Text.Json;
using CustomerSuppTicket.Common.Intefaces.Repositoy;
using CustomerSuppTicket.Entity.Models;

namespace CustomerSuppTicketEntity.Repository.Repositories
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

        private async Task<List<TicketEntity>> ReadAllAsync()
        {
            await _lock.WaitAsync();
            try
            {
                using var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (stream.Length == 0) return new List<TicketEntity>();
                return await JsonSerializer.DeserializeAsync<List<TicketEntity>>(stream, _opts) ?? new List<TicketEntity>();
            }
            finally { _lock.Release(); }
        }
            
        private async Task WriteAllAsync(List<TicketEntity> TicketEntitys)
        {
            await _lock.WaitAsync();
            try
            {
                using var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await JsonSerializer.SerializeAsync(stream, TicketEntitys, _opts);
            }
            finally { _lock.Release(); }
        }

        public async Task<TicketEntity> AddAsync(TicketEntity TicketEntity)
        {
            var list = await ReadAllAsync();
            if (TicketEntity.Id == Guid.Empty) TicketEntity.Id = Guid.NewGuid();
            TicketEntity.CreatedAt = DateTime.UtcNow;
            TicketEntity.UpdatedAt = TicketEntity.CreatedAt;
            list.Add(TicketEntity);
            await WriteAllAsync(list);
            return TicketEntity;
        }

        public async Task<IEnumerable<TicketEntity>> GetAllAsync()
        {
            var list = await ReadAllAsync();
            return list;
        }

        public async Task<TicketEntity?> GetByIdAsync(Guid id)
        {
            var list = await ReadAllAsync();
            return list.FirstOrDefault(t => t.Id == id);
        }

        public async Task UpdateAsync(TicketEntity TicketEntity)
        {
            var list = await ReadAllAsync();
            var idx = list.FindIndex(t => t.Id == TicketEntity.Id);
            if (idx == -1) return;
            TicketEntity.UpdatedAt = DateTime.UtcNow;
            list[idx] = TicketEntity;
            await WriteAllAsync(list);
        }

        public async Task UpdateBulkAsync(List<TicketEntity> TicketEntitys)
        {
            if (TicketEntitys == null || TicketEntitys.Count == 0)
                return;

            await WriteAllAsync(TicketEntitys);
        }
    }
}
