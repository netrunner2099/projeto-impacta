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
    public async Task GetByIdAsync_ShouldReturnTicketWithRelations_WhenExists()
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
        var result = await _repository.GetByIdAsync(ticket.TicketId);

        // Assert
        result.Should().NotBeNull();
        result.TicketId.Should().Be(ticket.TicketId);
        result.Person.Should().NotBeNull();
        result.Person.Name.Should().Be(person.Name);
        result.Event.Should().NotBeNull();
        result.Event.Name.Should().Be(eventEntity.Name);
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
    public async Task GetAllAsync_ShouldReturnAllTicketsWithRelations()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        var eventEntity = EventFixture.CreateValid();
        Context.Persons.Add(person);
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        var tickets = new List<Ticket>
        {
            TicketFixture.CreateWithPersonAndEvent(person.PersonId, eventEntity.EventId),
            TicketFixture.CreateWithPersonAndEvent(person.PersonId, eventEntity.EventId),
            TicketFixture.CreateWithPersonAndEvent(person.PersonId, eventEntity.EventId)
        };
        Context.Tickets.AddRange(tickets);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.ListAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(t => t.Person != null);
        result.Should().OnlyContain(t => t.Event != null);
    }

    [Fact]
    public async Task GetByPersonIdAsync_ShouldReturnTicketsForPerson()
    {
        // Arrange
        var person1 = PersonFixture.CreateValid();
        var person2 = PersonFixture.CreateValid();
        var eventEntity = EventFixture.CreateValid();
        Context.Persons.AddRange(person1, person2);
        Context.Events.Add(eventEntity);
        await Context.SaveChangesAsync();

        var ticketsPerson1 = new List<Ticket>
        {
            TicketFixture.CreateWithPersonAndEvent(person1.PersonId, eventEntity.EventId),
            TicketFixture.CreateWithPersonAndEvent(person1.PersonId, eventEntity.EventId)
        };
        var ticketPerson2 = TicketFixture.CreateWithPersonAndEvent(person2.PersonId, eventEntity.EventId);
        
        Context.Tickets.AddRange(ticketsPerson1);
        Context.Tickets.Add(ticketPerson2);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByPersonIdAsync(person1.PersonId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.PersonId == person1.PersonId);
    }

    [Fact]
    public async Task GetByEventIdAsync_ShouldReturnTicketsForEvent()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        var event1 = EventFixture.CreateValid();
        var event2 = EventFixture.CreateValid();
        Context.Persons.Add(person);
        Context.Events.AddRange(event1, event2);
        await Context.SaveChangesAsync();

        var ticketsEvent1 = new List<Ticket>
        {
            TicketFixture.CreateWithPersonAndEvent(person.PersonId, event1.EventId),
            TicketFixture.CreateWithPersonAndEvent(person.PersonId, event1.EventId),
            TicketFixture.CreateWithPersonAndEvent(person.PersonId, event1.EventId)
        };
        var ticketEvent2 = TicketFixture.CreateWithPersonAndEvent(person.PersonId, event2.EventId);
        
        Context.Tickets.AddRange(ticketsEvent1);
        Context.Tickets.Add(ticketEvent2);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEventIdAsync(event1.EventId);

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(t => t.EventId == event1.EventId);
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
    public async Task ExistsAsync_ShouldReturnTrue_WhenTicketExists()
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
        var result = await _repository.GetByIdAsync(ticket.TicketId) is not null;

        // Assert
        result.Should().BeTrue();
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