using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWebApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int ReaderId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        [Display(Name = "Review Date")]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        // Навигационные свойства
        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        [ForeignKey("ReaderId")]
        public Reader? Reader { get; set; }
    }
}