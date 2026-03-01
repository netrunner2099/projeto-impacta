using Bogus;
using Credenciamento.Application.Models;
using System;

namespace Credenciamento.Tests.Fixtures;

public static class EventModelFixture
{
    private static readonly Faker<EventModel> _faker = new Faker<EventModel>()
        .RuleFor(e => e.EventId, f => f.Random.Long(1, 10000))
        .RuleFor(e => e.Name, f => f.Company.CompanyName() + " Conference")
        .RuleFor(e => e.Description, f => f.Lorem.Paragraph())
        .RuleFor(e => e.Begin, f => f.Date.Future())
        .RuleFor(e => e.End, (f, e) => e.Begin.AddDays(f.Random.Number(1, 3)))
        .RuleFor(e => e.Price, f => f.Random.Decimal(50, 500));

    public static EventModel CreateValid()
    {
        return _faker.Generate();
    }

    public static EventModel CreateWithId(long id)
    {
        var model = _faker.Generate();
        model.EventId = id;
        return model;
    }
}