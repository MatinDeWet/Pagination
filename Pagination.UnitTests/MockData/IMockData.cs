using Pagination.UnitTests.Context;

namespace Pagination.UnitTests.MockData
{
    public interface IMockData
    {
        public void Seed(TestContext db);
    }
}
