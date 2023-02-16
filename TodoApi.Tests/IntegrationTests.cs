using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace TodoApi.Tests
{
    public class IntegrationTests
    {
        private TestServer _server;

        public HttpClient Client { get; private set; }

        public IntegrationTests()
        {
            SetUpClient();
        }

        [Fact]
        // Checking Email
        public async Task Test1()
        {
            var response = await Client.PostAsync(
                "/api/todoitems",
                new StringContent("name=test", Encoding.UTF8, "application/json")
            );

            response.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
        }

        private void SetUpClient()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {

                });

            _server = new TestServer(builder);

            Client = _server.CreateClient();
        }
    }
}
