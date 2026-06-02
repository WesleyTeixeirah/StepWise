using StepWise.API.DTOs;
using StepWise.API.Models;
using StepWise.API.Repositories;

namespace StepWise.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<TaskDetailsDTO>> GetAllAsync(string userId)
        {
            var tasks = await _taskRepository.GetByUserIdAsync(userId);
            return tasks.Select(MapToDTO);
        }

        public async Task<TaskDetailsDTO?> GetByIdAsync(string id, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId)
                return null;

            return MapToDTO(task);
        }

        public async Task<TaskDetailsDTO> CreateAsync(CreateTaskDTO dto, string userId)
        {
            var task = new TaskItem
            {
                UserId = userId,
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Prazo = dto.Prazo,
                Prioridade = dto.Prioridade,
                Tags = dto.Tags,
                Etapas = dto.Etapas.Select(e => new Step
                {
                    Titulo = e.Titulo,
                    Descricao = e.Descricao,
                    Concluida = false
                }).ToList()
            };

            await _taskRepository.CreateAsync(task);
            return MapToDTO(task);
        }

        public async Task<TaskDetailsDTO?> UpdateAsync(string id, UpdateTaskDTO dto, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId)
                return null;

            task.Titulo = dto.Titulo;
            task.Descricao = dto.Descricao;
            task.Prazo = dto.Prazo;
            task.Prioridade = dto.Prioridade;
            task.Tags = dto.Tags;

            await _taskRepository.UpdateAsync(id, task);
            return MapToDTO(task);
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId)
                return false;

            await _taskRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> CompleteStepAsync(string taskId, int stepIndex, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.UserId != userId)
                return false;

            if (stepIndex < 0 || stepIndex >= task.Etapas.Count)
                return false;

            task.Etapas[stepIndex].Concluida = true;
            await _taskRepository.UpdateAsync(taskId, task);

            return true;
        }

        public async Task<TaskDetailsDTO?> AddStepAsync(string taskId, CreateStepDTO dto, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.UserId != userId)
                return null;

            task.Etapas.Add(new Step
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Concluida = false
            });

            await _taskRepository.UpdateAsync(taskId, task);
            return MapToDTO(task);
        }

        public async Task<TaskDetailsDTO?> UpdateStepAsync(string taskId, int stepIndex, UpdateStepDTO dto, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.UserId != userId)
                return null;

            if (stepIndex < 0 || stepIndex >= task.Etapas.Count)
                return null;

            task.Etapas[stepIndex].Titulo = dto.Titulo;
            task.Etapas[stepIndex].Descricao = dto.Descricao;   

            await _taskRepository.UpdateAsync(taskId, task);
            return MapToDTO(task);
        }

        public async Task<bool> DeleteStepAsync(string taskId, int stepIndex, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.UserId != userId)
                return false;

            if (stepIndex < 0 || stepIndex >= task.Etapas.Count)
                return false;

            task.Etapas.RemoveAt(stepIndex);
            await _taskRepository.UpdateAsync(taskId, task);

            return true;
        }

        // 🔥 Mapper
        private TaskDetailsDTO MapToDTO(TaskItem task)
        {
            return new TaskDetailsDTO
            {
                Id = task.Id ?? "",
                Titulo = task.Titulo,
                Descricao = task.Descricao,
                Prazo = task.Prazo,
                Prioridade = task.Prioridade,
                Tags = task.Tags ?? new List<string>(),
                Etapas = task.Etapas.Select(e => new StepDetailsDTO
                {
                    Titulo = e.Titulo,
                    Descricao = e.Descricao,
                    Concluida = e.Concluida
                }).ToList()
            };
        }
    }
}
