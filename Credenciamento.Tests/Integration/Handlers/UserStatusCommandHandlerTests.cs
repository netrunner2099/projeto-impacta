using Credenciamento.Application.Commands.User;

namespace Credenciamento.Tests.Integration.Handlers;

public class ActivateUserCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<ActivateUserCommandHandler>> _loggerMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly ActivateUserCommandHandler _handler;

    public ActivateUserCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ActivateUserCommandHandler>>();
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new ActivateUserCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldActivateUser_WhenUserExists()
    {
        // Arrange
        var user = UserFixture.CreateValidWithId();
        user.Status = (byte)Domain.Enums.UserStatus.Inactive;
        var command = new ActivateUserCommand { UserId = user.UserId };

        _repositoryMock.Setup(x => x.GetByIdAsync(user.UserId)).ReturnsAsync(user);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.User>())).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("reativado");
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Domain.Entities.User>(u =>
            u.Status == (byte)Domain.Enums.UserStatus.Active)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Domain.Entities.User?)null);
        var command = new ActivateUserCommand { UserId = 999 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("não encontrado");
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }
}

public class InactivateUserCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<InactivateUserCommandHandler>> _loggerMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly InactivateUserCommandHandler _handler;

    public InactivateUserCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<InactivateUserCommandHandler>>();
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new InactivateUserCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldInactivateUser_WhenUserExists()
    {
        // Arrange
        var user = UserFixture.CreateValidWithId();
        user.Status = (byte)Domain.Enums.UserStatus.Active;
        var command = new InactivateUserCommand { UserId = user.UserId };

        _repositoryMock.Setup(x => x.GetByIdAsync(user.UserId)).ReturnsAsync(user);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.User>())).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("inativado");
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Domain.Entities.User>(u =>
            u.Status == (byte)Domain.Enums.UserStatus.Inactive)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Domain.Entities.User?)null);
        var command = new InactivateUserCommand { UserId = 999 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("não encontrado");
    }
}

public class DeleteUserCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<DeleteUserCommandHandler>> _loggerMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DeleteUserCommandHandler>>();
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new DeleteUserCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var userId = 5L;
        _repositoryMock.Setup(x => x.DeleteAsync(userId)).ReturnsAsync(true);
        var command = new DeleteUserCommand { UserId = userId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("excluído");
        _repositoryMock.Verify(x => x.DeleteAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.DeleteAsync(It.IsAny<long>())).ReturnsAsync(false);
        var command = new DeleteUserCommand { UserId = 999 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("não encontrado");
    }
}
