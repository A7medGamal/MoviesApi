using Microsoft.AspNetCore.Identity;

namespace MoviesApi.Model
{
    public class ApplicationUser: IdentityUser
    {
        [Required,MaxLength(128)]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
