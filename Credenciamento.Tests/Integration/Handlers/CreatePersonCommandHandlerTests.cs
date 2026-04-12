namespace Credenciamento.Tests.Integration.Handlers;

public class CreatePersonCommandHandlerTests : TestBase
{
    private readonly Mock<ILogger<CreatePersonCommandHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPersonService> _serviceMock;
    private readonly Mock<IValidator<CreatePersonCommand>> _validatorMock;
    private readonly CreatePersonCommandHandler _handler;

    public CreatePersonCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreatePersonCommandHandler>>();
        _mapperMock = new Mock<IMapper>();
        _serviceMock = new Mock<IPersonService>();
        _validatorMock = new Mock<IValidator<CreatePersonCommand>>();
        _handler = new CreatePersonCommandHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _serviceMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreatePerson_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            Name = "João Silva",
            Document = "123.456.789-00",
            Email = "joao.silva@example.com",
            Phone = "(11) 98765-4321",
            BirthDay = new DateTime(1990, 1, 1),
            ZipCode = "01234-567",
            Address = "Rua Teste",
            Number = "123",
            Neighborhood = "Centro",
            City = "São Paulo",
            State = "SP"
        };

        var personModel = new PersonModel
        {
            Name = command.Name,
            Document = command.Document,
            Email = command.Email,
            Phone = command.Phone
        };

        var createdPerson = new PersonModel
        {
            PersonId = 1,
            Name = command.Name,
            Document = command.Document,
            Email = command.Email,
            Phone = command.Phone
        };

        var expectedResponse = new CreatePersonCommandResponse
        {
            PersonId = 1,
            Name = command.Name,
            Document = command.Document,
            Email = command.Email
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<PersonModel>(command))
            .Returns(personModel);

        _serviceMock
            .Setup(x => x.AddAsync(personModel))
            .ReturnsAsync(createdPerson);

        _mapperMock
            .Setup(x => x.Map<CreatePersonCommandResponse>(createdPerson))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PersonId.Should().Be(1);
        result.Name.Should().Be(command.Name);
        result.Email.Should().Be(command.Email);
        result.Errors.Should().BeNull();

        _validatorMock.Verify(x => x.ValidateAsync(command, default), Times.Once);
        _serviceMock.Verify(x => x.AddAsync(It.IsAny<PersonModel>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrors_WhenValidationFails()
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            Name = "",
            Email = "invalid-email",
            Document = "invalid-cpf"
        };

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "O nome é obrigatório"),
            new ValidationFailure("Email", "E-mail inválido"),
            new ValidationFailure("Document", "CPF inválido")
        };

        var validationResult = new ValidationResult(validationErrors);

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().NotBeNull();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain("O nome é obrigatório");
        result.Errors.Should().Contain("E-mail inválido");
        result.Errors.Should().Contain("CPF inválido");

        _validatorMock.Verify(x => x.ValidateAsync(command, default), Times.Once);
        _serviceMock.Verify(x => x.AddAsync(It.IsAny<PersonModel>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldLogWarning_WhenValidationFails()
    {
        // Arrange
        var command = new CreatePersonCommand { Name = "" };

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "O nome é obrigatório")
        };

        var validationResult = new ValidationResult(validationErrors);

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(validationResult);

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
    public async Task Handle_ShouldReturnEmptyResponse_WhenExceptionIsThrown()
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            Name = "João Silva",
            Email = "joao@example.com"
        };

        var personModel = new PersonModel();
        var exceptionMessage = "Database connection failed";

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<PersonModel>(command))
            .Returns(personModel);

        _serviceMock
            .Setup(x => x.AddAsync(personModel))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PersonId.Should().Be(0);
        result.Errors.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var command = new CreatePersonCommand { Name = "Test" };
        var personModel = new PersonModel();
        var exception = new Exception("Test exception");

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<PersonModel>(command))
            .Returns(personModel);

        _serviceMock
            .Setup(x => x.AddAsync(personModel))
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
    public async Task Handle_ShouldMapCommandToPerson_Correctly()
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            Name = "Maria Santos",
            Document = "987.654.321-00",
            Email = "maria@example.com",
            Phone = "(21) 91234-5678",
            BirthDay = new DateTime(1985, 5, 15),
            ZipCode = "12345-678",
            Address = "Av. Principal",
            Number = "500",
            Complement = "Apto 101",
            Neighborhood = "Jardim",
            City = "Rio de Janeiro",
            State = "RJ"
        };

        var personModel = new PersonModel
        {
            Name = command.Name,
            Document = command.Document,
            Email = command.Email,
            Phone = command.Phone,
            BirthDay = command.BirthDay,
            ZipCode = command.ZipCode,
            Address = command.Address,
            Number = command.Number,
            Complement = command.Complement,
            Neighborhood = command.Neighborhood,
            City = command.City,
            State = command.State
        };

        var createdPerson = new PersonModel { PersonId = 1 };
        var expectedResponse = new CreatePersonCommandResponse { PersonId = 1 };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<PersonModel>(command))
            .Returns(personModel);

        _serviceMock
            .Setup(x => x.AddAsync(It.IsAny<PersonModel>()))
            .ReturnsAsync(createdPerson);

        _mapperMock
            .Setup(x => x.Map<CreatePersonCommandResponse>(createdPerson))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapperMock.Verify(x => x.Map<PersonModel>(It.Is<CreatePersonCommand>(c =>
            c.Name == command.Name &&
            c.Document == command.Document &&
            c.Email == command.Email &&
            c.City == command.City &&
            c.State == command.State)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldValidateBeforeMapping_InCorrectOrder()
    {
        // Arrange
        var command = new CreatePersonCommand { Name = "Test" };
        var callSequence = new List<string>();

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .Callback(() => callSequence.Add("Validate"))
            .ReturnsAsync(new ValidationResult());

        _mapperMock
            .Setup(x => x.Map<PersonModel>(command))
            .Callback(() => callSequence.Add("Map"))
            .Returns(new PersonModel());

        _serviceMock
            .Setup(x => x.AddAsync(It.IsAny<PersonModel>()))
            .Callback(() => callSequence.Add("Add"))
            .ReturnsAsync(new PersonModel());

        _mapperMock
            .Setup(x => x.Map<CreatePersonCommandResponse>(It.IsAny<PersonModel>()))
            .Returns(new CreatePersonCommandResponse());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        callSequence.Should().HaveCount(3);
        callSequence[0].Should().Be("Validate");
        callSequence[1].Should().Be("Map");
        callSequence[2].Should().Be("Add");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Handle_ShouldReturnValidationError_WhenNameIsNullOrEmpty(string invalidName)
    {
        // Arrange
        var command = new CreatePersonCommand { Name = invalidName };

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "O nome é obrigatório")
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(validationErrors));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().Contain("O nome é obrigatório");
        _serviceMock.Verify(x => x.AddAsync(It.IsAny<PersonModel>()), Times.Never);
    }
}