namespace Credenciamento.Tests.Integration.Handlers;

public class CreateEventCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<CreateEventCommandHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEventRepository> _repositoryMock;
    private readonly Mock<IValidator<CreateEventCommand>> _validatorMock;
    private readonly CreateEventCommandHandler _handler;

    public CreateEventCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateEventCommandHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IEventRepository>();
        _validatorMock = new Mock<IValidator<CreateEventCommand>>();
        _handler = new CreateEventCommandHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateEvent_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateEventCommand
        {
            Name = "Evento Corporativo 2026",
            Description = "Descrição do evento",
            Local = "São Paulo - SP",
            Begin = DateTime.UtcNow.AddDays(30),
            End = DateTime.UtcNow.AddDays(31),
            Price = 150m
        };

        var entity = EventFixture.CreateValid();
        entity.Name = command.Name;

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<Domain.Entities.Event>(command))
            .Returns(entity);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Event>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Event>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrors_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateEventCommand { Name = "" };

        var failures = new List<ValidationFailure>
        {
            new("Name", "Nome é obrigatório."),
            new("Begin", "Data de início é obrigatória.")
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Event>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrows()
    {
        // Arrange
        var command = new CreateEventCommand
        {
            Name = "Evento Teste",
            Begin = DateTime.UtcNow.AddDays(10),
            End = DateTime.UtcNow.AddDays(11),
            Price = 100m
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<Domain.Entities.Event>(command))
            .Returns(EventFixture.CreateValid());

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Event>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Erro ao cadastrar evento");
    }
}
