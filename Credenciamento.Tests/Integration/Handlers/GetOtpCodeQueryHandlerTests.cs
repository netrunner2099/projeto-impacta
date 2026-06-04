namespace Credenciamento.Tests.Integration.Handlers;

public class GetOtpCodeQueryHandlerTests : TestBase
{
    private readonly Mock<ILogger<GetOtpCodeQueryHandler>> _loggerMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly GetOtpCodeQueryHandler _handler;

    public GetOtpCodeQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetOtpCodeQueryHandler>>();
        _userServiceMock = new Mock<IUserService>();
        _handler = new GetOtpCodeQueryHandler(_loggerMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenOtpCodeIsGenerated()
    {
        // Arrange
        var email = "user@example.com";
        var query = new GetOtpCodeQuery { Email = email };

        _userServiceMock
            .Setup(x => x.GenerateOnetTimePasswordAsync(email))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _userServiceMock.Verify(x => x.GenerateOnetTimePasswordAsync(email), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenOtpCodeGenerationFails()
    {
        // Arrange
        var email = "invalid@example.com";
        var query = new GetOtpCodeQuery { Email = email };

        _userServiceMock
            .Setup(x => x.GenerateOnetTimePasswordAsync(email))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        _userServiceMock.Verify(x => x.GenerateOnetTimePasswordAsync(email), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
    {
        // Arrange
        var email = "error@example.com";
        var query = new GetOtpCodeQuery { Email = email };
        var exceptionMessage = "Database connection failed";

        _userServiceMock
            .Setup(x => x.GenerateOnetTimePasswordAsync(email))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        _userServiceMock.Verify(x => x.GenerateOnetTimePasswordAsync(email), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Handle_ShouldCallService_WithEmptyOrNullEmail(string email)
    {
        // Arrange
        var query = new GetOtpCodeQuery { Email = email };

        _userServiceMock
            .Setup(x => x.GenerateOnetTimePasswordAsync(email))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        _userServiceMock.Verify(x => x.GenerateOnetTimePasswordAsync(email), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var email = "error@example.com";
        var query = new GetOtpCodeQuery { Email = email };
        var exception = new Exception("Test exception");

        _userServiceMock
            .Setup(x => x.GenerateOnetTimePasswordAsync(email))
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
}