using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly MongoDbContext _context;

        public AccountController(MongoDbContext context)
        {
            _context = context;
        }

        // POST: api/account/register
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.UserName) || string.IsNullOrEmpty(registerDto.Password))
            {
                return BadRequest("Username and password are required.");
            }

            // Check if username is already taken
            var existingUser = await _context.Users.Find(u => u.UserName == registerDto.UserName.ToLower()).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return BadRequest("Username is already taken");
            }

            // Get the next auto-increment value for User ID
            int nextId = _context.GetNextSequenceValue("Users");

            // Hash the password using HMACSHA512
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                Id = nextId,
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                Country = registerDto.Country,
                Gender = registerDto.Gender
            };


            // Insert the new user into MongoDB
            await _context.Users.InsertOneAsync(user);

            return Ok(user);
        }

        // POST: api/account/login
        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.UserName) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Username and password are required.");
            }

            // Find the user by username
            var user = await _context.Users.Find(u => u.UserName == loginDto.UserName.ToLower()).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            // Verify the password hash
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }

            return Ok(user); // In real applications, you'd typically return a token here
        }
    }
}
