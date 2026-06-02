using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StepWise.API.DTOs;
using StepWise.API.Services;
using System.Security.Claims;

namespace StepWise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var user = await _userService.GetByIdAsync(GetUserId());
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado." });

            return Ok(new
            {
                id = user.Id,
                nome = user.Nome,
                email = user.Email
            });
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                return BadRequest(new { message = "O nome não pode ser vazio." });

            var user = await _userService.UpdateAsync(GetUserId(), dto);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado." });

            return Ok(new
            {
                id = user.Id,
                nome = user.Nome,
                email = user.Email
            });
        }
        [HttpPut("me/password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NovaSenha))
                return BadRequest(new { message = "A nova senha não pode ser vazia." });

            var result = await _userService.UpdatePasswordAsync(GetUserId(), dto);
            if (!result)
                return BadRequest(new { message = "Senha atual incorreta." });

            return Ok(new { message = "Senha alterada com sucesso." });
        }
    }
}