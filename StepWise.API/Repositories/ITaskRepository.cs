using StepWise.API.Models;

namespace StepWise.API.Repositories
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetByUserIdAsync(string userId);
        Task<TaskItem?> GetByIdAsync(string id);
        Task CreateAsync(TaskItem task);
        Task UpdateAsync(string id, TaskItem task);
        Task DeleteAsync(string id);
    }
}
