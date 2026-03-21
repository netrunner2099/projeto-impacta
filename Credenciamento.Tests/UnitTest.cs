using FluentAssertions;
using Xunit;

namespace Credenciamento.Tests
{
    public class UnitTest
    {
        [Fact]
        public void TestInfrastructure_ShouldPass()
        {
            // Arrange
            var expected = true;

            // Act
            var result = expected;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void FluentAssertions_ShouldWork()
        {
            // Arrange
            var number = 42;

            // Act & Assert
            number.Should().Be(42);
            number.Should().BeGreaterThan(0);
            number.Should().BeLessThan(100);
        }

        [Fact]
        public void Xunit_ShouldDiscoverThisTest()
        {
            // Este teste serve apenas para verificar que o xUnit está funcionando
            Assert.True(true);
        }
    }
}
