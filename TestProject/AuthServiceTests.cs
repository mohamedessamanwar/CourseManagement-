using BusinessAccessLayer.DTOS.AuthDtos;
using BusinessAccessLayer.Services.AuthService;
using BusinessAccessLayer.Services.Email;
using DataAccessLayer.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<IMailingService> _mailingServiceMock;
    private readonly Mock<IOptions<JWT>> _jwtMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
        );

        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null
        );

        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null
        );

        _mailingServiceMock = new Mock<IMailingService>();

        var jwtOptions = new JWT
        {
            Key = "your-secret-key-should-be-long-enough",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            DurationInDays = 7
        };

        _jwtMock = new Mock<IOptions<JWT>>();
        _jwtMock.Setup(j => j.Value).Returns(jwtOptions);

        _authService = new AuthService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _jwtMock.Object,
            _mailingServiceMock.Object,
            _signInManagerMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenUserIsCreated()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((User)null);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Admin"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { "Admin" });

        _userManagerMock.Setup(x => x.AddClaimsAsync(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>()))
            .ReturnsAsync(IdentityResult.Success);

        _mailingServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
     .ReturnsAsync(true);


        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal("Registration successful. Please check your email.", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnError_WhenUserAlreadyExists()
    {
        // Arrange
        var registerDto = new RegisterDto { Email = "test@example.com", Password = "Password123!" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync(new User { Email = registerDto.Email });

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.Equal("Email already exists", result.Message);
    }

    [Fact]
    public async Task GetTokenAsync_ShouldReturnToken_WhenLoginIsSuccessful()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
        var user = new User { Email = loginDto.Email, UserName = "testuser", Id = "12345" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, loginDto.Password, false, true))
            .ReturnsAsync(SignInResult.Success);

        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _userManagerMock.Setup(x => x.GetClaimsAsync(user))
            .ReturnsAsync(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            });

        // Mock token generation
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtMock.Object.Value.Key));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtMock.Object.Value.Issuer,
            audience: _jwtMock.Object.Value.Audience,
            claims: new List<Claim>(),
            expires: DateTime.Now.AddDays(_jwtMock.Object.Value.DurationInDays),
            signingCredentials: signingCredentials
        );

        _userManagerMock.Setup(x => x.GetClaimsAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<Claim>());

        // Act
        var result = await _authService.GetTokenAsync(loginDto);

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.NotNull(result.Token);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetTokenAsync_ShouldReturnError_WhenLoginFails()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "WrongPassword!" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(new User { Email = loginDto.Email });

        _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), loginDto.Password, false, true))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _authService.GetTokenAsync(loginDto);

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.Equal("Email or Password is incorrect!", result.Message);
    }
}
