using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using HyperStubs.Tests.Models;

namespace HyperStubs.Tests.StubServiceRandomObjectsTests
{
    [TestFixture]
    public class When_a_randomly_generated_nullable_object_with_all_primitive_types_is_stubbed
    {
        [SetUp]
        public void context()
        {
            
        }

        [Test]
        public void An_object_with_all_its_properties_stubbed_is_returned()
        {
            var stubService = new StubService();
            var obj = stubService.GetStubbedObject<NullablePrimitiveThing>();

            //TODO:  Make Randomizer methods for all primitive types
            Assert.NotNull(obj.Boolean);
            Assert.NotNull(obj.Byte);
            Assert.NotNull(obj.Char);
            Assert.NotNull(obj.DateTime);
            Assert.NotNull(obj.Decimal);
            Assert.NotNull(obj.Double);
            Assert.NotNull(obj.Float);
            Assert.NotNull(obj.Int16);
            Assert.NotNull(obj.Int32);
            Assert.NotNull(obj.Int64);
            Assert.NotNull(obj.IntPtr);
            Assert.NotNull(obj.SignedByte);
            Assert.NotNull(obj.Single);
            Assert.NotNull(obj.String);
            Assert.NotNull(obj.UnsignedInt16);
            Assert.NotNull(obj.UnsignedInt32);
            Assert.NotNull(obj.UnsignedInt64);
            Assert.NotNull(obj.UnsignedIntPtr);
        }
    }
}
