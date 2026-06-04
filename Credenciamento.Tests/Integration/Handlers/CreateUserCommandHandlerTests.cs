namespace Credenciamento.Tests.Integration.Handlers;

public class CreateUserCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<CreateUserCommandHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IValidator<CreateUserCommand>> _validatorMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateUserCommandHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IUserRepository>();
        _validatorMock = new Mock<IValidator<CreateUserCommand>>();
        _handler = new CreateUserCommandHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Name = "João Silva",
            Email = "joao@example.com",
            Password = "Senha@123",
            PasswordConfirm = "Senha@123",
            Role = 1
        };

        var entity = UserFixture.CreateValid();
        entity.Email = command.Email;

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.EmailExistsAsync(command.Email))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(x => x.Map<Domain.Entities.User>(command))
            .Returns(entity);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.User>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrors_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateUserCommand { Name = "", Email = "invalid" };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Name", "Nome é obrigatório."),
                new("Email", "E-mail inválido.")
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Name = "Maria",
            Email = "maria@example.com",
            Password = "Senha@123",
            PasswordConfirm = "Senha@123",
            Role = 1
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.EmailExistsAsync(command.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("e-mail"));
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrows()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Name = "Pedro",
            Email = "pedro@example.com",
            Password = "Senha@123",
            PasswordConfirm = "Senha@123",
            Role = 1
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.EmailExistsAsync(command.Email))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(x => x.Map<Domain.Entities.User>(command))
            .Returns(UserFixture.CreateValid());

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.User>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Erro ao cadastrar usuário");
    }
}
