using Bogus;
using Credenciamento.Domain.Entities;
using System;

namespace Credenciamento.Tests.Fixtures;

public static class TicketFixture
{
    private static readonly Faker<Ticket> _faker = new Faker<Ticket>()
        .RuleFor(t => t.PersonId, f => f.Random.Long(1, 1000))
        .RuleFor(t => t.EventId, f => f.Random.Long(1, 1000))
        .RuleFor(t => t.Price, f => f.Random.Decimal(50, 500))
        .RuleFor(t => t.Status, f => (byte)1)
        .RuleFor(t => t.CreatedAt, f => DateTime.Now)
        .RuleFor(t => t.UpdatedAt, f => null);

    public static Ticket CreateValid()
    {
        return _faker.Generate();
    }

    public static Ticket CreateValidWithId()
    {
        var ticket = _faker.Generate();
        ticket.TicketId = new Faker().Random.Long(1, 10000);
        return ticket;
    }

    public static Ticket CreateWithPersonAndEvent(long personId, long eventId)
    {
        var ticket = _faker.Generate();
        ticket.PersonId = personId;
        ticket.EventId = eventId;
        return ticket;
    }
}