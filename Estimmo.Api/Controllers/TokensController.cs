using Estimmo.Api.Models;
using Estimmo.Api.Options;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AdminUser> _signInManager;
        private readonly UserManager<AdminUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public AuthController(SignInManager<AdminUser> signInManager, UserManager<AdminUser> userManager,
            JwtOptions jwtOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions;
        }

        [HttpPost]
        [Route("/tokens")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginModel model)
        {
            var loginResult =
                await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, true);

            if (!loginResult.Succeeded)
            {
                return new StatusCodeResult(403);
            }

            var user = new AdminUser { UserName = model.UserName, Email = model.UserName };
            var roles = await _userManager.GetRolesAsync(user);

            var token = GetTokenForUser(user, roles);
            var output = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(output);
        }

        private JwtSecurityToken GetTokenForUser(AdminUser user, IEnumerable<string> roles)
        {
            var notBefore = DateTime.UtcNow;
            var expiry = DateTime.UtcNow.AddHours(_jwtOptions.ExpirationInHours);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.Key);
            var key = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer, _jwtOptions.Audience, claims, notBefore, expiry, credentials);

            return token;
        }
    }
}
