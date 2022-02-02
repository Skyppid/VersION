using DryIoc;
using NUnit.Framework;
using Spectre.Console.Testing;
using VersION;

namespace Tests.VersION
{
    [TestFixture]
    public sealed class TypeRegistrarTests
    {
        [Test]
        public void TestRegistrar()
        {
            TypeRegistrarBaseTests tests =
                new TypeRegistrarBaseTests(() => new TypeRegistrar(new Container(Rules.Default)));
            Assert.That(() => tests.RunAllTests(), Throws.Nothing);
        }
    }
}