using AutoMapperExtensions;
using System;
using Xunit;

namespace AutoMapperExtensionsTest
{
    public class UnitTest1
    {
        public UnitTest1()
        {
            AutoMapperConfig.Init();
        }

        [Fact]
        public void Test()
        {
            var a = new ModelA { Field1 = "test field1" };
            var b = a.MapTo<ModelB>();
            Assert.Equal(a.Field1, b.Field1);

            var c = a.MapTo<ModelC>();
        }

        [Fact]
        public void TestConvertUsing()
        {
            var a = new ModelA();
            var b = a.MapTo<ModelB>();
            var c = a.MapTo<ModelC>();
            b = a.MapTo<ModelB>();
            Assert.Equal(b.Time, c.Time);
        }
    }
}
