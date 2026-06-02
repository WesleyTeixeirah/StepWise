using Microsoft.AspNetCore.Mvc;
using Moq;
using StepWise.API.Controllers;
using StepWise.API.DTOs;
using StepWise.API.Services;
using Xunit;

namespace StepWise.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Correção: não passar parâmetros para interfaces
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_DeveRetornarOk_QuandoCadastroForValido()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Nome = "Virgilio Zenith",
                Email = "teste@email.com",
                Password = "123456"
            };

            var response = new AuthResponseDTO
            {
                Token = "token_teste",
                Nome = dto.Nome,
                Email = dto.Email
            };

            _authServiceMock
                .Setup(service => service.RegisterAsync(dto))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.Equal(dto.Email, value.Email);
        }

        [Fact]
        public async Task Register_DeveRetornarBadRequest_QuandoEmailJaExistir()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Nome = "Virgilio Zenith",
                Email = "teste@email.com",
                Password = "123456"
            };

            _authServiceMock
                .Setup(service => service.RegisterAsync(dto))
                .ReturnsAsync((AuthResponseDTO?)null);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_DeveRetornarOk_QuandoCredenciaisForemValidas()
        {
            // Arrange
            var dto = new LoginDTO
            {
                Email = "teste@email.com",
                Password = "123456"
            };

            var response = new AuthResponseDTO
            {
                Token = "token_teste",
                Nome = "Virgilio Zenith",
                Email = dto.Email
            };

            _authServiceMock
                .Setup(service => service.LoginAsync(dto))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.Equal(dto.Email, value.Email);
        }

        [Fact]
        public async Task Login_DeveRetornarUnauthorized_QuandoCredenciaisInvalidas()
        {
            // Arrange
            var dto = new LoginDTO
            {
                Email = "teste@email.com",
                Password = "senhaErrada"
            };

            _authServiceMock
                .Setup(service => service.LoginAsync(dto))
                .ReturnsAsync((AuthResponseDTO?)null);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
