using StepWise.API.DTOs;

namespace StepWise.API.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDetailsDTO>> GetAllAsync(string userId);
        Task<TaskDetailsDTO?> GetByIdAsync(string id, string userId);
        Task<TaskDetailsDTO> CreateAsync(CreateTaskDTO dto, string userId);
        Task<TaskDetailsDTO?> UpdateAsync(string id, UpdateTaskDTO dto, string userId);
        Task<bool> DeleteAsync(string id, string userId);
        Task<bool> CompleteStepAsync(string id, int stepIndex, string userId);
        Task<TaskDetailsDTO?> AddStepAsync(string taskId, CreateStepDTO dto, string userId);
        Task<TaskDetailsDTO?> UpdateStepAsync(string taskId, int stepIndex, UpdateStepDTO dto, string userId);
        Task<bool> DeleteStepAsync(string taskId, int stepIndex, string userId);
    }
}
