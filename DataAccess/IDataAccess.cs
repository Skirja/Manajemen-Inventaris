using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Manajemen_Inventaris.DataAccess
{
    /// <summary>
    /// Interface for database operations
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        /// Executes a SQL query that returns a result set
        /// </summary>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="parameters">The parameters for the SQL query</param>
        /// <returns>A DataTable containing the result set</returns>
        DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a SQL command that does not return a result set
        /// </summary>
        /// <param name="sql">The SQL command to execute</param>
        /// <param name="parameters">The parameters for the SQL command</param>
        /// <returns>The number of rows affected</returns>
        int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a SQL query that returns a single value
        /// </summary>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="parameters">The parameters for the SQL query</param>
        /// <returns>The first column of the first row in the result set</returns>
        object ExecuteScalar(string sql, Dictionary<string, object> parameters = null);
    }
}