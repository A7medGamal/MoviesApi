using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Model;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class MoviesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;
        public MoviesController(IMovieService movieService, IGenreService genreService, IMapper mapper)
        {
            _movieService = movieService;
            _genreService = genreService;
            _mapper = mapper;
        }

        private new List<string> _allowedExtentions = new List<string> { ".jpg",".jpeg",".png",".webp" };
        private long _allowedImageSize = 32000000;
        

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var Movies = await _movieService.GetAll();
            var data= _mapper.Map<IEnumerable<MovieDetailsDto>>(Movies);
            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie= await _movieService.GetById(id);
            if (movie == null)
                return NotFound();
            var data = _mapper.Map<MovieDetailsDto>(movie);
            return Ok(data);
        }
        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreid)
        {
            var Movie = await _movieService.GetAll(genreid);
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(Movie);
            return Ok(data);
           
        }
        [HttpPost]
        public async Task<IActionResult> CrateAsync([FromForm] MovieDto dto)
        {
            if (dto.poster == null)
                return BadRequest("poster is required");

            if (!_allowedExtentions.Contains(Path.GetExtension(dto.poster.FileName).ToLower()))
                return BadRequest("image type must be jpg,jpeg,png,webp");

            if (dto.poster.Length > _allowedImageSize)
                return BadRequest("max allowed size for poster is 30 mp");
           var _isValiddGenres = await _genreService.IsValidGenre(dto.GenreId);
            if (!_isValiddGenres)
                return BadRequest("its not valid genre id");

            using var dataStream = new MemoryStream();
            await dto.poster.CopyToAsync(dataStream);

            var movie = _mapper.Map<Movie>(dto);
            movie.poster=dataStream.ToArray();
            _movieService.Add(movie);
            return Ok(movie);


        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteAsync(int id, [FromForm]MovieDto dto)
        {
            var movie = await _movieService.GetById(id);
            if (movie == null)
                return NotFound($"no movie was found with id{id}");
            var _isValiddGenres = await _genreService.IsValidGenre(dto.GenreId);
            if (!_isValiddGenres)
                return BadRequest("its not valid genre id");
            if (dto.poster!= null)
            {
                if (!_allowedExtentions.Contains(Path.GetExtension(dto.poster.FileName).ToLower()))
                    return BadRequest("image type must be jpg,jpeg,png,webp");

                if (dto.poster.Length > _allowedImageSize)
                    return BadRequest("max allowed size for poster is 30 mp");
                using var dataStream = new MemoryStream();
                await dto.poster.CopyToAsync(dataStream);
                movie.poster=dataStream.ToArray();
            }
            movie.GenreId = dto.GenreId;
            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;

            movie.Storyline = dto.Storyline;
            _movieService.Delete(movie);
            return Ok(movie);

        }
        [HttpDelete( "{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie =await _movieService.GetById(id);
            if (movie == null)
             return NotFound($"no movie was found with id{id}");
            _movieService.Update(movie);
            return Ok();
        }

    }
}
