using System;
using System.Configuration;
using Rock.Immutable;
using Rock.Repository.Configuration;

namespace Rock.Repository
{
    public static class RepositoryFactoryProvider
    {
        private static readonly Semimutable<IRepositoryFactory> _current = new Semimutable<IRepositoryFactory>(GetDefault);

        public static IRepositoryFactory Current
        {
            get { return _current.Value; }
            internal set { _current.Value = value; }
        }

        public static void SetCurrent(IRepositoryFactory repository)
        {
            _current.SetValue(() => repository ?? GetDefault());
        }

        private static IRepositoryFactory GetDefault()
        {
            IRepositoryFactory value;

            return TryGetFactoryFromConfig(out value)
                ? value
                : null;
        }
        private static bool TryGetFactoryFromConfig(out IRepositoryFactory factory)
        {
            try
            {
                var rockRepositoryConfiguration =
                    (IRockRepositoryConfiguration)ConfigurationManager.GetSection("rock.repository");
                factory = rockRepositoryConfiguration.RepositoryFactory;
                return true;
            }
            catch (Exception)
            {
                factory = null;
                return false;
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="IRepository"/> that uses the calling app's configuration.
        /// </summary>
        /// <param name="name">The name of the repository.</param>
        /// <returns>An instance of <see cref="IRepository"/>that uses the calling app's configuration.</returns>
        public static IRepository Create(string name)
        {
            return Current.Create(name);
        }

        /// <summary>
        /// Indicates whether this <see cref="IRepositoryFactory"/> is configured to produce <see cref="IRepository"/> objects linked to <see cref="name"/>
        /// </summary>
        /// <param name="name">The name of the connection to make.</param>
        /// <returns>True, if the <see cref="IRepository"/> connection can be made, otherwise false.</returns>
        public static bool CanCreate(string name)
        {
            return Current.CanCreate(name);
        }
    }
}