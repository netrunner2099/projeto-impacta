namespace Credenciamento.Tests.Integration.Handlers;

public class PayTicketCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<PayTicketCommandHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ITicketRepository> _repositoryMock;
    private readonly PayTicketCommandHandler _handler;

    public PayTicketCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<PayTicketCommandHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<ITicketRepository>();
        _handler = new PayTicketCommandHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPayTicket_WhenTicketExists()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };

        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;
        ticketEntity.Price = 299.99m;
        ticketEntity.Status = (byte)TicketStatus.Created;

        var updatedTicket = TicketFixture.CreateValid();
        updatedTicket.TicketId = ticketId;
        updatedTicket.Status = (byte)TicketStatus.Paid;
        updatedTicket.Transaction = Guid.NewGuid().ToString();
        updatedTicket.Auth = "base64string";
        updatedTicket.UpdatedAt = DateTime.UtcNow;

        var expectedResponse = new PayTicketCommandResponse
        {
            TicketId = ticketId,
            Status = (byte)TicketStatus.Paid,
            Transaction = updatedTicket.Transaction,
            Auth = updatedTicket.Auth
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(updatedTicket);

        _mapperMock
            .Setup(x => x.Map<PayTicketCommandResponse>(updatedTicket))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TicketId.Should().Be(ticketId);
        result.Status.Should().Be((byte)TicketStatus.Paid);
        result.Transaction.Should().NotBeNullOrEmpty();
        result.Auth.Should().NotBeNullOrEmpty();

        _repositoryMock.Verify(x => x.GetByIdAsync(ticketId), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Ticket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenTicketDoesNotExist()
    {
        // Arrange
        var ticketId = 999L;
        var command = new PayTicketCommand { TicketId = ticketId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync((Ticket)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(ticketId), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenTicketNotFound()
    {
        // Arrange
        var ticketId = 999L;
        var command = new PayTicketCommand { TicketId = ticketId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync((Ticket)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

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
    public async Task Handle_ShouldSetStatusToPaid_WhenPayingTicket()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };

        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;
        ticketEntity.Status = (byte)TicketStatus.Created;

        var updatedTicket = TicketFixture.CreateValid();
        updatedTicket.Status = (byte)TicketStatus.Paid;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.Is<Ticket>(t => t.Status == (byte)TicketStatus.Paid)))
            .ReturnsAsync(updatedTicket);

        _mapperMock
            .Setup(x => x.Map<PayTicketCommandResponse>(updatedTicket))
            .Returns(new PayTicketCommandResponse { Status = (byte)TicketStatus.Paid });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be((byte)TicketStatus.Paid);
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Ticket>(t => 
            t.Status == (byte)TicketStatus.Paid)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldGenerateTransaction_WhenPayingTicket()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };

        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;

        var updatedTicket = TicketFixture.CreateValid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.Is<Ticket>(t => !string.IsNullOrEmpty(t.Transaction))))
            .ReturnsAsync(updatedTicket);

        _mapperMock
            .Setup(x => x.Map<PayTicketCommandResponse>(updatedTicket))
            .Returns(new PayTicketCommandResponse());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Ticket>(t => 
            !string.IsNullOrEmpty(t.Transaction))), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldGenerateAuth_WhenPayingTicket()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };

        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;
        ticketEntity.Price = 100.00m;

        var updatedTicket = TicketFixture.CreateValid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.Is<Ticket>(t => !string.IsNullOrEmpty(t.Auth))))
            .ReturnsAsync(updatedTicket);

        _mapperMock
            .Setup(x => x.Map<PayTicketCommandResponse>(updatedTicket))
            .Returns(new PayTicketCommandResponse());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Ticket>(t => 
            !string.IsNullOrEmpty(t.Auth))), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetUpdatedAt_WhenPayingTicket()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };

        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;

        var updatedTicket = TicketFixture.CreateValid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.Is<Ticket>(t => t.UpdatedAt != null)))
            .ReturnsAsync(updatedTicket);

        _mapperMock
            .Setup(x => x.Map<PayTicketCommandResponse>(updatedTicket))
            .Returns(new PayTicketCommandResponse());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Ticket>(t => 
            t.UpdatedAt.HasValue && 
            t.UpdatedAt.Value >= DateTime.UtcNow.AddSeconds(-5) && 
            t.UpdatedAt.Value <= DateTime.UtcNow.AddSeconds(5))), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUpdateFails()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };

        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Ticket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenUpdateFails()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };

        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

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
    public async Task Handle_ShouldReturnNull_WhenExceptionIsThrown()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };
        var exception = new Exception("Database error");

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };
        var exception = new Exception("Test exception");

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ThrowsAsync(exception);

        // Act
        await _handler.Handle(command, CancellationToken.None);

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
    public async Task Handle_ShouldUpdateAllRequiredFields_WhenPayingTicket()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };

        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;
        ticketEntity.Price = 150.50m;

        var updatedTicket = TicketFixture.CreateValid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.Is<Ticket>(t =>
                !string.IsNullOrEmpty(t.Transaction) &&
                !string.IsNullOrEmpty(t.Auth) &&
                t.Status == (byte)TicketStatus.Paid &&
                t.UpdatedAt.HasValue)))
            .ReturnsAsync(updatedTicket);

        _mapperMock
            .Setup(x => x.Map<PayTicketCommandResponse>(updatedTicket))
            .Returns(new PayTicketCommandResponse());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<Ticket>(t =>
            !string.IsNullOrEmpty(t.Transaction) &&
            !string.IsNullOrEmpty(t.Auth) &&
            t.Status == (byte)TicketStatus.Paid &&
            t.UpdatedAt.HasValue)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldGenerateUniqueTransaction_ForEachPayment()
    {
        // Arrange
        var ticketId = 1L;
        var command = new PayTicketCommand { TicketId = ticketId };

        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;

        var capturedTransactions = new List<string>();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Ticket>()))
            .Callback<Ticket>(t => capturedTransactions.Add(t.Transaction))
            .ReturnsAsync(ticketEntity);

        _mapperMock
            .Setup(x => x.Map<PayTicketCommandResponse>(It.IsAny<Ticket>()))
            .Returns(new PayTicketCommandResponse());

        // Act
        await _handler.Handle(command, CancellationToken.None);
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedTransactions.Should().HaveCount(2);
        capturedTransactions[0].Should().NotBe(capturedTransactions[1]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_ShouldReturnNull_WhenTicketIdIsInvalid(long invalidTicketId)
    {
        // Arrange
        var command = new PayTicketCommand { TicketId = invalidTicketId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(invalidTicketId))
            .ReturnsAsync((Ticket)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}