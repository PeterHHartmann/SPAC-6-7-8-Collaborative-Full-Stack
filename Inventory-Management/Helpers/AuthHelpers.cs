using Inventory_Management.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inventory_Management.Helpers
{
    public class AuthHelpers
    {
        private readonly IConfiguration _configuration;

        public AuthHelpers(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Generate JWT Token
        public string GenerateJWTToken(User user)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured"))); // Get the key from appsettings.json
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Create credentials
                var claims = new List<Claim> { // Create claims
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };
                // Create JWT token
                var jwtToken = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(2), // Token expires in 2 hours
                    signingCredentials: credentials); // Add credentials
                return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }
            catch (Exception e)
            {
                throw new Exception("Error generating JWT token", e);
            }
        }
    }
}
