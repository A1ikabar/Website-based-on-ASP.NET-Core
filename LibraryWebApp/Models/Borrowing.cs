using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryWebApp.Models
{
    public class Borrowing
    {
        [Key]
        [Column(Order = 0)]
        public int ReaderId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int BookId { get; set; }

        [Required]
        [Display(Name = "Borrowed Date")]
        public DateTime BorrowedAt { get; set; }

        [Display(Name = "Returned Date")]
        public DateTime? ReturnedAt { get; set; }

        // Навигационные свойства
        [ForeignKey("ReaderId")]
        public Reader? Reader { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}