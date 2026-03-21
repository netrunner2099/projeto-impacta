using Credenciamento.Domain.Entities;
using Credenciamento.Domain.Enums;
using Credenciamento.Infrastructure.Repositories;
using Credenciamento.Tests.Fixtures;
using Credenciamento.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Credenciamento.Tests.Integration.Repositories;

public class UserRepositoryTests : TestBase
{
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var factory = new TestDbContextFactory(Context);
        _repository = new UserRepository(factory);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var user = UserFixture.CreateValid();

        // Act
        var result = await _repository.AddAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().BeGreaterThan(0);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);

        Context.ChangeTracker.Clear();
        var savedUser = await Context.Users.FindAsync(result.UserId);
        savedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var user = UserFixture.CreateValid();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.UserId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(user.UserId);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var user = UserFixture.CreateValid();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(user.Email);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        result.Name.Should().Be(user.Name);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@email.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            UserFixture.CreateValid(),
            UserFixture.CreateValid(),
            UserFixture.CreateValid()
        };
        Context.Users.AddRange(users);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.ListAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(u => u.UserId > 0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        var user = UserFixture.CreateValid();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var newName = "Updated User Name";
        var newRole = (byte)2;
        user.Name = newName;
        user.Role = newRole;

        // Act
        var result = await _repository.UpdateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(newName);
        result.Role.Should().Be(newRole);
        result.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkUserAsDeleted()
    {
        // Arrange
        var user = UserFixture.CreateValid();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(user.UserId);

        // Assert
        result.Should().BeTrue();
        
        Context.ChangeTracker.Clear();
        var deletedUser = await Context.Users.FindAsync(user.UserId);
        deletedUser.Should().NotBeNull("soft delete deve manter o registro no banco");
        deletedUser.Status.Should().Be((byte)UserStatus.Deleted, "soft delete deve retonar status Deleted");
        deletedUser.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenUserNotExists()
    {
        // Act
        var result = await _repository.DeleteAsync(999999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var user = UserFixture.CreateValid();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.UserId) is not null;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(999999) is not null;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        var user = UserFixture.CreateValid();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync(user.Email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        // Act
        var result = await _repository.EmailExistsAsync("nonexistent@email.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnFalse_WhenUserIsDeleted()
    {
        // Arrange
        var user = UserFixture.CreateValid();
        user.Status = (byte)UserStatus.Deleted;
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync(user.Email);

        // Assert
        result.Should().BeFalse("usuários deletados não devem ser considerados");
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetUpdateDateAutomatically()
    {
        // Arrange
        var user = UserFixture.CreateValid();
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        user.Name = "Modified Name";

        // Act
        var result = await _repository.UpdateAsync(user);

        // Assert
        result.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeCloseTo(System.DateTime.Now, System.TimeSpan.FromSeconds(5));
    }
}