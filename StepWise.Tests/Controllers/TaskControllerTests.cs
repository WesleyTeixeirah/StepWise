using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StepWise.API.Controllers;
using StepWise.API.DTOs;
using StepWise.API.Services;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace StepWise.Tests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _controller = new TaskController(_taskServiceMock.Object);

            // Simula usuário autenticado
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetAll_DeveRetornarListaDeTarefas()
        {
            // Arrange
            var tasks = new List<TaskDetailsDTO>
            {
                new TaskDetailsDTO
                {
                    Id = "1",
                    Titulo = "Tarefa 1",
                    Descricao = "Descrição",
                    Prazo = "2026-12-31",
                    Prioridade = "Alta",
                    Tags = new List<string>(),
                    Etapas = new List<StepDetailsDTO>()
                }
            };

            _taskServiceMock
                .Setup(service => service.GetAllAsync("user123"))
                .ReturnsAsync(tasks.AsEnumerable());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<TaskDetailsDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoTarefaNaoExistir()
        {
            // Arrange
            _taskServiceMock
                .Setup(service => service.GetByIdAsync("1", "user123"))
                .ReturnsAsync((TaskDetailsDTO?)null);

            // Act
            var result = await _controller.GetById("1");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var dto = new CreateTaskDTO
            {
                Titulo = "Nova Tarefa",
                Descricao = "Descrição",
                Prazo = "2026-12-31",
                Prioridade = "Alta"
            };

            var createdTask = new TaskDetailsDTO
            {
                Id = "1",
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Prazo = dto.Prazo,
                Prioridade = dto.Prioridade,
                Tags = new List<string>(),
                Etapas = new List<StepDetailsDTO>()
            };

            _taskServiceMock
                .Setup(service => service.CreateAsync(dto, "user123"))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Delete_DeveRetornarOk_QuandoSucesso()
        {
            // Arrange
            _taskServiceMock
                .Setup(service => service.DeleteAsync("1", "user123"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete("1");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_DeveRetornarNotFound_QuandoFalhar()
        {
            // Arrange
            _taskServiceMock
                .Setup(service => service.DeleteAsync("1", "user123"))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete("1");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CompleteStep_DeveRetornarOk()
        {
            // Arrange
            _taskServiceMock
                .Setup(service => service.CompleteStepAsync("1", 0, "user123"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CompleteStep("1", 0);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
