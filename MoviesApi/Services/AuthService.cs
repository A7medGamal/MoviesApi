using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.Helper;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace MoviesApi.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Email = user.Email,
                //
                //ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authmodel = new AuthModel();
            var user =await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user,model.Password))
            {
                authmodel.Message = "Email Or pasword is wrong";
                return authmodel;

            }
            var jwtSecurityToken =await CreateJwtToken(user);
            var roleList = await _userManager.GetRolesAsync(user);
            authmodel.IsAuthenticated = true;
            //authmodel.ExpiresOn = jwtSecurityToken.ValidTo;
            authmodel.Email = user.Email;
            authmodel.Username=user.UserName;
            authmodel.Roles = roleList.ToList();
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            if(user.RefreshTokens.Any(t=>t.IsActive))
            {
                var activRefeshtoken=user.RefreshTokens.FirstOrDefault(t=>t.IsActive);
                authmodel.RefreshToken = activRefeshtoken.Token;
                authmodel.RefreshTokenExpirsOn = activRefeshtoken.ExpiresOn;
            }
            else
            {
                var refrshToken = GenerateRefreshToken();
                authmodel.RefreshToken = refrshToken.Token;
                authmodel.RefreshTokenExpirsOn = refrshToken.ExpiresOn;
                user.RefreshTokens.Add(refrshToken);
                await _userManager.UpdateAsync(user);

            }
            return authmodel;



        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<string> AddRoleToUserasync(AsignRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null || !await _roleManager.RoleExistsAsync(model.RoleName))
                return  "User ID Or Role Is not Found";
            if (await _userManager.IsInRoleAsync(user, model.RoleName))
                return "user is already inthis role";
            var result =await _userManager.AddToRoleAsync(user, model.RoleName);

            return result.Succeeded ? string.Empty : "somthing went erorr";

        }
        private RefreshToken GenerateRefreshToken()
        {
            var randoNumber = new byte[32];
            using var Generator = new RNGCryptoServiceProvider();
            Generator.GetBytes(randoNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randoNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

    }
}
