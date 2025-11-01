using AutoMapper;
using CustomerSuppTicket.Common.DTOs;
using CustomerSuppTicket.Common.ViewModels;

namespace CustomerSuppTicket.Common.Mappings
{
 public class TicketProfile : Profile
 {
 public TicketProfile()
 {
 CreateMap<TicketViewModel, TicketDto>()
 .ForMember(d => d.Id, opt => opt.MapFrom((src, dest) => dest != null && dest.Id != Guid.Empty ? dest.Id : Guid.NewGuid()))
 .ForMember(d => d.Summary, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Description) ? string.Empty : src.Description))
 .ForMember(d => d.Status, opt => opt.MapFrom(src => "New"))
 .ForMember(d => d.Resolution, opt => opt.MapFrom(src => string.Empty))
 .ForMember(d => d.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
 .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
 }
 }
}
