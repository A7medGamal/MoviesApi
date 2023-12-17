using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.Services;
using System.Net;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpirsOn);
            }

            return Ok(result);
        }
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleToUser([FromBody] AsignRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleToUserasync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("refreshtoken")]
        public async Task<IActionResult> refreshtoken()
        {
            var refreshtoken = Request.Cookies["refreshtoken"];
            var result =await _authService.RefreshTokenAsync(refreshtoken);
            if (!result.IsAuthenticated)
                return BadRequest(result);
            SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpirsOn);
            return Ok(result);

        }
        private void SetRefreshTokenInCookies(string refreshtoken,DateTime expires )
        {
            var Cookiesoption = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires

            };
            Response.Cookies.Append("refreshtoken", refreshtoken, Cookiesoption);
        }
    }
}
