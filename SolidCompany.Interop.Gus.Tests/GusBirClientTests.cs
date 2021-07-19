using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SolidCompany.Interop.Gus.Connected_Services;

namespace SolidCompany.Interop.Gus.Tests
{
    public class GusBirClientTests
    {
        private GusBirClient client;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            client = new GusBirClient("abcde12345abcde12345", GusEnironment.Test);
        }

#if NET5_0
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await client.DisposeAsync();
        }
#else
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            client.Dispose();
        }
#endif

        [Test]
        public async Task Can_find_legal_entity_by_NIP()
        {
            var result = await client.FindByNipAsync("5261040828");

            Assert.That(result.Success, Is.True);
            Assert.That(result.Entities, Is.Not.Null);
        }

        [Test]
        public async Task Can_find_legal_entity_by_REGON()
        {
            var result = await client.FindByRegonAsync("000331501");

            Assert.That(result.Success, Is.True);
            Assert.That(result.Entities, Is.Not.Null);
        }

        [Test]
        public async Task Can_find_legal_entity_by_KRS()
        {
            var result = await client.FindByKrsAsync("0000023302");

            Assert.That(result.Success, Is.True);
            Assert.That(result.Entities, Is.Not.Null);
        }

        [Test]
        public async Task Cant_find_not_existing_entity()
        {
            var result = await client.FindByNipAsync("1231231212");

            Assert.That(result.Success, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        }

        [Test]
        public async Task All_fields_are_equal_to_what_is_expected()
        {
            var result = await client.FindByNipAsync("5261040828");

            var entity = result.Entities.Single();

            Assert.That(entity.Regon, Is.EqualTo("000331501"));
            Assert.That(entity.Nip, Is.EqualTo("5261040828"));
            Assert.That(entity.Name, Is.EqualTo("GŁÓWNY URZĄD STATYSTYCZNY"));
            Assert.That(entity.Voivodeship, Is.EqualTo("MAZOWIECKIE"));
            Assert.That(entity.District, Is.EqualTo("m. st. Warszawa"));
            Assert.That(entity.Commune, Is.EqualTo("Śródmieście"));
            Assert.That(entity.City, Is.EqualTo("Warszawa"));
            Assert.That(entity.PostalCode, Is.EqualTo("00-925"));
            Assert.That(entity.Post, Is.EqualTo("Warszawa"));
            Assert.That(entity.Street, Is.EqualTo("ul. Test-Krucza"));
            Assert.That(entity.BuildingNumber, Is.EqualTo("208"));
            Assert.That(entity.LocalNumber, Is.Null);
        }
    }
}