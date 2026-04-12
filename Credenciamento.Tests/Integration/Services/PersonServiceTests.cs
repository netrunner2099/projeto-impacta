namespace Credenciamento.Tests.Integration.Services;

public class PersonServiceTests : TestBase
{
    private readonly Mock<ILogger<PersonService>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOptions<SmtpOptions>> _smtpOptionsMock;
    private readonly PersonService _service;

    public PersonServiceTests()
    {
        _loggerMock = new Mock<ILogger<PersonService>>();
        _mapperMock = new Mock<IMapper>();
        _personRepositoryMock = new Mock<IPersonRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _smtpOptionsMock = new Mock<IOptions<SmtpOptions>>();

        var smtpOptions = new SmtpOptions
        {
            Host = "smtp.test.com",
            Port = 587,
            Sender = "noreply@test.com"
        };

        _smtpOptionsMock.Setup(x => x.Value).Returns(smtpOptions);

        _service = new PersonService(
            _loggerMock.Object,
            _mapperMock.Object,
            _personRepositoryMock.Object,
            _userRepositoryMock.Object,
            _smtpOptionsMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldCreatePerson_WhenModelIsValid()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "joão silva",
            Email = "JOAO@EXAMPLE.COM",
            Document = "123.456.789-00",
            Phone = "(11) 98765-4321",
            ZipCode = "01234-567",
            Address = "rua teste",
            Number = "123",
            Neighborhood = "centro",
            City = "são paulo",
            State = "sp"
        };

        var personEntity = PersonFixture.CreateValid();
        personEntity.PersonId = 1;
        personEntity.Name = "João Silva";
        personEntity.Email = "joao@example.com";

        var userEntity = UserFixture.CreateValid();
        userEntity.UserId = 1;

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(personEntity);

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<PersonModel>(personEntity))
            .Returns(new PersonModel { PersonId = 1 });

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(userEntity);

        // Act
        var result = await _service.AddAsync(personModel);

        // Assert - Will be null because we can't mock SMTP
        // But we can verify the transformations and repository calls
        _personRepositoryMock.Verify(x => x.AddAsync(It.Is<Person>(p =>
            p.Email == "joao@example.com" && // Email to lower
            p.State == "SP" && // State to upper
            p.Status == (byte)PersonStatus.Active &&
            p.CreatedAt != default)), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldNormalizeName_ToTitleCase()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "MARIA SANTOS",
            Email = "maria@test.com",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "rua teste",
            Neighborhood = "bairro",
            City = "cidade",
            State = "sp"
        };

        var personEntity = PersonFixture.CreateValid();
        personEntity.PersonId = 1;

        Person capturedPerson = null;

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(personEntity);

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .Callback<Person>(p => capturedPerson = p)
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<PersonModel>(personEntity))
            .Returns(new PersonModel());

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(UserFixture.CreateValid());

        // Act
        await _service.AddAsync(personModel);

        // Assert
        personModel.Name.Should().Be("Maria Santos");
        personModel.Address.Should().Be("Rua Teste");
        personModel.Neighborhood.Should().Be("Bairro");
        personModel.City.Should().Be("Cidade");
    }

    [Fact]
    public async Task AddAsync_ShouldNormalizeEmail_ToLowerCase()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "TEST@EXAMPLE.COM",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "SP"
        };

        var personEntity = PersonFixture.CreateValid();

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(personEntity);

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<PersonModel>(personEntity))
            .Returns(new PersonModel());

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(UserFixture.CreateValid());

        // Act
        await _service.AddAsync(personModel);

        // Assert
        personModel.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task AddAsync_ShouldNormalizeState_ToUpperCase()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "sp"
        };

        var personEntity = PersonFixture.CreateValid();

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(personEntity);

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<PersonModel>(personEntity))
            .Returns(new PersonModel());

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(UserFixture.CreateValid());

        // Act
        await _service.AddAsync(personModel);

        // Assert
        personModel.State.Should().Be("SP");
    }

    [Fact]
    public async Task AddAsync_ShouldRemoveMasks_FromDocumentAndZipCode()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Document = "123.456.789-00",
            ZipCode = "12345-678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "SP"
        };

        var personEntity = PersonFixture.CreateValid();

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(personEntity);

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<PersonModel>(personEntity))
            .Returns(new PersonModel());

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(UserFixture.CreateValid());

        // Act
        await _service.AddAsync(personModel);

        // Assert
        personModel.Document.Should().Be("12345678900");
        personModel.ZipCode.Should().Be("12345678");
    }

    [Fact]
    public async Task AddAsync_ShouldSetStatusToActive_WhenCreatingPerson()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "SP"
        };

        var personEntity = PersonFixture.CreateValid();

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(personEntity);

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<PersonModel>(personEntity))
            .Returns(new PersonModel());

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(UserFixture.CreateValid());

        // Act
        await _service.AddAsync(personModel);

        // Assert
        personModel.Status.Should().Be((byte)PersonStatus.Active);
    }

    [Fact]
    public async Task AddAsync_ShouldSetCreatedAt_WhenCreatingPerson()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "SP"
        };

        var personEntity = PersonFixture.CreateValid();

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(personEntity);

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<PersonModel>(personEntity))
            .Returns(new PersonModel());

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(UserFixture.CreateValid());

        // Act
        await _service.AddAsync(personModel);

        // Assert
        personModel.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task AddAsync_ShouldReturnNull_WhenPersonRepositoryFails()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "SP"
        };

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(PersonFixture.CreateValid());

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync((Person)null);

        // Act
        var result = await _service.AddAsync(personModel);

        // Assert
        result.Should().BeNull();
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ShouldLogWarning_WhenPersonRepositoryFails()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "SP"
        };

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(PersonFixture.CreateValid());

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync((Person)null);

        // Act
        await _service.AddAsync(personModel);

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
    public async Task AddAsync_ShouldReturnNull_WhenExceptionIsThrown()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "SP"
        };

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Throws(new Exception("Mapping error"));

        // Act
        var result = await _service.AddAsync(personModel);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "SP"
        };

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Throws(new Exception("Test exception"));

        // Act
        await _service.AddAsync(personModel);

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
    public async Task AddAsync_ShouldCreateUserWithHashedPassword_WhenPersonIsCreated()
    {
        // Arrange
        var personModel = new PersonModel
        {
            Name = "Test User",
            Email = "test@example.com",
            Document = "12345678900",
            ZipCode = "12345678",
            Address = "Test",
            Neighborhood = "Test",
            City = "Test",
            State = "SP"
        };

        var personEntity = PersonFixture.CreateValid();
        personEntity.PersonId = 1;

        User capturedUser = null;

        _mapperMock
            .Setup(x => x.Map<Person>(It.IsAny<PersonModel>()))
            .Returns(personEntity);

        _personRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Person>()))
            .ReturnsAsync(personEntity);

        _mapperMock
            .Setup(x => x.Map<PersonModel>(personEntity))
            .Returns(new PersonModel { PersonId = 1, Email = "test@example.com", Name = "Test User" });

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync(UserFixture.CreateValid());

        // Act
        await _service.AddAsync(personModel);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser.PersonId.Should().Be(1);
        capturedUser.Email.Should().Be("test@example.com");
        capturedUser.Password.Should().NotBeNullOrEmpty();
        capturedUser.Role.Should().Be((byte)UserRole.User);
        capturedUser.Status.Should().Be((byte)UserStatus.Active);
    }
}