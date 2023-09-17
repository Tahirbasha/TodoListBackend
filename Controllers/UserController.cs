using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TodoList.DataContext;
using TodoList.Dto;
using TodoList.Models;

namespace TodoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly TodoDataContext _todoContext;
        private readonly IConfiguration _configuration;
        public UserController(TodoDataContext todoContext, IConfiguration configuration)
        {
            _todoContext = todoContext;
            _configuration = configuration;
        }

        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(int userId)
        {
            var image = await _todoContext.Images.FindAsync(userId);
            if (image == null)
            {
                return NotFound();
            }
            return File(image.ImageData, image.ContentType);
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadProfilePic([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid File");
            }
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var image = new ProfilePic
            {
                UserId = 1,
                FileName = file.FileName,
                ContentType = file.ContentType,
                ImageData = memoryStream.ToArray()
            };
            _todoContext.Images.Add(image);
            await _todoContext.SaveChangesAsync();
            return Ok(image.Id);

        }
        [HttpPost(nameof(RegisterUser))]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userCredentials)
        {
            var user = _todoContext.Users.FirstOrDefault(user => user.UserName.ToLower() == userCredentials.Username.ToLower());
            if (user is not null)
            {
                return BadRequest("User already exist");
            }
            try
            {
                /*                CreateHashedPassword(userCredentials.Password, out byte[] passwordHash, out byte[] passwordSalt);
                */
                using (var hmac = new System.Security.Cryptography.HMACSHA512())
                {
                    var passwordSalt = hmac.Key;
                    var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userCredentials.Password));
                    var User = new User()
                    {
                        UserName = userCredentials.Username,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    };
                    await _todoContext.Users.AddAsync(User);
                    await _todoContext.SaveChangesAsync();
                    return Ok("User created successfully!!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }

        }
        [HttpPost("LoginUser")]
        public async Task<IActionResult> Login([FromBody] UserRegisterDto userLoginDto)
        {
            var user = await _todoContext.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower().Equals(userLoginDto.Username.ToLower()));
            if (user is null)
            {
                return BadRequest("User does not exist");
            }
            else if (!VerifyPassword(userLoginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is incorrect!!!");
            }
            else
            {
                return Ok(CreateJwtToken(user));
            }
        }
/*        public void CreateHashedPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }*/

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if (appSettingsToken is null)
                throw new Exception("AppSettings Token is null!");

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(appSettingsToken));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
