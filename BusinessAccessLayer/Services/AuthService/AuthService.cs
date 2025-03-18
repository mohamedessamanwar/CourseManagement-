using BusinessAccessLayer.DTOS.AuthDtos;
using BusinessAccessLayer.Services.Email;
using DataAccessLayer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace BusinessAccessLayer.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JWT _jwt;
        private readonly IMailingService _mailingService;
        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt = null, IMailingService mailingService = null, SignInManager<User> signInManager = null)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _mailingService = mailingService;
            _signInManager = signInManager;
        }

        public async Task<AuthModel> RegisterAsync(RegisterDto model)
        {
            // Check if the email already exists
            var userCheck = await _userManager.FindByEmailAsync(model.Email);
            if (userCheck is not null)
            {
                return new AuthModel { Message = "Email already exists" };
            }
            // Create a new ApplicationUser
            var user = new User
            {
                UserName = model.Email.Split('@')[0],
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            // Attempt to create the user
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                // Collect all error messages
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthModel { Message = errors };
            }

            // Assign the user to the "User" role
            await _userManager.AddToRoleAsync(user, "Admin");
            // Add claims to the user
            var role = await AddClaimsAsync(user);
            if (role is not null)
            {
                return new AuthModel { Message = role };
            }
            await _mailingService.SendEmailAsync(model.Email, "Welcome to Our Platform!",
          $@"
             <h2>Welcome, {model.FirstName}!</h2>
             <p>We're thrilled to have you join us at <strong>[training company.]</strong>.</p>
             <p>We hope you enjoy our services and have a great experience with us.</p>
            <p>If you have any questions, feel free to reach out.</p>
            <br>
            <p>Best regards,</p>
           <p>The [training company.] Team</p>
          ");


            return new AuthModel
            {
                Message = "Registration successful. Please check your email.",
                IsAuthenticated = true
            };
        }
        private async Task<string> AddClaimsAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id) ,

            }
            .Union(roleClaims);
            var result = await _userManager.AddClaimsAsync(user, claims);
            string Error = null;
            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    Error += $"{error.Description},";
                }
            }
            return Error;

        }
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<AuthModel> GetTokenAsync(LoginDto model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            var Token = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = Token.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(Token),
                Username = user.UserName
            };
        }

    }
}
