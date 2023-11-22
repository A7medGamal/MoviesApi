using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MoviesApi.Model;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {

        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _genreService.GetAll();
            return Ok(genres);
        }
        [HttpPost]
        public async Task<IActionResult> CreatAsync(GenreDto dto)
        {
            Genre genre = new Genre { Name = dto.Name };
            await _genreService.Add(genre);
            return Ok(genre);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(byte id,[FromBody] GenreDto dto)
        {
            var genre = await _genreService.GetById(id);
            if (genre == null)
            {
                return NotFound($"No Genre was found withe ID:{id}");
            }
            genre.Name = dto.Name;
            _genreService.Update(genre);
            return Ok (genre);


        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _genreService.GetById(id);
            if (genre == null)
            {
                return NotFound($"No Genre was found withe ID:{id}");
            }

            _genreService.Delete(genre);
            return Ok();

        }


    }
}
