namespace Credenciamento.Tests.Integration.Handlers;

public class ListFutureEventQueryHandlerTests : TestBase
{
    private readonly Mock<ILogger<ListFutureEventQueryHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEventRepository> _repositoryMock;
    private readonly ListFutureEventQueryHandler _handler;

    public ListFutureEventQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ListFutureEventQueryHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IEventRepository>();
        _handler = new ListFutureEventQueryHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFutureEvents_WhenEventsExist()
    {
        // Arrange
        var futureEvents = new List<Event>
        {
            EventFixture.CreateValid(),
            EventFixture.CreateValid(),
            EventFixture.CreateValid()
        };

        var expectedModels = new List<EventModel>
        {
            new EventModel { EventId = 1, Name = "Event 1", Price = 100m },
            new EventModel { EventId = 2, Name = "Event 2", Price = 200m },
            new EventModel { EventId = 3, Name = "Event 3", Price = 300m }
        };

        var query = new ListFutureEventQuery();

        _repositoryMock
            .Setup(x => x.ListFutureAsync())
            .ReturnsAsync(futureEvents);

        _mapperMock
            .Setup(x => x.Map<List<EventModel>>(futureEvents))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Events.Should().NotBeNull();
        result.Events.Should().HaveCount(3);
        result.Events.Should().BeEquivalentTo(expectedModels);

        _repositoryMock.Verify(x => x.ListFutureAsync(), Times.Once);
        _mapperMock.Verify(x => x.Map<List<EventModel>>(futureEvents), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoFutureEventsExist()
    {
        // Arrange
        var emptyEvents = new List<Event>();
        var emptyModels = new List<EventModel>();
        var query = new ListFutureEventQuery();

        _repositoryMock
            .Setup(x => x.ListFutureAsync())
            .ReturnsAsync(emptyEvents);

        _mapperMock
            .Setup(x => x.Map<List<EventModel>>(emptyEvents))
            .Returns(emptyModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Events.Should().NotBeNull();
        result.Events.Should().BeEmpty();

        _repositoryMock.Verify(x => x.ListFutureAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenExceptionIsThrown()
    {
        // Arrange
        var query = new ListFutureEventQuery();
        var exceptionMessage = "Database connection failed";

        _repositoryMock
            .Setup(x => x.ListFutureAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Events.Should().BeNull();

        _repositoryMock.Verify(x => x.ListFutureAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var query = new ListFutureEventQuery();
        var exception = new Exception("Test exception");

        _repositoryMock
            .Setup(x => x.ListFutureAsync())
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

    [Fact]
    public async Task Handle_ShouldReturnOnlyFutureEvents_WhenRepositoryReturnsFiltered()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(30);
        var futureEvents = new List<Event>
        {
            EventFixture.CreateValid(),
            EventFixture.CreateValid()
        };

        futureEvents[0].Begin = futureDate;
        futureEvents[1].Begin = futureDate.AddDays(10);

        var expectedModels = new List<EventModel>
        {
            new EventModel { EventId = 1, Name = "Future Event 1", Begin = futureDate },
            new EventModel { EventId = 2, Name = "Future Event 2", Begin = futureDate.AddDays(10) }
        };

        var query = new ListFutureEventQuery();

        _repositoryMock
            .Setup(x => x.ListFutureAsync())
            .ReturnsAsync(futureEvents);

        _mapperMock
            .Setup(x => x.Map<List<EventModel>>(futureEvents))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Events.Should().HaveCount(2);
        result.Events.Should().OnlyContain(e => e.Begin >= DateTime.Now);
    }

    [Fact]
    public async Task Handle_ShouldMapAllProperties_WhenEventsHaveCompleteData()
    {
        // Arrange
        var event1 = EventFixture.CreateValid();
        event1.EventId = 1;
        event1.Name = "Tech Conference 2026";
        event1.Description = "Annual technology conference";
        event1.Local = "Convention Center";
        event1.Price = 299.99m;

        var futureEvents = new List<Event> { event1 };

        var expectedModel = new EventModel
        {
            EventId = event1.EventId,
            Name = event1.Name,
            Description = event1.Description,
            Local = event1.Local,
            Begin = event1.Begin,
            End = event1.End,
            Price = event1.Price,
            Status = event1.Status,
            CreatedAt = event1.CreatedAt
        };

        var expectedModels = new List<EventModel> { expectedModel };
        var query = new ListFutureEventQuery();

        _repositoryMock
            .Setup(x => x.ListFutureAsync())
            .ReturnsAsync(futureEvents);

        _mapperMock
            .Setup(x => x.Map<List<EventModel>>(futureEvents))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Events.Should().HaveCount(1);

        var firstEvent = result.Events.First();
        firstEvent.Should().BeEquivalentTo(expectedModel);
    }

    [Fact]
    public async Task Handle_ShouldHandleCancellationToken_Correctly()
    {
        // Arrange
        var futureEvents = new List<Event> { EventFixture.CreateValid() };
        var expectedModels = new List<EventModel> { new EventModel { EventId = 1 } };
        var query = new ListFutureEventQuery();
        var cancellationToken = new CancellationToken();

        _repositoryMock
            .Setup(x => x.ListFutureAsync())
            .ReturnsAsync(futureEvents);

        _mapperMock
            .Setup(x => x.Map<List<EventModel>>(futureEvents))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Events.Should().HaveCount(1);
    }
}