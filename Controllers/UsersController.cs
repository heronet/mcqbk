using System.Threading.Tasks;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Controllers
{
    [Authorize]
    public class UsersController : DefaultController
    {
        private readonly UserManager<EntityUser> _userManager;
        public UsersController(UserManager<EntityUser> userManager)
        {
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
                return Ok(UserToDto(user));
            }
            // Lookup by Username
            else if (searchBy == "username")
            {
                var user = await _userManager.FindByNameAsync(query);
                if (user == null)
                    return BadRequest("User Not Found");
                return Ok(UserToDto(user));
            }
            // Lookup by Full Name
            else if (searchBy == "name")
            {
                // TODO: Full Name Search
            }
            // If code reches this point, that means searchBy is invalid. So we return 400
            return BadRequest("Invalid Query");
        }
        private UserInfoDTO UserToDto(EntityUser user)
        {
            return new UserInfoDTO
            {
                Username = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                LastActive = user.LastActive
            };
        }
    }
}