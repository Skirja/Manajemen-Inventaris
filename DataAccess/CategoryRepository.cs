using System;
using System.Collections.Generic;
using System.Data;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.DataAccess
{
    /// <summary>
    /// Repository for category operations
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDataAccess _dataAccess;

        /// <summary>
        /// Initializes a new instance of the CategoryRepository class
        /// </summary>
        /// <param name="dataAccess">The data access instance</param>
        public CategoryRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <returns>A list of categories</returns>
        public List<Category> GetAllCategories()
        {
            string sql = "SELECT * FROM Categories ORDER BY Name";
            DataTable dataTable = _dataAccess.ExecuteQuery(sql);

            List<Category> categories = new List<Category>();
            foreach (DataRow row in dataTable.Rows)
            {
                categories.Add(MapRowToCategory(row));
            }

            return categories;
        }

        /// <summary>
        /// Gets a category by ID
        /// </summary>
        /// <param name="categoryId">The category ID</param>
        /// <returns>The category, or null if not found</returns>
        public Category GetCategoryById(int categoryId)
        {
            string sql = "SELECT * FROM Categories WHERE CategoryID = @CategoryID";
            var parameters = new Dictionary<string, object>
            {
                { "@CategoryID", categoryId }
            };

            DataTable dataTable = _dataAccess.ExecuteQuery(sql, parameters);
            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            return MapRowToCategory(dataTable.Rows[0]);
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="category">The category to create</param>
        /// <returns>The ID of the created category</returns>
        public int AddCategory(Category category)
        {
            string sql = @"INSERT INTO Categories (Name, Description, CreatedBy, CreatedDate) 
                           VALUES (@Name, @Description, @CreatedBy, @CreatedDate); 
                           SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@Name", category.Name },
                { "@Description", category.Description ?? (object)DBNull.Value },
                { "@CreatedBy", category.CreatedBy },
                { "@CreatedDate", category.CreatedDate }
            };

            return Convert.ToInt32(_dataAccess.ExecuteScalar(sql, parameters));
        }

        /// <summary>
        /// Updates a category
        /// </summary>
        /// <param name="category">The category to update</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool UpdateCategory(Category category)
        {
            string sql = @"
                UPDATE Categories
                SET Name = @Name,
                    Description = @Description
                WHERE CategoryID = @CategoryID";

            var parameters = new Dictionary<string, object>
            {
                { "@CategoryID", category.CategoryID },
                { "@Name", category.Name },
                { "@Description", category.Description }
            };

            int rowsAffected = _dataAccess.ExecuteNonQuery(sql, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="categoryId">The ID of the category to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool DeleteCategory(int categoryId)
        {
            // First check if there are any items using this category
            string checkSql = "SELECT COUNT(*) FROM Items WHERE CategoryID = @CategoryID";
            var checkParameters = new Dictionary<string, object>
            {
                { "@CategoryID", categoryId }
            };

            int itemCount = Convert.ToInt32(_dataAccess.ExecuteScalar(checkSql, checkParameters));
            if (itemCount > 0)
            {
                // Cannot delete a category that has items
                return false;
            }

            string sql = "DELETE FROM Categories WHERE CategoryID = @CategoryID";
            var parameters = new Dictionary<string, object>
            {
                { "@CategoryID", categoryId }
            };

            int rowsAffected = _dataAccess.ExecuteNonQuery(sql, parameters);
            return rowsAffected > 0;
        }

        private Category MapRowToCategory(DataRow row)
        {
            return new Category
            {
                CategoryID = Convert.ToInt32(row["CategoryID"]),
                Name = row["Name"].ToString(),
                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }
    }
}