namespace Credenciamento.Tests.Integration.Handlers;

public class VerifyLoginQueryHandlerTests : TestBase
{
    private readonly Mock<ILogger<VerifyLoginQueryHandler>> _loggerMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly VerifyLoginQueryHandler _handler;

    public VerifyLoginQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<VerifyLoginQueryHandler>>();
        _userServiceMock = new Mock<IUserService>();
        _personRepositoryMock = new Mock<IPersonRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _handler = new VerifyLoginQueryHandler(
            _loggerMock.Object,
            _userServiceMock.Object,
            _personRepositoryMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTokenAndPersonId_WhenLoginIsSuccessful()
    {
        // Arrange
        var email = "user@example.com";
        var password = "Password123";
        var personId = 1L;
        var userId = 10L;

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var user = new User
        {
            UserId = userId,
            PersonId = personId,
            Name = "John Doe",
            Email = email,
            Role = 1
        };

        var person = PersonFixture.CreateValid();
        person.PersonId = personId;
        person.Email = email;

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .ReturnsAsync(user);

        _personRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(person);

        _cacheServiceMock
            .Setup(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), It.IsAny<int>()))
            .Returns(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.PersonId.Should().Be(personId);

        _userServiceMock.Verify(x => x.LoginAsync(email, password), Times.Once);
        _personRepositoryMock.Verify(x => x.GetByEmailAsync(email), Times.Once);
        _cacheServiceMock.Verify(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), 30), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenLoginFails()
    {
        // Arrange
        var email = "invalid@example.com";
        var password = "WrongPassword";

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _userServiceMock.Verify(x => x.LoginAsync(email, password), Times.Once);
        _personRepositoryMock.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        _cacheServiceMock.Verify(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldStoreUserInCache_WhenLoginIsSuccessful()
    {
        // Arrange
        var email = "user@example.com";
        var password = "Password123";
        var personId = 1L;

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var user = new User
        {
            UserId = 10,
            PersonId = personId,
            Name = "John Doe",
            Email = email,
            Role = 1
        };

        var person = PersonFixture.CreateValid();
        person.PersonId = personId;

        UserModel capturedUserModel = null;
        string capturedCacheKey = null;
        int capturedTtl = 0;

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .ReturnsAsync(user);

        _personRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(person);

        _cacheServiceMock
            .Setup(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), It.IsAny<int>()))
            .Callback<string, UserModel, int>((key, model, ttl) =>
            {
                capturedCacheKey = key;
                capturedUserModel = model;
                capturedTtl = ttl;
            })
            .Returns(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        capturedUserModel.Should().NotBeNull();
        capturedUserModel.UserId.Should().Be(user.UserId);
        capturedUserModel.Email.Should().Be(email);
        capturedUserModel.Name.Should().Be(user.Name);
        capturedUserModel.Role.Should().Be(user.Role);
        capturedCacheKey.Should().StartWith("user:");
        capturedTtl.Should().Be(30);
    }

    [Fact]
    public async Task Handle_ShouldGenerateBase64Token_WhenLoginIsSuccessful()
    {
        // Arrange
        var email = "user@example.com";
        var password = "Password123";

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var user = new User
        {
            UserId = 10,
            PersonId = 1,
            Name = "John Doe",
            Email = email,
            Role = 1
        };

        var person = PersonFixture.CreateValid();

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .ReturnsAsync(user);

        _personRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(person);

        _cacheServiceMock
            .Setup(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), It.IsAny<int>()))
            .Returns(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Token.Should().NotBeNullOrEmpty();

        // Verifica se é Base64 válido
        var isBase64 = IsBase64String(result.Token);
        isBase64.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldSetCacheTtlTo30Minutes_WhenLoginIsSuccessful()
    {
        // Arrange
        var email = "user@example.com";
        var password = "Password123";

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var user = new User
        {
            UserId = 10,
            PersonId = 1,
            Email = email,
            Name = "Test User",
            Role = 1
        };

        var person = PersonFixture.CreateValid();

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .ReturnsAsync(user);

        _personRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(person);

        _cacheServiceMock
            .Setup(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), 30))
            .Returns(true);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _cacheServiceMock.Verify(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), 30), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenExceptionIsThrown()
    {
        // Arrange
        var email = "user@example.com";
        var password = "Password123";

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var exception = new Exception("Database error");

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
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
        var email = "user@example.com";
        var password = "Password123";

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var exception = new Exception("Test exception");

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
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
    public async Task Handle_ShouldIncludeUserRole_InCachedUserModel()
    {
        // Arrange
        var email = "admin@example.com";
        var password = "AdminPass123";
        var role = (byte)2; // Admin role

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var user = new User
        {
            UserId = 10,
            PersonId = 1,
            Email = email,
            Name = "Admin User",
            Role = role
        };

        var person = PersonFixture.CreateValid();

        UserModel capturedUserModel = null;

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .ReturnsAsync(user);

        _personRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(person);

        _cacheServiceMock
            .Setup(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), It.IsAny<int>()))
            .Callback<string, UserModel, int>((key, model, ttl) => capturedUserModel = model)
            .Returns(true);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        capturedUserModel.Should().NotBeNull();
        capturedUserModel.Role.Should().Be(role);
    }

    [Fact]
    public async Task Handle_ShouldMapAllUserProperties_ToCachedUserModel()
    {
        // Arrange
        var email = "user@example.com";
        var password = "Password123";

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var user = new User
        {
            UserId = 25,
            PersonId = 15,
            Email = email,
            Name = "Complete User",
            Role = 1
        };

        var person = PersonFixture.CreateValid();
        person.PersonId = 15;

        UserModel capturedUserModel = null;

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .ReturnsAsync(user);

        _personRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(person);

        _cacheServiceMock
            .Setup(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), It.IsAny<int>()))
            .Callback<string, UserModel, int>((key, model, ttl) => capturedUserModel = model)
            .Returns(true);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        capturedUserModel.Should().NotBeNull();
        capturedUserModel.UserId.Should().Be(25);
        capturedUserModel.PersonId.Should().Be(15);
        capturedUserModel.Email.Should().Be(email);
        capturedUserModel.Name.Should().Be("Complete User");
        capturedUserModel.Role.Should().Be(1);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("email@test.com", "")]
    [InlineData(null, "password")]
    [InlineData("email@test.com", null)]
    public async Task Handle_ShouldReturnNull_WhenCredentialsAreInvalid(string email, string password)
    {
        // Arrange
        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldExecuteInCorrectOrder_LoginThenGetPersonThenCache()
    {
        // Arrange
        var email = "user@example.com";
        var password = "Password123";
        var callSequence = new List<string>();

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var user = new User
        {
            UserId = 10,
            PersonId = 1,
            Email = email,
            Name = "Test",
            Role = 1
        };

        var person = PersonFixture.CreateValid();

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .Callback(() => callSequence.Add("Login"))
            .ReturnsAsync(user);

        _personRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .Callback(() => callSequence.Add("GetPerson"))
            .ReturnsAsync(person);

        _cacheServiceMock
            .Setup(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), It.IsAny<int>()))
            .Callback(() => callSequence.Add("Cache"))
            .Returns(true);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        callSequence.Should().HaveCount(3);
        callSequence[0].Should().Be("Login");
        callSequence[1].Should().Be("GetPerson");
        callSequence[2].Should().Be("Cache");
    }

    [Fact]
    public async Task Handle_ShouldHandleUserWithoutPersonId_Correctly()
    {
        // Arrange
        var email = "user@example.com";
        var password = "Password123";

        var query = new VerifyLoginQuery
        {
            Email = email,
            Password = password
        };

        var user = new User
        {
            UserId = 10,
            PersonId = null, // No PersonId
            Email = email,
            Name = "User Without Person",
            Role = 1
        };

        var person = PersonFixture.CreateValid();
        person.PersonId = 5;

        UserModel capturedUserModel = null;

        _userServiceMock
            .Setup(x => x.LoginAsync(email, password))
            .ReturnsAsync(user);

        _personRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(person);

        _cacheServiceMock
            .Setup(x => x.SetObject(It.IsAny<string>(), It.IsAny<UserModel>(), It.IsAny<int>()))
            .Callback<string, UserModel, int>((key, model, ttl) => capturedUserModel = model)
            .Returns(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PersonId.Should().Be(5); // From person entity
        capturedUserModel.PersonId.Should().Be(0); // Null coalesced to 0
    }

    // Helper method
    private bool IsBase64String(string base64)
    {
        if (string.IsNullOrEmpty(base64))
            return false;

        try
        {
            Convert.FromBase64String(base64);
            return true;
        }
        catch
        {
            return false;
        }
    }
}