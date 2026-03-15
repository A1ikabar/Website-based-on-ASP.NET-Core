using System.ComponentModel.DataAnnotations;

namespace LibraryWebApp.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        public string? Country { get; set; }

        // Навигационное свойство
        public ICollection<Book>? Books { get; set; }
    }
}