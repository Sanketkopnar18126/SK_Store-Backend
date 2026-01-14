using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateAccessToken(User user)
    {
        var jwtKey = _config["JwtSettings:SecretKey"];

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new Exception("JWT SecretKey is missing in configuration");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, user.PublicId.ToString()), 
                new Claim("userId", user.PublicId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("fullname", user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_config["JwtSettings:AccessTokenExpirationMinutes"])
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public RefreshToken GenerateRefreshToken(Guid userPublicId)
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        return new RefreshToken(
            token: Convert.ToBase64String(bytes),
            expiresAt: DateTime.UtcNow.AddDays(7),
            createdByIp: null,
            userId: userPublicId
        );
    }
}
