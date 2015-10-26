using System;
using System.Xml.Serialization;

namespace Rock.Repository.Configuration
{
    public class XmlSerializingRockRepositoryConfiguration : IRockRepositoryConfiguration
    {
        private readonly Lazy<IRepositoryFactory> _repositoryFactory;

        public XmlSerializingRockRepositoryConfiguration()
        {
            _repositoryFactory = new Lazy<IRepositoryFactory>(() => FactoryProxy.CreateInstance());
        }

        [XmlElement("repositoryFactory")]
        public RepositoryFactoryProxy FactoryProxy { get; set; }

        [XmlIgnore]
        public IRepositoryFactory RepositoryFactory => _repositoryFactory.Value;
    }
}