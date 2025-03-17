using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Manajemen_Inventaris.DataAccess
{

    public class SqlDataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public SqlDataAccess()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public SqlDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    DataTable dataTable = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    try
                    {
                        connection.Open();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    try
                    {
                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    try
                    {
                        connection.Open();
                        return command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
                        throw;
                    }
                }
            }
        }
    }
}