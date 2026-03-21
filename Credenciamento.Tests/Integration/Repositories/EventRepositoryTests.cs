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

public class EventRepositoryTests : TestBase
{
    private readonly EventRepository _repository;

    public EventRepositoryTests()
    {
        var factory = new TestDbContextFactory(Context);
        _repository = new EventRepository(factory);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEventToDatabase()
    {
        // Arrange
        var eventEntity = EventFixture.CreateValid();

        // Act
        var result = await _repository.AddAsync(eventEntity);

        // Assert
        result.Should().NotBeNull();
        result.EventId.Should().BeGreaterThan(0);
        result.Name.Should().Be(eventEntity.Name);
        result.Description.Should().Be(eventEntity.Description);

        Context.ChangeTracker.Clear();
        var savedEvent = await Context.Events.FindAsync(result.EventId);
        savedEvent.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEvent_WhenExists()
    {
        // Arrange
        var eventEntity = EventFixture.CreateValid();
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(eventEntity.EventId);

        // Assert
        result.Should().NotBeNull();
        result.EventId.Should().Be(eventEntity.EventId);
        result.Name.Should().Be(eventEntity.Name);
        result.Price.Should().Be(eventEntity.Price);
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
    public async Task GetAllAsync_ShouldReturnAllEvents()
    {
        // Arrange
        var events = new List<Event>
        {
            EventFixture.CreateValid(),
            EventFixture.CreateValid(),
            EventFixture.CreateValid()
        };
        Context.Events.AddRange(events);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.ListAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(e => e.EventId > 0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEvent()
    {
        // Arrange
        var eventEntity = EventFixture.CreateValid();
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        var newName = "Updated Conference Name";
        var newPrice = 299.99m;
        eventEntity.Name = newName;
        eventEntity.Price = newPrice;

        // Act
        var result = await _repository.UpdateAsync(eventEntity);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(newName);
        result.Price.Should().Be(newPrice);
        result.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkEventAsDeleted()
    {
        // Arrange
        var eventEntity = EventFixture.CreateValid();
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(eventEntity.EventId);

        // Assert
        result.Should().BeTrue();
        
        Context.ChangeTracker.Clear();
        var deletedEvent = await Context.Events.FindAsync(eventEntity.EventId);
        deletedEvent.Should().NotBeNull("soft delete deve manter o registro no banco");
        deletedEvent.Status.Should().Be((byte)EventStatus.Deleted);
        deletedEvent.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenEventNotExists()
    {
        // Act
        var result = await _repository.DeleteAsync(999999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEventExists()
    {
        // Arrange
        var eventEntity = EventFixture.CreateValid();
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(eventEntity.EventId) is not null;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEventDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(999999) is not null;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_ShouldSetCreatedAtAutomatically()
    {
        // Arrange
        var eventEntity = EventFixture.CreateValid();

        // Act
        var result = await _repository.AddAsync(eventEntity);

        // Assert
        result.CreatedAt.Should().BeCloseTo(System.DateTime.Now, System.TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOnlyModifiedFields()
    {
        // Arrange
        var eventEntity = EventFixture.CreateValid();
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        var originalCreatedAt = eventEntity.CreatedAt;
        eventEntity.Name = "Modified Name";

        // Act
        var result = await _repository.UpdateAsync(eventEntity);

        // Assert
        result.CreatedAt.Should().Be(originalCreatedAt);
        result.UpdatedAt.Should().NotBeNull();
    }
}