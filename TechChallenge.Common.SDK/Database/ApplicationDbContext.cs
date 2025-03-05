using Microsoft.EntityFrameworkCore;

namespace TechChallenge.Common.SDK.Database
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Contato> Contato { get; set; }
    }
}