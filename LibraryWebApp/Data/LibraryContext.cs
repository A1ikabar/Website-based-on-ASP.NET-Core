using Microsoft.EntityFrameworkCore;
using LibraryWebApp.Models;

namespace LibraryWebApp.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка составного ключа для Borrowing
            modelBuilder.Entity<Borrowing>()
                .HasKey(b => new { b.ReaderId, b.BookId });

            // Настройка связей Borrowing -> Reader
            modelBuilder.Entity<Borrowing>()
                .HasOne(b => b.Reader)
                .WithMany(r => r.Borrowings)
                .HasForeignKey(b => b.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связей Borrowing -> Book
            modelBuilder.Entity<Borrowing>()
                .HasOne(b => b.Book)
                .WithMany(b => b.Borrowings)
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связей Review -> Book
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связей Review -> Reader
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reader)
                .WithMany(r => r.Reviews)
                .HasForeignKey(r => r.ReaderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed данные (опционально)
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FullName = "George Orwell", Country = "United Kingdom" },
                new Author { Id = 2, FullName = "Stephen King", Country = "USA" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "1984", Genre = BookGenre.Fiction, PublicationYear = 1949, AuthorId = 1 },
                new Book { Id = 2, Title = "The Shining", Genre = BookGenre.Thriller, PublicationYear = 1977, AuthorId = 2 }
            );

            modelBuilder.Entity<Reader>().HasData(
                new Reader { Id = 1, FullName = "Alexey Petrov", LibraryCardNumber = "LIB-001", PhoneNumber = "+7 (123) 456-78-90" },
                new Reader { Id = 2, FullName = "Maria Ivanova", LibraryCardNumber = "LIB-002", PhoneNumber = "+7 (234) 567-89-01" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}