﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.AuthServices;
using RelationshipAnalysis.Settings.Authentication;

namespace RelationshipAnalysis.Test.Services.AuthServices;

public class JwtTokenGeneratorTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtTokenGenerator _sut;

    public JwtTokenGeneratorTests()
    {
        _jwtSettings = new JwtSettings
        {
            Key = "ThisIsASecretKeyForTestingThisIsASecretKeyForTestingThisIsASecretKeyForTesting",
            ExpireMinutes = 60
        };
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(_jwtSettings);

        _sut = new JwtTokenGenerator(mockJwtSettings.Object);
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnTokenWithExpectedClaims()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "Admin" } }
            }
        };

        var handler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Role, "Admin")
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                SecurityAlgorithms.HmacSha256
            )
        };

        var expectedToken = handler.CreateToken(tokenDescriptor);
        var expectedTokenString = handler.WriteToken(expectedToken);

        // Act
        var actualToken = _sut.GenerateJwtToken(user);

        // Assert
        Assert.Equal(expectedTokenString, actualToken);
    }

    [Fact]
    public void GenerateJwtToken_ShouldSetTokenExpirationCorrectly()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "Admin" } }
            }
        };

        // Act
        var token = _sut.GenerateJwtToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        Assert.NotNull(jwtToken);
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        Assert.True(jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes));
    }
}