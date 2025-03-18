using BusinessAccessLayer.DTOS;
using BusinessAccessLayer.DTOS.AuthDtos;
using BusinessAccessLayer.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            _logger.LogInformation("Register request received for email: {Email}", registerDto.Email);

            try
            {
                AuthModel result = await _authService.RegisterAsync(registerDto);
                if (result.IsAuthenticated)
                {
                    _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);
                    return Ok(new Response<AuthModel>(result, true, "Success"));
                }

                _logger.LogWarning("Registration failed for email: {Email}", registerDto.Email);
                return BadRequest(new Response<AuthModel>(result, false, "Failed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration: {Email}", registerDto.Email);
                return StatusCode(500, new Response<string>(null, false, "An error occurred while processing your request."));
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login request received for email: {Email}", loginDto.Email);

            try
            {
                AuthModel result = await _authService.GetTokenAsync(loginDto);
                if (result.IsAuthenticated)
                {
                    _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);
                    return Ok(new Response<AuthModel>(result, true, "Success"));
                }

                _logger.LogWarning("Login failed for email: {Email}", loginDto.Email);
                return BadRequest(new Response<AuthModel>(result, false, "Failed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user login: {Email}", loginDto.Email);
                return StatusCode(500, new Response<string>(null, false, "An error occurred while processing your request."));
            }
        }
    }
}
