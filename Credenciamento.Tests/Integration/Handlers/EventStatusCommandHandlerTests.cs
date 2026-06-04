using Credenciamento.Application.Commands.Event;

namespace Credenciamento.Tests.Integration.Handlers;

public class ActivateEventCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<ActivateEventCommandHandler>> _loggerMock;
    private readonly Mock<IEventRepository> _repositoryMock;
    private readonly ActivateEventCommandHandler _handler;

    public ActivateEventCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ActivateEventCommandHandler>>();
        _repositoryMock = new Mock<IEventRepository>();
        _handler = new ActivateEventCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldActivateEvent_WhenEventExists()
    {
        // Arrange
        var entity = EventFixture.CreateValidWithId();
        entity.Status = (byte)Domain.Enums.EventStatus.Inactive;
        var command = new ActivateEventCommand { EventId = entity.EventId };

        _repositoryMock.Setup(x => x.GetByIdAsync(entity.EventId)).ReturnsAsync(entity);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Event>())).ReturnsAsync(entity);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Domain.Entities.Event>(e =>
            e.Status == (byte)Domain.Enums.EventStatus.Active)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEventNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Domain.Entities.Event?)null);
        var command = new ActivateEventCommand { EventId = 999 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("não encontrado");
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Event>()), Times.Never);
    }
}

public class InactivateEventCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<InactivateEventCommandHandler>> _loggerMock;
    private readonly Mock<IEventRepository> _repositoryMock;
    private readonly InactivateEventCommandHandler _handler;

    public InactivateEventCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<InactivateEventCommandHandler>>();
        _repositoryMock = new Mock<IEventRepository>();
        _handler = new InactivateEventCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldInactivateEvent_WhenEventExists()
    {
        // Arrange
        var entity = EventFixture.CreateValidWithId();
        entity.Status = (byte)Domain.Enums.EventStatus.Active;
        var command = new InactivateEventCommand { EventId = entity.EventId };

        _repositoryMock.Setup(x => x.GetByIdAsync(entity.EventId)).ReturnsAsync(entity);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Event>())).ReturnsAsync(entity);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Domain.Entities.Event>(e =>
            e.Status == (byte)Domain.Enums.EventStatus.Inactive)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEventNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Domain.Entities.Event?)null);
        var command = new InactivateEventCommand { EventId = 999 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("não encontrado");
    }
}

public class DeleteEventCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<DeleteEventCommandHandler>> _loggerMock;
    private readonly Mock<IEventRepository> _repositoryMock;
    private readonly DeleteEventCommandHandler _handler;

    public DeleteEventCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DeleteEventCommandHandler>>();
        _repositoryMock = new Mock<IEventRepository>();
        _handler = new DeleteEventCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldMarkEventAsDeleted_WhenEventExists()
    {
        // Arrange
        var entity = EventFixture.CreateValidWithId();
        var command = new DeleteEventCommand { EventId = entity.EventId };

        _repositoryMock.Setup(x => x.GetByIdAsync(entity.EventId)).ReturnsAsync(entity);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Event>())).ReturnsAsync(entity);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Domain.Entities.Event>(e =>
            e.Status == (byte)Domain.Enums.EventStatus.Deleted)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEventNotFound()
    {
        // Arrange
        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Domain.Entities.Event?)null);
        var command = new DeleteEventCommand { EventId = 999 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("não encontrado");
    }
}
