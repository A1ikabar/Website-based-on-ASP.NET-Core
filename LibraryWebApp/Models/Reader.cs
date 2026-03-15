using System.ComponentModel.DataAnnotations;

namespace LibraryWebApp.Models
{
    public class Reader
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Library card number is required")]
        [Display(Name = "Library Card Number")]
        public string LibraryCardNumber { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        // Навигационные свойства
        public ICollection<Borrowing>? Borrowings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}