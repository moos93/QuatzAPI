// Controllers/UserController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApi.DataAccess;
using UserApi.Models;
using System.Windows;
using UserApi.Triggers;

namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly WelcomeEmailTrigger _emailTrigger;

        public UserController(ApplicationDbContext context, WelcomeEmailTrigger emailTrigger)
        {
            _context = context;
            _emailTrigger = emailTrigger;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            await _emailTrigger.ExecuteAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        
      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id) return BadRequest();
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("pendingemails")]
        public async Task<ActionResult<List<User>>> GetPendingEmails()
        {
            var users = await _context.Users
                .Where(u => !u.HasReceivedEmail)
                .ToListAsync();
            return Ok(users);
        }

        // 2. Update HasReceivedEmail field for a user
        [HttpPut("updateemailstatus")]
        public async Task<IActionResult> UpdateEmailStatus([FromBody] User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.HasReceivedEmail = user.HasReceivedEmail;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{userId}/mark-email-sent")]
        public async Task<IActionResult> MarkEmailAsSent(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Update HasReceivedEmail to true
            user.HasReceivedEmail = true;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
