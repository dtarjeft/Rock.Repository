namespace Rock.Repository.Sql
{
    public interface ISqlScenarioConfigurationProvider
    {
        ISqlScenarioConfiguration GetConfiguration(string name);
        bool HasConfiguration(string name);
    }
}