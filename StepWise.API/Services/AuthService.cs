using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StepWise.API.DTOs;
using StepWise.API.Models;
using StepWise.API.Repositories;

namespace StepWise.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return null;

            var user = new User
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CriadoEm = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);

            return new AuthResponseDTO
            {
                Token = GenerateToken(user),
                Nome = user.Nome,
                Email = user.Email
            };
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.SenhaHash))
                return null;

            return new AuthResponseDTO
            {
                Token = GenerateToken(user),
                Nome = user.Nome,
                Email = user.Email
            };
        }

        private string GenerateToken(User user)
        {
            var secret = _configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT Secret não configurado.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Nome)
            };

            var expiresInHours = int.Parse(
                _configuration["Jwt:ExpiresInHours"] ?? "8"
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiresInHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
