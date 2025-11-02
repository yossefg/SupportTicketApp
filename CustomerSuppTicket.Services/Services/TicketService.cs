using CustomerSuppTicket.Common.DTOs;
using CustomerSuppTicket.Common.Intefaces.Services;
using CustomerSuppTicket.Entity.Models;
using CustomerSuppTicket.Common.Intefaces.Repositoy;
using CustomerSuppTicket.Common.ViewModels;

namespace CustomerSuppTicket.Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly ISummarizerService _summarizerService;
        private readonly ITicketRepository _repo;
        private readonly IEmailSender _emailSender;
        public TicketService(ITicketRepository repo, IEmailSender emailSender , ISummarizerService summarizerService)

        {
            _summarizerService = summarizerService;
            _repo = repo;
            _emailSender = emailSender;
        }

        public async Task<IEnumerable<TicketDto>> GetAllAsync()
        {
            var tickets = await _repo.GetAllAsync();
            return tickets.Select(t => MapToDto(t));
        }

        public async Task<TicketDto?> GetByIdAsync(Guid id)
        {
            var t = await _repo.GetByIdAsync(id);
            return t is null ? null : MapToDto(t);
        }
        private async Task<string> getSummarize(string text) {
            return await _summarizerService.SummarizeFaultDescriptionAsync(text);
        }
        public async Task<TicketDto> CreateAsync(TicketDto dto)
        {
            var summarize  = await getSummarize(dto.Description ?? string.Empty);
            var entity = new TicketEntity
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Name = dto.Name ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                Summary = summarize,
                Description = dto.Description ?? string.Empty,
                ImageUrl = dto.ImageUrl,
                Status = dto.Status ?? "New",
                Resolution = dto.Resolution,
                CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt == default ? DateTime.UtcNow : dto.UpdatedAt
            };
            var added = await _repo.AddAsync(entity);

            // send email notification
            if (!string.IsNullOrWhiteSpace(added.Email))
            {
                var subject = $"Ticket received: {added.Summary}";
                var body = $"Hello {added.Name},\n\nYour ticket has been received. Ticket Id: {added.Id}\nSummary: {added.Summary}\nStatus: {added.Status}\n\nRegards,\nSupport Team";
                try
                {
                    await _emailSender.SendEmailAsync(dto.Name,entity.Id.ToString(),added.Email);
                }
                catch
                {
                }
            }

            return MapToDto(added);
        }

        public async Task<TicketDto?> UpdateAsync(Guid id, TicketDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return null;
            existing.Name = dto.Name ?? existing.Name;
            existing.Email = dto.Email ?? existing.Email;
            existing.Summary = dto.Summary ?? existing.Summary;
            existing.Description = dto.Description ?? existing.Description;
            existing.ImageUrl = dto.ImageUrl ?? existing.ImageUrl;
            existing.Status = dto.Status ?? existing.Status;
            existing.Resolution = dto.Resolution ?? existing.Resolution;
            await _repo.UpdateAsync(existing);
            return MapToDto(existing);
        }

        private TicketDto MapToDto(TicketEntity t) => new TicketDto
        {
            Id = t.Id,
            Name = t.Name,
            Email = t.Email,
            Summary = t.Summary,
            Description = t.Description,
            ImageUrl = t.ImageUrl,
            Status = t.Status,
            Resolution = t.Resolution,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        };

        public async Task<TicketDto?> UpdateStatusAsync(TicketUpdateStatusViewModel dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing is null) return null;
            existing.Status = dto.Status ?? existing.Status;
            existing.Resolution = dto.Resolution ?? existing.Resolution;
            existing.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(existing);
            return MapToDto(existing);
        }

        public async Task<List<TicketDto>> UpdateStatusBulkAsync(List<TicketUpdateStatusViewModel> list)
        {
            var tickets = (await _repo.GetAllAsync()).ToList();

            foreach (var updated in list)
            {
                var ticket = tickets.FirstOrDefault(t => t.Id == updated.Id);
                if (ticket == null) continue;

                ticket.Status = updated.Status;
                ticket.Resolution = updated.Resolution;
            }

            await _repo.UpdateBulkAsync(tickets);

            return tickets.Select(t => MapToDto(t)).ToList();
        }
    }
}