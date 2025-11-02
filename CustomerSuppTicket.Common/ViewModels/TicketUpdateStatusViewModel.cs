namespace CustomerSuppTicket.Common.ViewModels
{
    public class TicketUpdateStatusViewModel
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
        public string? Resolution { get; set; }
    }
}