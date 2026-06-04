namespace Credenciamento.Tests.Integration.Handlers;

public class ListTicketAdminQueryHandlerTests : TestBase
{
    private readonly Mock<ILogger<ListTicketAdminQueryHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ITicketRepository> _repositoryMock;
    private readonly ListTicketAdminQueryHandler _handler;

    public ListTicketAdminQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ListTicketAdminQueryHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<ITicketRepository>();
        _handler = new ListTicketAdminQueryHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllTickets_WhenNoFilterIsApplied()
    {
        // Arrange
        var tickets = new List<Domain.Entities.Ticket>
        {
            TicketFixture.CreateValid(),
            TicketFixture.CreateValid(),
            TicketFixture.CreateValid()
        };

        var expectedModels = tickets.Select(t => new TicketModel { TicketId = t.TicketId, Price = t.Price }).ToList();
        var query = new ListTicketAdminQuery();

        _repositoryMock.Setup(x => x.ListAllAsync()).ReturnsAsync(tickets);
        _mapperMock.Setup(x => x.Map<IEnumerable<TicketModel>>(It.IsAny<IEnumerable<Domain.Entities.Ticket>>()))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Tickets.Should().HaveCount(3);
        _repositoryMock.Verify(x => x.ListAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTicketsByEvent_WhenEventIdFilterIsApplied()
    {
        // Arrange
        var eventId = 42L;
        var tickets = new List<Domain.Entities.Ticket>
        {
            TicketFixture.CreateWithPersonAndEvent(1, eventId),
            TicketFixture.CreateWithPersonAndEvent(2, eventId)
        };

        var expectedModels = tickets.Select(t => new TicketModel { TicketId = t.TicketId }).ToList();
        var query = new ListTicketAdminQuery { EventId = eventId };

        _repositoryMock.Setup(x => x.ListByEventIdAsync(eventId)).ReturnsAsync(tickets);
        _mapperMock.Setup(x => x.Map<IEnumerable<TicketModel>>(It.IsAny<IEnumerable<Domain.Entities.Ticket>>()))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Tickets.Should().HaveCount(2);
        _repositoryMock.Verify(x => x.ListByEventIdAsync(eventId), Times.Once);
        _repositoryMock.Verify(x => x.ListAllAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStatus_WhenStatusFilterIsApplied()
    {
        // Arrange
        var paidStatus = (byte)TicketStatus.Paid;
        var t1 = TicketFixture.CreateValid(); t1.Status = paidStatus;
        var t2 = TicketFixture.CreateValid(); t2.Status = paidStatus;
        var t3 = TicketFixture.CreateValid(); t3.Status = (byte)TicketStatus.Canceled;
        var tickets = new List<Domain.Entities.Ticket> { t1, t2, t3 };

        var filteredTickets = tickets.Where(t => t.Status == paidStatus).ToList();
        var expectedModels = filteredTickets.Select(t => new TicketModel { TicketId = t.TicketId }).ToList();
        var query = new ListTicketAdminQuery { Status = paidStatus };

        _repositoryMock.Setup(x => x.ListAllAsync()).ReturnsAsync(tickets);
        _mapperMock.Setup(x => x.Map<IEnumerable<TicketModel>>(It.IsAny<IEnumerable<Domain.Entities.Ticket>>()))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Tickets.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldFilterByPayment_WhenPaymentFilterIsApplied()
    {
        // Arrange
        var pixPayment = (byte)TicketPayment.Pix;
        var p1 = TicketFixture.CreateValid(); p1.Payment = pixPayment;
        var p2 = TicketFixture.CreateValid(); p2.Payment = pixPayment;
        var p3 = TicketFixture.CreateValid(); p3.Payment = (byte)TicketPayment.CreditCard;
        var tickets = new List<Domain.Entities.Ticket> { p1, p2, p3 };

        var filteredTickets = tickets.Where(t => t.Payment == pixPayment).ToList();
        var expectedModels = filteredTickets.Select(t => new TicketModel { TicketId = t.TicketId }).ToList();
        var query = new ListTicketAdminQuery { Payment = pixPayment };

        _repositoryMock.Setup(x => x.ListAllAsync()).ReturnsAsync(tickets);
        _mapperMock.Setup(x => x.Map<IEnumerable<TicketModel>>(It.IsAny<IEnumerable<Domain.Entities.Ticket>>()))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Tickets.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoTicketsExist()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ListAllAsync()).ReturnsAsync(new List<Domain.Entities.Ticket>());
        _mapperMock.Setup(x => x.Map<IEnumerable<TicketModel>>(It.IsAny<IEnumerable<Domain.Entities.Ticket>>()))
            .Returns(new List<TicketModel>());

        var query = new ListTicketAdminQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Tickets.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorMessage_WhenRepositoryThrows()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ListAllAsync()).ThrowsAsync(new Exception("Database error"));
        var query = new ListTicketAdminQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Erro ao listar tickets");
    }
}
