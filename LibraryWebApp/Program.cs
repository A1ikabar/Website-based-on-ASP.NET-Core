using Microsoft.EntityFrameworkCore;
using LibraryWebApp.Data;

var builder = WebApplication.CreateBuilder(args); //web ASP.NET init

builder.Services.AddControllersWithViews(); //for controllers

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite("Data Source=library.db"));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

// Создание базы данных при запуске
/*
var scope = app.Services.CreateScope();
try
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    dbContext.Database.EnsureCreated();
}
finally
{
    if (scope != null)
        scope.Dispose(); 
}*/
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    dbContext.Database.EnsureCreated(); //бд если нет бд
}

app.Run();