using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWebApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Genre")]
        public BookGenre Genre { get; set; }

        [Display(Name = "Publication Year")]
        public int PublicationYear { get; set; }

        [Required]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        // Навигационные свойства
        [ForeignKey("AuthorId")]
        public Author? Author { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Borrowing>? Borrowings { get; set; }
    }
}