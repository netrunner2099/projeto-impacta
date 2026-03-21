using Bogus;
using Credenciamento.Domain.Entities;
using System;

namespace Credenciamento.Tests.Fixtures;

public static class PersonFixture
{
    private static readonly Faker<Domain.Entities.Person> _faker = new Faker<Domain.Entities.Person>()
        .RuleFor(p => p.Name, f => f.Person.FullName)
        .RuleFor(p => p.Document, f => f.Random.ReplaceNumbers("###########"))
        .RuleFor(p => p.Email, f => f.Person.Email)
        .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber("(##) #####-####"))
        .RuleFor(p => p.ZipCode, f => f.Address.ZipCode("#####-###"))
        .RuleFor(p => p.Address, f => f.Address.StreetAddress())
        .RuleFor(p => p.Number, f => f.Random.Number(1, 9999).ToString())
        .RuleFor(p => p.Complement, f => f.Address.SecondaryAddress())
        .RuleFor(p => p.Neighborhood, f => f.Address.County())
        .RuleFor(p => p.City, f => f.Address.City())
        .RuleFor(p => p.State, f => f.Address.StateAbbr())
        .RuleFor(p => p.Status, f => (byte)1)
        .RuleFor(p => p.CreatedAt, f => DateTime.Now)
        .RuleFor(p => p.UpdatedAt, f => null);

    public static Domain.Entities.Person CreateValid()
    {
        return _faker.Generate();
    }

    public static Domain.Entities.Person CreateValidWithId()
    {
        var person = _faker.Generate();
        person.PersonId = new Faker().Random.Long(1, 10000);
        return person;
    }

    public static Domain.Entities.Person CreateInvalid()
    {
        var person = _faker.Generate();
        person.Name = string.Empty; // Nome inválido
        return person;
    }
}