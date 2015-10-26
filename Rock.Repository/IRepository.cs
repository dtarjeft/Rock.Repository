using System.Collections.Generic;

namespace Rock.Repository
{
    public interface IRepository : ILoad, ISave, ICustomRepo, IDelete
    {

    }
    public interface ILoad
    {
        /// <summary>
        /// Loads a single <typeparam name="T"/> object populated with information from the repository.
        /// </summary>
        /// <typeparam name="T">The type of the object to load from the repository.</typeparam>
        /// <param name="searchParams">Parameters to refine the search.</param>
        /// <returns>A single <typeparam name="T" /> populated with information that corresponds to <see cref="searchParams"/>.</returns>
        /// <remarks><see cref="searchParams"/> Keys should be the name of the field that corresponds to a given <see cref="IRepository"/>'s specific structure.
        /// Value should be the value of that field.</remarks>
        T Load<T>(IDictionary<string, object> searchParams) where T : IDataObject;

        /// <summary>
        /// Loads multiple <typeparam name="T" /> objects populated with information from the repository.
        /// </summary>
        /// <typeparam name="T">The type of the object to load from the repository.</typeparam>
        /// <param name="searchParams">Parameters to refine the search.</param>
        /// <returns>A single <typeparam name="T" /> populated with information that corresponds to <see cref="searchParams"/>.</returns>
        /// <remarks><see cref="searchParams"/> Keys should be the name of the field that corresponds to a given <see cref="IRepository"/>'s specific structure.
        /// Value should be the value of that field.</remarks>
        IList<T> LoadMultiple<T>(IDictionary<string, object> searchParams) where T : IDataObject;
    }
    public interface ISave
    {
        T Save<T>(T toSave) where T : IDataObject;
        int SaveMultiple<T>(IEnumerable<T> toSave) where T : IDataObject;
    }
    public interface ICustomRepo
    {
        IList<T> Custom<T>(string searchMethod, IDictionary<string, object> searchParams);
    }
    public interface IDelete
    {
        void Delete<T>(T toDelete) where T : IAmDeletable;
    }
}