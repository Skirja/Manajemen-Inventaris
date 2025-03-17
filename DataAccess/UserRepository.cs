using System;
using System.Collections.Generic;
using System.Data;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.DataAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataAccess _dataAccess;

        public UserRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public User GetById(int userId)
        {
            string sql = "SELECT UserID, Username, Password, Email, Company FROM Users WHERE UserID = @UserID";
            var parameters = new Dictionary<string, object>
            {
                { "@UserID", userId }
            };

            DataTable result = _dataAccess.ExecuteQuery(sql, parameters);

            if (result.Rows.Count == 0)
            {
                return null;
            }

            return MapRowToUser(result.Rows[0]);
        }

        public User GetByUsername(string username)
        {
            string sql = "SELECT UserID, Username, Password, Email, Company FROM Users WHERE Username = @Username";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            DataTable result = _dataAccess.ExecuteQuery(sql, parameters);

            if (result.Rows.Count == 0)
            {
                return null;
            }

            return MapRowToUser(result.Rows[0]);
        }

        public User ValidateUser(string username, string password)
        {
            string sql = "SELECT UserID, Username, Password, Email, Company FROM Users WHERE Username = @Username AND Password = @Password";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", username },
                { "@Password", password }
            };

            DataTable result = _dataAccess.ExecuteQuery(sql, parameters);

            if (result.Rows.Count == 0)
            {
                return null;
            }

            return MapRowToUser(result.Rows[0]);
        }

        public bool UsernameExists(string username)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", username }
            };

            object result = _dataAccess.ExecuteScalar(sql, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public int Create(User user)
        {
            string sql = @"
                INSERT INTO Users (Username, Password, Email, Company)
                VALUES (@Username, @Password, @Email, @Company);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Password", user.Password },
                { "@Email", user.Email },
                { "@Company", user.Company }
            };

            object result = _dataAccess.ExecuteScalar(sql, parameters);
            return Convert.ToInt32(result);
        }

        public bool Update(User user)
        {
            string sql = @"
                UPDATE Users
                SET Username = @Username,
                    Password = @Password,
                    Email = @Email,
                    Company = @Company
                WHERE UserID = @UserID";

            var parameters = new Dictionary<string, object>
            {
                { "@UserID", user.UserID },
                { "@Username", user.Username },
                { "@Password", user.Password },
                { "@Email", user.Email },
                { "@Company", user.Company }
            };

            int rowsAffected = _dataAccess.ExecuteNonQuery(sql, parameters);
            return rowsAffected > 0;
        }

        public bool Delete(int userId)
        {
            string sql = "DELETE FROM Users WHERE UserID = @UserID";
            var parameters = new Dictionary<string, object>
            {
                { "@UserID", userId }
            };

            int rowsAffected = _dataAccess.ExecuteNonQuery(sql, parameters);
            return rowsAffected > 0;
        }

        private User MapRowToUser(DataRow row)
        {
            return new User
            {
                UserID = Convert.ToInt32(row["UserID"]),
                Username = row["Username"].ToString(),
                Password = row["Password"].ToString(),
                Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : null,
                Company = row["Company"] != DBNull.Value ? row["Company"].ToString() : null
            };
        }
    }
}