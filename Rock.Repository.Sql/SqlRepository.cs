using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Rock.Repository.Sql
{
    internal class SqlRepository : IRepository
    {
        private string ConnStr { get; }
        public string Name { get; }

        public SqlRepository(ISqlScenarioConfiguration configuration)
        {
            Name = configuration.Name;
            ConnStr = configuration.ConnString;
        }
        private SqlConnection Conn()
        {
            return new SqlConnection(ConnStr);
        }

        #region Custom

        public IList<T> Custom<T>(string searchMethod, IDictionary<string, object> searchParams = null)
        {
            using (var sqlConnection = Conn())
            {
                sqlConnection.Open();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        return
                            sqlConnection.Query<T>(searchMethod,
                                searchParams, transaction,
                                commandType: CommandType.StoredProcedure).ToList();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Load

        public IList<T> LoadMultiple<T>(IDictionary<string, object> searchParams) where T : IDataObject
        {
            using (var sqlConnection = Conn())
            {
                sqlConnection.Open();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var tablename = typeof(T).Name;

                        return Load<T>($"dbo.{tablename}_GetList", searchParams, transaction, sqlConnection);
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public T Load<T>(IDictionary<string, object> searchParams) where T : IDataObject
        {
            using (var sqlConnection = Conn())
            {
                sqlConnection.Open();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var tablename = typeof(T).Name;

                        return Load<T>($"dbo.{tablename}_GetInfo", searchParams, transaction, sqlConnection).Single();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private IList<T> Load<T>(string sProc, IDictionary<string, object> primaryKeyValue, SqlTransaction transaction, SqlConnection sqlConnection) where T : IDataObject
        {
            return sqlConnection.Query<T>(sProc, primaryKeyValue, transaction, commandType: CommandType.StoredProcedure).ToList();
        }
        #endregion

        #region Save

        public T Save<T>(T toSave) where T : IDataObject
        {
            using (var sqlConnection = Conn())
            {
                sqlConnection.Open();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var back = Save(toSave, transaction, sqlConnection);
                        transaction.Commit();
                        return back;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private T Save<T>(T toSave, SqlTransaction transaction, SqlConnection sqlConnection) where T : IDataObject
        {
            var sProc = "dbo." + typeof(T).Name + "_Save";
            var props = typeof(T).GetProperties();

            var paramsToSave = new DynamicParameters();
            // TODO: Combine these?
            foreach (var property in props.Where(prop => !prop.GetCustomAttributes().Any() || prop.GetCustomAttribute<SaveAsStringAttribute>() != null))
            {
                paramsToSave.Add(property.Name, property.GetValue(toSave) == null ? null : property.GetValue(toSave).ToString());
            }
            foreach (var property in props.Where(property => property.GetCustomAttribute<SaveAsClassNameAttribute>() != null))
            {
                paramsToSave.Add(toSave.GetType().Name, property.GetValue(toSave) == null ? null : property.GetValue(toSave).ToString());
            }
            return sqlConnection.Query<T>(sProc, paramsToSave, transaction, commandType: CommandType.StoredProcedure).SingleOrDefault();
        }

        public int SaveMultiple<T>(IEnumerable<T> toSave) where T : IDataObject
        {
            var tableVp = ToDataTable(toSave);

            using (var sqlConnection = Conn())
            {
                sqlConnection.Open();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var back = SaveMultiple<T>(tableVp, transaction, sqlConnection);
                        transaction.Commit();
                        return back;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private int SaveMultiple<T>(DataTable toSave, SqlTransaction transaction, SqlConnection sqlConnection)
            where T : IDataObject
        {
            var sProc = "dbo." + typeof(T).Name + "_SaveList";
            return sqlConnection.Execute(sProc, new Dictionary<string, object> { { typeof(T).Name, toSave.AsTableValuedParameter("dbo." + typeof(T).Name + "Type") } }, transaction, commandType: CommandType.StoredProcedure);
        }

        private DataTable ToDataTable<T>(IEnumerable<T> toTable) where T : IDataObject
        {
            var dt = new DataTable();
            var properties = typeof(T).GetProperties();
            foreach (
                var t in
                    properties.Where(
                        t =>
                            t.GetCustomAttribute<NoSaveAttribute>() == null &&
                            t.GetCustomAttribute<NoTableSaveAttribute>() == null))
            {
                var type = t.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = Nullable.GetUnderlyingType(type);
                }
                dt.Columns.Add(t.Name, type);
            }
            foreach (var t in toTable)
            {
                var row = dt.NewRow();
                foreach (DataColumn column in dt.Columns)
                {
                    var content = properties.Single(property => string.Equals(property.Name, column.ColumnName)).GetValue(t);
                    row[column] = content ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }
            dt.AcceptChanges();
            return dt;
        }

        #endregion

        #region Delete
        public void Delete<T>(T toSave) where T : IAmDeletable
        {
            using (var sqlConnection = Conn())
            {
                sqlConnection.Open();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        Delete(toSave, transaction, sqlConnection);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void Delete<T>(T toDelete, SqlTransaction transaction, SqlConnection sqlConnection) where T : IAmDeletable
        {
            var sProc = "dbo." + toDelete.GetType().Name + "_Save";
            var props = typeof(T).GetProperties();

            var paramsToSave = new DynamicParameters();
            // Combine these two later.
            foreach (var property in props.Where(prop => !prop.GetCustomAttributes().Any() || prop.GetCustomAttribute<SaveAsStringAttribute>() != null))
            {
                paramsToSave.Add(property.Name, property.GetValue(toDelete) == null ? null : property.GetValue(toDelete).ToString());
            }
            foreach (var property in props.Where(property => property.GetCustomAttribute<SaveAsStringAttribute>() != null))
            {
                paramsToSave.Add(property.Name, property.GetValue(toDelete) == null ? null : property.GetValue(toDelete).ToString());
            }
            paramsToSave.Add("IsDelete", toDelete.IsDelete); // If this accidentally gets called (somehow?) IsDelete defaults to false.
            sqlConnection.Query<T>(sProc, paramsToSave, transaction, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }
}