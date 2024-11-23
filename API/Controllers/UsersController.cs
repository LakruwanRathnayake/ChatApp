using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using API.Entities;
using API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<UsersController> _logger;
        public UsersController(MongoDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/users/all/{userId}
        [HttpGet("all/{userId}")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(string userId)
        {
            // Convert the userId from string to int
            if (!int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest(new
                {
                    Message = "Invalid user ID format.",
                    StatusCode = 400
                });
            }

            // Filter to exclude the current user's data
            var users = await _context.Users.Find(u => u.Id != parsedUserId).ToListAsync();
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserById(int id)
        {
            var user = await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new
                {
                    Message = $"User with id {id} not found.",
                    StatusCode = 404
                });
            }

            return Ok(user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<AppUser>> UpdateUser(int id, [FromForm] AppUserUpdateDto updateUserDto)
        {
            // Log the incoming updateUserDto data for debugging
            _logger.LogInformation("Updating user with ID: {UserId}", id);
            _logger.LogInformation("UpdateUserDto data: UserName={UserName}, Email={Email}, Phone={Phone}, Country={Country}, Gender={Gender}, ProfilePhotoLength={ProfilePhotoLength}",
                updateUserDto.UserName,
                updateUserDto.Email,
                updateUserDto.Phone,
                updateUserDto.Country,
                updateUserDto.Gender,
                updateUserDto.ProfilePhoto?.Length ?? 0);

            var user = await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new
                {
                    Message = $"User with id {id} not found.",
                    StatusCode = 404
                });
            }

            // Update user properties
            user.UserName = updateUserDto.UserName;
            user.Email = updateUserDto.Email;
            user.Phone = updateUserDto.Phone;
            user.Country = updateUserDto.Country;
            user.Gender = updateUserDto.Gender;

            // Check if a profile photo is provided
            if (updateUserDto.ProfilePhoto != null && updateUserDto.ProfilePhoto.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await updateUserDto.ProfilePhoto.CopyToAsync(stream);
                    user.ProfilePhoto = stream.ToArray(); // Store the photo as a byte array
                    _logger.LogInformation("Profile photo updated for user ID: {UserId}", id);
                }
            }

            // Update user in the database
            var result = await _context.Users.ReplaceOneAsync(u => u.Id == id, user);
            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return Ok(user);
            }
            else
            {
                return BadRequest(new
                {
                    Message = "Failed to update user.",
                    StatusCode = 400
                });
            }
        }

        [HttpGet("matches/{userId}")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetMatchedUsers(string userId)
        {
            // Convert the userId from string to int
            if (!int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest(new
                {
                    Message = "Invalid user ID format.",
                    StatusCode = 400
                });
            }

            // Fetch the current user to get their gender and country
            var currentUser = await _context.Users.Find(u => u.Id == parsedUserId).FirstOrDefaultAsync();
            if (currentUser == null)
            {
                return NotFound(new
                {
                    Message = $"User with id {userId} not found.",
                    StatusCode = 404
                });
            }

            // Filter by opposite gender and same country
            var matchedUsers = await _context.Users.Find(u =>
                u.Id != parsedUserId &&                         // Exclude current user
                u.Gender != currentUser.Gender &&               // Opposite gender
                u.Country == currentUser.Country                // Same country
            ).ToListAsync();

            return Ok(matchedUsers);
        }
    }

}
