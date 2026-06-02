using StepWise.API.Models;

namespace StepWise.API.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task CreateAsync(User user);
        Task<User?> GetByIdAsync(string id);
        Task UpdateAsync(string id, User user);
    }
}
