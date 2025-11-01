using System.ComponentModel.DataAnnotations;

namespace CustomerSuppTicket.Common.ViewModels
{
 public class TicketViewModel
 {
 [Required]
 [StringLength(100, ErrorMessage = "Name must be at most100 characters.")]
 public string? Name { get; set; }

 [Required]
 [EmailAddress(ErrorMessage = "Invalid email address.")]
 [StringLength(256, ErrorMessage = "Email must be at most256 characters.")]
 public string? Email { get; set; }

 [Required]
 [StringLength(2000, ErrorMessage = "Description must be at most2000 characters.")]
 public string? Description { get; set; }

    }
}
