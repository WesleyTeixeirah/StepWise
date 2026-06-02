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
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskService.GetAllAsync(GetUserId());
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var task = await _taskService.GetByIdAsync(id, GetUserId());
            if (task == null)
                return NotFound(new { message = "Tarefa não encontrada." });
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDTO dto)
        {
            var task = await _taskService.CreateAsync(dto, GetUserId());
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTaskDTO dto)
        {
            var task = await _taskService.UpdateAsync(id, dto, GetUserId());
            if (task == null)
                return NotFound(new { message = "Tarefa não encontrada." });
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _taskService.DeleteAsync(id, GetUserId());
            if (!result)
                return NotFound(new { message = "Tarefa não encontrada." });
            return Ok(new { message = "Tarefa excluída com sucesso." });
        }

        [HttpPost("{id}/steps")]
        public async Task<IActionResult> AddStep(string id, [FromBody] CreateStepDTO dto)
        {
            var task = await _taskService.AddStepAsync(id, dto, GetUserId());
            if (task == null)
                return NotFound(new { message = "Tarefa não encontrada." });
            return Ok(task);
        }

        [HttpPut("{id}/steps/{stepIndex}")]
        public async Task<IActionResult> UpdateStep(string id, int stepIndex, [FromBody] UpdateStepDTO dto)
        {
            var task = await _taskService.UpdateStepAsync(id, stepIndex, dto, GetUserId());
            if (task == null)
                return NotFound(new { message = "Tarefa ou etapa não encontrada." });
            return Ok(task);
        }

        [HttpPatch("{id}/steps/{stepIndex}/complete")]
        public async Task<IActionResult> CompleteStep(string id, int stepIndex)
        {
            var result = await _taskService.CompleteStepAsync(id, stepIndex, GetUserId());
            if (!result)
                return NotFound(new { message = "Tarefa ou etapa não encontrada." });
            return Ok(new { message = "Etapa concluída com sucesso." });

        }

        [HttpDelete("{id}/steps/{stepIndex}")]
        public async Task<IActionResult> DeleteStep(string id, int stepIndex)
        {
            var result = await _taskService.DeleteStepAsync(id, stepIndex, GetUserId());
            if (!result)
                return NotFound(new { message = "Tarefa ou etapa não encontrada." });
            return Ok(new { message = "Etapa excluída com sucesso." });
        }
    }
}