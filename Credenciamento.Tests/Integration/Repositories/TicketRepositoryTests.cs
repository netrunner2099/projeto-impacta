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

public class TicketRepositoryTests : TestBase
{
    private readonly TicketRepository _repository;

    public TicketRepositoryTests()
    {
        var factory = new TestDbContextFactory(Context);
        _repository = new TicketRepository(factory);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTicketToDatabase()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        var eventEntity = EventFixture.CreateValid();
        Context.Persons.Add(person);
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        var ticket = TicketFixture.CreateWithPersonAndEvent(person.PersonId, eventEntity.EventId);

        // Act
        var result = await _repository.AddAsync(ticket);

        // Assert
        result.Should().NotBeNull();
        result.TicketId.Should().BeGreaterThan(0);
        result.PersonId.Should().Be(person.PersonId);
        result.EventId.Should().Be(eventEntity.EventId);

        Context.ChangeTracker.Clear();
        var savedTicket = await Context.Tickets.FindAsync(result.TicketId);
        savedTicket.Should().NotBeNull();
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
    public async Task UpdateAsync_ShouldUpdateTicket()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        var eventEntity = EventFixture.CreateValid();
        Context.Persons.Add(person);
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        var ticket = TicketFixture.CreateWithPersonAndEvent(person.PersonId, eventEntity.EventId);
        Context.Tickets.Add(ticket);
        await Context.SaveChangesAsync();

        var newPrice = 199.99m;
        ticket.Price = newPrice;

        // Act
        var result = await _repository.UpdateAsync(ticket);

        // Assert
        result.Should().NotBeNull();
        result.Price.Should().Be(newPrice);
        result.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkTicketAsDeleted()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        var eventEntity = EventFixture.CreateValid();
        Context.Persons.Add(person);
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        var ticket = TicketFixture.CreateWithPersonAndEvent(person.PersonId, eventEntity.EventId);
        Context.Tickets.Add(ticket);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(ticket.TicketId);

        // Assert
        result.Should().BeTrue();
        
        Context.ChangeTracker.Clear();
        var deletedTicket = await Context.Tickets.FindAsync(ticket.TicketId);
        deletedTicket.Should().NotBeNull("soft delete deve manter o registro no banco");
        deletedTicket.Status.Should().Be((byte)TicketStatus.Deleted);
        deletedTicket.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenTicketNotExists()
    {
        // Act
        var result = await _repository.DeleteAsync(999999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenTicketDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(999999) is not null;

        // Assert
        result.Should().BeFalse();
    }
}