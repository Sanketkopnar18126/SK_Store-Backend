using AutoMapper;
using My_Store.Application.DTOs.User;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        try 
        {
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Email already exists");

            var user = new User(
                fullName: dto.FullName,
                email: dto.Email,
                passwordHash: BCrypt.Net.BCrypt.HashPassword(dto.Password),
                phone: dto.Phone
            );
             
           var token= await GenerateAuthResponseAsync(user);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return token;

        }
        catch(Exception ex) 
        {
            throw new Exception("Registration Failed :" + ex);
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email)
                   ?? throw new Exception("Invalid email or password");

        var data= BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        return await GenerateAuthResponseAsync(user);
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        user.RefreshTokens.Add(refreshToken);
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.CommitAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            User = _mapper.Map<UserResponseDto>(user)
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = _unitOfWork.Users.Query()
            .Where(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken))
            .FirstOrDefault();

        if (user == null)
            throw new Exception("Invalid refresh token");

        var tokenEntity = user.RefreshTokens.First(rt => rt.Token == refreshToken);

        if (!tokenEntity.IsActive())
            throw new Exception("Refresh token expired or revoked");

        // Revoke old token
        tokenEntity.Revoke();

        // Generate a new one
        var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

        user.RefreshTokens.Add(newRefreshToken);
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.CommitAsync();

        var newAccessToken = _tokenService.GenerateAccessToken(user);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            User = _mapper.Map<UserResponseDto>(user)
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = _unitOfWork.Users.Query()
            .Where(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken))
            .FirstOrDefault();

        if (user == null)
            throw new Exception("Invalid refresh token");

        var token = user.RefreshTokens.First(rt => rt.Token == refreshToken);

        token.Revoke();
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.CommitAsync();
    }
}
