using System;

namespace Rock.Repository.Sql
{
    public class SqlRepositoryFactory : IRepositoryFactory
    {
        public ISqlScenarioConfigurationProvider ConfigurationProvider { get; }

        public SqlRepositoryFactory(ISqlScenarioConfigurationProvider configurationProvider)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException(nameof(configurationProvider));
            }
            ConfigurationProvider = configurationProvider;
        }

        public SqlRepositoryFactory(XmlDeserializingSqlScenarioConfigurationProvider sqlSettings)
            : this((ISqlScenarioConfigurationProvider) sqlSettings) // Interestingly, this is where the app.config name constraint on the second-lowest tag (the one above <sql>) comes from.
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="IRepository"/> that uses the calling app's configuration.
        /// </summary>
        /// <param name="name">The name of the repository.</param>
        /// <returns>An instance of <see cref="IRepository"/>that uses the calling app's configuration.</returns>
        public IRepository Create(string name)
        {
            var config = ConfigurationProvider.GetConfiguration(name);
            return new SqlRepository(config);
        }

        /// <summary>
        /// Indicates whether this <see cref="IRepositoryFactory"/> is configured to produce <see cref="IRepository"/> objects linked to <see cref="name"/>
        /// </summary>
        /// <param name="name">The name of the connection to make.</param>
        /// <returns>True, if the <see cref="IRepository"/> connection can be made, otherwise false.</returns>
        public bool CanCreate(string name)
        {
            return ConfigurationProvider.HasConfiguration(name);
        }
    }
}