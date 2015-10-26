using System.Xml.Serialization;

namespace Rock.Repository.Sql
{
    public class SqlScenarioConfiguration : ISqlScenarioConfiguration
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("connString")]
        public string ConnString { get; set; }
    }
}