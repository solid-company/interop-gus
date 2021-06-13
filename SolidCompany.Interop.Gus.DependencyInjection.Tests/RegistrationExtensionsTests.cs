using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using SolidCompany.Interop.Gus.Connected_Services;

namespace SolidCompany.Interop.Gus.DependencyInjection.Tests
{
    public class RegistrationExtensionsTests
    {
        private IHost host;

        [SetUp]
        public void Setup()
        {
            host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddGusClient(c =>
                    {
                        c.Environment = GusEnironment.Test;
                        c.Key = "abcde12345abcde12345";
                    });
                })
                .Build();
        }

        [TearDown]
        public void TearDown()
        {
            host.Dispose();
        }

        [Test]
        public void Can_resolve_client_from_container()
        {
            using var serviceScope = host.Services.CreateScope();

            var client = serviceScope.ServiceProvider.GetRequiredService<IGusBirClient>();

            Assert.That(client, Is.Not.Null);
        }

        [Test]
        public async Task Can_find_legal_entity_by_NIP()
        {
            using var serviceScope = host.Services.CreateScope();

            var client = serviceScope.ServiceProvider.GetRequiredService<IGusBirClient>();

            var entity = await client.FindByNipAsync("5261040828");

            Assert.That(entity, Is.Not.Null);
        }
    }
}