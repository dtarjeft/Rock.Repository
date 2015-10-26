namespace Rock.Repository
{
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IRepository"/> that uses the calling app's configuration.
        /// </summary>
        /// <param name="name">The name of the repository.</param>
        /// <returns>An instance of <see cref="IRepository"/>that uses the calling app's configuration.</returns>
        IRepository Create(string name);

        /// <summary>
        /// Indicates whether this <see cref="IRepositoryFactory"/> is configured to produce <see cref="IRepository"/> objects linked to <see cref="name"/>
        /// </summary>
        /// <param name="name">The name of the connection to make.</param>
        /// <returns>True, if the <see cref="IRepository"/> connection can be made, otherwise false.</returns>
        bool CanCreate(string name);
    }
}