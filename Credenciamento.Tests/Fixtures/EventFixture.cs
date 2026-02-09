using Bogus;
using Credenciamento.Domain.Entities;
using System;

namespace Credenciamento.Tests.Fixtures;

public static class EventFixture
{
    private static readonly Faker<Event> _faker = new Faker<Event>()
        .RuleFor(e => e.Name, f => f.Company.CompanyName() + " Conference")
        .RuleFor(e => e.Description, f => f.Lorem.Paragraph())
        .RuleFor(e => e.Begin, f => f.Date.Future())
        .RuleFor(e => e.End, (f, e) => e.Begin.AddDays(f.Random.Number(1, 3)))
        .RuleFor(e => e.Price, f => f.Random.Decimal(50, 500))
        .RuleFor(e => e.Status, f => (byte)1)
        .RuleFor(e => e.CreatedAt, f => DateTime.Now)
        .RuleFor(e => e.UpdatedAt, f => null);

    public static Event CreateValid()
    {
        return _faker.Generate();
    }

    public static Event CreateValidWithId()
    {
        var evt = _faker.Generate();
        evt.EventId = new Faker().Random.Long(1, 10000);
        return evt;
    }

    public static Event CreatePastEvent()
    {
        var evt = _faker.Generate();
        evt.Begin = DateTime.Now.AddDays(-5);
        evt.End = DateTime.Now.AddDays(-3);
        return evt;
    }
}