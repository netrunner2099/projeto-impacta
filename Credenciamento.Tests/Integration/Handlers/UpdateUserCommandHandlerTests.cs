namespace Credenciamento.Tests.Integration.Handlers;

public class UpdateUserCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<UpdateUserCommandHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IValidator<UpdateUserCommand>> _validatorMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateUserCommandHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IUserRepository>();
        _validatorMock = new Mock<IValidator<UpdateUserCommand>>();
        _handler = new UpdateUserCommandHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenUserExistsAndCommandIsValid()
    {
        // Arrange
        var user = UserFixture.CreateValidWithId();
        var command = new UpdateUserCommand
        {
            UserId = user.UserId,
            Name = "Nome Atualizado",
            Email = "novo@example.com",
            Role = 1
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.GetByIdAsync(user.UserId))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.User>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Domain.Entities.User>(u =>
            u.Name == command.Name && u.Email == command.Email)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrors_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new UpdateUserCommand { UserId = 1, Name = "" };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Name", "Nome é obrigatório.")
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            UserId = 999,
            Name = "Teste",
            Email = "teste@example.com",
            Role = 1
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("não encontrado"));
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }
}
