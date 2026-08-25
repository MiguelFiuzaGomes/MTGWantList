using Microsoft.EntityFrameworkCore;

namespace MTGWantList.Data;

public class AppDbContext : DbContext
{
    // ASP.NET will provide the database configuration
    // when it creates this context through dependency injection.
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}