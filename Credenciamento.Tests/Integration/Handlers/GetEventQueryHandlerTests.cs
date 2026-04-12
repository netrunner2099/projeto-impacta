namespace Credenciamento.Tests.Integration.Handlers;

public class GetEventQueryHandlerTests : TestBase
{
    private readonly Mock<ILogger<GetEventQueryHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEventRepository> _repositoryMock;
    private readonly GetEventQueryHandler _handler;

    public GetEventQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetEventQueryHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IEventRepository>();
        _handler = new GetEventQueryHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEvent_WhenEventExists()
    {
        // Arrange
        var eventId = 1L;
        var eventEntity = EventFixture.CreateValid();
        eventEntity.EventId = eventId;

        var expectedResponse = new GetEventQueryResponse
        {
            EventId = eventEntity.EventId,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Local = eventEntity.Local,
            Begin = eventEntity.Begin,
            End = eventEntity.End,
            Price = eventEntity.Price,
            Status = eventEntity.Status
        };

        var query = new GetEventQuery { EventId = eventId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ReturnsAsync(eventEntity);

        _mapperMock
            .Setup(x => x.Map<GetEventQueryResponse>(eventEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.EventId.Should().Be(eventId);
        result.Name.Should().Be(eventEntity.Name);
        result.Description.Should().Be(eventEntity.Description);
        result.Price.Should().Be(eventEntity.Price);

        _repositoryMock.Verify(x => x.GetByIdAsync(eventId), Times.Once);
        _mapperMock.Verify(x => x.Map<GetEventQueryResponse>(eventEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = 999L;
        var query = new GetEventQuery { EventId = eventId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ReturnsAsync((Event)null);

        _mapperMock
            .Setup(x => x.Map<GetEventQueryResponse>(null))
            .Returns((GetEventQueryResponse)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(eventId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenExceptionIsThrown()
    {
        // Arrange
        var eventId = 1L;
        var query = new GetEventQuery { EventId = eventId };
        var exceptionMessage = "Database connection failed";

        _repositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(eventId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var eventId = 1L;
        var query = new GetEventQuery { EventId = eventId };
        var exception = new Exception("Test exception");

        _repositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ThrowsAsync(exception);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task Handle_ShouldReturnNull_WhenEventIdIsInvalid(long invalidEventId)
    {
        // Arrange
        var query = new GetEventQuery { EventId = invalidEventId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(invalidEventId))
            .ReturnsAsync((Event)null);

        _mapperMock
            .Setup(x => x.Map<GetEventQueryResponse>(null))
            .Returns((GetEventQueryResponse)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(invalidEventId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapCorrectly_WhenEventHasAllProperties()
    {
        // Arrange
        var eventId = 1L;
        var eventEntity = EventFixture.CreateValid();
        eventEntity.EventId = eventId;
        eventEntity.Name = "Tech Conference 2026";
        eventEntity.Description = "Annual technology conference";
        eventEntity.Local = "Convention Center";
        eventEntity.Price = 299.99m;

        var expectedResponse = new GetEventQueryResponse
        {
            EventId = eventEntity.EventId,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Local = eventEntity.Local,
            Begin = eventEntity.Begin,
            End = eventEntity.End,
            Price = eventEntity.Price,
            Status = eventEntity.Status,
            CreatedAt = eventEntity.CreatedAt
        };

        var query = new GetEventQuery { EventId = eventId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ReturnsAsync(eventEntity);

        _mapperMock
            .Setup(x => x.Map<GetEventQueryResponse>(eventEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResponse);
        _mapperMock.Verify(x => x.Map<GetEventQueryResponse>(It.Is<Event>(e => 
            e.EventId == eventId && 
            e.Name == "Tech Conference 2026")), Times.Once);
    }
}