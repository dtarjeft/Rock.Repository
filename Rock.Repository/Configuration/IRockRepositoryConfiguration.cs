namespace Rock.Repository.Configuration
{
    public interface IRockRepositoryConfiguration
    {
        IRepositoryFactory RepositoryFactory { get; }
    }
}