using Credenciamento.Domain.Entities;
using Credenciamento.Domain.Enums;
using Credenciamento.Infrastructure.Repositories;
using Credenciamento.Tests.Fixtures;
using Credenciamento.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Credenciamento.Tests.Integration.Repositories;

public class PersonRepositoryTests : TestBase
{
    private readonly PersonRepository _repository;

    public PersonRepositoryTests()
    {
        var factory = new TestDbContextFactory(Context);
        _repository = new PersonRepository(factory);
    }

    [Fact]
    public async Task AddAsync_ShouldAddPersonToDatabase()
    {
        // Arrange
        var person = PersonFixture.CreateValid();

        // Act
        var result = await _repository.AddAsync(person);

        // Assert
        result.Should().NotBeNull();
        result.PersonId.Should().BeGreaterThan(0);
        result.Name.Should().Be(person.Name);

        Context.ChangeTracker.Clear();
        var savedPerson = await Context.Persons.FindAsync(result.PersonId);
        savedPerson.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPerson_WhenExists()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(person.PersonId);

        // Assert
        result.Should().NotBeNull();
        result.PersonId.Should().Be(person.PersonId);
        result.Email.Should().Be(person.Email);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPersons()
    {
        // Arrange
        var persons = new List<Person>
        {
            PersonFixture.CreateValid(),
            PersonFixture.CreateValid(),
            PersonFixture.CreateValid()
        };
        Context.Persons.AddRange(persons);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.ListAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePerson()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        var newName = "Updated Name";
        person.Name = newName;

        // Act
        var result = await _repository.UpdateAsync(person);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(newName);
        result.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkPersonAsDeleted()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(person.PersonId);

        // Assert
        result.Should().BeTrue();
        
        Context.ChangeTracker.Clear();
        var deletedPerson = await Context.Persons.FindAsync(person.PersonId);
        deletedPerson.Should().NotBeNull("soft delete deve manter o registro no banco");
        deletedPerson.Status.Should().Be((byte)PersonStatus.Deleted);
        deletedPerson.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenPersonNotExists()
    {
        // Act
        var result = await _repository.DeleteAsync(999999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenPersonExists()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(person.PersonId) is not null;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenPersonDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(999999) is not null;

        // Assert
        result.Should().BeFalse();
    }
}
