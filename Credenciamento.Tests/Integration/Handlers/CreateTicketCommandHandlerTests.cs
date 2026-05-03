namespace Credenciamento.Tests.Integration.Handlers;

public class CreateTicketCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<CreateTicketCommandHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly Mock<IValidator<CreateTicketCommand>> _validatorMock;
    private readonly CreateTicketCommandHandler _handler;

    public CreateTicketCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateTicketCommandHandler>>();
        _mapperMock = new Mock<IMapper>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        _validatorMock = new Mock<IValidator<CreateTicketCommand>>();
        _handler = new CreateTicketCommandHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _ticketRepositoryMock.Object,
            _eventRepositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateTicket_WhenCommandIsValid()
    {
        // Arrange
        var eventId = 1L;
        var personId = 1L;
        var eventPrice = 299.99m;

        var command = new CreateTicketCommand
        {
            EventId = eventId,
            PersonId = personId,
            Payment = 1,
            Status = 1
        };

        var eventEntity = EventFixture.CreateValid();
        eventEntity.EventId = eventId;
        eventEntity.Price = eventPrice;

        var ticketEntity = new Ticket
        {
            TicketId = 0,
            EventId = eventId,
            PersonId = personId,
            Payment = 1,
            Status = 1
        };

        var createdTicket = new Ticket
        {
            TicketId = 1,
            EventId = eventId,
            PersonId = personId,
            Price = eventPrice,
            Payment = 1,
            Status = 1,
            CreatedAt = DateTime.UtcNow
        };

        var expectedResponse = new CreateTicketCommandResponse
        {
            TicketId = 1,
            EventId = eventId,
            PersonId = personId,
            Price = eventPrice,
            Payment = 1,
            Status = 1
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _eventRepositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ReturnsAsync(eventEntity);

        _mapperMock
            .Setup(x => x.Map<Ticket>(command))
            .Returns(ticketEntity);

        _ticketRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Ticket>()))
            .ReturnsAsync(createdTicket);

        _mapperMock
            .Setup(x => x.Map<CreateTicketCommandResponse>(createdTicket))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TicketId.Should().Be(1);
        result.EventId.Should().Be(eventId);
        result.PersonId.Should().Be(personId);
        result.Price.Should().Be(eventPrice);
        result.ErrorMessages.Should().BeNull();

        _validatorMock.Verify(x => x.ValidateAsync(command, default), Times.Once);
        _eventRepositoryMock.Verify(x => x.GetByIdAsync(eventId), Times.Once);
        _ticketRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Ticket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrors_WhenValidationFails()
    {
        // Arrange
        var command = new CreateTicketCommand
        {
            EventId = 0,
            PersonId = 0
        };

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("EventId", "O evento é obrigatório"),
            new ValidationFailure("PersonId", "A pessoa é obrigatória")
        };

        var validationResult = new ValidationResult(validationErrors);

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ErrorMessages.Should().NotBeNull();
        result.ErrorMessages.Should().HaveCount(2);
        result.ErrorMessages.Should().Contain("O evento é obrigatório");
        result.ErrorMessages.Should().Contain("A pessoa é obrigatória");

        _validatorMock.Verify(x => x.ValidateAsync(command, default), Times.Once);
        _eventRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<long>()), Times.Never);
        _ticketRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Ticket>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = 999L;
        var command = new CreateTicketCommand
        {
            EventId = eventId,
            PersonId = 1
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _eventRepositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ReturnsAsync((Event)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ErrorMessages.Should().NotBeNull();
        result.ErrorMessages.Should().HaveCount(1);
        result.ErrorMessages.Should().Contain("Evento não encontrado");

        _eventRepositoryMock.Verify(x => x.GetByIdAsync(eventId), Times.Once);
        _ticketRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Ticket>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSetTicketPriceFromEvent_WhenCreatingTicket()
    {
        // Arrange
        var eventId = 1L;
        var eventPrice = 150.50m;

        var command = new CreateTicketCommand
        {
            EventId = eventId,
            PersonId = 1
        };

        var eventEntity = EventFixture.CreateValid();
        eventEntity.EventId = eventId;
        eventEntity.Price = eventPrice;

        var ticketEntity = new Ticket();
        var createdTicket = new Ticket
        {
            TicketId = 1,
            Price = eventPrice
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _eventRepositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ReturnsAsync(eventEntity);

        _mapperMock
            .Setup(x => x.Map<Ticket>(command))
            .Returns(ticketEntity);

        _ticketRepositoryMock
            .Setup(x => x.AddAsync(It.Is<Ticket>(t => t.Price == eventPrice)))
            .ReturnsAsync(createdTicket);

        _mapperMock
            .Setup(x => x.Map<CreateTicketCommandResponse>(createdTicket))
            .Returns(new CreateTicketCommandResponse { Price = eventPrice });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Price.Should().Be(eventPrice);
        _ticketRepositoryMock.Verify(x => x.AddAsync(It.Is<Ticket>(t => t.Price == eventPrice)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenTicketInsertFails()
    {
        // Arrange
        var eventId = 1L;
        var command = new CreateTicketCommand
        {
            EventId = eventId,
            PersonId = 1
        };

        var eventEntity = EventFixture.CreateValid();
        eventEntity.EventId = eventId;

        var ticketEntity = new Ticket();

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _eventRepositoryMock
            .Setup(x => x.GetByIdAsync(eventId))
            .ReturnsAsync(eventEntity);

        _mapperMock
            .Setup(x => x.Map<Ticket>(command))
            .Returns(ticketEntity);

        _ticketRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ErrorMessages.Should().NotBeNull();
        result.ErrorMessages.Should().HaveCount(1);
        result.ErrorMessages.Should().Contain("Erro ao inserir ticket");

        _ticketRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Ticket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogWarning_WhenValidationFails()
    {
        // Arrange
        var command = new CreateTicketCommand();

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("EventId", "O evento é obrigatório")
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(validationErrors));

        // Act
        await _handler.Handle(command, CancellationToken.None);

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
    public async Task Handle_ShouldLogWarning_WhenEventNotFound()
    {
        // Arrange
        var command = new CreateTicketCommand { EventId = 999, PersonId = 1 };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _eventRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Event)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

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
    public async Task Handle_ShouldReturnNull_WhenExceptionIsThrown()
    {
        // Arrange
        var command = new CreateTicketCommand { EventId = 1, PersonId = 1 };
        var exception = new Exception("Database error");

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
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
        var command = new CreateTicketCommand { EventId = 1, PersonId = 1 };
        var exception = new Exception("Test exception");

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
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
    public async Task Handle_ShouldSetCreatedAtAutomatically_WhenCreatingTicket()
    {
        // Arrange
        var command = new CreateTicketCommand { EventId = 1, PersonId = 1 };
        var eventEntity = EventFixture.CreateValid();
        var ticketEntity = new Ticket();
        var createdTicket = new Ticket { TicketId = 1 };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _eventRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(eventEntity);

        _mapperMock
            .Setup(x => x.Map<Ticket>(command))
            .Returns(ticketEntity);

        _ticketRepositoryMock
            .Setup(x => x.AddAsync(It.Is<Ticket>(t => t.CreatedAt != default)))
            .ReturnsAsync(createdTicket);

        _mapperMock
            .Setup(x => x.Map<CreateTicketCommandResponse>(createdTicket))
            .Returns(new CreateTicketCommandResponse());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _ticketRepositoryMock.Verify(x => x.AddAsync(It.Is<Ticket>(t => 
            t.CreatedAt >= DateTime.UtcNow.AddSeconds(-5) && 
            t.CreatedAt <= DateTime.UtcNow.AddSeconds(5))), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldExecuteInCorrectOrder_ValidationThenEventCheckThenInsert()
    {
        // Arrange
        var command = new CreateTicketCommand { EventId = 1, PersonId = 1 };
        var callSequence = new List<string>();

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .Callback(() => callSequence.Add("Validate"))
            .ReturnsAsync(new ValidationResult());

        _eventRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .Callback(() => callSequence.Add("CheckEvent"))
            .ReturnsAsync(EventFixture.CreateValid());

        _mapperMock
            .Setup(x => x.Map<Ticket>(command))
            .Returns(new Ticket());

        _ticketRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Ticket>()))
            .Callback(() => callSequence.Add("Insert"))
            .ReturnsAsync(new Ticket { TicketId = 1 });

        _mapperMock
            .Setup(x => x.Map<CreateTicketCommandResponse>(It.IsAny<Ticket>()))
            .Returns(new CreateTicketCommandResponse());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        callSequence.Should().HaveCount(3);
        callSequence[0].Should().Be("Validate");
        callSequence[1].Should().Be("CheckEvent");
        callSequence[2].Should().Be("Insert");
    }
}