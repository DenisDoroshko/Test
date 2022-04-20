using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Web.Models
{
    public class PaymentViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Card number is required")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Month is required")]
        public int? Month { get; set; }

        [Required(ErrorMessage = "Year is required")]
        public int? Year { get; set; }

        [Required(ErrorMessage = "CVC is required")]
        public int? CVC { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public decimal? Amount { get; set; }
    }
}
