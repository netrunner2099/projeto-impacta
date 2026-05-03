using Microsoft.EntityFrameworkCore;

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

        // Recarrega do banco para confirmar que foi salvo
        Context.ChangeTracker.Clear();
        var savedPerson = await Context.Persons.FirstOrDefaultAsync(p => p.Email == person.Email);
        savedPerson.Should().NotBeNull();
        savedPerson.PersonId.Should().BeGreaterThan(0);
        savedPerson.Name.Should().Be(person.Name);
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

        Context.ChangeTracker.Clear();
        var savedPerson = await Context.Persons.FirstOrDefaultAsync(p => p.Email == person.Email);
        savedPerson.Should().NotBeNull();

        var newName = "Updated Name";
        savedPerson.Name = newName;

        // Act
        var result = await _repository.UpdateAsync(savedPerson);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(newName);
        result.UpdatedAt.Should().NotBeNull();
        
        // Verifica no banco
        Context.ChangeTracker.Clear();
        var updatedPerson = await Context.Persons.FindAsync(savedPerson.PersonId);
        updatedPerson.Name.Should().Be(newName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeletePerson()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        // Debug: verificar se o ID foi gerado
        Context.ChangeTracker.Clear();
        var savedPerson = await Context.Persons.FirstOrDefaultAsync(p => p.Email == person.Email);
        savedPerson.Should().NotBeNull();
        savedPerson.PersonId.Should().BeGreaterThan(0);

        // Act
        var result = await _repository.DeleteAsync(savedPerson.PersonId);

        // Assert
        result.Should().BeTrue();

        Context.ChangeTracker.Clear();
        var deletedPerson = await Context.Persons.FindAsync(savedPerson.PersonId);
        deletedPerson.Should().NotBeNull();
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
    public async Task GetByEmailAsync_ShouldReturnPerson_WhenExists()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(person.Email);

        // Assert
        result.Should().NotBeNull();
        result.PersonId.Should().Be(person.PersonId);
        result.Email.Should().Be(person.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DocumentExistsAsync_ShouldReturnTrue_WhenDocumentExists()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        var checkPerson = new Person { Document = person.Document, PersonId = 0 };

        // Act
        var result = await _repository.DocumentExistsAsync(checkPerson);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DocumentExistsAsync_ShouldReturnFalse_WhenDocumentNotExists()
    {
        // Arrange
        var checkPerson = new Person { Document = "99999999999", PersonId = 0 };

        // Act
        var result = await _repository.DocumentExistsAsync(checkPerson);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DocumentExistsAsync_ShouldReturnFalse_WhenSamePerson()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        // Recarrega a pessoa para obter o ID gerado
        Context.ChangeTracker.Clear();
        var savedPerson = await Context.Persons.FirstOrDefaultAsync(p => p.Email == person.Email);
        savedPerson.Should().NotBeNull();

        // Act
        var result = await _repository.DocumentExistsAsync(savedPerson);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task PhoneNumberExistsAsync_ShouldReturnTrue_WhenPhoneExists()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        var savedPerson = await Context.Persons.FirstOrDefaultAsync(p => p.Email == person.Email);

        var checkPerson = new Person { Phone = savedPerson.Phone, PersonId = 0 };

        // Act
        var result = await _repository.PhoneNumberExistsAsync(checkPerson);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PhoneNumberExistsAsync_ShouldReturnFalse_WhenPhoneNotExists()
    {
        // Arrange
        var checkPerson = new Person { Phone = "(99) 99999-9999", PersonId = 0 };

        // Act
        var result = await _repository.PhoneNumberExistsAsync(checkPerson);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        var person = PersonFixture.CreateValid();
        Context.Persons.Add(person);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        var savedPerson = await Context.Persons.FirstOrDefaultAsync(p => p.Email == person.Email);

        var checkPerson = new Person { Email = savedPerson.Email, PersonId = 0 };

        // Act
        var result = await _repository.EmailExistsAsync(checkPerson);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailNotExists()
    {
        // Arrange
        var checkPerson = new Person { Email = "nonexistent@example.com", PersonId = 0 };

        // Act
        var result = await _repository.EmailExistsAsync(checkPerson);

        // Assert
        result.Should().BeFalse();
    }
}