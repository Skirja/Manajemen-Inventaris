using System;
using System.Collections.Generic;
using System.Linq;
using Manajemen_Inventaris.DataAccess;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.Services
{
    /// <summary>
    /// Service for inventory operations
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IItemRepository _itemRepository;
        private const int DEFAULT_LOW_STOCK_THRESHOLD = 5;

        /// <summary>
        /// Initializes a new instance of the InventoryService class
        /// </summary>
        /// <param name="categoryRepository">The category repository</param>
        /// <param name="itemRepository">The item repository</param>
        public InventoryService(ICategoryRepository categoryRepository, IItemRepository itemRepository)
        {
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
        }

        #region Category Operations

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <returns>A list of categories</returns>
        public List<Category> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        /// <summary>
        /// Gets a category by ID
        /// </summary>
        /// <param name="categoryId">The category ID</param>
        /// <returns>The category, or null if not found</returns>
        public Category GetCategoryById(int categoryId)
        {
            return _categoryRepository.GetCategoryById(categoryId);
        }

        /// <summary>
        /// Gets all categories with their item counts
        /// </summary>
        /// <returns>A list of categories with item counts</returns>
        public List<CategoryWithItemCount> GetCategoriesWithItemCount()
        {
            // Get all categories
            var categories = _categoryRepository.GetAllCategories();

            // Get all items to count by category
            var items = _itemRepository.GetAllItems();

            // Group items by category and count
            var itemCounts = items
                .GroupBy(i => i.CategoryID)
                .ToDictionary(g => g.Key, g => g.Count());

            // Map to CategoryWithItemCount
            var result = categories.Select(c => new CategoryWithItemCount
            {
                CategoryID = c.CategoryID,
                Name = c.Name,
                Description = c.Description,
                ItemCount = itemCounts.ContainsKey(c.CategoryID) ? itemCounts[c.CategoryID] : 0
            }).ToList();

            return result;
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="category">The category to create</param>
        /// <returns>The ID of the created category</returns>
        public int AddCategory(Category category)
        {
            return _categoryRepository.AddCategory(category);
        }

        /// <summary>
        /// Updates a category
        /// </summary>
        /// <param name="category">The category to update</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool UpdateCategory(Category category)
        {
            return _categoryRepository.UpdateCategory(category);
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="categoryId">The ID of the category to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool DeleteCategory(int categoryId)
        {
            return _categoryRepository.DeleteCategory(categoryId);
        }

        #endregion

        #region Item Operations

        /// <summary>
        /// Gets all items
        /// </summary>
        /// <returns>A list of items</returns>
        public List<Item> GetAllItems()
        {
            return _itemRepository.GetAllItems();
        }

        /// <summary>
        /// Gets items by category ID
        /// </summary>
        /// <param name="categoryId">The category ID</param>
        /// <returns>A list of items in the specified category</returns>
        public List<Item> GetItemsByCategoryId(int categoryId)
        {
            return _itemRepository.GetItemsByCategoryId(categoryId);
        }

        /// <summary>
        /// Gets an item by ID
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <returns>The item, or null if not found</returns>
        public Item GetItemById(int itemId)
        {
            return _itemRepository.GetItemById(itemId);
        }

        /// <summary>
        /// Searches for items by name or description
        /// </summary>
        /// <param name="searchTerm">The search term</param>
        /// <returns>A list of matching items</returns>
        public List<Item> SearchItems(string searchTerm)
        {
            return _itemRepository.SearchItems(searchTerm);
        }

        /// <summary>
        /// Creates a new item
        /// </summary>
        /// <param name="item">The item to create</param>
        /// <returns>The ID of the created item</returns>
        public int CreateItem(Item item)
        {
            return _itemRepository.CreateItem(item);
        }

        /// <summary>
        /// Updates an item
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool UpdateItem(Item item)
        {
            return _itemRepository.UpdateItem(item);
        }

        /// <summary>
        /// Deletes an item
        /// </summary>
        /// <param name="itemId">The ID of the item to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool DeleteItem(int itemId)
        {
            return _itemRepository.DeleteItem(itemId);
        }

        /// <summary>
        /// Adjusts the quantity of an item
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <param name="quantityChange">The quantity change (positive for stock in, negative for stock out)</param>
        /// <param name="notes">Notes about the adjustment</param>
        /// <param name="userId">The ID of the user making the adjustment</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool AdjustItemQuantity(int itemId, int quantityChange, string notes, int userId)
        {
            return _itemRepository.AdjustItemQuantity(itemId, quantityChange, notes, userId);
        }

        /// <summary>
        /// Gets the history for an item
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <returns>A list of history records for the item</returns>
        public List<ItemHistory> GetItemHistory(int itemId)
        {
            return _itemRepository.GetItemHistory(itemId);
        }

        /// <summary>
        /// Adds a history record for an item
        /// </summary>
        /// <param name="history">The history record to add</param>
        /// <returns>The ID of the created history record</returns>
        public int AddItemHistory(ItemHistory history)
        {
            return _itemRepository.AddItemHistory(history);
        }

        /// <summary>
        /// Gets low stock items
        /// </summary>
        /// <param name="threshold">The low stock threshold</param>
        /// <returns>A list of items with stock below the threshold</returns>
        public List<Item> GetLowStockItems(int threshold)
        {
            return _itemRepository.GetLowStockItems(threshold);
        }

        /// <summary>
        /// Gets low stock items using the default threshold
        /// </summary>
        /// <returns>A list of items with stock below the default threshold</returns>
        public List<Item> GetLowStockItems()
        {
            return _itemRepository.GetLowStockItems(DEFAULT_LOW_STOCK_THRESHOLD);
        }

        #endregion

        #region Dashboard Operations

        /// <summary>
        /// Gets items added in the last X days
        /// </summary>
        /// <param name="days">The number of days</param>
        /// <returns>A list of items added in the last X days</returns>
        public List<Item> GetRecentItemsByDays(int days)
        {
            var allItems = _itemRepository.GetAllItems();
            DateTime cutoffDate = DateTime.Now.AddDays(-days);

            return allItems
                .Where(item => item.CreatedDate >= cutoffDate)
                .OrderByDescending(item => item.CreatedDate)
                .ToList();
        }

        /// <summary>
        /// Gets the most recent activity across all items
        /// </summary>
        /// <param name="count">The number of records to retrieve</param>
        /// <returns>A list of the most recent activity</returns>
        public List<ItemHistory> GetRecentItemHistory(int count)
        {
            // Get all items to have access to item names
            var items = _itemRepository.GetAllItems()
                .ToDictionary(i => i.ItemID, i => i);

            // Get all histories from all items
            var allHistories = new List<ItemHistory>();
            foreach (var item in items.Values)
            {
                var histories = _itemRepository.GetItemHistory(item.ItemID);
                foreach (var history in histories)
                {
                    // Enrich the history with item name
                    history.ItemName = item.Name;
                    allHistories.Add(history);
                }
            }

            // Order by timestamp descending and take the most recent ones
            return allHistories
                .OrderByDescending(h => h.ChangedDate)
                .Take(count)
                .ToList();
        }

        #endregion

        #region Reports Operations

        /// <summary>
        /// Gets inventory movement statistics for a date range
        /// </summary>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <returns>Dictionary with movement statistics</returns>
        public Dictionary<string, int> GetInventoryMovementStats(DateTime startDate, DateTime endDate)
        {
            // Get all the item history in the date range
            var histories = _itemRepository.GetItemHistory()
                .Where(h => h.ChangedDate >= startDate && h.ChangedDate <= endDate)
                .ToList();

            var stats = new Dictionary<string, int>
            {
                { "StockIn", 0 },
                { "StockOut", 0 },
                { "Created", 0 },
                { "Updated", 0 },
                { "Deleted", 0 },
                { "Total", histories.Count }
            };

            // Count by change type
            foreach (var history in histories)
            {
                string changeType = history.ChangeType?.ToLower() ?? "";

                if (changeType.Contains("stockin") || changeType.Contains("increase"))
                {
                    stats["StockIn"]++;
                }
                else if (changeType.Contains("stockout") || changeType.Contains("decrease"))
                {
                    stats["StockOut"]++;
                }
                else if (changeType.Contains("create"))
                {
                    stats["Created"]++;
                }
                else if (changeType.Contains("update"))
                {
                    stats["Updated"]++;
                }
                else if (changeType.Contains("delete"))
                {
                    stats["Deleted"]++;
                }
            }

            return stats;
        }

        /// <summary>
        /// Gets category distribution statistics
        /// </summary>
        /// <returns>List of categories with their item counts</returns>
        public List<CategoryWithItemCount> GetCategoryDistribution()
        {
            // Just reuse the existing method for categories with item count
            return GetCategoriesWithItemCount();
        }

        /// <summary>
        /// Gets inventory movements grouped by date
        /// </summary>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <param name="categoryId">Optional category ID filter</param>
        /// <returns>Dictionary with dates and movement counts</returns>
        public Dictionary<DateTime, int> GetInventoryMovementsByDate(DateTime startDate, DateTime endDate, int categoryId = 0)
        {
            // Get all item history
            var allHistory = _itemRepository.GetItemHistory();

            // Filter by date range
            var filteredHistory = allHistory
                .Where(h => h.ChangedDate >= startDate && h.ChangedDate <= endDate);

            // Filter by category if specified
            if (categoryId > 0)
            {
                // Get all items in the category
                var categoryItems = _itemRepository.GetItemsByCategoryId(categoryId);
                var categoryItemIds = categoryItems.Select(i => i.ItemID).ToList();

                // Filter history to only items in the category
                filteredHistory = filteredHistory.Where(h => categoryItemIds.Contains(h.ItemID));
            }

            // Group by date (truncate time component)
            var result = filteredHistory
                .GroupBy(h => h.ChangedDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            // Ensure all dates in range are included
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (!result.ContainsKey(date))
                {
                    result[date] = 0;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets AI recognition statistics
        /// </summary>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <returns>Dictionary with AI recognition statistics</returns>
        public Dictionary<string, int> GetAIRecognitionStats(DateTime startDate, DateTime endDate)
        {
            // Get all items created in the date range
            var items = _itemRepository.GetAllItems()
                .Where(i => i.CreatedDate >= startDate && i.CreatedDate <= endDate)
                .ToList();

            var stats = new Dictionary<string, int>
            {
                { "TotalItems", items.Count },
                { "WithAITags", items.Count(i => !string.IsNullOrEmpty(i.AITags)) },
                { "WithoutAITags", items.Count(i => string.IsNullOrEmpty(i.AITags)) },
                { "WithImages", items.Count(i => !string.IsNullOrEmpty(i.ImagePath)) },
                { "WithoutImages", items.Count(i => string.IsNullOrEmpty(i.ImagePath)) }
            };

            return stats;
        }

        /// <summary>
        /// Gets item history for reporting with filtering
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="categoryId">Optional category ID filter</param>
        /// <param name="changeType">Optional change type filter</param>
        /// <returns>A filtered list of item history records</returns>
        public List<ItemHistory> GetItemHistoryForReporting(DateTime startDate, DateTime endDate, int categoryId = 0, string changeType = null)
        {
            // Get all item history
            var allHistory = _itemRepository.GetItemHistory();

            // Filter by date range
            var filteredHistory = allHistory
                .Where(h => h.ChangedDate >= startDate && h.ChangedDate <= endDate);

            // Filter by category if specified
            if (categoryId > 0)
            {
                // Get all items in the category
                var categoryItems = _itemRepository.GetItemsByCategoryId(categoryId);
                var categoryItemIds = categoryItems.Select(i => i.ItemID).ToList();

                // Filter history to only items in the category
                filteredHistory = filteredHistory.Where(h => categoryItemIds.Contains(h.ItemID));
            }

            // Filter by change type if specified
            if (!string.IsNullOrEmpty(changeType))
            {
                filteredHistory = filteredHistory.Where(h =>
                    h.ChangeType != null &&
                    h.ChangeType.ToLower().Contains(changeType.ToLower()));
            }

            return filteredHistory.ToList();
        }

        #endregion
    }
}