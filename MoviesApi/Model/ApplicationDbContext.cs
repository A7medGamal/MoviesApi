using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) 
        {
        }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<Movie> Movie { get; set; }
    }
}
