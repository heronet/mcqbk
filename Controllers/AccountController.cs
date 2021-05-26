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
    public class AccountController : DefaultController
    {
        private readonly UserManager<EntityUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly SignInManager<EntityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<EntityUser> userManager, SignInManager<EntityUser> signInManager, RoleManager<IdentityRole> roleManager, TokenService tokenService)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userManager = userManager;
        }
        /// <summary>
        /// POST api/account/register
        /// </summary>
        /// <param name="registerDTO"></param>
        /// <returns><see cref="UserAuthDTO" /></returns>
        [HttpPost("register")]
        public async Task<ActionResult<UserAuthDTO>> RegisterUser(RegisterDTO registerDTO)
        {
            var memberRoleExists = await _roleManager.RoleExistsAsync("Member");
            if (!memberRoleExists)
            {
                var role = new IdentityRole("Member");
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

            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (addToRoleResult.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return await UserToDto(user, roles.ToList());
            }

            return BadRequest("Can't add user");
        }
        /// <summary>
        /// POST api/account/login
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns><see cref="UserAuthDTO" /></returns>
        [HttpPost("login")]
        public async Task<ActionResult<UserAuthDTO>> LoginUser(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email.ToLower().Trim());

            // Return If user was not found
            if (user == null) return BadRequest("Invalid Email");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password: loginDTO.Password, false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return await UserToDto(user, roles.ToList());
            }

            return BadRequest("Invalid Password");
        }
        /// <summary>
        /// POST api/account/refresh
        /// </summary>
        /// <param name="userAuthDTO"></param>
        /// <returns><see cref="UserAuthDTO" /></returns>
        [Authorize]
        [HttpPost("refresh")]
        public async Task<ActionResult<UserAuthDTO>> RefreshToken(UserAuthDTO userAuthDTO)
        {

            var user = await _userManager.FindByIdAsync(userAuthDTO.Id);

            // Return If user was not found
            if (user == null) return BadRequest("Invalid User");

            var roles = await _userManager.GetRolesAsync(user);
            return await UserToDto(user, roles.ToList());
        }

        /// <summary>
        /// Utility Method.
        /// Converts a WhotUser to an AuthUserDto
        /// </summary>
        /// <param name="user"></param>
        /// <returns><see cref="UserAuthDTO" /></returns>
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