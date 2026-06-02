using StepWise.API.DTOs;
using StepWise.API.Models;
using StepWise.API.Repositories;

namespace StepWise.API.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> UpdateAsync(string id, UpdateUserDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            user.Nome = dto.Nome;
            await _userRepository.UpdateAsync(id, user);
            return user;
        }
        public async Task<bool> UpdatePasswordAsync(string id, UpdatePasswordDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.SenhaAtual, user.SenhaHash))
            return false;

            user.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
            await _userRepository.UpdateAsync(id, user);
            return true;
        }
    }
}