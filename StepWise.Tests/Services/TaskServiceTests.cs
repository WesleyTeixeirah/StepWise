using Moq;
using StepWise.API.DTOs;
using StepWise.API.Models;
using StepWise.API.Repositories;
using StepWise.API.Services;
using Xunit;

namespace StepWise.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _taskService = new TaskService(_taskRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_DeveCriarTarefaComSucesso()
        {
            var userId = "user123";

            var dto = new CreateTaskDTO
            {
                Titulo = "Estudar .NET",
                Descricao = "Revisar conteúdos",
                Prazo = "2026-12-31",
                Prioridade = "Alta"
            };

            _taskRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<TaskItem>()))
                .Returns(Task.CompletedTask);

            var result = await _taskService.CreateAsync(dto, userId);

            Assert.NotNull(result);
            Assert.Equal("Estudar .NET", result.Titulo);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDeTarefas()
        {
            var userId = "user123";

            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = "1", UserId = userId, Titulo = "Tarefa 1" },
                new TaskItem { Id = "2", UserId = userId, Titulo = "Tarefa 2" }
            };

            _taskRepositoryMock
                .Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(tasks);

            var result = await _taskService.GetAllAsync(userId);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoUsuarioForDiferente()
        {
            var task = new TaskItem
            {
                Id = "1",
                UserId = "user123",
                Titulo = "Tarefa Teste"
            };

            _taskRepositoryMock
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(task);

            var result = await _taskService.GetByIdAsync("1", "outroUsuario");

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizarTarefaComSucesso()
        {
            var task = new TaskItem
            {
                Id = "1",
                UserId = "user123"
            };

            var dto = new UpdateTaskDTO
            {
                Titulo = "Atualizada",
                Descricao = "Descrição",
                Prazo = "2026-12-31",
                Prioridade = "Alta",
                Tags = new List<string>()
            };

            _taskRepositoryMock
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(task);

            var result = await _taskService.UpdateAsync("1", dto, "user123");

            Assert.NotNull(result);
            Assert.Equal("Atualizada", result!.Titulo);
        }

        [Fact]
        public async Task DeleteAsync_DeveRemoverTarefaComSucesso()
        {
            var task = new TaskItem
            {
                Id = "1",
                UserId = "user123"
            };

            _taskRepositoryMock
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(task);

            _taskRepositoryMock
                .Setup(repo => repo.DeleteAsync("1"))
                .Returns(Task.CompletedTask);

            var result = await _taskService.DeleteAsync("1", "user123");

            Assert.True(result);
        }

        [Fact]
        public async Task AddStepAsync_DeveAdicionarEtapa()
        {
            var task = new TaskItem
            {
                Id = "1",
                UserId = "user123",
                Etapas = new List<Step>()
            };

            var dto = new CreateStepDTO
            {
                Titulo = "Etapa 1",
                Descricao = "Descrição"
            };

            _taskRepositoryMock
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(task);

            var result = await _taskService.AddStepAsync("1", dto, "user123");

            Assert.NotNull(result);
            Assert.Single(result!.Etapas);
        }

        [Fact]
        public async Task CompleteStepAsync_DeveMarcarEtapaComoConcluida()
        {
            var task = new TaskItem
            {
                Id = "1",
                UserId = "user123",
                Etapas = new List<Step>
                {
                    new Step { Titulo = "Etapa 1", Concluida = false }
                }
            };

            _taskRepositoryMock
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(task);

            var result = await _taskService.CompleteStepAsync("1", 0, "user123");

            Assert.True(result!.Etapas[0].Concluida);
        }
    }
}
