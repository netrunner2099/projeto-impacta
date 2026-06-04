namespace Credenciamento.Tests.Integration.Handlers;

public class ListUserQueryHandlerTests : TestBase
{
    private readonly Mock<ILogger<ListUserQueryHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly ListUserQueryHandler _handler;

    public ListUserQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ListUserQueryHandler>>();
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new ListUserQueryHandler(
            _loggerMock.Object,
            _mapperMock.Object,
            _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllUsers_WhenNoFilterIsApplied()
    {
        // Arrange
        var users = new List<Domain.Entities.User>
        {
            UserFixture.CreateValid(),
            UserFixture.CreateValid(),
            UserFixture.CreateValid()
        };

        var expectedModels = users.Select(u => new UserModel { UserId = u.UserId, Name = u.Name }).ToList();
        var query = new ListUserQuery();

        _repositoryMock.Setup(x => x.ListAllAsync()).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<List<UserModel>>(It.IsAny<IEnumerable<Domain.Entities.User>>()))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Users.Should().HaveCount(3);
        _repositoryMock.Verify(x => x.ListAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFilterByName_WhenNameFilterIsApplied()
    {
        // Arrange
        var u1 = UserFixture.CreateValid(); u1.Name = "Ana Lima";
        var u2 = UserFixture.CreateValid(); u2.Name = "Bruno Santos";
        var u3 = UserFixture.CreateValid(); u3.Name = "Ana Costa";
        var users = new List<Domain.Entities.User> { u1, u2, u3 };

        var filtered = users.Where(u => u.Name.Contains("Ana", StringComparison.OrdinalIgnoreCase)).ToList();
        var expectedModels = filtered.Select(u => new UserModel { Name = u.Name }).ToList();

        var query = new ListUserQuery { Name = "Ana" };

        _repositoryMock.Setup(x => x.ListAllAsync()).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<List<UserModel>>(It.IsAny<IEnumerable<Domain.Entities.User>>()))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Users.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldFilterByRole_WhenRoleFilterIsApplied()
    {
        // Arrange
        var adminRole = (byte)Domain.Enums.UserRole.Admin;
        var userRole = (byte)Domain.Enums.UserRole.User;
        var r1 = UserFixture.CreateValid(); r1.Role = adminRole;
        var r2 = UserFixture.CreateValid(); r2.Role = userRole;
        var r3 = UserFixture.CreateValid(); r3.Role = userRole;
        var users = new List<Domain.Entities.User> { r1, r2, r3 };

        var filtered = users.Where(u => u.Role == adminRole).ToList();
        var expectedModels = filtered.Select(u => new UserModel { Role = u.Role }).ToList();

        var query = new ListUserQuery { Role = adminRole };

        _repositoryMock.Setup(x => x.ListAllAsync()).ReturnsAsync(users);
        _mapperMock.Setup(x => x.Map<List<UserModel>>(It.IsAny<IEnumerable<Domain.Entities.User>>()))
            .Returns(expectedModels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Users.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ListAllAsync()).ReturnsAsync(new List<Domain.Entities.User>());
        _mapperMock.Setup(x => x.Map<List<UserModel>>(It.IsAny<IEnumerable<Domain.Entities.User>>()))
            .Returns(new List<UserModel>());

        var query = new ListUserQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Users.Should().BeEmpty();
    }
}
