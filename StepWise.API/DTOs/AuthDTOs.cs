using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StepWise.API.Models;
using StepWise.API.DTOs;
using BCrypt.Net;

namespace StepWise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly JwtService _jwtService;

        public AuthController(IConfiguration config, JwtService jwtService)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var db = client.GetDatabase(config["MongoDB:DatabaseName"]);

            _users = db.GetCollection<User>("Users");
            _jwtService = jwtService;
        }

        // 🔥 REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var existingUser = await _users
                .Find(u => u.Email == dto.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
                return BadRequest(new { message = "Usuário já existe" });

            var user = new User
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CriadoEm = DateTime.UtcNow
            };

            await _users.InsertOneAsync(user);

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                Nome = user.Nome,
                Email = user.Email
            });
        }

        // 🔐 LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _users
                .Find(u => u.Email == dto.Email)
                .FirstOrDefaultAsync();

            if (user == null)
                return Unauthorized(new { message = "Usuário não encontrado" });

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.SenhaHash))
                return Unauthorized(new { message = "Senha inválida" });

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                Nome = user.Nome,
                Email = user.Email
            });
        }
    }
}
