using CustomerSuppTicket.Common.DTOs;
using CustomerSuppTicket.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerSuppTicket.Common.Intefaces.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAllAsync();
        Task<TicketDto?> GetByIdAsync(Guid id);
        Task<TicketDto> CreateAsync(TicketDto dto);
        Task<TicketDto?> UpdateAsync(Guid id, TicketDto dto);

        Task<TicketDto?> UpdateStatusAsync(TicketUpdateStatusViewModel dto);

        Task<List<TicketDto>> UpdateStatusBulkAsync(List<TicketUpdateStatusViewModel> list);

    }
}
