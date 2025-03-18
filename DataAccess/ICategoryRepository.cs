using System.Collections.Generic;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.DataAccess
{
    /// <summary>
    /// Interface for category repository operations
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <returns>A list of categories</returns>
        List<Category> GetAllCategories();

        /// <summary>
        /// Gets a category by ID
        /// </summary>
        /// <param name="categoryId">The category ID</param>
        /// <returns>The category, or null if not found</returns>
        Category GetCategoryById(int categoryId);

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="category">The category to create</param>
        /// <returns>The ID of the created category</returns>
        int AddCategory(Category category);

        /// <summary>
        /// Updates a category
        /// </summary>
        /// <param name="category">The category to update</param>
        /// <returns>True if successful, false otherwise</returns>
        bool UpdateCategory(Category category);

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="categoryId">The ID of the category to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        bool DeleteCategory(int categoryId);
    }
}