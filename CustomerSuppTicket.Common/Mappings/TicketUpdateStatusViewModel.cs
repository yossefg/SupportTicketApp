namespace CustomerSuppTicket.Common.Mappings
{
    public class TicketUpdateStatusViewModel
    {
        public Guid Id { get; set; }
     
        public string? Status { get; set; }
        public string? Resolution { get; set; }
    }
}