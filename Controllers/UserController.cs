using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return await _context.Users
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        // POST: api/user
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostUser(UserRegister user)
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

        private bool EmailExists(string email)
        {
            var emailLower = email.ToLower();
            return _context.Users.Any(e => e.Email.ToLower() == emailLower);
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
