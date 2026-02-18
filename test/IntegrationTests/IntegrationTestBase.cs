using IntegrationTests.Helpers;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        protected readonly HttpClient _client;
        protected readonly CustomWebApplicationFactory<Program> _factory;

        protected IntegrationTestBase(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }
    }
}
