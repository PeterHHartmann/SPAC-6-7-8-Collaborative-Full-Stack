using Inventory_Management.Helpers;
using Inventory_Management.Managers;
using Inventory_Management.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly UserManager _userManager;
        private readonly AuthHelpers _authHelpers;

        public LoginController(UserManager userManager, AuthHelpers authHelpers)
        {
            _userManager = userManager;
            _authHelpers = authHelpers;
        }

        // POST: api/login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                var user = await _userManager.GetByUsername(loginModel.Username); // Get user by username
                if (user == null)
                {
                    return Unauthorized("Invalid username"); // Return unauthorized if user not found
                }
                if (!_userManager.VerifyPassword(user, loginModel.Password)) // Verify password
                {
                    return Unauthorized("Invalid password"); // Return unauthorized if password is incorrect
                }
                var token = _authHelpers.GenerateJWTToken(user); // Generate JWT token
                return Ok(new { token }); // Return token
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while logging in: {ex.Message}");
            }
        }

    }
}
