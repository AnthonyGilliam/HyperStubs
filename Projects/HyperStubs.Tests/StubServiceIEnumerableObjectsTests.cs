using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using HyperStubs.Tests.Models;

namespace HyperStubs.Tests.StubServiceTests
{
    [TestFixture]
    public class When_any_IEnumerable_object_is_stubbed
    {
        [SetUp]
        public void Context()
        {
            
        }

        [Test]
        public void All_of_the_objects_it_contains_are_fully_hydrated()
        {
            var stubService = new StubService();
            var people = stubService.GetStubbedObject<IEnumerable<SimplePerson>>();

            Assert.NotNull(people);
            Assert.IsNotEmpty(people);
            Assert.That(people.All(person => !string.IsNullOrWhiteSpace(person.FirstName)));
            Assert.That(people.All(person => !string.IsNullOrWhiteSpace(person.MiddleName)));
            Assert.That(people.All(person => !string.IsNullOrWhiteSpace(person.LastName)));
            Assert.That(people.All(person => !string.IsNullOrWhiteSpace(person.NickName)));
            Assert.That(people.All(person => !string.IsNullOrWhiteSpace(person.PhoneNumber)));
            Assert.That(people.All(person => person.Age != 0));
        }

        [Ignore]
        [Test]
        public void Its_descendant_property_depth_count_never_exceeds_the_DataGenerators_MAX_PROPERTY_DEPTH_COUNT_for_one_to_many_objects()
        {
            Assert.Fail("This test has not been writen yet.");
        }
    }
}