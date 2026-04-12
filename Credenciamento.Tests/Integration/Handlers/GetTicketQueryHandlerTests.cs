namespace Credenciamento.Tests.Integration.Handlers;

public class GetTicketQueryHandlerTests : TestBase
{
    private readonly Mock<ILogger<GetTicketQueryHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ITicketRepository> _repositoryMock;
    private readonly Mock<IQrCodeClient> _qrCodeClientMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly GetTicketQueryHandler _handler;
    private const string BaseUrl = "https://example.com";

    public GetTicketQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetTicketQueryHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<ITicketRepository>();
        _qrCodeClientMock = new Mock<IQrCodeClient>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock
            .Setup(x => x["Environment:BaseUrl"])
            .Returns(BaseUrl);

        _handler = new GetTicketQueryHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object,
            _qrCodeClientMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTicket_WhenTicketExistsByTicketId()
    {
        // Arrange
        var ticketId = 1L;
        var transaction = "TXN123456";
        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;
        ticketEntity.Transaction = transaction;

        var qrCodeResponse = new QRCodeResponse
        {
            Base64 = "base64string",
            MimeType = "image/png",
            DataUrl = "data:image/png;base64,..."
        };

        var expectedResponse = new GetTicketQueryResponse
        {
            TicketId = ticketId,
            Transaction = transaction,
            EventId = ticketEntity.EventId,
            PersonId = ticketEntity.PersonId,
            Price = ticketEntity.Price
        };

        var query = new GetTicketQuery { TicketId = ticketId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _qrCodeClientMock
            .Setup(x => x.GenerateAsync(It.Is<QrCodeRequest>(r => 
                r.Data == $"{BaseUrl}/ticket/index/{transaction}")))
            .ReturnsAsync(qrCodeResponse);

        _mapperMock
            .Setup(x => x.Map<GetTicketQueryResponse>(ticketEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TicketId.Should().Be(ticketId);
        result.Transaction.Should().Be(transaction);
        result.QRCodeResponse.Should().NotBeNull();
        result.QRCodeResponse.Should().Be(qrCodeResponse);

        _repositoryMock.Verify(x => x.GetByIdAsync(ticketId), Times.Once);
        _qrCodeClientMock.Verify(x => x.GenerateAsync(It.IsAny<QrCodeRequest>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTicket_WhenTicketExistsByTransaction()
    {
        // Arrange
        var transaction = "TXN123456";
        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.Transaction = transaction;

        var qrCodeResponse = new QRCodeResponse
        {
            Base64 = "base64string"
        };

        var expectedResponse = new GetTicketQueryResponse
        {
            TicketId = ticketEntity.TicketId,
            Transaction = transaction
        };

        var query = new GetTicketQuery { Transaction = transaction };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(0))
            .ReturnsAsync((Ticket)null);

        _repositoryMock
            .Setup(x => x.GetByTransactionAsync(transaction))
            .ReturnsAsync(ticketEntity);

        _qrCodeClientMock
            .Setup(x => x.GenerateAsync(It.IsAny<QrCodeRequest>()))
            .ReturnsAsync(qrCodeResponse);

        _mapperMock
            .Setup(x => x.Map<GetTicketQueryResponse>(ticketEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Transaction.Should().Be(transaction);
        result.QRCodeResponse.Should().Be(qrCodeResponse);

        _repositoryMock.Verify(x => x.GetByIdAsync(0), Times.Once);
        _repositoryMock.Verify(x => x.GetByTransactionAsync(transaction), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenTicketDoesNotExist()
    {
        // Arrange
        var ticketId = 999L;
        var query = new GetTicketQuery { TicketId = ticketId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync((Ticket)null);

        _repositoryMock
            .Setup(x => x.GetByTransactionAsync(null))
            .ReturnsAsync((Ticket)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(ticketId), Times.Once);
        _repositoryMock.Verify(x => x.GetByTransactionAsync(null), Times.Once);
        _qrCodeClientMock.Verify(x => x.GenerateAsync(It.IsAny<QrCodeRequest>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldLogWarning_WhenTicketNotFound()
    {
        // Arrange
        var ticketId = 999L;
        var transaction = "INVALID_TXN";
        var query = new GetTicketQuery { TicketId = ticketId, Transaction = transaction };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync((Ticket)null);

        _repositoryMock
            .Setup(x => x.GetByTransactionAsync(transaction))
            .ReturnsAsync((Ticket)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldGenerateQrCodeWithCorrectUrl_WhenTicketExists()
    {
        // Arrange
        var ticketId = 1L;
        var transaction = "TXN789012";
        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;
        ticketEntity.Transaction = transaction;

        var qrCodeResponse = new QRCodeResponse();
        var expectedUrl = $"{BaseUrl}/ticket/index/{transaction}";

        var query = new GetTicketQuery { TicketId = ticketId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _qrCodeClientMock
            .Setup(x => x.GenerateAsync(It.Is<QrCodeRequest>(r => r.Data == expectedUrl)))
            .ReturnsAsync(qrCodeResponse);

        _mapperMock
            .Setup(x => x.Map<GetTicketQueryResponse>(ticketEntity))
            .Returns(new GetTicketQueryResponse());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _qrCodeClientMock.Verify(x => x.GenerateAsync(It.Is<QrCodeRequest>(r => 
            r.Data == expectedUrl)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenExceptionIsThrown()
    {
        // Arrange
        var ticketId = 1L;
        var query = new GetTicketQuery { TicketId = ticketId };
        var exception = new Exception("Database error");

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var ticketId = 1L;
        var query = new GetTicketQuery { TicketId = ticketId };
        var exception = new Exception("Test exception");

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
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
    public async Task Handle_ShouldTryGetByIdFirst_ThenByTransaction()
    {
        // Arrange
        var transaction = "TXN123";
        var ticketEntity = TicketFixture.CreateValid();
        var callSequence = new List<string>();

        var query = new GetTicketQuery { Transaction = transaction };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(0))
            .Callback(() => callSequence.Add("GetById"))
            .ReturnsAsync((Ticket)null);

        _repositoryMock
            .Setup(x => x.GetByTransactionAsync(transaction))
            .Callback(() => callSequence.Add("GetByTransaction"))
            .ReturnsAsync(ticketEntity);

        _qrCodeClientMock
            .Setup(x => x.GenerateAsync(It.IsAny<QrCodeRequest>()))
            .ReturnsAsync(new QRCodeResponse());

        _mapperMock
            .Setup(x => x.Map<GetTicketQueryResponse>(ticketEntity))
            .Returns(new GetTicketQueryResponse());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        callSequence.Should().HaveCount(2);
        callSequence[0].Should().Be("GetById");
        callSequence[1].Should().Be("GetByTransaction");
    }

    [Fact]
    public async Task Handle_ShouldNotCallGetByTransaction_WhenTicketFoundById()
    {
        // Arrange
        var ticketId = 1L;
        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;
        ticketEntity.Transaction = "TXN123";

        var query = new GetTicketQuery { TicketId = ticketId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _qrCodeClientMock
            .Setup(x => x.GenerateAsync(It.IsAny<QrCodeRequest>()))
            .ReturnsAsync(new QRCodeResponse());

        _mapperMock
            .Setup(x => x.Map<GetTicketQueryResponse>(ticketEntity))
            .Returns(new GetTicketQueryResponse());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetByIdAsync(ticketId), Times.Once);
        _repositoryMock.Verify(x => x.GetByTransactionAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldMapTicketCorrectly_WhenTicketHasAllProperties()
    {
        // Arrange
        var ticketId = 1L;
        var ticketEntity = TicketFixture.CreateValid();
        ticketEntity.TicketId = ticketId;
        ticketEntity.EventId = 10;
        ticketEntity.PersonId = 20;
        ticketEntity.Price = 150.00m;
        ticketEntity.Transaction = "TXN999";
        ticketEntity.Auth = "AUTH123";
        ticketEntity.Payment = 1;
        ticketEntity.Status = 1;

        var expectedResponse = new GetTicketQueryResponse
        {
            TicketId = ticketId,
            EventId = 10,
            PersonId = 20,
            Price = 150.00m,
            Transaction = "TXN999",
            Auth = "AUTH123",
            Payment = 1,
            Status = 1
        };

        var qrCodeResponse = new QRCodeResponse { Base64 = "qrcode" };

        var query = new GetTicketQuery { TicketId = ticketId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId))
            .ReturnsAsync(ticketEntity);

        _qrCodeClientMock
            .Setup(x => x.GenerateAsync(It.IsAny<QrCodeRequest>()))
            .ReturnsAsync(qrCodeResponse);

        _mapperMock
            .Setup(x => x.Map<GetTicketQueryResponse>(ticketEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResponse, options => options.Excluding(x => x.QRCodeResponse));
        result.QRCodeResponse.Should().Be(qrCodeResponse);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(0, null)]
    [InlineData(null, "")]
    public async Task Handle_ShouldReturnNull_WhenBothTicketIdAndTransactionAreInvalid(long? ticketId, string transaction)
    {
        // Arrange
        var query = new GetTicketQuery { TicketId = ticketId, Transaction = transaction };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(ticketId ?? 0))
            .ReturnsAsync((Ticket)null);

        _repositoryMock
            .Setup(x => x.GetByTransactionAsync(transaction))
            .ReturnsAsync((Ticket)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}