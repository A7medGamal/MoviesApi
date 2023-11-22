using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Services
{
    public class MovieService : IMovieService
    { 
        private readonly ApplicationDbContext _Context;

        public MovieService(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<Movie> Add(Movie movie)
        {
            await _Context.AddAsync(movie);
            _Context.SaveChangesAsync();
            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _Context.Update(movie);
            _Context.SaveChanges();
            return movie;
        }
    

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
            return await _Context.Movie.Where(m=>m.GenreId==genreId || genreId==0).Include(m => m.Genre).OrderByDescending(m => m.Rate).ToListAsync();
        }

        public async Task<Movie> GetById(int id)
        {
            return await _Context.Movie.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);
        }

        public Movie Update(Movie movie)
        {
            _Context.Update(movie);
            _Context.SaveChanges();
            return movie;
        }
    }
}
