namespace Rock.Repository.Sql
{
    public interface ISqlScenarioConfiguration
    {
        string Name { get; set; }
        string ConnString { get; set; }
    }
}