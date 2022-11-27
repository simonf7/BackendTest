using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendTest.Models;
using BackendTest.Helpers;

namespace BackendTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // Bit hacky, should've been done with Middleware for scalability
            if (authorizationHeader.StartsWith("Bearer "))
            {
                authorizationHeader = authorizationHeader.Substring(7);
            }

            var userId = JWT.ValidateJWT(authorizationHeader);
            if (userId == null)
            {
                var problemDetails = new ValidationProblemDetails(ModelState);
                problemDetails.Errors.Add("unauthorised", new[] { "Invalid token." });
                return StatusCode(StatusCodes.Status401Unauthorized, problemDetails);
            }

            return await _context.Users
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        // POST: api/user
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostUser([FromForm] UserRegister user)
        {
            // make sure this email isn't already used
            if (EmailExists(user.Email))
            {
                var problemDetails = new ValidationProblemDetails(ModelState);
                problemDetails.Errors.Add("duplicate", new[] { "User with this email address already exists." });
                return StatusCode(StatusCodes.Status400BadRequest, problemDetails);
            }

            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = PasswordHasher.HashPasswordV3(user.Password)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, ItemToDTO(newUser));
        }

        // POST: api/user/login
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> LoginUser([FromForm] UserLogin userLogin)
        {
            var user = FindUserByEmail(userLogin.Email);

            // if user with email found, check the password matches the hash
            if (user != null)
            {
                if (!PasswordHasher.VerifyHashedPasswordV3(user.Password, userLogin.Password))
                {
                    // password and hash don't match
                    user = null;
                }
            }

            // no user, return unauthorised
            if (user == null)
            {
                var problemDetails = new ValidationProblemDetails(ModelState);
                problemDetails.Errors.Add("unauthorised", new[] { "Email and password are not authorised." });
                return StatusCode(StatusCodes.Status401Unauthorized, problemDetails);
            }

            string token = JWT.CreateJWT(user.Id.ToString(), user.Email);

            return StatusCode(StatusCodes.Status201Created, new { token = token });
        }

        private User FindUserByEmail(string email)
        {
            var emailLower = email.ToLower();

            return _context.Users
                .Where(e => e.Email.ToLower() == emailLower)
                .FirstOrDefault();
        }

        private bool EmailExists(string email)
        {
            return FindUserByEmail(email) != null;
        }

        private UserDTO ItemToDTO(User user) =>
            new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
    }
}
