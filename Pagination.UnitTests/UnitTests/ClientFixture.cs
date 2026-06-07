using Bogus;
using MockQueryable;
using Pagination.UnitTests.Models;

namespace Pagination.UnitTests.UnitTests;

public class ClientFixture
{
    public IQueryable<Client> ClientsQueryable { get; }

    public ClientFixture()
    {
        var faker = new Faker<Client>()
            .RuleFor(c => c.Id, f => f.IndexFaker + 1)
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName));

        var clients = faker.Generate(1_000);

        ClientsQueryable = clients.BuildMock();
    }
}
