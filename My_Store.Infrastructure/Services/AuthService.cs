using AutoMapper;
using My_Store.Application.DTOs.User;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
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

        // ✅ SAVE USER FIRST
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CommitAsync();

        // ✅ THEN GENERATE TOKENS
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email)
                   ?? throw new Exception("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        return await GenerateAuthResponseAsync(user);
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.PublicId);

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
            .FirstOrDefault(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

        if (user == null)
            throw new UnauthorizedAccessException("Refresh token not found");

        var token = user.RefreshTokens
            .FirstOrDefault(rt => rt.Token == refreshToken);

        if (token == null || !token.IsActive())
            throw new UnauthorizedAccessException("Refresh token expired");

        // 🔁 Rotate
        token.Revoke();

        var newRefreshToken = _tokenService.GenerateRefreshToken(user.PublicId);
        user.RefreshTokens.Add(newRefreshToken);

        await _unitOfWork.CommitAsync();

        return new AuthResponseDto
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            //RefreshToken = newRefreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            User = _mapper.Map<UserResponseDto>(user)
        };
    }



    public async Task LogoutAsync(string refreshToken)
    {
        var user = _unitOfWork.Users.Query()
            .FirstOrDefault(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

        if (user == null)
            return; // already logged out

        var token = user.RefreshTokens
            .FirstOrDefault(rt => rt.Token == refreshToken);

        if (token == null)
            return;

        token.Revoke();
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.CommitAsync();
    }

}
