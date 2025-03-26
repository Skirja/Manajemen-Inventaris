using System.Collections.Generic;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Pages.Dashboard.Inventory;
using System;

namespace Manajemen_Inventaris.Services
{
    /// <summary>
    /// Interface for inventory service operations
    /// </summary>
    public interface IInventoryService
    {
        #region Category Operations

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
        /// Gets all categories with their item counts
        /// </summary>
        /// <returns>A list of categories with item counts</returns>
        List<CategoryWithItemCount> GetCategoriesWithItemCount();

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

        #endregion

        #region Item Operations

        /// <summary>
        /// Gets all items
        /// </summary>
        /// <returns>A list of items</returns>
        List<Item> GetAllItems();

        /// <summary>
        /// Gets items by category ID
        /// </summary>
        /// <param name="categoryId">The category ID</param>
        /// <returns>A list of items in the specified category</returns>
        List<Item> GetItemsByCategoryId(int categoryId);

        /// <summary>
        /// Gets an item by ID
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <returns>The item, or null if not found</returns>
        Item GetItemById(int itemId);

        /// <summary>
        /// Searches for items by name or description
        /// </summary>
        /// <param name="searchTerm">The search term</param>
        /// <returns>A list of matching items</returns>
        List<Item> SearchItems(string searchTerm);

        /// <summary>
        /// Creates a new item
        /// </summary>
        /// <param name="item">The item to create</param>
        /// <returns>The ID of the created item</returns>
        int CreateItem(Item item);

        /// <summary>
        /// Updates an item
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <returns>True if successful, false otherwise</returns>
        bool UpdateItem(Item item);

        /// <summary>
        /// Deletes an item
        /// </summary>
        /// <param name="itemId">The ID of the item to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        bool DeleteItem(int itemId);

        /// <summary>
        /// Adjusts the quantity of an item
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <param name="quantityChange">The quantity change (positive for stock in, negative for stock out)</param>
        /// <param name="notes">Notes about the adjustment</param>
        /// <param name="userId">The ID of the user making the adjustment</param>
        /// <returns>True if successful, false otherwise</returns>
        bool AdjustItemQuantity(int itemId, int quantityChange, string notes, int userId);

        /// <summary>
        /// Gets the history for an item
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <returns>A list of history records for the item</returns>
        List<ItemHistory> GetItemHistory(int itemId);

        /// <summary>
        /// Adds a history record for an item
        /// </summary>
        /// <param name="history">The history record to add</param>
        /// <returns>The ID of the created history record</returns>
        int AddItemHistory(ItemHistory history);

        /// <summary>
        /// Gets low stock items
        /// </summary>
        /// <returns>A list of items with stock below the threshold</returns>
        List<Item> GetLowStockItems();

        #endregion

        #region Dashboard Operations

        /// <summary>
        /// Gets items added in the last X days
        /// </summary>
        /// <param name="days">The number of days</param>
        /// <returns>A list of items added in the last X days</returns>
        List<Item> GetRecentItemsByDays(int days);

        /// <summary>
        /// Gets the most recent activity
        /// </summary>
        /// <param name="count">The number of records to retrieve</param>
        /// <returns>A list of the most recent activity</returns>
        List<ItemHistory> GetRecentItemHistory(int count);

        #endregion

        #region Reports Operations

        /// <summary>
        /// Gets inventory movement statistics for a date range
        /// </summary>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <returns>Dictionary with movement statistics</returns>
        Dictionary<string, int> GetInventoryMovementStats(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets category distribution statistics
        /// </summary>
        /// <returns>List of categories with their item counts</returns>
        List<CategoryWithItemCount> GetCategoryDistribution();

        /// <summary>
        /// Gets inventory movements grouped by date
        /// </summary>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <param name="categoryId">Optional category ID filter</param>
        /// <returns>Dictionary with dates and movement counts</returns>
        Dictionary<DateTime, int> GetInventoryMovementsByDate(DateTime startDate, DateTime endDate, int categoryId = 0);

        /// <summary>
        /// Gets AI recognition statistics
        /// </summary>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <returns>Dictionary with AI recognition statistics</returns>
        Dictionary<string, int> GetAIRecognitionStats(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets item history for reporting with filtering
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="categoryId">Optional category ID filter</param>
        /// <param name="changeType">Optional change type filter</param>
        /// <returns>A filtered list of item history records</returns>
        List<ItemHistory> GetItemHistoryForReporting(DateTime startDate, DateTime endDate, int categoryId = 0, string changeType = null);

        #endregion
    }
}