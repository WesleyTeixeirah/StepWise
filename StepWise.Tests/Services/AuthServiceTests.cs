using Moq;
using StepWise.API.DTOs;
using StepWise.API.Models;
using StepWise.API.Repositories;
using StepWise.API.Services;
using StepWise.Tests.Helpers;
using Xunit;

namespace StepWise.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            var configuration = TestConfiguration.GetConfiguration();
            _authService = new AuthService(_userRepositoryMock.Object, configuration);
        }

        [Fact]
        public async Task RegisterAsync_DeveCriarUsuarioComSucesso()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Nome = "Virgilio Zenith",
                Email = "teste@email.com",
                Password = "123456"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            _userRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Email, result!.Email);
            Assert.Equal(dto.Nome, result.Nome);
            Assert.False(string.IsNullOrEmpty(result.Token));
        }

        [Fact]
        public async Task RegisterAsync_DeveRetornarNull_QuandoEmailJaExistir()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Nome = "Virgilio Zenith",
                Email = "teste@email.com",
                Password = "123456"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(dto.Email))
                .ReturnsAsync(new User());

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarToken_QuandoCredenciaisForemValidas()
        {
            // Arrange
            var senha = "123456";
            var hash = BCrypt.Net.BCrypt.HashPassword(senha);

            var user = new User
            {
                Id = "507f1f77bcf86cd799439011",
                Nome = "Virgilio Zenith",
                Email = "teste@email.com",
                SenhaHash = hash
            };

            var dto = new LoginDTO
            {
                Email = user.Email,
                Password = senha
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result!.Email);
            Assert.False(string.IsNullOrEmpty(result.Token));
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarNull_QuandoCredenciaisForemInvalidas()
        {
            // Arrange
            var user = new User
            {
                Email = "teste@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456")
            };

            var dto = new LoginDTO
            {
                Email = user.Email,
                Password = "senhaErrada"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarNull_QuandoUsuarioNaoExistir()
        {
            // Arrange
            var dto = new LoginDTO
            {
                Email = "inexistente@email.com",
                Password = "123456"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.Null(result);
        }
    }
}
