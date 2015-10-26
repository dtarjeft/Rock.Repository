using NUnit.Framework;

namespace Rock.Repository.Tests
{
    [TestFixture]
    public class Repos
    {
        [TestCase("rubiks")]
        [TestCase("atlas")]
        public void LoadFromConfig(string toLoad)
        {
            Assert.That(RepositoryFactoryProvider.CanCreate(toLoad));
            Assert.That(RepositoryFactoryProvider.Create(toLoad) != null);
        }
    }
}