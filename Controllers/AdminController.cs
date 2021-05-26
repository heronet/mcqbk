using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers
{
    // [Authorize(Roles = "Admin")] 
    public class AdminController : DefaultController
    {
        private readonly UserManager<EntityUser> _userManager;
        private readonly SignInManager<EntityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;

        public AdminController(
            UserManager<EntityUser> userManager,
            SignInManager<EntityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            TokenService tokenService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserAuthDTO>> RegisterAdmin(RegisterDTO registerDTO)
        {
            var adminExists = await _roleManager.RoleExistsAsync("Admin");
            if (!adminExists)
            {
                var role = new IdentityRole("Admin");
                var roleResult = await _roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                    return BadRequest(roleResult);
            }
            var user = new EntityUser
            {
                UserName = registerDTO.Username.ToLower().Trim(),
                Email = registerDTO.Email.ToLower().Trim(),
                PhoneNumber = registerDTO.Phone
            };
            var result = await _userManager.CreateAsync(user, password: registerDTO.Password);
            if (!result.Succeeded) return BadRequest(result);

            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
            if (addToRoleResult.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return await UserToDto(user, roles.ToList());
            }
            return BadRequest("Can't add Admin");
        }
        private async Task<UserAuthDTO> UserToDto(EntityUser user, List<string> roles)
        {
            return new UserAuthDTO
            {
                Username = user.UserName,
                Token = await _tokenService.GenerateToken(user),
                Id = user.Id,
                Email = user.Email,
                Roles = roles
            };
        }
    }
}