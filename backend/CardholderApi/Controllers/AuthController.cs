using CardholderApi.Services;
using CardholderApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CardholderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
    {
        [HttpPost]
        public IActionResult Login()
        {
            logger.LogInformation("Login attempt started");

            try
            {
                var token = authService.GenerateJwtToken();
                logger.LogInformation("JWT token generated successfully");

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while generating JWT token");
                
                return StatusCode(500, new { message = "An unexpected error occurred during login." });
            }
        }
    }
}