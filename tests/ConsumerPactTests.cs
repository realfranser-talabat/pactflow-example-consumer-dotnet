using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using PactNet;
using Consumer;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using PactNet.Matchers;
using FluentAssertions;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using System.Threading.Tasks;

namespace tests
{
    public class ConsumerPactTests
    {
        private const string BurgerProductId = "27";

        private readonly IPactBuilderV3 _pact;
        // private readonly int port = 9222;

        private readonly List<object> _products;

        public ConsumerPactTests(ITestOutputHelper output)
        {

            _products = new List<object>()
            {
                new { id = BurgerProductId, name = "burger", type = "food" },
                new { id = "111", name = "pizza", type = "food" },
            };

            var config = new PactConfig
            {
                PactDir = Path.Join("..", "..", "..", "..", "pacts"),
                Outputters = new List<IOutput> { new XunitOutput(output), new ConsoleOutput() },
                LogLevel = PactLogLevel.Debug
            };

            _pact = Pact.V3("pactflow-example-consumer-dotnet", "pactflow-example-provider-dotnet", config).WithHttpInteractions();
        }

        [Fact]
        public async Task RetrieveProducts()
        {
            // Arrange
            _pact.UponReceiving("A request to get products")
                        .Given("products exist")
                        .WithRequest(HttpMethod.Get, "/products")
                    .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(_products);

            await _pact.VerifyAsync(async ctx =>
            {
                // Act
                var consumer = new ProductClient();
                List<Product> result = await consumer.GetProducts(ctx.MockServerUri.ToString().TrimEnd('/'));
                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                Assert.Equal(BurgerProductId,result[0].id);
                Assert.Equal("burger",result[0].name);
                Assert.Equal("food",result[0].type);
            });
        }
        
        [Fact]
        public async Task RetrieveProductById()
        {
            // Arrange
            _pact.UponReceiving("A request to get product by id")
                        .Given("products exist")
                        .WithRequest(HttpMethod.Get, $"/product/{BurgerProductId}")
                    .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(_products[0]);

            await _pact.VerifyAsync(async ctx =>
            {
                // Act
                var consumer = new ProductClient();
                var result = await consumer.GetProductById(ctx.MockServerUri.ToString().TrimEnd('/'), BurgerProductId);
                // Assert
                result.Should().NotBeNull();
                Assert.Equal(BurgerProductId, result.id);
                Assert.Equal("burger",result.name);
                Assert.Equal("food",result.type);
            });
        }
    }
}
