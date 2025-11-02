using System.Text.Json;
using CustomerSuppTicket.Common.Intefaces.Repositories;
using CustomerSuppTicket.Entity.Models;

namespace CustomerSuppTicket.Repository.Repositories
{
    public class JsonUserRepository : IUserRepository
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };

        public JsonUserRepository(string filePath)
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

        private async Task<List<UserEntity>> ReadAllAsync()
        {
            await _lock.WaitAsync();
            try
            {
                using var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (stream.Length == 0) return new List<UserEntity>();
                return await JsonSerializer.DeserializeAsync<List<UserEntity>>(stream, _opts) ?? new List<UserEntity>();
            }
            finally { _lock.Release(); }
        }

        private async Task WriteAllAsync(List<UserEntity> users)
        {
            await _lock.WaitAsync();
            try
            {
                using var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await JsonSerializer.SerializeAsync(stream, users, _opts);
            }
            finally { _lock.Release(); }
        }

        public async Task<UserEntity> AddAsync(UserEntity user)
        {
            var list = await ReadAllAsync();
            list.Add(user);
            await WriteAllAsync(list);
            return user;
        }


        public async Task<UserEntity?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            var list = await ReadAllAsync();
            return list.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
