using Microsoft.EntityFrameworkCore;
using MoviesApi.Model;

namespace MoviesApi.Services
{
    public class GenreService : IGenreService
    {
        private readonly ApplicationDbContext _context;

        public GenreService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Genre> Add(Genre genre)
        {
            await _context.Genre.AddAsync(genre);
            _context.SaveChanges();
            return genre;
        }

        public Genre Delete(Genre genre)
        {
            _context.Genre.Remove(genre);
            _context.SaveChanges();
            return genre;

        }

       public async Task<IEnumerable<Genre>>GetAll()
        {
            return await _context.Genre.OrderBy(g => g.Name).ToArrayAsync();
        }

        public async Task<Genre> GetById(byte id)
        {
            return await _context.Genre.SingleOrDefaultAsync(g => g.Id == id);
        }

        public Genre Update(Genre genre)
        {
            _context.Genre.Update(genre);
            _context.SaveChanges();
            return genre;
        }

        public async Task<bool> IsValidGenre(byte id)
        {
           return await _context.Genre.AnyAsync(g => g.Id == id);

        }
    }
}
