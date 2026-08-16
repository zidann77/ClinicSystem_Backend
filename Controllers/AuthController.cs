using BackendClinicProject.GlobalClasses;
using ClinicBusinessLogic;
using ClinicDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecurityLayer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendClinicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                // Step 1: Find the user by Username
                var user = clsUser.Login(request.Username);

                // If no user is found, return 401 Unauthorized
                if (user == null)
                    return Unauthorized("Invalid credentials");

                // Step 2: Verify the password using your custom clsPasswordHasher
                bool isValidPassword = clsPasswordHasher.VerifyPassword(request.Password, user.Password);

                if (!isValidPassword)
                    return Unauthorized("Invalid credentials");

                // Step 3: Fetch the user's role
                //var role = clsRole.Find(user.RoleID ?? 0);
                //string roleName = role != null ? role.RoleName : "User";


                // Step 4: Create claims
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.RoleName)
                };

                // Step 5: Create key
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456789_MUST_BE_LONG"));

                // Step 6: Define signing credentials
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Step 7: Create JWT token
                var token = new JwtSecurityToken(
                    issuer: "ClinicApi",
                    audience: "ClinicApiUsers",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                // Step 8: Return the serialized JWT token
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred during login process.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
