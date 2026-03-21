using Bogus;
using Credenciamento.Domain.Entities;
using System;

namespace Credenciamento.Tests.Fixtures;

public static class UserFixture
{
    private static readonly Faker<User> _faker = new Faker<User>()
        .RuleFor(u => u.Name, f => f.Person.FullName)
        .RuleFor(u => u.Email, f => f.Person.Email)
        .RuleFor(u => u.Password, f => f.Internet.Password(12))
        .RuleFor(u => u.Role, f => (byte)1)
        .RuleFor(u => u.Status, f => (byte)1)
        .RuleFor(u => u.CreatedAt, f => DateTime.Now)
        .RuleFor(u => u.UpdatedAt, f => null);

    public static User CreateValid()
    {
        return _faker.Generate();
    }

    public static User CreateValidWithId()
    {
        var user = _faker.Generate();
        user.UserId = new Faker().Random.Long(1, 10000);
        return user;
    }

    public static User CreateAdmin()
    {
        var user = _faker.Generate();
        user.Role = 2; // Assumindo 2 = Admin
        return user;
    }
}