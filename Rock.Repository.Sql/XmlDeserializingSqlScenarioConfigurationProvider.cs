using System;
using System.Linq;
using System.Xml.Serialization;

namespace Rock.Repository.Sql
{
    public class XmlDeserializingSqlScenarioConfigurationProvider : ISqlScenarioConfigurationProvider
    {
        private SqlScenarioConfiguration[] _configurations = new SqlScenarioConfiguration[0];

        [XmlElement("sql")]
        public SqlScenarioConfiguration[] Configurations
        {
            get { return _configurations; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _configurations = value;
            }
        }

        public ISqlScenarioConfiguration GetConfiguration(string name)
        {
            return _configurations.First(c => c.Name == name);
        }

        public bool HasConfiguration(string name)
        {
            return _configurations.Any(c => c.Name == name);
        }
    }
}