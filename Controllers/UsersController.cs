using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : DefaultController
    {
        private readonly UserManager<EntityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public UsersController(UserManager<EntityUser> userManager, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult> FindUsers(string searchBy, string query)
        {
            // Lookup by Email
            if (searchBy == "email")
            {
                var user = await _userManager.FindByEmailAsync(query);
                if (user == null)
                    return BadRequest("User Not Found");
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(UserToDto(user, roles.ToList()));
            }
            // Lookup by Username
            else if (searchBy == "username")
            {
                var user = await _userManager.FindByNameAsync(query);
                if (user == null)
                    return BadRequest("User Not Found");
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(UserToDto(user, roles.ToList()));
            }
            // If code reches this point, that means searchBy is invalid. So we return 400
            return BadRequest("Invalid Query");
        }
        [HttpGet("all")]
        public async Task<ActionResult<GetResponseWithPage<UserInfoDTO>>> GetUsers(
            [FromQuery] int pageSize,
            [FromQuery] int pageCount
        )
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = await _userManager.Users.Where(u => u.Id != currentUser).ToListAsync();
            long usersCount = users.Count;

            var userDtos = new List<UserInfoDTO>();
            foreach (var user in users.Skip(pageSize * (pageCount - 1)).Take(pageSize))
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(UserToDto(user, roles.ToList()));
            }

            return Ok(new GetResponseWithPage<UserInfoDTO> { Data = userDtos, Size = usersCount });
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (currentUser == id)
                return BadRequest("You cannot delete yourself");
            var user = await _dbContext.Users.Where(u => u.Id == id)
                .Include(u => u.ParticipatedExams)
                .ThenInclude(er => er.Exam)
                .SingleOrDefaultAsync();
            if (user == null)
                return BadRequest("User doesn't exist");
            var exams = await _dbContext.Exams
                .Where(e => e.CreatorId == user.Id)
                .Include(e => e.SubmissionResults)
                .ToListAsync();
            _dbContext.RemoveRange(exams);

            if (user == null)
                return BadRequest("User Not Found");
            _dbContext.Users.Remove(user);

            if (await _dbContext.SaveChangesAsync() > 0)
                return NoContent();
            return Ok("Failed to delete user");
        }

        private UserInfoDTO UserToDto(EntityUser user, List<string> roles)
        {
            return new UserInfoDTO
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Roles = roles
            };
        }
    }
}