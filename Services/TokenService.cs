using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<EntityUser> _userManager;
        public readonly string _jwtSecret;
        public readonly string _env;
        public TokenService(IConfiguration configuration, UserManager<EntityUser> userManager)
        {
            _userManager = userManager;
            _configuration = configuration;

            _env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (_env == "Development")
                _jwtSecret = _configuration["JWT_SECRET"];
            else
                _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        }

        public async Task<string> GenerateToken(EntityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));

            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var descriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(3)
            };
            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }
    }
}