using HyperStubs.Tests.Models;
using NUnit.Framework;

namespace HyperStubs.Tests.StubServiceBasicObjectTests
{
    [TestFixture]
    public class When_a_simple_object_is_stubbed
    {
        [Test]
        public void All_its_properties_are_populated()
        {
            var stubService = new StubService();
            var person = stubService.GetStubbedObject<SimplePerson>();

            Assert.NotNull(person);
            Assert.That(!string.IsNullOrWhiteSpace(person.FirstName));
            Assert.That(!string.IsNullOrWhiteSpace(person.LastName));
            Assert.That(!string.IsNullOrWhiteSpace(person.MiddleName));
            Assert.That(!string.IsNullOrWhiteSpace(person.NickName));
            Assert.That(!string.IsNullOrWhiteSpace(person.PhoneNumber));
            Assert.That(person.Age != 0);
        }
    }

    [TestFixture]
    public class When_a_complex_object_is_stubbed
    {
        [Test]
        public void All_its_premative_properties_are_stubbed()
        {
            var stubService = new StubService();
            var person = stubService.GetStubbedObject<ComplexPerson>();

            Assert.NotNull(person);
            Assert.That(!string.IsNullOrWhiteSpace(person.FirstName));
            Assert.That(!string.IsNullOrWhiteSpace(person.LastName));
            Assert.That(!string.IsNullOrWhiteSpace(person.MiddleName));
            Assert.That(!string.IsNullOrWhiteSpace(person.NickName));
            Assert.That(!string.IsNullOrWhiteSpace(person.PhoneNumber));
            Assert.That(person.Age != 0);
        }

        [Test]
        public void Its_first_layer_of_complex_properties_are_stubbed()
        {
            var stubService = new StubService();
            var person = stubService.GetStubbedObject<ComplexPerson>();

            Assert.NotNull(person.House);
            Assert.That(person.House.Bathrooms != 0);
            Assert.That(person.House.Floors != 0);
        }

        [Test]
        public void Its_second_layer_of_complex_properties_are_stubbed()
        {
            var stubService = new StubService();
            var person = stubService.GetStubbedObject<ComplexPerson>();

            Assert.NotNull(person.House.Address);
            Assert.That(!string.IsNullOrWhiteSpace(person.House.Address.Street1));
            Assert.That(!string.IsNullOrWhiteSpace(person.House.Address.Street2));
            Assert.That(!string.IsNullOrWhiteSpace(person.House.Address.City));
            Assert.That(!string.IsNullOrWhiteSpace(person.House.Address.State));
            Assert.That(!string.IsNullOrWhiteSpace(person.House.Address.County));
            Assert.That(!string.IsNullOrWhiteSpace(person.House.Address.Country));
            Assert.That(!string.IsNullOrWhiteSpace(person.House.Address.Zip));
        }
    }
}