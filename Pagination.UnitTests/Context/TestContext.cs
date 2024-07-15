using Microsoft.EntityFrameworkCore;
using Pagination.UnitTests.Models;

namespace Pagination.UnitTests.Context
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options) : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(TestContext).Assembly);

            base.OnModelCreating(builder);
        }
    }
}
