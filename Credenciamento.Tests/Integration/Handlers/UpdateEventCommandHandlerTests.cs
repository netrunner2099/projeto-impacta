namespace Credenciamento.Tests.Integration.Handlers;

public class UpdateEventCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<UpdateEventCommandHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEventRepository> _repositoryMock;
    private readonly Mock<IValidator<UpdateEventCommand>> _validatorMock;
    private readonly UpdateEventCommandHandler _handler;

    public UpdateEventCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateEventCommandHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IEventRepository>();
        _validatorMock = new Mock<IValidator<UpdateEventCommand>>();
        _handler = new UpdateEventCommandHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateEvent_WhenEventExistsAndCommandIsValid()
    {
        // Arrange
        var eventId = 10L;
        var entity = EventFixture.CreateValidWithId();
        entity.EventId = eventId;

        var command = new UpdateEventCommand
        {
            EventId = eventId,
            Name = "Novo Nome do Evento",
            Description = "Nova descrição",
            Local = "Rio de Janeiro - RJ",
            Begin = DateTime.UtcNow.AddDays(15),
            End = DateTime.UtcNow.AddDays(16),
            Price = 200m
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ReturnsAsync(entity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Event>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Errors.Should().BeNullOrEmpty();
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Domain.Entities.Event>(e => e.Name == command.Name && e.Price == command.Price)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrors_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new UpdateEventCommand { EventId = 1, Name = "" };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Name", "Nome é obrigatório.")
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEventNotFound()
    {
        // Arrange
        var command = new UpdateEventCommand
        {
            EventId = 999,
            Name = "Evento",
            Begin = DateTime.UtcNow.AddDays(5),
            End = DateTime.UtcNow.AddDays(6),
            Price = 100m
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Domain.Entities.Event?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("não encontrado");
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Event>()), Times.Never);
    }
}
