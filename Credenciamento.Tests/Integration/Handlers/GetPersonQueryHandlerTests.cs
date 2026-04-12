namespace Credenciamento.Tests.Integration.Handlers;

public class GetPersonQueryHandlerTests : TestBase
{
    private readonly Mock<ILogger<GetPersonQueryHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPersonRepository> _repositoryMock;
    private readonly GetPersonQueryHandler _handler;

    public GetPersonQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetPersonQueryHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IPersonRepository>();
        _handler = new GetPersonQueryHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPerson_WhenPersonExists()
    {
        // Arrange
        var personId = 1L;
        var personEntity = PersonFixture.CreateValid();
        personEntity.PersonId = personId;

        var expectedResponse = new GetPersonQueryResponse
        {
            PersonId = personEntity.PersonId,
            Name = personEntity.Name,
            Document = personEntity.Document,
            Email = personEntity.Email,
            Phone = personEntity.Phone,
            BirthDay = null,
            ZipCode = personEntity.ZipCode,
            Address = personEntity.Address,
            Number = personEntity.Number,
            Complement = personEntity.Complement,
            Neighborhood = personEntity.Neighborhood,
            City = personEntity.City,
            State = personEntity.State,
            Status = personEntity.Status
        };

        var query = new GetPersonQuery { PersonId = personId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(personId))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<GetPersonQueryResponse>(personEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PersonId.Should().Be(personId);
        result.Name.Should().Be(personEntity.Name);
        result.Document.Should().Be(personEntity.Document);
        result.Email.Should().Be(personEntity.Email);
        result.Phone.Should().Be(personEntity.Phone);
        result.Address.Should().Be(personEntity.Address);
        result.City.Should().Be(personEntity.City);
        result.State.Should().Be(personEntity.State);

        _repositoryMock.Verify(x => x.GetByIdAsync(personId), Times.Once);
        _mapperMock.Verify(x => x.Map<GetPersonQueryResponse>(personEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenPersonDoesNotExist()
    {
        // Arrange
        var personId = 999L;
        var query = new GetPersonQuery { PersonId = personId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(personId))
            .ReturnsAsync((Person)null);

        _mapperMock
            .Setup(x => x.Map<GetPersonQueryResponse>(null))
            .Returns((GetPersonQueryResponse)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(personId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenExceptionIsThrown()
    {
        // Arrange
        var personId = 1L;
        var query = new GetPersonQuery { PersonId = personId };
        var exceptionMessage = "Database connection failed";

        _repositoryMock
            .Setup(x => x.GetByIdAsync(personId))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(personId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var personId = 1L;
        var query = new GetPersonQuery { PersonId = personId };
        var exception = new Exception("Test exception");

        _repositoryMock
            .Setup(x => x.GetByIdAsync(personId))
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
    public async Task Handle_ShouldReturnNull_WhenPersonIdIsInvalid(long invalidPersonId)
    {
        // Arrange
        var query = new GetPersonQuery { PersonId = invalidPersonId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(invalidPersonId))
            .ReturnsAsync((Person)null);

        _mapperMock
            .Setup(x => x.Map<GetPersonQueryResponse>(null))
            .Returns((GetPersonQueryResponse)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(invalidPersonId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapAllPersonProperties_WhenPersonHasCompleteData()
    {
        // Arrange
        var personId = 1L;
        var personEntity = PersonFixture.CreateValid();
        personEntity.PersonId = personId;
        personEntity.Name = "Maria Santos";
        personEntity.Document = "12345678900";
        personEntity.Email = "maria@example.com";
        personEntity.Phone = "(11) 98765-4321";
        personEntity.ZipCode = "01234-567";
        personEntity.Address = "Rua Teste";
        personEntity.Number = "123";
        personEntity.Complement = "Apto 101";
        personEntity.Neighborhood = "Centro";
        personEntity.City = "São Paulo";
        personEntity.State = "SP";

        var expectedResponse = new GetPersonQueryResponse
        {
            PersonId = personEntity.PersonId,
            Name = personEntity.Name,
            Document = personEntity.Document,
            Email = personEntity.Email,
            Phone = personEntity.Phone,
            ZipCode = personEntity.ZipCode,
            Address = personEntity.Address,
            Number = personEntity.Number,
            Complement = personEntity.Complement,
            Neighborhood = personEntity.Neighborhood,
            City = personEntity.City,
            State = personEntity.State,
            Status = personEntity.Status,
            CreatedAt = personEntity.CreatedAt
        };

        var query = new GetPersonQuery { PersonId = personId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(personId))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<GetPersonQueryResponse>(personEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResponse);
        _mapperMock.Verify(x => x.Map<GetPersonQueryResponse>(It.Is<Person>(p =>
            p.PersonId == personId &&
            p.Name == "Maria Santos" &&
            p.City == "São Paulo" &&
            p.State == "SP")), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldIncludeAddressInformation_WhenPersonHasAddress()
    {
        // Arrange
        var personId = 1L;
        var personEntity = PersonFixture.CreateValid();
        personEntity.PersonId = personId;
        personEntity.ZipCode = "12345-678";
        personEntity.Address = "Av. Principal";
        personEntity.Number = "500";
        personEntity.Complement = "Sala 10";
        personEntity.Neighborhood = "Jardim";
        personEntity.City = "Rio de Janeiro";
        personEntity.State = "RJ";

        var expectedResponse = new GetPersonQueryResponse
        {
            PersonId = personId,
            ZipCode = personEntity.ZipCode,
            Address = personEntity.Address,
            Number = personEntity.Number,
            Complement = personEntity.Complement,
            Neighborhood = personEntity.Neighborhood,
            City = personEntity.City,
            State = personEntity.State
        };

        var query = new GetPersonQuery { PersonId = personId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(personId))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<GetPersonQueryResponse>(personEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ZipCode.Should().Be("12345-678");
        result.Address.Should().Be("Av. Principal");
        result.Number.Should().Be("500");
        result.Complement.Should().Be("Sala 10");
        result.Neighborhood.Should().Be("Jardim");
        result.City.Should().Be("Rio de Janeiro");
        result.State.Should().Be("RJ");
    }

    [Fact]
    public async Task Handle_ShouldIncludeContactInformation_WhenPersonHasContacts()
    {
        // Arrange
        var personId = 1L;
        var personEntity = PersonFixture.CreateValid();
        personEntity.PersonId = personId;
        personEntity.Email = "joao.silva@example.com";
        personEntity.Phone = "(21) 91234-5678";

        var expectedResponse = new GetPersonQueryResponse
        {
            PersonId = personId,
            Email = personEntity.Email,
            Phone = personEntity.Phone
        };

        var query = new GetPersonQuery { PersonId = personId };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(personId))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<GetPersonQueryResponse>(personEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("joao.silva@example.com");
        result.Phone.Should().Be("(21) 91234-5678");
    }

    [Fact]
    public async Task Handle_ShouldHandleCancellationToken_Correctly()
    {
        // Arrange
        var personId = 1L;
        var personEntity = PersonFixture.CreateValid();
        var expectedResponse = new GetPersonQueryResponse { PersonId = personId };
        var query = new GetPersonQuery { PersonId = personId };
        var cancellationToken = new CancellationToken();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(personId))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<GetPersonQueryResponse>(personEntity))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.PersonId.Should().Be(personId);
    }
}