using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LibraryWebApp.Data;

namespace LibraryWebApp
{
    public class LibraryContextFactory : IDesignTimeDbContextFactory<LibraryContext>
    {
        public LibraryContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>();

            // Укажите ту же строку подключения, что и в Program.cs
            optionsBuilder.UseSqlite("Data Source=Library.db");

            return new LibraryContext(optionsBuilder.Options);
        }
    }
}